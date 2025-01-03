using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoseNetworkController : MonoBehaviourPun
{
    private HoseInteractable _hoseInteractable;

    private void Awake()
    {
        _hoseInteractable = GetComponent<HoseInteractable>();
    }

    private void OnEnable()
    {
        if (_hoseInteractable != null)
        {
            _hoseInteractable.OnHoseActivated += HandleHoseActivated;
            _hoseInteractable.OnHoseDeactivated += HandleHoseDeactivated;
        }
    }

    private void OnDisable()
    {
        if (_hoseInteractable != null)
        {
            _hoseInteractable.OnHoseActivated -= HandleHoseActivated;
            _hoseInteractable.OnHoseDeactivated -= HandleHoseDeactivated;
        }
    }

    private void HandleHoseActivated()
    {
        photonView.RPC(nameof(RPC_PlayParticleEffect), RpcTarget.All);
    }

    private void HandleHoseDeactivated()
    {
        photonView.RPC(nameof(RPC_StopParticleEffect), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_PlayParticleEffect()
    {
        _hoseInteractable.wateringParticle.Play();
    }

    [PunRPC]
    private void RPC_StopParticleEffect()
    {
        _hoseInteractable.wateringParticle.Stop();
    }
}
