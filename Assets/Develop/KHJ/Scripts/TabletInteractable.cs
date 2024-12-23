using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TabletInteractable : XRGrabInteractable
{
    [SerializeField] private Transform _holder;

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        transform.parent = _holder;
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
}
