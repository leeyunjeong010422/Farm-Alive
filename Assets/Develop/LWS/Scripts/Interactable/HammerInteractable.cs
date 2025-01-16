using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerInteractable : MonoBehaviourPunCallbacks
{
    private bool _isGrapped = false;

    private ParticleSystem _particle;

    private void Awake()
    {
        // _particle = GetComponentInChildren<ParticleSystem>();
        // 
        // _particle.Stop();
    }

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
            // _particle.Play();

            SoundManager.Instance.PlaySFX("SFX_Hammer");

            PhotonView repairView = repair.GetComponent<PhotonView>();
            if (repairView != null)
            {
                repairView.RPC("RPC_PlusRepairCount", RpcTarget.AllBuffered);
            }
        }

        if (collision.collider.gameObject.layer == 6)
        {
            SoundManager.Instance.PlaySFX("SFX_PlayerHit1");
        }
    }
}
