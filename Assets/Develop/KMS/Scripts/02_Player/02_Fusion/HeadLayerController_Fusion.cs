using Fusion;
using UnityEngine;

public class HeadLayerController_Fusion : NetworkBehaviour
{
    public Camera vrCamera;
    public GameObject headObject;
    public GameObject[] controllerObjects;
    public GameObject[] rayInteractors;

    void Start()
    {
        if (Object.HasStateAuthority) // 이 오브젝트가 로컬 플레이어인지 확인
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
                if (controller != null)
                {
                    controller.SetActive(false);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (!Object.HasStateAuthority)
        {
            foreach (GameObject rayInteractor in rayInteractors)
            {
                if (rayInteractor != null && rayInteractor.activeSelf)
                {
                    rayInteractor.SetActive(false);
                }
            }
        }
    }
}
