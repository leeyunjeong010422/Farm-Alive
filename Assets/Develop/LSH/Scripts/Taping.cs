using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class Taping : MonoBehaviourPun
{
    [SerializeField] BoxCover boxCover, currentBox;
    [SerializeField] BoxTrigger boxTrigger;
    [SerializeField] bool isTaping = false;
    [SerializeField] Vector3 firstPosition;
    [SerializeField] Vector3 secendPosition;
    [SerializeField] GameObject startPoint;
    [SerializeField] GameObject endPoint;
    [SerializeField] Collider objCollider;
    [SerializeField] float checkDistance = 0.5f;

    [SerializeField] bool isCanSealed;
    [SerializeField] bool isStart = false;
    [SerializeField] bool isEnd = false;


    private void Update()
    {
        if (isTaping && currentBox != null)
        {
            isCanSealed = true;
        }
        else
        {
            isCanSealed = false;
        }
    }

    public void StartTaping(BoxTrigger boxTriggerBox, BoxCover box)
    {
        Debug.Log($"boxTriggerBox: {boxTriggerBox}, box: {box}");
        if (box == null || box.IsOpen) return;

        currentBox = box;
        boxTrigger = boxTriggerBox;
        isTaping = true;
        Debug.Log($"테이핑 시작: {box.name}");
    }

    public void StopTaping()
    {
        isTaping = false;
        currentBox = null;
        boxTrigger = null;
        Debug.Log("테이핑 중단");
    }

    public void StartPosition()
    {
        if (isCanSealed)
        {
            firstPosition = this.gameObject.transform.position;
            Debug.Log(firstPosition);
            Debug.Log(gameObject.transform.position);
            Debug.Log(currentBox.rightPoint.transform.position);

            if (Vector3.Distance(firstPosition, currentBox.rightPoint.transform.position) < checkDistance)
            {
                Debug.Log("첫거리가0.1이하");
                isStart = true;
                startPoint = currentBox.rightPoint;
                endPoint = currentBox.leftPoint;
            }
            else if(Vector3.Distance(firstPosition, currentBox.leftPoint.transform.position) < checkDistance)
            {
                Debug.Log("첫거리가0.1이하");
                isStart = true;
                startPoint = currentBox.leftPoint;
                endPoint = currentBox.rightPoint;
            }
        }
        
    }

    public void EndPosition()
    {
        if (isCanSealed && isStart)
        {
            secendPosition = this.gameObject.transform.position;

            if (Vector3.Distance(secendPosition, endPoint.transform.position) < checkDistance)
            {
                Debug.Log("둘거리가0.1이하");
                isEnd = true;
            }

            if (isStart && isEnd)
            {
                photonView.RPC(nameof(CompleteBox), RpcTarget.All);
            }
        }
    }

    [PunRPC]
    private void CompleteBox()
    {
            isTaping = false;
            boxTrigger.CompleteTaping();
    }

    public void OnGrab(SelectEnterEventArgs args)
    {
        if (objCollider != null)
        {
            objCollider.isTrigger = true;
        }
    }

    public void OnRelease(SelectExitEventArgs args)
    {
        if (objCollider != null)
        {
            objCollider.isTrigger = false;
        }
    }
}