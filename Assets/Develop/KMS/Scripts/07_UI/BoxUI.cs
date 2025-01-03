using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static QuestManager;

public class BoxUI : MonoBehaviour
{
    public BoxTrigger boxTrigger;
    public TMP_Text boxTextPrefab;
    public Vector3 position;
    public Quaternion rotation;
    private TMP_Text boxTextInstance;

    private void Start()
    {
        if (!boxTrigger || !boxTextPrefab)
        {
            Debug.LogError("BoxTrigger 또는 boxTextPrefab가 없습니다!");
            return;
        }
        CreateTextInstance();
        boxTrigger.RequiredItemsChanged += UpdateUIText;
    }

    private void OnDestroy()
    {
        if (boxTrigger)
        {
            boxTrigger.RequiredItemsChanged -= UpdateUIText;
        }
    }

    private void CreateTextInstance()
    {
        boxTextInstance = Instantiate(boxTextPrefab, boxTrigger.transform);
        boxTextInstance.transform.localPosition = position;
        boxTextInstance.transform.localRotation = rotation;
        boxTextInstance.text = "packaging box";
    }

    private void UpdateUIText(List<RequiredItem> requiredItems)
    {
        string displayText = "Box 안 내용물:\n";
        foreach (var item in requiredItems)
        {
            displayText += $"{item.itemPrefab.name}: {item.requiredcount}\n";
        }

        boxTextInstance.text = displayText;
    }
}
