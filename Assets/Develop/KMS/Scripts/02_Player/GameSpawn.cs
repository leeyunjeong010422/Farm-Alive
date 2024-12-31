using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSpawn : MonoBehaviour
{
    [Tooltip("소환할 플레이어 프리팹")]
    public GameObject playerPrefab;

    [Tooltip("플레이어 소환 위치")]
    public Transform spawnPoint;

    private GameObject _player;

    public void OnEnable()
    {
        SpawnPlayer();
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
}
