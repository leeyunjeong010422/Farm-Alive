using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectUI : MonoBehaviour
{
    public TMP_Text nameTextPrefab;
    public float height = 2.0f;
    public Vector3 position;
    public Quaternion rotation;
    public float scale;
    private TMP_Text nameTextInstance;

    private void Start()
    {
        if (nameTextPrefab)
        {
            nameTextInstance = Instantiate(nameTextPrefab, transform.position + Vector3.up * height, rotation);
            string objectName = gameObject.name.Contains("(Clone)") ? gameObject.name.Replace("(Clone)", "").Trim() : gameObject.name;
            nameTextInstance.text = objectName;

            if (position != Vector3.zero)
            {
                nameTextInstance.transform.position = new Vector3(transform.position.x + position.x, transform.position.y + position.y, transform.position.z + position.z);
            }

            if (scale != 0)
            {
                nameTextInstance.transform.localScale = Vector3.one * scale;
            }

            nameTextInstance.transform.SetParent(transform);
        }
        else
        {
            Debug.LogError("nameTextPrefab이 없습니다!");
        }
    }
}
