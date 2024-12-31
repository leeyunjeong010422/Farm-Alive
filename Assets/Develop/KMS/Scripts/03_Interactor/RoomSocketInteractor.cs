using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RoomSocketInteractor : XRSocketInteractor
{
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if (args.interactableObject != null)
        {
            RoomInfoHolder roomInfoHolder = args.interactableObject.transform.GetComponent<RoomInfoHolder>();
            if (roomInfoHolder)
            {
                string roomName = roomInfoHolder.RoomName;
                Debug.Log($"{args.interactableObject.transform.name}가 소켓에서 빠졌습니다. 방 이름: {roomName}");
                EnterSelectedRoom(roomName);
            }
            else
            {
                Debug.LogWarning("RoomInfoHolder가 해당 오브젝트에 없습니다!");
            }
        }
    }

    private void EnterSelectedRoom(string roomName)
    {
        if (!string.IsNullOrEmpty(roomName))
        {
            Debug.Log($"'{roomName}' 방에 입장 시도...");
            PunManager.Instance.JoinRoom(roomName);
        }
        else
        {
            Debug.LogWarning("입장할 방 이름이 비어 있습니다!");
            Debug.Log(roomName);
        }
    }
}
