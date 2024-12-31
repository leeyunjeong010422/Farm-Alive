using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ListRoomSocketInteractor : XRSocketInteractor
{
    [Tooltip("소켓+큐브 프리팹")]
    public GameObject socketCubePrefab;

    [Tooltip("시작 위치")]
    public Transform startPosition;

    [Tooltip("방 목록 간격")]
    public Vector3 offset = new Vector3(1.5f, -10f, 0);

    [Tooltip("한 줄에 표시할 개수")]
    public int maxColumns = 5;

    // 방 목록 리스트
    private List<GameObject> activeRoomObjects = new List<GameObject>();

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if(args.interactableObject != null)
        {
            Debug.Log($"{args.interactableObject.transform.name}가 소켓에서 빠졌습니다. 방 목록 호출!");
            UpdateRoomList();
        }
    }

    private void UpdateRoomList()
    {
        if (!socketCubePrefab || !startPosition)
        {
            Debug.LogError("SocketCubePrefab 또는 StartPosition이 설정되지 않았습니다!");
            return;
        }

        // 기존 방 목록 삭제
        foreach (GameObject roomObject in activeRoomObjects)
        {
            Destroy(roomObject);
        }
        activeRoomObjects.Clear();

        // Photon 방 목록 가져오기
        List<RoomInfo> roomInfos = PunManager.Instance.GetRoomList();
        if (roomInfos.Count == 0)
        {
            Debug.Log("현재 표시할 방 목록이 없습니다.");
            return;
        }

        // 새로운 방 목록 생성
        for (int i = 0; i < roomInfos.Count; i++)
        {
            int row = i / maxColumns;
            int column = i % maxColumns;

            Vector3 spawnPosition = startPosition.position +
                                    new Vector3(column * offset.x, row * offset.y, 0);

            GameObject socketCube = Instantiate(socketCubePrefab, spawnPosition, Quaternion.identity, transform);
            
            // 큐브의 RoomInfoHolder에 방 이름 저장
            RoomInfoHolder roomInfoHolder = socketCube.GetComponentInChildren<RoomInfoHolder>();
            roomInfoHolder.RoomName = roomInfos[i].Name;

            // 큐브의 텍스트 업데이트
            TMP_Text roomText = socketCube.GetComponentInChildren<TMP_Text>();
            if (roomText)
            {
                RoomInfo room = roomInfos[i];
                roomText.text = $"Room Name: {room.Name}\nPlayer: {room.PlayerCount}/{room.MaxPlayers}";
            }
            Debug.Log($"소켓 및 방 생성 완료: {roomInfos[i].Name}, 위치: {spawnPosition}");

            activeRoomObjects.Add(socketCube);
        }
    }
}
