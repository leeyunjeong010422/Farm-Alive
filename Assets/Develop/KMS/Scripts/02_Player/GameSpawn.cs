using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpawn : MonoBehaviour
{
    [Tooltip("소환할 플레이어 프리팹")]
    public GameObject playerPrefab;

    [Tooltip("플레이어 소환 위치")]
    public Transform spawnPoint;

    private GameObject _player;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ReturnToFusion();
        }
    }

    private void SpawnPlayer()
    {
        // 네트워크 상에서 플레이어 생성
        _player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
        
        if (_player)
        {
            Debug.Log($"게임 씬 플레이어 소환 완료: {FirebaseManager.Instance.GetUserId()}");
        }
        else
        {
            Debug.LogError("게임 씬 플레이어 소환 실패!");
        }
    }

    public void ReturnToFusion()
    {
        ClearSingletonManagers();
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

    private void ClearSingletonManagers()
    {
        // SectionManager 제거
        if (SectionManager.Instance != null)
        {
            Debug.Log("SectionManager 제거");
            Destroy(SectionManager.Instance.gameObject);
        }

        // LightingManager 제거
        if (LightingManager.Instance != null)
        {
            Debug.Log("LightingManager 제거");
            Destroy(LightingManager.Instance.gameObject);
            LightingManager.Instance = null;
        }
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
