using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NutrientInteractable : XRGrabInteractable
{
    private CupInteractable _cup;
    private LiquidContainer _liquidContainer;
    private Coroutine _pourCoroutine;

    protected override void Awake()
    {
        base.Awake();

        _cup = GetComponent<CupInteractable>();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        if (args.interactorObject is XRSocketInteractor)
        {
            _liquidContainer = args.interactorObject.transform.GetComponent<LiquidContainer>();
            if (_liquidContainer != null && _cup != null)
            {
                _pourCoroutine = StartCoroutine(Pour());
            }
            SoundManager.Instance.StopSFXLoop("SFX_NutrientContainerPoured");
        }
        SoundManager.Instance.PlaySFX("SFX_NutrientContainerSelected");
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if (_pourCoroutine != null)
        {
            StopCoroutine( _pourCoroutine );
            _pourCoroutine = null;
        }

        SoundManager.Instance.PlaySFX("SFX_NutrientContainerExited");
    }

    private IEnumerator Pour()
    {
        while (true)
        {
            float amount = _cup.pourRate * Time.deltaTime;
            _liquidContainer.ReceiveLiquid(amount);

            yield return null;
        }
    }
}
