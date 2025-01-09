using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomListUpdater : MonoBehaviour
{
    [Header("Room List Settings")]
    [Tooltip("Room 오브젝트")]
    public GameObject[] roomObjects;
    [Tooltip("방 리스트 갱신 간격")]
    public float updateInterval = 3f;

    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();
    private Coroutine updateCoroutine;

    private void OnEnable()
    {
        // 방 리스트 갱신 시작
        StartUpdatingRoomList();
    }
    private void OnDisable()
    {
        // 갱신 종료
        StopUpdatingRoomList();

        // 방 리스트 초기화
        ClearRoomList();
    }

    private void StartUpdatingRoomList()
    {
        if (updateCoroutine == null)
        {
            updateCoroutine = StartCoroutine(RoomListUpdateCoroutine());
        }
    }

    private void StopUpdatingRoomList()
    {
        if (updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
            updateCoroutine = null;
        }
    }

    private IEnumerator RoomListUpdateCoroutine()
    {
        while (true)
        {
            UpdateRoomListUI();
            yield return new WaitForSeconds(updateInterval);
        }
    }

    private void UpdateRoomListUI()
    {
        // 방 리스트 초기화
        ClearRoomList();

        // Photon에서 최신 방 리스트 가져오기
        cachedRoomList = PunManager.Instance.GetRoomList();

        // 최대 roomObjects.Length 개수만큼 방 리스트 표시
        int count = Mathf.Min(roomObjects.Length, cachedRoomList.Count);


        for (int i = 0; i < count; i++)
        {
            roomObjects[i].SetActive(true);

            // 방 정보 설정
            ObjectUI roomText = roomObjects[i].GetComponent<ObjectUI>();
            if (roomText)
            {
                roomText.nameTextPrefab.text = $"{cachedRoomList[i].Name} ({cachedRoomList[i].PlayerCount}/{cachedRoomList[i].MaxPlayers})";
            }
            else
            {
                Debug.Log("없음");
            }
        }
    }

    private void ClearRoomList()
    {
        foreach(var roomObject in roomObjects)
        {
            roomObject.SetActive(false);
        }
    }
}
