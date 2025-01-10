using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class Door : MonoBehaviour
{
    [SerializeField]
    HingeJoint m_DoorJoint;

    [SerializeField]
    TransformJoint m_DoorPuller;

    [SerializeField]
    float m_HandleOpenValue = 0.1f;
    [SerializeField]
    float m_HandleCloseValue = 0.5f;
    [SerializeField]
    float m_HingeCloseAngle = 5.0f;

    JointLimits m_OpenDoorLimits;
    JointLimits m_ClosedDoorLimits;
    bool m_Closed = false;
    float m_LastHandleValue = 1.0f;

    void Start()
    {
        m_OpenDoorLimits = m_DoorJoint.limits;
        m_ClosedDoorLimits = m_OpenDoorLimits;
        m_ClosedDoorLimits.min = 0.0f;
        m_ClosedDoorLimits.max = 0.0f;
        m_DoorJoint.limits = m_ClosedDoorLimits;
        m_Closed = true;
    }

    void Update()
    {
        // Door auto-close logic
        if (!m_Closed)
        {
            if (m_LastHandleValue < m_HandleCloseValue)
                return;

            if (Mathf.Abs(m_DoorJoint.angle) < m_HingeCloseAngle)
            {
                m_DoorJoint.limits = m_ClosedDoorLimits;
                m_Closed = true;
            }
        }
    }

    public void BeginDoorPulling(SelectEnterEventArgs args)
    {
        m_DoorPuller.connectedBody = args.interactorObject.GetAttachTransform(args.interactableObject);
        m_DoorPuller.enabled = true;
    }

    public void EndDoorPulling()
    {
        m_DoorPuller.enabled = false;
        m_DoorPuller.connectedBody = null;
    }

    public void DoorHandleUpdate(float handleValue)
    {
        m_LastHandleValue = handleValue;

        // remove m_Locked check => no lock concept
        if (!m_Closed)
            return;

        // if handle is pulled enough => open door
        if (handleValue < m_HandleOpenValue)
        {
            m_DoorJoint.limits = m_OpenDoorLimits;
            m_Closed = false;
        }
    }
}