using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.XR;

public class SoundManager : MonoBehaviour
{
    public AudioMixer test; // 오디오 믹서
    public GameObject sliderUI; // 슬라이더 UI 패널
    public XRNode rightControllerNode = XRNode.RightHand; // 오른손 컨트롤러

    private float maxVolumeDb = 20f;
    private float minVolumeDb = -80f;
    private float buttonHoldTime = 1f; // 버튼을 눌러야 하는 시간
    private float buttonHoldCounter = 0f; // 버튼 누름 시간 추적
    private bool isSliderUIActive = false; // 슬라이더 UI 활성화 상태

    private Transform _mainCamera; // 메인 카메라의 Transform
    public float distanceFromCamera = 3f; // 카메라에서 슬라이더 UI가 떨어질 거리

    private void Start()
    {
        _mainCamera = Camera.main.transform;

        if (!_mainCamera)
        {
            Debug.LogError("Main Camera를 찾을 수 없습니다!");
        }

        if (sliderUI)
        {
            sliderUI.SetActive(false);
        }
    }

    public void SetBGMVolume(float volume)
    {
        float dBValue = Mathf.Lerp(minVolumeDb, maxVolumeDb, volume);
        test.SetFloat("PlayerVoiceVolum", dBValue);
    }

    private void Update()
    {
        if (IsControllerButtonPressed(rightControllerNode, CommonUsages.primaryButton))
        {
            buttonHoldCounter += Time.deltaTime;

            if (buttonHoldCounter >= buttonHoldTime)
            {
                ToggleSliderUI();
                buttonHoldCounter = 0f;
            }
        }
#if UNITY_EDITOR
        else if (Input.GetKey(KeyCode.Slash))
        {
            buttonHoldCounter += Time.deltaTime;

            if (buttonHoldCounter >= buttonHoldTime)
            {
                ToggleSliderUI();
                buttonHoldCounter = 0f;
            }
        }
#endif
        else
        {
            buttonHoldCounter = 0f;
        }

        if (isSliderUIActive && sliderUI != null && _mainCamera != null)
        {
            UpdateSliderUIPosition();
        }
    }

    private void ToggleSliderUI()
    {
        isSliderUIActive = !isSliderUIActive;

        if (sliderUI != null)
        {
            sliderUI.SetActive(isSliderUIActive);

            if (isSliderUIActive)
            {
                UpdateSliderUIPosition();
            }
        }
    }

    private void UpdateSliderUIPosition()
    {
        Vector3 newPosition = _mainCamera.position + _mainCamera.forward * distanceFromCamera;
        sliderUI.transform.position = newPosition;
        sliderUI.transform.rotation = Quaternion.LookRotation(sliderUI.transform.position - _mainCamera.position);
    }

    private bool IsControllerButtonPressed(XRNode controllerNode, InputFeatureUsage<bool> button)
    {
        // 컨트롤러 노드에서 버튼 입력 상태 확인
        InputDevice device = InputDevices.GetDeviceAtXRNode(controllerNode);
        if (device.isValid && device.TryGetFeatureValue(button, out bool isPressed))
        {
            return isPressed;
        }
        return false;
    }
}
