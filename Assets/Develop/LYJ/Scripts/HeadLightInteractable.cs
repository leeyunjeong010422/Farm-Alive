using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HeadLightInteractable : XRGrabInteractable
{
    private Light _headLight;
    private Light _directionalLight;

    protected override void Awake()
    {
        base.Awake();

        if (_headLight == null)
        {
            _headLight = GetComponentInChildren<Light>();
            if (_headLight == null)
            {
                Debug.LogError("HeadLight가 존재하지 않습니다.");
            }
        }

        if (_directionalLight == null)
        {
            Light[] lights = FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    _directionalLight = light;
                    break;
                }
            }

            if (_directionalLight == null)
            {
                Debug.LogError("씬에 Directional Light가 없습니다.");
            }
        }

        HeadLightState();
    }

    private void Update()
    {
        HeadLightState();
    }

    private void HeadLightState()
    {
        if (_directionalLight == null || _headLight == null) return;

        if (_directionalLight.enabled)
        {
            if (_headLight.enabled)
            {
                _headLight.enabled = false;
                Debug.Log("헤드라이트 끄기");
            }
        }
        else
        {
            if (!_headLight.enabled)
            {
                _headLight.enabled = true;
                Debug.Log("헤드라이트 켜기");
            }
        }
    }
}
