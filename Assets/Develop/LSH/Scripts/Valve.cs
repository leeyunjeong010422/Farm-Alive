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

        if (hosePoint == null)
        {
            Debug.LogError("호스포인가 없음");
            return;
        }


        if (other.gameObject.tag == "Player")
        {
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

        Transform startPoint = hoseTransform.Find("StartPoint");
        if (startPoint == null)
        {
            Debug.LogError("스타트포인트가 없음");
            return;
        }

        if (startPoint == null)
        {
            Debug.LogError("스타트포인트가 없음");
            foreach (Transform child in hoseTransform)
            {
                Debug.Log($"호스의 하위 오브젝트: {child.name}");
            }
            return;
        }


        AlignHose(hoseTransform, startPoint);


        Rigidbody hoseRigidbody = grabInteractable.GetComponent<Rigidbody>();
        if (hoseRigidbody != null)
        {
            hoseRigidbody.isKinematic = true;
        }

        isHoseConnected = true;

        Debug.Log("호스 연결 완료!!");
    }

    private void AlignHose(Transform hoseTransform, Transform startPoint)
    {
        Vector3 positionOffset = hosePoint.position - startPoint.position;
        hoseTransform.position += positionOffset;

        Quaternion rotationOffset = Quaternion.FromToRotation(startPoint.forward, hosePoint.forward);
        hoseTransform.rotation = rotationOffset * hoseTransform.rotation;

        hoseTransform.SetParent(transform);
    }
}