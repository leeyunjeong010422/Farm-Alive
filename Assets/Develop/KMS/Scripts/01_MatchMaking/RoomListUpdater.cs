using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        ClearRoomList();
        Debug.Log("방 목록 갱신!");

        cachedRoomList = PunManager.Instance.GetRoomList();

        if (cachedRoomList == null || cachedRoomList.Count == 0)
        {
            Debug.LogWarning("방 목록이 없음");
            return;
        }

        int count = Mathf.Min(roomObjects.Length, cachedRoomList.Count);

        for (int i = 0; i < count; i++)
        {
            if (i >= cachedRoomList.Count)
            {
                Debug.LogWarning($"Index {i} out of range for cachedRoomList.");
                break;
            }

            roomObjects[i].SetActive(true);

            TMP_Text roomText = roomObjects[i].GetComponentInChildren<TMP_Text>();
            if (roomText)
            {
                // CustomProperties에서 게임 모드와 스테이지 가져오기
                string gameMode = cachedRoomList[i].CustomProperties.TryGetValue("gameMode", out object gameModeValue) ? gameModeValue.ToString() : "Unknown";
                string stage = cachedRoomList[i].CustomProperties.TryGetValue("selectedStage", out object stageValue) ? stageValue.ToString() : "Unknown";

                E_GameMode e_GameMode = ((E_GameMode)(int.Parse(gameMode)));
                E_StageMode e_StageMode = ((E_StageMode)(int.Parse(stage)));

                roomText.text = $"{cachedRoomList[i].Name} ({cachedRoomList[i].PlayerCount}/{cachedRoomList[i].MaxPlayers})\nGame Mode : {e_GameMode.ToString()}\nStage : {e_StageMode.ToString()}";
            }

            Button roomButton = roomObjects[i].GetComponentInChildren<Button>();
            if (roomButton != null)
            {
                string roomName = cachedRoomList[i].Name;
                roomButton.onClick.RemoveAllListeners();
                roomButton.onClick.AddListener(() => JoinRoom(roomName));
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

    private void JoinRoom(string roomName)
    {
        Debug.Log($"Trying to join room: {roomName}");
        PunManager.Instance.JoinRoom(roomName);
    }
}
