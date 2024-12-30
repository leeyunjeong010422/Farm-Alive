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
            Debug.Log($"{args.interactableObject.transform.name}가 소켓에서 빠졌습니다. 해당 방으로 입장 합니다!");
        }
    }
}
