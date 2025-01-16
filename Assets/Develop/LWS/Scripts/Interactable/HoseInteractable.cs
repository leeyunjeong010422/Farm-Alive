using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HoseInteractable : XRGrabInteractable
{
    [SerializeField] public ParticleSystem wateringParticle;
    [SerializeField] float _pourRate;

    public event Action OnHoseActivated;
    public event Action OnHoseDeactivated;

    protected override void OnEnable()
    {
        base.OnEnable();

        wateringParticle.Stop();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        SoundManager.Instance.PlaySFX("SFX_HoseSelected");

        if (args.interactorObject is XRSocketInteractor)
        {
            LiquidContainer receiver = args.interactorObject.transform.GetComponent<LiquidContainer>();
            if (receiver != null)
            {
                float amount = _pourRate * Time.deltaTime;
                receiver.ReceiveLiquid(amount);
            }

            wateringParticle.Stop();
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        SoundManager.Instance.PlaySFX("SFX_HoseExited");
    }

    protected override void OnActivated(ActivateEventArgs args)
    {
        base.OnActivated(args);

        OnHoseActivated?.Invoke();
    }

    protected override void OnDeactivated(DeactivateEventArgs args)
    {
        base.OnDeactivated(args);

        OnHoseDeactivated?.Invoke();
    }
}
