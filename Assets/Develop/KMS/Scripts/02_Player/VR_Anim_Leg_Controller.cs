using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

public class VR_Anim_Leg_Controller : MonoBehaviourPun
{
    [Header("Controller Settings")]
    public Transform cameraTransform;
    public Animator animator;        
    public float moveSpeed = 1.0f;   
    public float rotationSpeed = 100.0f;
    public float hmdRotationThreshold = 30f;
    public float hmdLerpSpeed = 0.07f;

    [Header("Raycast Settings")]
    public float raycastDistance = 1f;
    public LayerMask obstacleLayer;
    public float sphereCastRadius = 0.2f;

    private Vector2 _leftInputAxis;
    private Vector2 _rightInputAxis;
    public XRNode _leftControllerNode = XRNode.LeftHand;     // 왼손 컨트롤러
    public XRNode _rightControllerNode = XRNode.RightHand;     // 오른손 컨트롤러

    private bool _isPrimaryPressed = false;
    private bool _isSecondaryPressed = false;
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
        if (photonView.IsMine)
        {    // 컨트롤러 조이스틱 입력 가져오기
            InputDevice leftController = InputDevices.GetDeviceAtXRNode(_leftControllerNode);
            if (leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out _leftInputAxis))
            {
                if (_leftInputAxis != Vector2.zero && CanMoveInDirection(_leftInputAxis))
                {
                    MoveCharacter();
                    animator.SetFloat("Speed", _leftInputAxis.magnitude); // Speed 파라미터 전달
                }
                else
                {
                    animator.SetFloat("Speed", 0f); // 정지 상태
                    SoundManager.Instance.StopSFXLoop("SFX_PlayerMove");
                }

                photonView.RPC("SyncAnimationRPC", RpcTarget.Others, _leftInputAxis.magnitude);
            }

#if UNITY_EDITOR
            // 캐릭터 속도 조절 버튼
            {
                InputDevice rightController = InputDevices.GetDeviceAtXRNode(_rightControllerNode);
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
#endif
            {
            // 오른손 컨트롤러 입력 처리 (회전)
            InputDevice rightController2 = InputDevices.GetDeviceAtXRNode(_rightControllerNode);
                if (rightController2.TryGetFeatureValue(CommonUsages.primary2DAxis, out _rightInputAxis))
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
        }
    }

    private void RotateCharacter(float rotationInput)
    {
        if (cameraTransform != null)
        {
            Vector3 cameraPosition = cameraTransform.position;

            // 카메라 중심으로 회전
            transform.RotateAround(cameraPosition, Vector3.up, rotationInput * rotationSpeed * Time.deltaTime);
        }
        else
        {
            // 기존 회전 방식 (카메라가 없을 경우)
            float rotationAngle = rotationInput * rotationSpeed * Time.deltaTime;
            transform.Rotate(0, rotationAngle, 0);
        }
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
        if (photonView.IsMine)
        {
            // 카메라 방향을 기준으로 이동 벡터 계산
            Vector3 forward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
            Vector3 right = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;

            Vector3 moveDirection = (forward * _leftInputAxis.y + right * _leftInputAxis.x).normalized;

            // 캐릭터 이동
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            SoundManager.Instance.PlaySFXLoop("SFX_PlayerMove");
        }
    }

    private bool CanMoveInDirection(Vector2 inputAxis)
    {
        Vector3 forward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        Vector3 right = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;

        Vector3 moveDirection = (forward * inputAxis.y + right * inputAxis.x).normalized;

        // Raycast origin 설정 (카메라 위치를 기준)
        Vector3 origin = cameraTransform.position;

        RaycastHit[] hits = Physics.SphereCastAll(origin, sphereCastRadius, moveDirection, raycastDistance, obstacleLayer);
        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                //Debug.Log($"이동 방해하는 오브젝트: {hit.collider.gameObject.name}");
                return false;
            }
        }

        return true;
    }

    [PunRPC]
    public void SyncAnimationRPC(float speed)
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", speed);
        }
    }
}