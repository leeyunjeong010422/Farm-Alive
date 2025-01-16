using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BoxInteractable : XRGrabInteractable
{
    HashSet<IXRInteractor> interactorSet = new();

    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        interactorSet.Add(interactor);
        
        if (CheckTwoHanded())
            return base.IsSelectableBy(interactor);
        else
            return false;
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);

        interactorSet.Remove(args.interactorObject);
    }

    private bool CheckTwoHanded()
    {
        // Interactor 개수 판단
        if (interactorSet.Count >= 2)
            return true;
        else
            return false;
    }
}
