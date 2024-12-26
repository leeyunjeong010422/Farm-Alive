using Fusion;
using UnityEngine;

public class HeadLayerController_Fusion : NetworkBehaviour
{
    public Camera vrCamera;         // VR 카메라를 드래그 앤 드롭으로 연결
    public GameObject headObject;   // 머리 오브젝트를 연결

    void Start()
    {
        if (Object.HasStateAuthority) // 이 오브젝트가 로컬 플레이어인지 확인
        {
            if (vrCamera != null && headObject != null)
            {
                // 머리 오브젝트의 레이어를 설정
                headObject.layer = LayerMask.NameToLayer("HeadLayer");

                // 카메라에서 해당 레이어를 제외
                vrCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("HeadLayer"));
            }
        }
        else
        {
            // 원격 플레이어일 경우 머리 오브젝트를 보이도록 설정
            headObject.layer = LayerMask.NameToLayer("Default");
        }
    }
}
