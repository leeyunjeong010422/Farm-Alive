using UnityEngine;
using UnityEngine.XR;

public class VR_Anim_Controller_Local : MonoBehaviour
{
    [Header("Controller Settings")]
    public Transform cameraTransform;                       
    public Animator animator;                               
    public float moveSpeed = 1.0f;                          
    public float rotationSpeed = 100.0f;
    public float hmdRotationThreshold = 30f;
    public float hmdLerpSpeed = 0.07f;

    private Vector2 _leftInputAxis;
    private Vector2 _rightInputAxis;
    private XRNode _leftControllerNode = XRNode.LeftHand;
    private XRNode _rightControllerNode = XRNode.RightHand;

    private bool _isMoving;
    private bool _isRotating;

    private float _previousHmdYaw;

    private void Start()
    {
        // 초기 기준 HMD Yaw 값 설정
        _previousHmdYaw = cameraTransform.eulerAngles.y;
    }

    void Update()
    {
        InputDevice controller = InputDevices.GetDeviceAtXRNode(_leftControllerNode);
        if (controller.TryGetFeatureValue(CommonUsages.primary2DAxis, out _leftInputAxis))
        {
            if (_leftInputAxis != Vector2.zero)
            {
                MoveCharacter();
                animator.SetFloat("Speed", _leftInputAxis.magnitude);
            }
            else
            {
                animator.SetFloat("Speed", 0f);
            }
        }

        // 오른손 컨트롤러 입력 처리 (회전)
        InputDevice rightController = InputDevices.GetDeviceAtXRNode(_rightControllerNode);
        if (rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out _rightInputAxis))
        {
            if (Mathf.Abs(_rightInputAxis.x) > 0.1f)
            {
                if (!_isRotating)
                {
                    _isRotating = true;
                }

                RotateCharacter(_rightInputAxis.x);
            }
            else
            {
                if (_isRotating)
                {
                    _isRotating = false;
                }
            }
        }

        // 조이스틱 회전 중이 아닐 경우에만 HMD 회전 적용
        if (!_isRotating)
        {
            RotateCharacterWithHMD();
        }
    }

    private void RotateCharacter(float rotationInput)
    {
        float rotationAngle = rotationInput * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, rotationAngle, 0);
    }

    private void RotateCharacterWithHMD()
    {
        float currentHmdYaw = cameraTransform.eulerAngles.y;
        float yawDelta = Mathf.DeltaAngle(_previousHmdYaw, currentHmdYaw);

        if (Mathf.Abs(yawDelta) > hmdRotationThreshold)
        {
            Vector3 targetRotation = new Vector3(0, currentHmdYaw, 0);

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotation), hmdLerpSpeed);
            _previousHmdYaw = currentHmdYaw;
        }
    }

    private void MoveCharacter()
    {
        // 카메라 방향을 기준으로 이동 벡터 계산
        Vector3 forward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        Vector3 right = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;

        Vector3 moveDirection = (forward * _leftInputAxis.y + right * _leftInputAxis.x).normalized;

        // 캐릭터 이동
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
}