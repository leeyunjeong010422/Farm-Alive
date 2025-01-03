using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkDirectInteractor : XRDirectInteractor
{
    [SerializeField] private PhotonView _photonView;

    /// <summary>
    /// 물체를 잡았을 때 동작하는 메서드.
    /// </summary>
    /// <param name="args"></param>
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        IXRSelectInteractable selectInteractable = args.interactableObject;
        
        if (args.interactorObject.transform.GetComponent<PhotonView>().IsMine == false)
            return;

        // 1. 잡은 플레이어가 잡은 물체의 소유권을 가져오기.
        PhotonView interactablePV = selectInteractable.transform.GetComponent<PhotonView>();
        if (!interactablePV) return;
        interactablePV.TransferOwnership(PhotonNetwork.LocalPlayer);
        Debug.Log("소유권 변경");

        // 2. 잡은 사실 알리기
        _photonView.RPC(nameof(SyncSelect), RpcTarget.Others, _photonView.ViewID, interactablePV.ViewID, true);
    }

    /// <summary>
    /// 물체를 놓았을 때 동작하는 메서드.
    /// </summary>
    /// <param name="args"></param>
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if (!_photonView.IsMine)
            return;

        IXRSelectInteractable selectInteractable = args.interactableObject;

        // 1. 놓은 플레이어가 잡은 물체의 소유권을 방장에게 다시 돌려주기.
        PhotonView interactablePV = selectInteractable.transform.GetComponent<PhotonView>();
        if (!interactablePV) return;
        interactablePV.TransferOwnership(PhotonNetwork.MasterClient);

        // 2. 놓은 사실 알리기
        _photonView.RPC(nameof(SyncSelect), RpcTarget.Others, _photonView.ViewID, interactablePV.ViewID, false);
    }

    [PunRPC]
    private void SyncSelect(int interactorID, int interactableID, bool isSelected)
    {
        PhotonView interactorPV =  PhotonView.Find(interactorID);
        PhotonView interactablePV =  PhotonView.Find(interactableID);
        Rigidbody interactableRb = interactablePV.GetComponent<Rigidbody>();
        if (interactablePV.GetComponent<TabletInteractable>() != null)
            return;

        IXRSelectInteractor interactor = interactorPV.GetComponent<IXRSelectInteractor>();
        IXRSelectInteractable interactable = interactablePV.GetComponent<IXRSelectInteractable>();

        if (isSelected)
        {
            interactionManager.SelectEnter(interactor, interactable);
        }
        else
        {
            if (!hasSelection)
                return;

            interactionManager.SelectExit(interactor, interactable);
        }
    }
}
