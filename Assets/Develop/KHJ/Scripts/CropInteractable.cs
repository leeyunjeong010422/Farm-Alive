using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CropInteractable : XRGrabInteractable
{
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        if (args.interactorObject is XRSocketInteractor)
        {
            SectionManager.Instance.Crops[SectionManager.Instance.CurSection].Add(GetComponent<Crop>());

            // 상호작용 X
            interactionLayers = (1 << 29);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if (args.interactorObject is XRSocketInteractor)
        {
            SectionManager.Instance.Crops[SectionManager.Instance.CurSection].Remove(GetComponent<Crop>());

            // Plant 레이어 설정
            interactionLayers = (1 << 1);
        }
    }
}
