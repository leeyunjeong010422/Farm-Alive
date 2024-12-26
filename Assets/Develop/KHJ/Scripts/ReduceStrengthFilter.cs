using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;

public class ReduceStrengthFilter : MonoBehaviour, IXRInteractionStrengthFilter
{
    public float reduceRate;
    public bool canProcess => isActiveAndEnabled;

    public float Process(IXRInteractor interactor, IXRInteractable interactable, float interactionStrength)
    {
        return interactionStrength * reduceRate;
    }
}
