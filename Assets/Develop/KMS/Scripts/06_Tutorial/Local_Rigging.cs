using UnityEngine;
using UnityEngine.XR;

public class Local_Rigging : MonoBehaviour
{
    public Transform leftHandIK;            // 왼손 IK 
    public Transform righttHandIK;          // 오른손 IK
    public Transform headIK;                // HMD IK

    public Transform leftHandController;    // 왼손 컨트롤러
    public Transform rightHandController;   // 오른손 컨트롤러
    public Transform hmd;                   // HMD
    public Transform cameraOffset;

    public Vector3[] leftOffset;            // 왼손 Offset
    public Vector3[] rightOffset;           // 오른손 Offset
    public Vector3[] headOffset;            // hmd Offset

    [Tooltip("리깅 움직임")]
    public float smoothValue = 0.1f;        // 부드럽게 움직일 값
    [Tooltip("캐릭터 기본 높이")]
    public float modelHeight = 1.1176f;     // 캐릭터 높이 값
    [Tooltip("캐릭터 높이 조절속도")]
    public float heightAdjustSpeed = 1f; // 높이 조절 속도

    private float cameraHeightAdjustment = 0;

    /// <summary>
    /// 컨트롤러가 움직인 후 IK의 Transform을 맞추려고.
    /// </summary>
    private void LateUpdate()
    {
        // 오른손 컨트롤러의 조이스틱 Y축 입력으로 modelHeight 조정
        AdjustModelHeight();

        // 로컬 플레이어의 동작 처리
        MappingHandTranform(leftHandIK, leftHandController, true);
        MappingHandTranform(righttHandIK, rightHandController, false);
        MappingBodyTransform(hmd);
        MappingHeadTransform(headIK, hmd);
    }

    /// <summary>
    /// 오른손 컨트롤러 조이스틱 Y축으로 cameraHeightAdjustment 조정
    /// </summary>
    private void AdjustModelHeight()
    {
        InputDevice rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystickInput))
        {
            if (Mathf.Abs(joystickInput.y) > 0.01f)
            {
                cameraHeightAdjustment = joystickInput.y * heightAdjustSpeed * Time.deltaTime;
                cameraOffset.position = new Vector3(cameraOffset.position.x, cameraOffset.position.y + cameraHeightAdjustment, cameraOffset.position.z);
            }
        }

    }

    /// <summary>
    /// 컨트롤러의 싱크를 맞추기 위한 Offset.
    /// </summary>
    /// <param name="ik"></param>
    /// <param name="controller"></param>
    /// <param name="isLeft"></param>
    private void MappingHandTranform(Transform ik, Transform controller, bool isLeft)
    {
        // ik의 Transform = Controller의 Transform
        var offset = isLeft ? leftOffset : rightOffset;

        // 컨트롤러 위치 값. [0]
        ik.position = controller.TransformPoint(offset[0]);
        // 컨트롤러 회전 값. [1]
        ik.rotation = controller.rotation * Quaternion.Euler(offset[1]);
    }

    /// <summary>
    /// HMD와 캐릭터의 몸이 같이 돌아가도록 설정한 메서드.
    /// </summary>
    /// <param name="hmd"></param>
    private void MappingBodyTransform(Transform hmd)
    {
        this.transform.position = new Vector3(hmd.position.x, hmd.position.y - modelHeight, hmd.position.z);
        float yaw = hmd.eulerAngles.y;
        var targetRotation = new Vector3(this.transform.eulerAngles.x, yaw, this.transform.eulerAngles.z);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(targetRotation), smoothValue);
    }

    /// <summary>
    /// HMD의 IK Offset.
    /// </summary>
    /// <param name="ik"></param>
    /// <param name="hmd"></param>
    private void MappingHeadTransform(Transform ik, Transform hmd)
    {
        ik.position = hmd.TransformPoint(headOffset[0]);
        ik.rotation = hmd.rotation * Quaternion.Euler(headOffset[1]);
    }
}
