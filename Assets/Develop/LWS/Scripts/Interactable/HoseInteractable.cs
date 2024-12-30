using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HoseInteractable : XRGrabInteractable
{
    [SerializeField] ParticleSystem _wateringParticle;
    [SerializeField] float _pourRate;

    protected override void OnEnable()
    {
        base.OnEnable();

        _wateringParticle.Stop();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        if (args.interactorObject is XRSocketInteractor)
        {
            LiquidContainer receiver = args.interactorObject.transform.GetComponent<LiquidContainer>();
            if (receiver != null)
            {
                float amount = _pourRate * Time.deltaTime;
                receiver.ReceiveLiquid(amount);
            }

            _wateringParticle.Stop();
        }
    }

    protected override void OnActivated(ActivateEventArgs args)
    {
        base.OnActivated(args);

        if (_wateringParticle != null)
            _wateringParticle.Play();
    }

    protected override void OnDeactivated(DeactivateEventArgs args)
    {
        base.OnDeactivated(args);

        if (_wateringParticle != null && _wateringParticle.isPlaying)
            _wateringParticle.Stop();
    }
}
