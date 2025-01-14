using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestItemSlot : MonoBehaviour
{
    public enum E_SlotState
    {
        Empty, Lack, Sufficient, SIZE
    }

    [SerializeField] private List<GameObject> _layouts;
    [SerializeField] private TextMeshProUGUI cropName;
    [SerializeField] private Image cropImage;
    [SerializeField] private TextMeshProUGUI cropNamrequiredCount;

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            _layouts.Add(transform.GetChild(i).gameObject);
        }
    }

    public void OnUpdate(List<QuestManager.Quest> questList, int idx, int cropIdx)
    {
        CropData cropData = questList[idx].requiredItems[cropIdx].itemPrefab.GetComponent<Crop>().CropData;
        float count = questList[idx].requiredItems[cropIdx].requiredcount;
        bool isEmpty = questList[idx].requiredItems == null;
        bool isSuccess = questList[idx].isSuccess;

        UpdateLayout(isEmpty, isSuccess);
        UpdateCropImage(cropData);
        UpdateCropName(cropData);
        UpdateRequiredCount(count);
    }

    // 상태에 따라 레이아웃 변경
    private void UpdateLayout(bool isEmpty, bool isSuccess)
    {
        if (isEmpty)
        {
            ChangeLayout(E_SlotState.Empty);
            return;
        }

        if (isSuccess)
        {
            ChangeLayout(E_SlotState.Sufficient);
        }
        else
        {
            ChangeLayout(E_SlotState.Lack);
        }
    }

    // 작물 이름 갱신
    private void UpdateCropName(CropData cropData)
    {
        if (cropData == null)
        {
            cropName.text = "";
            return;
        }
        cropName.text = cropData.cropName;
    }

    // 작물 이미지 갱신
    private void UpdateCropImage(CropData cropData)
    {
        if (cropData == null)
        {
            cropImage.sprite = null;
            return;
        }
        cropImage.sprite = cropData.image;
    }

    // 남은 납품량 갱신
    private void UpdateRequiredCount(float count)
    {
        cropNamrequiredCount.text = count.ToString();
    }

    private void ChangeLayout(E_SlotState state)
    {
        for (int i = 0; i < _layouts.Count; i++)
        {
            _layouts[i].SetActive(i == (int)state);
        }
    }
}