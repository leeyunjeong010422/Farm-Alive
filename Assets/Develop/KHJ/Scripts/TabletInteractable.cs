using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TabletInteractable : XRGrabInteractable
{
    [SerializeField] private Transform _holder;
    [SerializeField] private PhotonView _photonView;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        _photonView.RPC(nameof(SyncTransform), RpcTarget.Others, true);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        _photonView.RPC(nameof(SyncTransform), RpcTarget.All, false);
    }

    [PunRPC]
    private void SyncTransform(bool isSelected)
    {
        if (isSelected)
            transform.parent = null;
        else
        {
            transform.parent = _holder;
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }
}
