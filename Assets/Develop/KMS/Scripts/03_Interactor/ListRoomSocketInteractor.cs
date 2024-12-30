using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ListRoomSocketInteractor : XRSocketInteractor
{
    [Tooltip("Room Layer 이름")]
    public GameObject roomListPrefabs;
    public string roomLayerName = "Room";
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if(args.interactableObject != null)
        {
            Debug.Log($"{args.interactableObject.transform.name}가 소켓에서 빠졌습니다. 방 목록 호출!");
            ActivateRoomObject();
        }
    }

    private void ActivateRoomObject()
    {
        int roomLayer = LayerMask.NameToLayer(roomLayerName);
        if (roomLayer == -1)
        {
            Debug.LogWarning($"Layer '{roomLayerName}'가 존재하지 않습니다.");
            return;
        }

        ActivateChildrenFind(roomListPrefabs.transform, roomLayer);

    }

    private void ActivateChildrenFind(Transform parent, int layer)
    {
        foreach (Transform child in parent)
        {
            if (child.gameObject.layer == layer)
            {
                child.gameObject.SetActive(true);
                Debug.Log($"활성화된 오브젝트: {child.gameObject.name}");
            }

            // 자식 객체가 더 있다면 재귀적으로 탐색
            ActivateChildrenFind(child, layer);
        }
    }
}
