using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeUpdater : MonoBehaviour
{
    [SerializeField] string _barrelName;
    [SerializeField] private Slider _slider;

    private void Start()
    {
        LiquidContainer[] liquidContainers = FindObjectsOfType<LiquidContainer>();
        foreach (var item in liquidContainers)
        {
            if (item.transform.parent.name == _barrelName)
            {
                _slider.maxValue = item.MaxAmount;
                _slider.value = item.MaxAmount;
                item.OnGaugeUpdated.AddListener(UpdateGauge);
            }
        }
    }

    private void UpdateGauge(float gaugeValue)
    {
        _slider.value = gaugeValue;
    }
}
