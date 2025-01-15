using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ModeSlectInteractable : XRGrabInteractable
{
    [Header("다음 오브젝트 상황")]
    public GameObject stageObject;
    public GameObject parentObject;

    [Header("GameMode")]
    public E_GameMode gameMode;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    protected override void Awake()
    {
        base.Awake();

        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
#if UNITY_EDITOR
        Debug.Log($"{args.interactableObject.transform.name}가 선택되었습니다.");
#endif
        StartCoroutine(SelectObjectDestroy(args.interactableObject.transform.gameObject));
    }

    IEnumerator SelectObjectDestroy(GameObject targetObject)
    {
        // 3초 대기
        yield return new WaitForSeconds(3f);

        // 물체를 초기 위치와 회전으로 즉시 이동
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        // 선택된 게임 모드 넘겨주기
        PunManager.Instance.SetGameMode(gameMode);

        stageObject.SetActive(true);
        parentObject.SetActive(false);
    }
}
