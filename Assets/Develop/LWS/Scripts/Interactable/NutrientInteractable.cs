using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NutrientInteractable : XRGrabInteractable
{
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {

        CupInteractable cup = transform.GetComponent<CupInteractable>();

        base.OnSelectEntered(args);

        if (args.interactorObject is XRSocketInteractor)
        {
            LiquidContainer receiver = args.interactorObject.transform.GetComponent<LiquidContainer>();
            if (receiver != null)
            {
                float amount = cup.pourRate * Time.deltaTime;
                receiver.ReceiveLiquid(amount);
            }

            cup.particleSystemLiquid.Stop();
            SoundManager.Instance.StopSFXLoop("SFX_NutrientContainerPoured");
        }

        SoundManager.Instance.PlaySFX("SFX_NutrientContainerSelected");
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        SoundManager.Instance.PlaySFX("SFX_NutrientContainerExited");
    }
}
