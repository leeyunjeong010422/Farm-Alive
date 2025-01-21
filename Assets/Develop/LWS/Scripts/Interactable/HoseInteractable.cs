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

    private LiquidContainer _liquidContainer;
    private Coroutine _pourCoroutine;

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
            _liquidContainer = args.interactorObject.transform.GetComponent<LiquidContainer>();
            if (_liquidContainer != null)
            {
                _pourCoroutine = StartCoroutine(Pour());
            }

            wateringParticle.Stop();
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        
        if (_pourCoroutine != null)
        {
            StopCoroutine(_pourCoroutine);
            _pourCoroutine = null;
        }

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

    private IEnumerator Pour()
    {
        while (true)
        {
            float amount = _pourRate * Time.deltaTime;
            _liquidContainer.ReceiveLiquid(amount);

            yield return null;
        }
    }
}
