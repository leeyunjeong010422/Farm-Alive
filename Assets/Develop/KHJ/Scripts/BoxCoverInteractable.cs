using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BoxCoverInteractable : XRGrabInteractable
{
    [Header("박스 몸체")]
    [SerializeField] private GameObject _body;

    [Header("박스 커버")]
    [SerializeField] private PhotonView _photonView;
    [SerializeField] private Rigidbody _rigid;
    [Tooltip("임계 각도 차이")]
    [SerializeField] private float _angleRange;

    private Rigidbody _bodyRigid;
    private BoxCover _bodyBoxCover;

    [field: SerializeField]
    public bool IsOpen {  get; private set; }

    protected override void Awake()
    {
        base.Awake();

        _bodyRigid = _body.GetComponent<Rigidbody>();
        _bodyBoxCover = _body.GetComponent<BoxCover>();
    }

    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        if (_bodyBoxCover.IsPackaged)
            return false;
        else
            return base.IsSelectableBy(interactor);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        SoundManager.Instance.PlaySFXLoop("SFX_BoxOpen");
        _photonView.RPC(nameof(SelectEnterBox), RpcTarget.All);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        SoundManager.Instance.StopSFXLoop("SFX_BoxOpen");
        _photonView.RPC(nameof(SelectExitBox), RpcTarget.All, CheckOpen());
    }

    [PunRPC]
    private void SelectEnterBox()
    {
        _bodyRigid.isKinematic = true;

        _rigid.constraints = RigidbodyConstraints.None;
    }

    [PunRPC]
    private void SelectExitBox(bool isOpen)
    {
        if (!isOpen)
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