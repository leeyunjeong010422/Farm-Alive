using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AngleController : MonoBehaviour
{
    public XRBaseController controller;

    void Update()
    {
        Quaternion rotation = controller.transform.rotation;

        Vector3 angle = rotation.eulerAngles;

        Debug.Log("컨트롤러의 각도 : " + angle);
    }
}
