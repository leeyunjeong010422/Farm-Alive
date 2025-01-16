using System;
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
        SoundManager.Instance.PlaySFX("SFX_Lobby_CropHovered");

        string highStage = FirebaseManager.Instance.GetHighStage();

        // 현재 스테이지와 HighStage 비교 (비연속 Enum 대응)
        Array stageModes = Enum.GetValues(typeof(E_StageMode));
        int currentIndex = Array.IndexOf(stageModes, stageMode);
        int highStageIndex = Array.IndexOf(stageModes, Enum.Parse(typeof(E_StageMode), highStage));

        if (currentIndex <= highStageIndex + 1) // 선택 가능 조건
        {
            base.OnHoverEntered(args);

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

                    // 캐싱된 데이터 사용
                    var stageData = FirebaseManager.Instance.GetCachedStageData((int)stageMode);

                    if (stageData != null)
                    {
                        int stars = stageData.stars;
                        float playTime = stageData.playTime;

                        tmp.text = $"스테이지: {stageMode}\n" +
                                   $"스타: {stars}\n" +
                                   $"플레이 타임: {playTime}초";
                    }
                    else
                    {
                        tmp.text = $"스테이지: {stageMode}\n" +
                               "스타: 데이터 없음\n" +
                               "플레이 타임: 데이터 없음";
                    }
                }
            }
        }
        else // 선택 불가능
        {
            Debug.LogWarning($"현재 스테이지({stageMode})는 선택할 수 없습니다. HighStage는 {highStage}입니다.");
            ShowUnavailableUI();
        }
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        TurnOnUi(false);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        SoundManager.Instance.PlaySFX("SFX_Lobby_CropSelected");
        TurnOnUi(false);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        string highStage = FirebaseManager.Instance.GetHighStage();

        Array stageModes = Enum.GetValues(typeof(E_StageMode));
        int currentIndex = Array.IndexOf(stageModes, stageMode);
        int highStageIndex = Array.IndexOf(stageModes, Enum.Parse(typeof(E_StageMode), highStage));

        if (currentIndex <= highStageIndex + 1) // 선택 가능 조건
        {
            base.OnSelectExited(args);
            StartCoroutine(SelectObjectDestroy(args.interactableObject.transform.gameObject));
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"스테이지({stageMode})는 선택 불가능합니다. OnSelectExited 동작을 무시합니다.");
#endif
            StartCoroutine(ResetPosition());
        }
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
                Vector3 spawnPosition = transform.position + directionToCamera * 2.0f + Vector3.up * 1.0f;

                instantiatedInputField = Instantiate(globalInputFieldPrefab, spawnPosition, Quaternion.LookRotation(-directionToCamera));
                instantiatedInputField.transform.localScale = Vector3.one * 0.02f;
            }
            else
            {
                instantiatedInputField.SetActive(true);
            }
        }
    }

    // 선택 불가능 시 알림 UI 표시
    private void ShowUnavailableUI()
    {
        if (instantiatedUI)
        {
            TextMeshPro tmp = instantiatedUI.GetComponentInChildren<TextMeshPro>();
            if (tmp)
            {
                tmp.text = $"{gameObject.name} 선택 불가";
            }
        }
    }

    private IEnumerator ResetPosition()
    {
        yield return new WaitForSeconds(0.5f);

        transform.position = initialPosition;
        transform.rotation = initialRotation;
    }
}
