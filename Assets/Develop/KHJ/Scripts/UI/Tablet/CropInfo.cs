using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CropInfo : MonoBehaviour
{
    [SerializeField] private CropData _cropData;
    [SerializeField] private TextMeshProUGUI _cropNameText;
    [SerializeField] private TextMeshProUGUI _digCountText;
    [SerializeField] private TextMeshProUGUI _maxMoistureText;
    [SerializeField] private TextMeshProUGUI _maxNutrientText;

    private void Awake()
    {
        _cropNameText.text = _cropData.cropName;
        _digCountText.text = _cropData.digCount.ToString();
        _maxMoistureText.text = _cropData.maxMoisture.ToString();
        _maxNutrientText.text = _cropData.maxNutrient.ToString();
    }
}
