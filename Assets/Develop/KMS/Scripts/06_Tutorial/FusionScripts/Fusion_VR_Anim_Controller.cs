using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Fusion_VR_Anim_Controller : NetworkBehaviour
{
    [Header("Controller Settings")]
    public XRNode controllerNode = XRNode.LeftHand;     // 왼손 컨트롤러
    public float moveSpeed = 1.0f;                      // 이동 속도

    [Header("References")]
    public Transform cameraTransform;                  // 카메라 Transform
    public Animator animator;                          // Animator 컴포넌트

    private Vector2 _inputAxis;                         // 조이스틱 입력값

    public override void FixedUpdateNetwork()
    {
        if (Object.HasInputAuthority)
        {
            // 컨트롤러 조이스틱 입력 가져오기
            InputDevice controller = InputDevices.GetDeviceAtXRNode(controllerNode);
            if (controller.TryGetFeatureValue(CommonUsages.primary2DAxis, out _inputAxis))
            {
                if (_inputAxis != Vector2.zero)
                {
                    MoveCharacter();
                    animator.SetFloat("Speed", _inputAxis.magnitude); // Speed 파라미터 전달
                }
                else
                {
                    animator.SetFloat("Speed", 0f); // 정지 상태
                }

                // 다른 클라이언트와 애니메이션 동기화
                RPC_SyncAnimation(_inputAxis.magnitude);
            }
        }
    }

    private void MoveCharacter()
    {
        if (Object.HasInputAuthority)
        {
            // 카메라 방향을 기준으로 이동 벡터 계산
            Vector3 forward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
            Vector3 right = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;

            Vector3 moveDirection = (forward * _inputAxis.y + right * _inputAxis.x).normalized;

            // 캐릭터 이동
            transform.position += moveDirection * moveSpeed * Runner.DeltaTime;
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_SyncAnimation(float speed)
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", speed);
        }
    }
}
