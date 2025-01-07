using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EnterRoomSocketInteractor : XRSocketInteractor
{
    private bool isSelected;

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        if(args.interactableObject != null && !isSelected)
        {
            isSelected = true;
            Debug.Log($"{args.interactableObject.transform.name}가 소켓에서 빠졌습니다.");
            PunManager.Instance.CreateAndMoveToPunRoom();

            //StartCoroutine(SelectObjectDestroy(args.interactableObject.transform.gameObject));
        }
    }

    IEnumerator SelectObjectDestroy(GameObject targetObject)
    {
        // 3초 대기
        yield return new WaitForSeconds(3f);

        // 오브젝트가 null이 아닌지 확인 후 삭제
        if (targetObject != null)
        {
            Debug.Log($"{targetObject.name} 삭제 완료");
            Destroy(targetObject);
        }
    }
}
