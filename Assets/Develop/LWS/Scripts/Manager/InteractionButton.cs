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
                Debug.Log("WaterBarrelRepair 컴포넌트를 찾을 수 없습니다!");
            }
        }

        if (_nutrientBarrelRepair == null)
        {
            _nutrientBarrelRepair = FindObjectOfType<NutrientBarrelRepair>();
            if (_nutrientBarrelRepair == null)
            {
                Debug.Log("NutrientBarrelRepair 컴포넌트를 찾을 수 없습니다!");
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
            MessageDisplayManager.Instance.ShowMessage("수리를 먼저 해야 물을 줄 수 있습니다!");
            return;
        }

        SectionManager.Instance.IncreaseMoisture(); // 물 주기 동작
    }


    /// <summary>
    /// 비료를 주는 버튼에 사용하기 위할 필드 및 메서드
    /// </summary>
    private bool _isNutrienting = false;

    [SerializeField] private NutrientBarrelRepair _nutrientBarrelRepair;



    public void Nutrienting()
    {
        if (_isNutrienting) return;

        if (_nutrientBarrelRepair == null)
        {
            Debug.LogError("NutrientBarrelRepair가 연결되지 않았습니다!");
            return;
        }

        if (_nutrientBarrelRepair.IsBroken())
        {
            MessageDisplayManager.Instance.ShowMessage("수리를 먼저 해야 비료를 줄 수 있습니다!");
            return;
        }

        SectionManager.Instance.IncreaseNutrient();
    }

    //TODO : 이벤트 추가 후 해충제거 버튼 및 습기 제거 버튼 구현
    
    /// <summary>
    /// 살충제 뿌리는 버튼에 사용하기 위한 필드 및 메서드
    /// </summary>
    private bool _isSprayingPesticide = false;

    [SerializeField] private PesticideRepair _sprayingPesticideRepair;
    public void SprayingPesticide()
    {
        if (!_isSprayingPesticide) return;

        if (_sprayingPesticideRepair == null)
        {
            Debug.LogError("PesticideRepair가 연결되지 않았습니다.");
            return;
        }

        if (_sprayingPesticideRepair.IsBroken())
        {
            MessageDisplayManager.Instance.ShowMessage("수리를 먼저 해야 살충제를 줄 수 있습니다!");
            return;
        }

        //TODO: 살충제 뿌리기 (원석님)
    }

    private bool _isSprayingMoistureRemover = false;

    [SerializeField] private MoistureRemoverRepair _moistureRemoverRepair;

    public void SprayingMoisture()
    {
        if (!_isSprayingMoistureRemover) return;

        if (_moistureRemoverRepair == null)
        {
            Debug.LogError("PesticideRepair가 연결되지 않았습니다.");
            return;
        }

        if (_moistureRemoverRepair.IsBroken())
        {
            MessageDisplayManager.Instance.ShowMessage("수리를 먼저 해야 습기제거제를 줄 수 있습니다!");
            return;
        }

        //TODO: 습기제거제 뿌리기 (원석님)
    }


}
