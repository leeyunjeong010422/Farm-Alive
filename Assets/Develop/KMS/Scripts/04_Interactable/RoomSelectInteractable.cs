using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RoomSelectInteractable : XRGrabInteractable
{
    [Header("부모 오브젝트")]
    public GameObject parentObject;
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    protected override void Awake()
    {
        base.Awake();

        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
#if UNITY_EDITOR
        Debug.Log($"{args.interactableObject.transform.name}가 선택 되었습니다.");
#endif
        StartCoroutine(SelectObjectDestroy(args.interactableObject.transform.gameObject));
    }

    IEnumerator SelectObjectDestroy(GameObject targetObject)
    {
        // 3초 대기
        yield return new WaitForSeconds(3f);

        if (targetObject)
        {
            // 물체를 초기 위치와 회전으로 즉시 이동
            transform.position = _initialPosition;
            transform.rotation = _initialRotation;
        }

        PunManager.Instance.JoinRoom(transform.name);

        parentObject.SetActive(false);
    }
}
