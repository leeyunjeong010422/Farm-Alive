using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Tooltip("소환할 캐릭터 프리펩")]
    public GameObject playerPrefab;
    [Tooltip("플레이어 소환 위치")]
    public Transform spawnPoint;

    private GameObject _spawnedPlayer;

    private void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (!playerPrefab || !spawnPoint)
        {
            Debug.LogError("PlayerPrefab 또는 SpawnPoint가 설정되지 않았습니다.");
            return;
        }

        _spawnedPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

        if (_spawnedPlayer)
        {
            Debug.Log("플레이어가 성공적으로 생성되었습니다!");
        }
        else
        {
            Debug.LogError("플레이어 생성 실패!");
        }
    }
}
