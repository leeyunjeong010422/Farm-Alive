using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class OptionUI : MonoBehaviour
{
    public GameObject canvas;
    public float distanceFromCamera = 3f;
    public XRNode controllerNode = XRNode.LeftHand;

    private bool isCanvasActive = false;
    private bool isButtonPressed = false;

    void Start()
    {
        if (canvas)
        {
            canvas.SetActive(false);
        }
    }

    void Update()
    {
        // 컨트롤러의 Y 버튼 입력 상태 확인
        bool buttonPressed = IsControllerButtonPressed(controllerNode, CommonUsages.primaryButton);

        // 버튼이 눌렸다가 떼어진 상태일 때만 토글 동작
        if (buttonPressed && !isButtonPressed)
        {
            ToggleCanvas();
        }

        // 버튼 상태 업데이트
        isButtonPressed = buttonPressed;

        // 캔버스가 활성화되어 있으면 위치 업데이트
        if (isCanvasActive && canvas)
        {
            UpdateCanvasPosition();
        }
    }

    /// <summary>
    /// 캔버스를 활성화/비활성화 전환
    /// </summary>
    private void ToggleCanvas()
    {
        isCanvasActive = !isCanvasActive;

        if (canvas)
        {
            canvas.SetActive(isCanvasActive);

            if (isCanvasActive)
            {
                UpdateCanvasPosition();
            }
        }
    }

    /// <summary>
    /// 캔버스 위치 및 회전을 카메라 기준으로 업데이트
    /// </summary>
    private void UpdateCanvasPosition()
    {
        if (!canvas) return;

        canvas.transform.position = Camera.main.transform.position + Camera.main.transform.forward * distanceFromCamera;
        canvas.transform.rotation = Quaternion.LookRotation(canvas.transform.position - Camera.main.transform.position);
    }

    /// <summary>
    /// 컨트롤러의 버튼 입력 확인
    /// </summary>
    private bool IsControllerButtonPressed(XRNode node, InputFeatureUsage<bool> button)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(node);
        if (device.isValid && device.TryGetFeatureValue(button, out bool isPressed))
        {
            return isPressed;
        }
        return false;
    }
}