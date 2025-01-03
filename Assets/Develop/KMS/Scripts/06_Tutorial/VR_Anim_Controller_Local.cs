using UnityEngine;
using UnityEngine.XR;

public class VR_Anim_Controller_Local : MonoBehaviour
{
    public XRNode controllerNode = XRNode.LeftHand;     // 왼손 컨트롤러
    public float moveSpeed = 1.0f;                      // 이동 속도
    public Transform cameraTransform;                   // 카메라 Transform
    public Animator animator;                           // Animator 컴포넌트
    private Vector2 inputAxis;                          // 조이스틱 입력값

    void Update()
    {
        InputDevice controller = InputDevices.GetDeviceAtXRNode(controllerNode);
        if (controller.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis))
        {
            if (inputAxis != Vector2.zero)
            {
                MoveCharacter();
                animator.SetFloat("Speed", inputAxis.magnitude); // Speed 파라미터 전달
            }
            else
            {
                animator.SetFloat("Speed", 0f); // 정지 상태
            }
        }

    }

    private void MoveCharacter()
    {
        // 카메라 방향을 기준으로 이동 벡터 계산
        Vector3 forward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        Vector3 right = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;

        Vector3 moveDirection = (forward * inputAxis.y + right * inputAxis.x).normalized;

        // 캐릭터 이동
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
}