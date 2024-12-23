using UnityEngine;

public class HeadLayerController_Local : MonoBehaviour
{
    public Camera vrCamera;         // VR 카메라를 드래그 앤 드롭으로 연결
    public GameObject headObject;   // 머리 오브젝트를 연결

    void Start()
    {
        if (vrCamera != null && headObject != null)
        {
            // 머리 오브젝트의 레이어를 설정
            headObject.layer = LayerMask.NameToLayer("HeadLayer");

            // 카메라에서 해당 레이어를 제외
            vrCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("HeadLayer"));
        }
    }
}
