using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class XRInputManager : MonoBehaviour
{
    public static bool GetButtonDown(XRNode node, InputFeatureUsage<bool> button)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(node);
        if (device.isValid && device.TryGetFeatureValue(button, out bool value))
        {
            return value;
        }
        return false;
    }
}
