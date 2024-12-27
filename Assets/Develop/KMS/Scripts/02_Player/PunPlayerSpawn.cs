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
    }

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

    private void OnEnable()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        // 네트워크 상에서 플레이어 생성
        _player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);

        if (_player)
        {
            Debug.Log($"플레이어 소환 완료: {FirebaseManager.Instance.GetUserId()}");
        }
        else
        {
            Debug.LogError("플레이어 소환 실패!");
        }
    }

    public void ReturnToFusion()
    {
        if (_player != null)
        {
            Debug.Log("Pun 플레이어 삭제...");
            PhotonNetwork.Destroy(_player);
        }

        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Pun 방 나가기...");
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            Debug.LogWarning($"현재 상태에서는 LeaveRoom을 호출할 수 없습니다: {PhotonNetwork.NetworkClientState}");
        }

        // VoiceConnection 초기화
        var voiceConnection = FindObjectOfType<Photon.Voice.Unity.VoiceConnection>();
        if (voiceConnection != null)
        {
            Debug.Log("VoiceConnection 상태 초기화 중...");
            voiceConnection.Client.Disconnect();
            Destroy(voiceConnection.gameObject); // VoiceConnection 객체 삭제
        }

        // 3. 로딩 씬 호출
        Debug.Log("로딩 씬으로 이동...");
        SceneManager.LoadScene("LoadingScene");
    }
}
