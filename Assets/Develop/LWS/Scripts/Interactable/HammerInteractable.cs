using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerInteractable : MonoBehaviourPunCallbacks
{
    private bool _isGrapped = false;

    public void OnGrab()
    {
        _isGrapped = true;
    }

    public void OnRelease()
    {
        _isGrapped = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_isGrapped) return;

        if (!photonView.AmOwner) return;

        Repair repair = collision.collider.gameObject.GetComponent<Repair>();

        if (repair != null)
        {
            PhotonView repairView = repair.GetComponent<PhotonView>();
            if (repairView != null)
            {
                repairView.RPC("RPC_PlusRepairCount", RpcTarget.AllBuffered);
            }
        }
    }
}
