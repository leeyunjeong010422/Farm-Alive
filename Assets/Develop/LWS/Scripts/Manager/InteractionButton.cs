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
    [SerializeField] ParticleSystem _wateringParticle;
    [SerializeField] float _wateringDuration;
    private bool _isWatering;

    public void Watering()
    {
        if (_isWatering) return;

        StartCoroutine(WateringRoutine());
    }

    private IEnumerator WateringRoutine()
    {
        _isWatering = true;
        _wateringParticle.Play();
        SectionManager.Instance.IncreaseMoisture();

        yield return new WaitForSeconds(_wateringDuration);

        _wateringParticle.Stop();
        _isWatering=false;
    }

    /// <summary>
    /// 비료를 주는 버튼에 사용하기 위할 필드 및 메서드
    /// </summary>
    [SerializeField] ParticleSystem _nutrientingParticle;
    [SerializeField] float _nutrientingDuration;
    private bool _isNutrienting;

    public void Nutrienting()
    {
        if (_isNutrienting) return;

        StartCoroutine(NutrientingRoutine());
    }

    private IEnumerator NutrientingRoutine()
    {
        _isNutrienting = true;
        _nutrientingParticle.Play();
        SectionManager.Instance.IncreaseNutrient();

        yield return new WaitForSeconds(_nutrientingDuration);

        _nutrientingParticle.Stop();
        _isNutrienting=false;
    }

    //TODO : 이벤트 추가 후 해충제거 버튼 및 습기 제거 버튼 구현
}
