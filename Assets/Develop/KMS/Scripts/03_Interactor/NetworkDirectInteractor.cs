using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkDirectInteractor : XRDirectInteractor
{
    [SerializeField] private PhotonView photonView;

    private Vector3 _velocity;
    private Vector3 _angularVelocity;

    /// <summary>
    /// 물체를 잡았을 때 동작하는 메서드.
    /// </summary>
    /// <param name="args"></param>
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        IXRSelectInteractable selectInteractable = args.interactableObject;

        // 1. 잡은 플레이어가 잡은 물체의 소유권을 가져오기.
        PhotonView interactablePV = selectInteractable.transform.GetComponent<PhotonView>();
        interactablePV.RequestOwnership();

        // 2. 잡은 사실 알리기
        photonView.RPC(nameof(SyncSelect), RpcTarget.Others, interactablePV.ViewID, true,
            0f, 0f, 0f, 0f, 0f, 0f);
    }

    /// <summary>
    /// 물체를 놓았을 때 동작하는 메서드.
    /// </summary>
    /// <param name="args"></param>
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        IXRSelectInteractable selectInteractable = args.interactableObject;

        // 1. 놓은 플레이어가 잡은 물체의 소유권을 방장에게 다시 돌려주기.
        PhotonView interactablePV = selectInteractable.transform.GetComponent<PhotonView>();
        Rigidbody rb = selectInteractable.transform.GetComponent<Rigidbody>();
        _velocity = rb.velocity;
        _angularVelocity = rb.angularVelocity;

        interactablePV.TransferOwnership(PhotonNetwork.MasterClient);

        // 2. 놓은 사실 알리기
        photonView.RPC(nameof(SyncSelect), RpcTarget.Others, interactablePV.ViewID, false,
            _velocity.x, _velocity.y, _velocity.z, _angularVelocity.x, _angularVelocity.y, _angularVelocity.z);
    }

    [PunRPC]
    private void SyncSelect(int interactable, bool isSelected, 
        float velocityX, float velocityY, float velocityZ,
        float angularVelocityX, float angularVelocityY, float angularVelocityZ)
    {

        PhotonView photonView =  PhotonView.Find(interactable);
        Rigidbody interactableRigid = photonView.transform.GetComponent<Rigidbody>();
        if (photonView.GetComponent<TabletInteractable>() != null)
            return;

        if (isSelected)
        {
            interactableRigid.useGravity = false;
            interactableRigid.isKinematic = true;
        }
        else
        {
            interactableRigid.useGravity = true;
            interactableRigid.isKinematic = false;

            Vector3 velocity = new(velocityX, velocityY, velocityZ);
            Vector3 angularVelocity = new(angularVelocityX, angularVelocityY, angularVelocityZ);
            interactableRigid.velocity = velocity;
            interactableRigid.angularVelocity = angularVelocity;
        }
    }
}
