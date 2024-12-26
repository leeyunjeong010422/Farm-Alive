using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;

public class ReduceTabletStrengthFilter : MonoBehaviour, IXRInteractionStrengthFilter
{
    public bool canProcess => isActiveAndEnabled;

    public float Process(IXRInteractor interactor, IXRInteractable interactable, float interactionStrength)
    {
        return interactionStrength * 0.5f;
    }
}
