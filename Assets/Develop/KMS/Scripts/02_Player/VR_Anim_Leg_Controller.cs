using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class VR_Anim_Leg_Controller : MonoBehaviourPun
{
    public XRNode leftControllerNode = XRNode.LeftHand;     // 왼손 컨트롤러
    public XRNode rightControllerNode = XRNode.RightHand;     // 오른손 컨트롤러
    public float moveSpeed = 1.0f;                      // 이동 속도
    public Transform cameraTransform;                   // 카메라 Transform
    public Animator animator;                           // Animator 컴포넌트
    private Vector2 inputAxis;                          // 조이스틱 입력값
    private bool _isPrimaryPressed = false;
    private bool _isSecondaryPressed = false;

    #region XR Origin과 캐릭터 분리시
    //private void Start()
    //{
    //    //if (!photonView.IsMine) return;

    //    animator = GetComponentInChildren<Animator>();
    //    if (animator == null)
    //        Debug.LogError("Animator를 찾을 수 없습니다.");
    //    else
    //        Debug.Log("Animator 연결 완료");
    //}
    #endregion

    void Update()
    {
        if (photonView.IsMine)
        {    // 컨트롤러 조이스틱 입력 가져오기
            InputDevice leftController = InputDevices.GetDeviceAtXRNode(leftControllerNode);
            if (leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis))
            {
                //Debug.Log("입력 받음.");

                if (inputAxis != Vector2.zero)
                {
                    //Debug.Log("입력 값 변경.");

                    MoveCharacter();
                    animator.SetFloat("Speed", inputAxis.magnitude); // Speed 파라미터 전달
                    //Debug.Log("Speed: " + inputAxis.magnitude);
                }
                else
                {
                    animator.SetFloat("Speed", 0f); // 정지 상태
                }

                photonView.RPC("SyncAnimationRPC", RpcTarget.Others, inputAxis.magnitude);
            }

            // 캐릭터 속도 조절 버튼
            {
                InputDevice rightController = InputDevices.GetDeviceAtXRNode(rightControllerNode);
                if (rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool isPressed))
                {
                    if (isPressed && !_isPrimaryPressed) // 눌림이 시작되었을 때만 처리
                    {
                        _isPrimaryPressed = true; // 버튼 눌림 상태 기록
                        moveSpeed *= 2f;
                    }
                    else if (!isPressed) // 버튼이 떼어졌을 때 상태 초기화
                    {
                        _isPrimaryPressed = false;
                    }
                }

                if (rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isPressed2))
                {
                    if (isPressed2 && !_isSecondaryPressed)
                    {
                        _isSecondaryPressed = true;
                        moveSpeed /= 2f;
                    }
                    else if (!isPressed2)
                    {
                        _isSecondaryPressed = false;
                    }
                }
            }
        }

        #region test
        //if (photonView.IsMine)
        //{
        //    if (Input.GetKeyDown(KeyCode.C))
        //    {
        //        Debug.Log("입력 값 변경.");

        //        MoveCharacter();
        //        animator.SetFloat("Speed", 1); // Speed 파라미터 전달
        //        Debug.Log("C키 눌림!");
        //        photonView.RPC("SyncAnimationRPC", RpcTarget.Others, 1.0f);
        //    }
        //    else if (Input.GetKeyUp(KeyCode.C))
        //    {
        //        animator.SetFloat("Speed", 0f); // 정지 상태
        //        photonView.RPC("SyncAnimationRPC", RpcTarget.Others, 0f);
        //    }
        //}
        #endregion

    }

    private void MoveCharacter()
    {
        if (photonView.IsMine)
        {
            // 카메라 방향을 기준으로 이동 벡터 계산
            Vector3 forward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
            Vector3 right = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;

            Vector3 moveDirection = (forward * inputAxis.y + right * inputAxis.x).normalized;

            // 캐릭터 이동
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
    }

    [PunRPC]
    private void SyncAnimationRPC(float speed)
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", speed);
        }
    }
}