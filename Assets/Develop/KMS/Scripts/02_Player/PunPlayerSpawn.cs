using Fusion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PunPlayerSpawn : MonoBehaviour
{
    [Tooltip("소환할 플레이어 프리팹")]
    public GameObject playerPrefab;

    [Tooltip("플레이어 소환 위치")]
    public Transform spawnPoint;

    private GameObject _player;

    private void Start()
    {
        RemoveNetworkRunner();
        SpawnPlayer();
    }
#if UNITY_EDITOR
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Z))
    //    {
    //        ReturnToFusion();
    //    }
    //}
#endif

    private void RemoveNetworkRunner()
    {
        var runner = FindObjectOfType<NetworkRunner>();
        if (runner != null)
        {
            Debug.Log("씬 이동 후 남아 있는 Fusion서버 ShutDown.");
            // Fusion 서버 ShutDown만 시킴.
            // NetworkRunner는 삭제 시키지 않았는데
            // 한번 Shut Down을 한 경우라면 재사용이 불가능한 것 같다.
            // (다시 false를 true로 변경.)
            runner.Shutdown();
            Destroy(runner.gameObject);
        }
    }

    private void SpawnPlayer()
    {
        // 네트워크 상에서 플레이어 생성
        _player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);

        if (_player)
        {
            Debug.Log($"플레이어 소환 완료: {FirebaseManager.Instance.GetUserId()}");
            if (SceneManager.GetActiveScene().name == "04_PunWaitingRoom")
            {
                PhotonNetwork.AutomaticallySyncScene = true;
                Debug.Log("04_대기실 입장으로 맵동기화 진행.");
            }
        }
        else
        {
            Debug.LogError("플레이어 소환 실패!");
        }
    }

    public void ReturnToFusion()
    {
        ClearPunObject();

        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Pun 방 나가기...");
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            Debug.LogWarning($"현재 상태에서는 LeaveRoom을 호출할 수 없습니다: {PhotonNetwork.NetworkClientState}");
        }


        Debug.Log("서버 교체 중...");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"PhotonNetwork.NetworkClientState = {PhotonNetwork.NetworkClientState}");
    }

    public void ClearPunObject()
    {
        var voiceConnection = FindObjectOfType<Photon.Voice.Unity.VoiceConnection>();
        if (voiceConnection != null)
        {
            Debug.Log("VoiceConnection 상태 초기화 중...");
            if (voiceConnection.Client.InRoom)
            {
                Debug.Log("VoiceConnection 방에서 나가기...");
                voiceConnection.Client.OpLeaveRoom(false);
            }
            voiceConnection.Client.Disconnect(); // 완전히 연결 해제
        }

        if (_player != null)
        {
            var photonView = _player.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                Debug.Log("Pun 플레이어 삭제...");
                PhotonNetwork.Destroy(_player); // 네트워크 상에서 캐릭터 삭제
            }
            else
            {
                Debug.LogWarning("이 객체는 자신의 것이 아니므로 삭제할 수 없습니다.");
            }
        }
    }
}
