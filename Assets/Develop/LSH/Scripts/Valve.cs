using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;
public class Valve : MonoBehaviourPun
{
    [SerializeField] XRKnob knob;
    [SerializeField] GameObject waterEffect;
    [SerializeField] bool isWaterOn = false;
    [SerializeField] Hose hoseConnected;
    [SerializeField] Transform effectPosition;
    [SerializeField] public Transform hoseeffectPosition;

    private void Start()
    {
        knob.onValueChange.AddListener(OnChangeValue);
        hoseConnected = GetComponent<Hose>();
    }

    private void OnChangeValue(float valveValue)
    {
        if (valveValue > 0.5f && !isWaterOn)
        {
            photonView.RPC(nameof(StartWater), RpcTarget.All);
        }
        else if (valveValue <= 0.5f && isWaterOn)
        {
            photonView.RPC(nameof(StopWater), RpcTarget.All);
        }
    }

    [PunRPC]
    private void StartWater()
    {
        if (waterEffect != null && !hoseConnected.isHoseConnected)
        {
            waterEffect.transform.position = effectPosition.position;
            waterEffect.SetActive(true);
        }

        if (waterEffect != null && hoseConnected.isHoseConnected)
        {
            waterEffect.transform.position = hoseeffectPosition.position;
            waterEffect.SetActive(true);
        }

        isWaterOn = true;
    }

    [PunRPC]
    private void StopWater()
    {
        if (waterEffect != null)
            waterEffect.SetActive(false);

        isWaterOn = false;
    }
}