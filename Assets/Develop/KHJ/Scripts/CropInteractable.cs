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
}
