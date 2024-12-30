using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BoxCoverInteractable : XRGrabInteractable
{
    [Header("박스 몸체")]
    [SerializeField] private GameObject _body;

    [Header("박스 커버")]
    [Tooltip("RigidBody")]
    [SerializeField] private Rigidbody _rigid;
    [Tooltip("임계 각도 차이")]
    [SerializeField] private float _angleRange;

    private Rigidbody _bodyRigid;

    [field: SerializeField]
    public bool IsOpen {  get; private set; }

    protected override void Awake()
    {
        base.Awake();

        _bodyRigid = _body.GetComponent<Rigidbody>();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        _bodyRigid.isKinematic = true;

        _rigid.constraints = RigidbodyConstraints.None;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if (!CheckOpen())
        {
            Close();
        }

        _bodyRigid.isKinematic = false;
    }

    private bool CheckOpen()
    {
        bool isPerpendicular = Vector3.Dot(transform.up, _body.transform.up) <= _angleRange;
        bool isInner = 1 - Vector3.Dot(transform.forward, _body.transform.up) <= _angleRange;

        Debug.Log($"Perpendicular: {Vector3.Dot(transform.up, _body.transform.up) <= _angleRange}");
        Debug.Log($"Inner: {1 - Vector3.Dot(transform.forward, _body.transform.up) <= _angleRange}");

        IsOpen = !(isPerpendicular && isInner);
        return IsOpen;
    }

    private void Close()
    {
        _rigid.constraints = RigidbodyConstraints.FreezeRotation;
    }
}