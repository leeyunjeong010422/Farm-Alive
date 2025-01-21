using Photon.Pun;
using UnityEngine;

public class PunHeadLayerController : MonoBehaviourPun
{
    public Camera vrCamera;         // VR 카메라를 드래그 앤 드롭으로 연결
    public GameObject headObject;   // 머리 오브젝트를 연결
    public GameObject[] controllerObjects;   // 컨트롤러 오브젝트 배열 (로컬 플레이어의 컨트롤러)

    void Start()
    {
        if (photonView.IsMine) // 이 오브젝트가 로컬 플레이어인지 확인
        {
            if (vrCamera && headObject)
            {
                // 머리 오브젝트의 레이어를 설정
                headObject.layer = LayerMask.NameToLayer("HeadLayer");

                // 카메라에서 해당 레이어를 제외
                vrCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("HeadLayer"));
            }

            // Controller 레이어 설정 및 카메라에서 포함
            foreach (GameObject controller in controllerObjects)
            {
                if (controller)
                {
                    controller.layer = LayerMask.NameToLayer("Controller");
                    vrCamera.cullingMask |= (1 << LayerMask.NameToLayer("Controller"));
                }
            }
        }
        else
        {
            // 원격 플레이어 처리
            if (headObject)
            {
                // 원격 플레이어의 머리 레이어를 Default로 설정하여 보이게 함
                headObject.layer = LayerMask.NameToLayer("Default");
            }

            // 원격 플레이어의 컨트롤러 비활성화
            foreach (GameObject controller in controllerObjects)
            {
                if (controller)
                {
                    controller.SetActive(false);
                }
            }
        }
    }
}
