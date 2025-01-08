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

    [SerializeField] private WaterBarrelRepair _waterBarrelRepair;

    private void Start()
    {
        if (_waterBarrelRepair == null)
        {
            _waterBarrelRepair = FindObjectOfType<WaterBarrelRepair>();
            if (_waterBarrelRepair == null)
            {
                Debug.LogError("WaterBarrelRepair 컴포넌트를 찾을 수 없습니다!");
            }
        }
    }

    public void Watering()
    {
        if (_isWatering) return;

        if (_waterBarrelRepair == null)
        {
            Debug.LogError("WaterBarrelRepair가 연결되지 않았습니다!");
            return;
        }

        // 고장 상태 확인
        if (_waterBarrelRepair.IsBroken()) // 고장 상태일 경우
        {
            MessageDisplayManager.Instance.ShowMessage("고장을 먼저 수리해야 물을 줄 수 있습니다!");
            return;
        }

        SectionManager.Instance.IncreaseMoisture(); // 물 주기 동작
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
