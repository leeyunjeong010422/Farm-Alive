using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShutterController : MonoBehaviour
{
    [SerializeField] Transform shutter;
    [SerializeField] Vector3 closedPosition;
    [SerializeField] Vector3 openPosition;
    [SerializeField] float moveSpeed = 5f;

    [SerializeField] bool isShutterOpen = false;

    void Update()
    {
        if (isShutterOpen)
        {
            shutter.localPosition = Vector3.MoveTowards(shutter.localPosition, openPosition, moveSpeed * Time.deltaTime);
        }
        else
        {
            shutter.localPosition = Vector3.MoveTowards(shutter.localPosition, closedPosition, moveSpeed * Time.deltaTime);
        }
    }

    [PunRPC]
    public void OpenShutter()
    {
        isShutterOpen = true;
    }

    [PunRPC]
    public void CloseShutter()
    {
        isShutterOpen = false;
    }
}
