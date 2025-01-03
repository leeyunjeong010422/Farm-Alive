using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EnterRoomSocketInteractor : XRSocketInteractor
{
    private bool isSelected;

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        if(args.interactableObject != null && !isSelected)
        {
            isSelected = true;
            Debug.Log($"{args.interactableObject.transform.name}가 소켓에서 빠졌습니다. 방으로 입장합니다!");
            PunManager.Instance.CreateAndMoveToPunRoom();
        }
    }
}
