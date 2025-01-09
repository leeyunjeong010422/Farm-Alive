using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class StageSelectInteractable : XRGrabInteractable
{
    public GameObject parentObject;

    [Header("UI 설정")]
    public GameObject uiPrefab;
    public Vector3 uiOffset;
    public Quaternion rotation;
    public float scale;
    
    private GameObject instantiatedUI;

    [Header("Global Keyboard 설정")]
    public GameObject globalInputFieldPrefab;
    private GameObject instantiatedInputField;

    [Header("Stage Number")]
    public E_StageMode stageMode;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    protected override void Awake()
    {
        base.Awake();

        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
#if UNITY_EDITOR
        Debug.Log($"{args.interactableObject.transform.name}가 Hover가 선택되었습니다.");
#endif
        if (instantiatedUI)
        {
            instantiatedUI.SetActive(true);
        }
        else if (uiPrefab)
        {
            instantiatedUI = Instantiate(uiPrefab, transform.position + transform.right + uiOffset, rotation);
            instantiatedUI.transform.SetParent(transform);

            TextMeshPro tmp = instantiatedUI.GetComponentInChildren<TextMeshPro>();
            if (tmp)
            {
                tmp.transform.localScale = Vector3.one * scale;
                // TODO : 파이어베이스에서 HighstStage값을 가져와서 해당 값과 스테이지 이름을 비교해서
                // 텍스트 출력하기
                tmp.text = $"{gameObject.name} 선택 가능";
            }
        }
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
#if UNITY_EDITOR
        Debug.Log($"{args.interactableObject.transform.name}가 Hover가 종료 되었습니다.");
#endif
        TurnOnUi(false);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
#if UNITY_EDITOR
        Debug.Log($"{args.interactableObject.transform.name}가 Select가 되었습니다.");
#endif
        // TODO : 파이어베이스에서 HighstStage값을 가져와서 해당 값과 스테이지 이름을 비교해서
        // 선택시에 해당 스테이지를 전달 할수 있는지 파악하기.

        TurnOnUi(false);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
#if UNITY_EDITOR
        Debug.Log($"{args.interactableObject.transform.name}가 Select가 종료 되었습니다.");
#endif
        StartCoroutine(SelectObjectDestroy(args.interactableObject.transform.gameObject));
    }

    IEnumerator SelectObjectDestroy(GameObject targetObject)
    {
        // 3초 대기
        yield return new WaitForSeconds(3f);

        transform.position = initialPosition;
        transform.rotation = initialRotation;

        // 선택된 StageNumber 반환.
        Debug.Log($"스테이지 반환 : {stageMode}");
        PunManager.Instance.SetStageNumber(stageMode);

        ActivateGlobalKeyboard();

        parentObject.SetActive(false);
    }

    private void TurnOnUi(bool isOn)
    {
        if (instantiatedUI)
        {
            instantiatedUI.SetActive(isOn);
        }
    }

    private void ActivateGlobalKeyboard()
    {
        if (globalInputFieldPrefab)
        {
            if (!instantiatedInputField)
            {
                Transform mainCameraTransform = Camera.main.transform;

                Vector3 directionToCamera = (mainCameraTransform.position - transform.position).normalized;
                Vector3 spawnPosition = transform.position + directionToCamera * 2.0f + Vector3.up * 1.0f; // 앞쪽 2m, 위쪽 1m

                instantiatedInputField = Instantiate(globalInputFieldPrefab, spawnPosition, Quaternion.LookRotation(-directionToCamera));
                instantiatedInputField.transform.localScale = Vector3.one * 0.02f;
            }
            else
            {
                instantiatedInputField.SetActive(true);
            }
        }
    }
}
