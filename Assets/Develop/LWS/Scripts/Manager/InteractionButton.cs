using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine;

public class InteractionButton : MonoBehaviour
{
    /// <summary>
    /// 물을 주는 버튼에 사용하기 위할 필드 및 메서드
    /// </summary>
    [Header("물 주기")]
    private bool _isWatering = false;

    public void Watering()
    {
        if (_isWatering) return;

        SectionManager.Instance.IncreaseMoisture();
    }


    /// <summary>
    /// 비료를 주는 버튼에 사용하기 위할 필드 및 메서드
    /// </summary>
    private bool _isNutrienting = false;

    public void Nutrienting()
    {
        if (_isNutrienting) return;

        SectionManager.Instance.IncreaseNutrient();
    }

    //TODO : 이벤트 추가 후 해충제거 버튼 및 습기 제거 버튼 구현
}
