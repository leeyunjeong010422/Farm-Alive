using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Crop : MonoBehaviourPun
{
    public enum E_CropState
    {
        Growing, GrowStopped, GrowCompleted
    }

    [Header("작물의 현재 상태")]
    [SerializeField] private E_CropState _cropState;

    [Header("수치")]
    [Tooltip("수확가능 상태로 변경될 때까지 성장가능 상태에서 머물러야 하는 시간")]
    [SerializeField] private float _growthTime;
    [Tooltip("성장가능 상태에서 머무른 시간")]
    [SerializeField] private float _elapsedTime;

    // TODO: 성장가능 상태 판단 로직 추가
    [SerializeField] private bool _canGrowth;

    private XRGrabInteractable _grabInteractable;

    private bool CanGrowth { 
        set
        { 
            _canGrowth = value;
            _cropState = _canGrowth ? E_CropState.Growing : E_CropState.GrowStopped;
        }
    }

    private void Awake()
    {
        _grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        Init();
    }

    private void Update()
    {
        Grow();
    }

    private void Init()
    {
        _canGrowth = true;
        _cropState = E_CropState.Growing;
        _elapsedTime = 0.0f;
    }

    private void Grow()
    {
        if (!_canGrowth)
            return;

        _elapsedTime += Time.deltaTime;

        if (_elapsedTime >= _growthTime)
        {
            CompleteGrow();
        }
    }

    private void CompleteGrow()
    {
        _cropState = E_CropState.GrowCompleted;
        _canGrowth = false;
        _grabInteractable.enabled = true;

        // TODO: 성장완료 피드백 변경
        transform.localScale *= 1.2f;
    }
}
