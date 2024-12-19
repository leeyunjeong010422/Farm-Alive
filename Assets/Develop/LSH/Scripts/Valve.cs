using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;
public class Valve : MonoBehaviour
{
    [SerializeField] XRKnob knob;
    [SerializeField] GameObject waterEffect;
    [SerializeField] bool isHoseConnected = false;
    [SerializeField] Transform hosePoint;

    private void Start()
    {
        knob.onValueChange.AddListener(OnChangeValue);
    }

    private void OnChangeValue(float valveValue)
    {
        Debug.Log("값변경");
        if (valveValue > 0.5f)
        {
            StartWater();
        }
        else
        {
            StopWater();
        }
    }

    private void StartWater()
    {
        Debug.Log("물활성화");
    }

    private void StopWater()
    {
        Debug.Log("물비활성화");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("충돌");
        if (other.gameObject.tag == "Player")
        {
            other.transform.SetParent(transform);

            XRGrabInteractable grabInteractable = other.GetComponent<XRGrabInteractable>();
            if (grabInteractable != null)
            {
                grabInteractable.selectExited.AddListener(x => OnHoseReleased(grabInteractable));
            }

        }
    }

    private void OnHoseReleased(XRGrabInteractable grabInteractable)
    {
        if (isHoseConnected) return;

        Transform hoseTransform = grabInteractable.transform;
        hoseTransform.SetParent(transform);

        hoseTransform.position = hosePoint.position;
        hoseTransform.rotation = hosePoint.rotation;

        Rigidbody hoseRigidbody = grabInteractable.GetComponent<Rigidbody>();
        if (hoseRigidbody != null)
        {
            hoseRigidbody.isKinematic = true;
        }

        isHoseConnected = true;

        Debug.Log("호스 연결 완료!!");
    }

}