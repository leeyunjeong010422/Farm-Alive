using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BoxCoverInteractable : XRGrabInteractable
{
    [Header("¹Ú½º ¸öÃ¼")]
    [SerializeField] private GameObject _body;

    private Rigidbody _bodyRigid;

    protected override void Awake()
    {
        base.Awake();

        _bodyRigid = _body.GetComponent<Rigidbody>();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        _bodyRigid.isKinematic = true;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        _bodyRigid.isKinematic = false;
    }
}
