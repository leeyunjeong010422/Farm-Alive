using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class PlayerSpawn : MonoBehaviourPun
{
    [SerializeField] private Camera cam;
    [SerializeField] private AudioListener audioListener;
    [SerializeField] private TrackedPoseDriver trackedPoseDriver;

    [SerializeField] private ActionBasedControllerManager leftControllerManager;
    [SerializeField] private ActionBasedControllerManager rightControllerManager;

    [SerializeField] private GameObject leftControllerObject;
    [SerializeField] private GameObject rightControllerObject;

    [SerializeField] private ActionBasedController leftController;
    [SerializeField] private ActionBasedController rightController;

    private void Awake()
    {
        Debug.Log($"PhotonView Owner: {photonView.Owner}, IsMine: {photonView.IsMine}");

        if (photonView.IsMine)
        {
            // TODO : 내 캐릭터가 아닌 VR플레이어는
            // 1. 카메라를 비활성화 시킨다.
            cam.enabled = true;
            // 2. 오디오 리스너를 비활성화 시킨다.
            audioListener.enabled = true;
            // 3. TRacked Pose Driver를 비활성화 하여, 입력에 따라 카메라가 움직이지 않도록 한다.
            trackedPoseDriver.enabled = true;
            // 4. 컨트롤러의 입력을 비활성화 시킨다.
            leftController.enabled = true;
            rightController.enabled = true;

            leftControllerManager.enabled = true;
            rightControllerManager.enabled = true;
        }

        leftControllerObject.SetActive(true);
        rightControllerObject.SetActive(true);
    }
}
