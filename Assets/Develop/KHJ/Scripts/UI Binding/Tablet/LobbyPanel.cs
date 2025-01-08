//using Photon.Pun;
//using Photon.Realtime;
//using System.Collections.Generic;
//using System.Reflection;
//using UnityEngine;
//using UnityEngine.EventSystems;

//public class LobbyPanel : UIBinder
//{
//    [SerializeField] RectTransform roomContent;
//    [SerializeField] RoomEntry roomEntryPrefab;

//    private Dictionary<string, RoomEntry> roomDictionary = new Dictionary<string, RoomEntry>();

//    private void Awake()
//    {
//        BindAll();
//    }

//    private void Start()
//    {
//        AddEvent("LeaveLobbyButton", EventType.Click, LeaveLobby);
//    }

//    public void LeaveLobby(PointerEventData eventData)
//    {
//        Debug.Log("로비 퇴장 요청");
//        PhotonNetwork.LeaveLobby();
//    }

//    public void UpdateRoomList(List<RoomInfo> roomList)
//    {
//        foreach (RoomInfo info in roomList)
//        {
//            // 방이 사라진 경우 + 방이 비공개인 경우 + 입장이 불가능한 방인 경우
//            if (info.RemovedFromList == true || info.IsVisible == false || info.IsOpen == false)
//            {
//                // 예외 상황 : 로비 들어가자마자 사라지는 방인 경우
//                if (roomDictionary.ContainsKey(info.Name) == false)
//                    continue;

//                Destroy(roomDictionary[info.Name].gameObject);
//                roomDictionary.Remove(info.Name);
//            }

//            // 새로운 방이 생성된 경우
//            else if (roomDictionary.ContainsKey(info.Name) == false)
//            {
//                RoomEntry roomEntry = Instantiate(roomEntryPrefab, roomContent);
//                roomDictionary.Add(info.Name, roomEntry);
//                roomEntry.SetRoomInfo(info);
//            }

//            // 방의 정보가 변경된 경우
//            else if (roomDictionary.ContainsKey(info.Name) == true)
//            {
//                RoomEntry roomEntry = roomDictionary[info.Name];
//                roomEntry.SetRoomInfo(info);
//            }
//        }
//    }

//    public void ClearRoomEntries()
//    {
//        foreach (string name in roomDictionary.Keys)
//        {
//            Destroy(roomDictionary[name].gameObject);
//        }
//        roomDictionary.Clear();
//    }
//}
