using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SoketController : MonoBehaviourPun 
{
    [SerializeField] GameObject cropObj;
    [SerializeField] Collider[] colliders;
    [SerializeField] SoketController[] sockets;

    [SerializeField] XRSocketInteractor socket;

    private void Start()
    {
        colliders = transform.parent.parent.GetComponents<Collider>();
        socket = GetComponent<XRSocketInteractor>();
        sockets = transform.parent.GetComponentsInChildren<SoketController>();

        socket.selectEntered.AddListener(OnSelectEntered);
        socket.selectExited.AddListener(OnSelectExited);
    }

    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        GameObject selectObject = args.interactableObject.transform.gameObject;
        PhotonView objView = selectObject.GetComponent<PhotonView>();

        BoxTrigger boxTrigger = transform.parent.parent.parent.GetComponent<BoxTrigger>();
        boxTrigger.CountUpdate(objView.ViewID, false);

        photonView.RPC(nameof(ColliderEnabled), RpcTarget.All, objView.ViewID, false);
        
    }

    public void OnSelectExited(SelectExitEventArgs args)
    {
        GameObject selectObject = args.interactableObject.transform.gameObject;
        PhotonView objView = selectObject.GetComponent<PhotonView>();

        BoxTrigger boxTrigger = transform.parent.parent.parent.GetComponent<BoxTrigger>();
        boxTrigger.CountUpdate(objView.ViewID, true);

        photonView.RPC(nameof(ColliderEnabled), RpcTarget.All, objView.ViewID, true);
    }

    [PunRPC]
    private void ColliderEnabled(int viewId, bool isBool)
    {
        if (!isBool)
        {
            PhotonView objView = PhotonView.Find(viewId);
            cropObj = objView.gameObject;
        }

        foreach (Collider col in colliders)
        {
            Physics.IgnoreCollision(col, cropObj.transform.GetChild(0).GetChild(3).GetComponent<Collider>(), isBool);
            Debug.Log("충돌무시");
        }

        foreach (SoketController socket in sockets)
        {
            if(socket.cropObj != null)
            Physics.IgnoreCollision(cropObj.transform.GetChild(0).GetChild(3).GetComponent<Collider>(), socket.cropObj.transform.GetChild(0).GetChild(3).GetComponent<Collider>(), isBool);
        }

        if (isBool)
        {
            cropObj = null;
        }
    }

    private void OnDestroy()
    {
        socket.selectEntered.RemoveListener(OnSelectEntered);
        socket.selectExited.RemoveListener(OnSelectExited);
    }

}