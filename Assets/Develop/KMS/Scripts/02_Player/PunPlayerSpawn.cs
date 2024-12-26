using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunPlayerSpawn : MonoBehaviour
{
    [Tooltip("소환할 플레이어 프리팹")]
    public GameObject playerPrefab;

    [Tooltip("플레이어 소환 위치")]
    public Transform spawnPoint;

    private void OnEnable()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        // 네트워크 상에서 플레이어 생성
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);

        if (player)
        {
            Debug.Log($"플레이어 소환 완료: {FirebaseManager.Instance.GetUserId()}");
        }
        else
        {
            Debug.LogError("플레이어 소환 실패!");
        }
    }
}
