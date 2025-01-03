using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class isSelectedTest : MonoBehaviour
{
    public XRGrabInteractable grabInteractable;

    private void Update()
    {

        if (grabInteractable != null && grabInteractable.isSelected)
        {
            // 물건이 잡혀 있을 때의 처리
            Debug.Log("잡고 있는 물건이 맞습니다.");
        }
        else
        {
            // 물건이 잡히지 않은 상태일 때의 처리
            Debug.Log("물건을 잡고 있지 않습니다.");
        }
    }
}
