using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CropInteractable : XRGrabInteractable
{
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log("CropInteractable OnSelectEntered");
        base.OnSelectEntered(args);

        if (args.interactorObject is XRSocketInteractor)
        {
            Debug.Log("with socket");
            Crop crop = GetComponent<Crop>();

            SectionManager.Instance.Crops[SectionManager.Instance.CurSection].Add(crop);
            crop.ChangeState(Crop.E_CropState.GrowStopped);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if (args.interactorObject is XRSocketInteractor)
        {
            SectionManager.Instance.Crops[SectionManager.Instance.CurSection].Remove(GetComponent<Crop>());
        }
    }

    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        if (interactor is XRSocketInteractor)
            return base.IsSelectableBy(interactor);

        Crop crop = GetComponent<Crop>();
        if (crop.CurState == Crop.E_CropState.Seeding || crop.CurState == Crop.E_CropState.GrowCompleted)
        {
            Debug.Log("return base");
            return base.IsSelectableBy(interactor);
        }
        else
        {
            Debug.Log("return false");
            return false;
        }
    }
}
