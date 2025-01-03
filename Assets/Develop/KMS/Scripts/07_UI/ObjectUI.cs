using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectUI : MonoBehaviour
{
    public TMP_Text nameTextPrefab;
    public float height = 2.0f;
    public Quaternion rotation;
    private TMP_Text nameTextInstance;

    private void Start()
    {
        if (nameTextPrefab)
        {
            nameTextInstance = Instantiate(nameTextPrefab, transform.position + Vector3.up * height, rotation);
            string objectName = gameObject.name.Contains("(Clone)") ? gameObject.name.Replace("(Clone)", "").Trim() : gameObject.name;
            nameTextInstance.text = objectName;
            nameTextInstance.transform.SetParent(transform);
        }
        else
        {
            Debug.LogError("nameTextPrefab이 없습니다!");
        }
    }
}
