using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Hose : MonoBehaviourPun
{
    [SerializeField] Transform hoseAttachmentPoint;
    public bool isHoseConnected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isHoseConnected) 
            return;

        if (other.CompareTag("Player"))
        {
            XRGrabInteractable grab = other.GetComponent<XRGrabInteractable>();
            if (grab != null)
            {
                grab.selectExited.AddListener(x => OnHoseConnect(grab));
            }
        }
    }

    private void OnHoseConnect(XRGrabInteractable grab)
    {
        /*if (isHoseConnected) return;

        Transform hoseRoot = grab.transform;
        Transform startPoint = hoseRoot.Find("StartPoint");
        Transform endPoint = hoseRoot.Find("EndPoint");

        PositionHose(hoseRoot, startPoint);

        Rigidbody hoseRigidbody = grab.GetComponent<Rigidbody>();
        if (hoseRigidbody != null)
        {
            hoseRigidbody.isKinematic = true;
        }

        isHoseConnected = true;*/

        if (isHoseConnected) 
            return;

        photonView.RPC(nameof(ConnectHose), RpcTarget.All, grab.transform.name);
    }

    [PunRPC]
    private void ConnectHose(string hoseName)
    {
        Transform hoseRoot = GameObject.Find(hoseName).transform;
        Transform startPoint = hoseRoot.Find("StartPoint");

        PositionHose(hoseRoot, startPoint);

        Rigidbody hoseRigidbody = hoseRoot.GetComponent<Rigidbody>();
        if (hoseRigidbody != null)
        {
            hoseRigidbody.isKinematic = true;
        }

        isHoseConnected = true;
    }


    private void PositionHose(Transform hoseRoot, Transform startPoint)
    {
        hoseRoot.rotation = Quaternion.identity;
        startPoint.rotation = Quaternion.Euler(-90f, 0f, 0f);

        Vector3 positionOffset = hoseAttachmentPoint.position - startPoint.position;
        hoseRoot.position += positionOffset;

        hoseRoot.SetParent(transform);
    }
}
