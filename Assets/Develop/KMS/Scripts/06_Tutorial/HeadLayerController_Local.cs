using UnityEngine;

public class HeadLayerController_Local : MonoBehaviour
{
    public Camera vrCamera;         // VR 카메라를 드래그 앤 드롭으로 연결
    public GameObject headObject;   // 머리 오브젝트를 연결
    public GameObject[] controllerObjects; // 컨트롤러 오브젝트 배열
    void Start()
    {
        if (vrCamera != null && headObject != null)
        {
            // 머리 오브젝트의 레이어를 설정
            headObject.layer = LayerMask.NameToLayer("HeadLayer");

            // 카메라에서 해당 레이어를 제외
            vrCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("HeadLayer"));
        }

        // Controller 레이어 설정 및 카메라에서 제외
        foreach (GameObject controller in controllerObjects)
        {
            if (controller != null)
            {
                controller.layer = LayerMask.NameToLayer("Controller");
                vrCamera.cullingMask |= (1 << LayerMask.NameToLayer("Controller"));
            }
        }
    }
}
