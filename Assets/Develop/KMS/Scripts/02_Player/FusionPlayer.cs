using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class FusionPlayer : NetworkBehaviour
{
    [Header("Camera and VR Components")]
    [SerializeField] private Camera cam;
    [SerializeField] private AudioListener audioListener;
    [SerializeField] private TrackedPoseDriver trackedPoseDriver;

    [SerializeField] private ActionBasedControllerManager leftControllerManager;
    [SerializeField] private ActionBasedControllerManager rightControllerManager;
    [SerializeField] private ActionBasedController leftController;
    [SerializeField] private ActionBasedController rightController;

    private void Start()
    {
        // 로컬 플레이어만 입력 및 장치 활성화
        if (Object.HasInputAuthority)
        {
            EnableLocalPlayerComponents();
        }
        else
        {
            DisableNonLocalPlayerComponents();
        }
    }

    private void EnableLocalPlayerComponents()
    {
        Debug.Log("로컬 플레이어 컴포넌트 활성화");

        cam.enabled = true;
        audioListener.enabled = true;
        trackedPoseDriver.enabled = true;
        leftController.enabled = true;
        rightController.enabled = true;

        leftControllerManager.enabled = true;
        rightControllerManager.enabled = true;
    }

    private void DisableNonLocalPlayerComponents()
    {
        Debug.Log("로컬 플레이어가 아님. 컴포넌트 비활성화");

        // 1. 카메라를 비활성화 시킨다.
        cam.enabled = false;
        // 2. 오디오 리스너를 비활성화 시킨다.
        audioListener.enabled = false;
        // 3. TRacked Pose Driver를 비활성화 하여, 입력에 따라 카메라가 움직이지 않도록 한다.
        trackedPoseDriver.enabled = false;
        // 4. 컨트롤러의 입력을 비활성화 시킨다.
        leftController.enabled = false;
        rightController.enabled = false;

        leftControllerManager.enabled = false;
        rightControllerManager.enabled = false;
    }
}
