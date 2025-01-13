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

            cropObj.transform.GetChild(0).GetChild(3).GetComponent<Collider>().isTrigger = true;
        }

        if (isBool)
        {
            cropObj.transform.GetChild(0).GetChild(3).GetComponent<Collider>().isTrigger = false;
            cropObj = null;
        }
    }

    private void OnDestroy()
    {
        socket.selectEntered.RemoveListener(OnSelectEntered);
        socket.selectExited.RemoveListener(OnSelectExited);
    }

}