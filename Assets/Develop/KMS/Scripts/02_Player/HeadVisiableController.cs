using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadVisiableController : MonoBehaviour
{
    public Camera vrCamera;
    public GameObject headObject;
    private bool _isCameraInside = false;

    void Start()
    {
        if (!vrCamera)
        {
            Debug.LogError("VR 카메라가 연결되지 않았습니다.");
        }

        if (!headObject)
        {
            Debug.LogError("머리 오브젝트가 연결되지 않았습니다.");
        }

        headObject.layer = LayerMask.NameToLayer("HeadLayer");
        vrCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("HeadLayer"));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == vrCamera.gameObject)
        {
            _isCameraInside = true;
            vrCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("HeadLayer"));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == vrCamera.gameObject)
        {
            _isCameraInside = false;
            vrCamera.cullingMask |= (1 << LayerMask.NameToLayer("HeadLayer"));
        }
    }
}
