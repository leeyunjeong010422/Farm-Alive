using GameData;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Crop : MonoBehaviourPun
{
    public enum E_CropState
    {
        Seeding, Growing, GrowStopped, GrowCompleted, Waste, SIZE
    }

    [SerializeField] private CropData _cropData;
    [SerializeField] private int _ID;
    [SerializeField] private string _name;

    [Header("작물 외형")]
    [Tooltip("성장 단계에 따라 변화하는 외형")]
    [SerializeField] private GameObject[] _GFXs;

    [Header("작물의 현재 상태")]
    [SerializeField] private E_CropState _curState;

    [Header("수치")]
    [Tooltip("상품성")]
    [SerializeField] private float _value;
    [Tooltip("밭에 심어지기 위해 밭이 경작돼야하는 횟수")]
    [SerializeField] private int _digCount;
    [Tooltip("수확가능 상태로 변경될 때까지 성장가능 상태에서 머물러야 하는 시간")]
    [SerializeField] private float _growthTime;
    [Tooltip("성장가능 상태에서 머무른 시간")]
    [SerializeField] private float _elapsedTime;
    [Tooltip("성장가능 상태가 되기 위해 필요한 수분")]
    [SerializeField] private int _idleMaxMoisture;
    [Tooltip("가뭄일 때 성장가능 상태가 되기 위해 필요한 수분")]
    [SerializeField] private int _droughtMaxMoisture;
    [Tooltip("작물의 현재 수분")]
    [SerializeField] private int _curMoisture;
    [Tooltip("성장가능 상태가 되기 위해 필요한 영양분")]
    [SerializeField] private int _idleMaxNutrient;
    [Tooltip("가뭄일 때 성장가능 상태가 되기 위해 필요한 영양분")]
    [SerializeField] private int _droughtMaxNutrient;
    [Tooltip("작물의 현재 영양분")]
    [SerializeField] private int _curNutrient;
    [Tooltip("병충해 발생 시 피해 배율")]
    [SerializeField] private float _damageRate;
    [Tooltip("병충해 해결 제한시간")]
    [SerializeField] private float _damageLimitTime;
    [Tooltip("온도하강 해결 제한시간")]
    [SerializeField] private float _temperatureDecreaseLimitTime;
    [Tooltip("온도상승 해결 제한시간")]
    [SerializeField] private float _temperatureIncreaseLimitTime;

    private EventManager _eventManager;

    private BaseState[] _states = new BaseState[(int)E_CropState.SIZE];
    private CropInteractable _cropInteractable;
    private int _maxGrowthStep;
    private int _curGrowthStep;
    private int _curMaxMoisture;
    private int _curMaxNutrient;

    public E_CropState CurState { get { return _curState; } }
    public float Value {  get { return _value; } }
    public int DigCount {  get { return _digCount; } }

    private void Awake()
    {
        Transform GFX = transform.GetChild(0);
        _GFXs = new GameObject[GFX.childCount];
        for (int i = 0; i < GFX.childCount; i++)
        {
            _GFXs[i] = GFX.GetChild(i).gameObject;
        }
        _maxGrowthStep = _GFXs.Length - 1;
        _curGrowthStep = 0;

        _value = 1f;
        _curMaxMoisture = _idleMaxMoisture;
        _curMaxNutrient = _idleMaxNutrient;

        _states[(int)E_CropState.Seeding] = new SeedingState(this);
        _states[(int)E_CropState.Growing] = new GrowingState(this);
        _states[(int)E_CropState.GrowStopped] = new GrowStoppedState(this);
        _states[(int)E_CropState.GrowCompleted] = new GrowCompletedState(this);
        _states[(int)E_CropState.Waste] = new WasteState(this);

        _cropInteractable = GetComponent<CropInteractable>();
    }

    private void Start()
    {
        _eventManager = GameObject.FindGameObjectWithTag("EventManager").GetComponent<EventManager>();
    }

    private void OnEnable()
    {
        Init();
    }

    private void Update()
    {
        _states[(int)_curState].StateUpdate();
    }

    private void OnDestroy()
    {
        _states[(int)_curState].StateExit();
    }

    public void UpdateData()
    {
        _ID = _cropData.ID;
        _name = _cropData.cropName;
        _digCount = _cropData.digCount;
        _idleMaxMoisture = _cropData.maxMoisture;
        _idleMaxNutrient = _cropData.maxNutrient;
        _growthTime = _cropData.growthTime;
        _droughtMaxMoisture = _cropData.droughtMaxMoisture;
        _droughtMaxNutrient = _cropData.droughtMaxNutrient;
        _damageRate = _cropData.damageRate;
        _damageLimitTime = _cropData.damageLimitTime;
        _temperatureDecreaseLimitTime = _cropData.temperatureDecreaseLimitTime;
        _temperatureIncreaseLimitTime = _cropData.temperatureIncreaseLimitTime;
    }

    private void Init()
    {
        _GFXs[0].SetActive(true);
        for (int i = 1; i < _GFXs.Length; i++)
            _GFXs[i].SetActive(false);

        _curState = E_CropState.Seeding;
        _states[(int)_curState].StateEnter();

        _elapsedTime = 0.0f;
        _curMoisture = 0;
        _curNutrient = 0;

        UpdateData();
    }

    [PunRPC]
    public void ChangeState(E_CropState state)
    {
        _states[(int)_curState].StateExit();
        _curState = state;
        _states[(int)_curState].StateEnter();
    }

    public void IncreaseMoisture() => _curMoisture++;
    public void IncreaseNutrient() => _curNutrient++;

    private bool CheckGrowthCondition()
    {
        return CheckMoisture() && CheckNutrient();
    }

    private bool CheckMoisture() => _curMoisture >= _curMaxMoisture;
    private bool CheckNutrient() => _curNutrient >= _curMaxNutrient;


    #region 돌발이벤트 반응함수
    public void OnDownpourStarted()
    {
        photonView.RPC(nameof(ChangeState), RpcTarget.All, E_CropState.GrowStopped);
    }

    public void OnDownpourEnded()
    {
        photonView.RPC(nameof(ChangeState), RpcTarget.All, E_CropState.Growing);
    }

    public void OnBlightStarted()
    {
        if (_blightCoroutine == null)
        {
            _blightCoroutine = StartCoroutine(BlightRoutine());
        }
    }

    public void OnBlightEnded()
    {
        if (_blightCoroutine != null)
        {
            StopCoroutine(_blightCoroutine);
            _blightCoroutine = null;
        }
    }

    Coroutine _blightCoroutine;
    IEnumerator BlightRoutine()
    {
        yield return new WaitForSeconds(_damageLimitTime);

        _value *= _damageRate;

        yield return null;
    }

    public void OnDroughtStarted()
    {
        _curMaxMoisture = _droughtMaxMoisture;
    }

    public void OnDroughtEnded()
    {
        _curMaxNutrient = _droughtMaxNutrient;
    }

    public void OnHighTemperatureStarted()
    {
        if (_temperatureCoroutine == null)
        {
            _temperatureCoroutine = StartCoroutine(TemperatureRoutine(true));
        }
    }

    public void OnHighTemperatureEnded()
    {
        if (_temperatureCoroutine != null)
        {
            StopCoroutine(_temperatureCoroutine);
            _temperatureCoroutine = null;
        }
    }

    public void OnLowTemperatureStarted()
    {
        if (_temperatureCoroutine == null)
        {
            _temperatureCoroutine = StartCoroutine(TemperatureRoutine(false));
        }
    }

    public void OnLowTemperatureEnded()
    {
        if (_temperatureCoroutine != null)
        {
            StopCoroutine(_temperatureCoroutine);
            _temperatureCoroutine = null;
        }
    }

    Coroutine _temperatureCoroutine;
    IEnumerator TemperatureRoutine(bool isHigh)
    {
        yield return new WaitForSeconds(isHigh ? _temperatureIncreaseLimitTime : _temperatureDecreaseLimitTime);

        (_states[(int)E_CropState.Waste] as WasteState).isHigh = isHigh;
        photonView.RPC(nameof(ChangeState), RpcTarget.All, E_CropState.Waste);

        yield return null;
    }
    #endregion

    #region 작물 상태 행동 및 전이
    private class CropState : BaseState
    {
        public Crop crop;
        public CropState(Crop crop) => this.crop = crop;

        public override void StateEnter()
        {
            throw new System.NotImplementedException();
        }

        public override void StateExit()
        {
            throw new System.NotImplementedException();
        }

        public override void StateUpdate()
        {
            throw new System.NotImplementedException();
        }
    }

    private class SeedingState : CropState
    {
        public SeedingState(Crop crop) : base(crop) { }

        public override void StateEnter() { }
        public override void StateExit() { }
        public override void StateUpdate() { }
    }

    private class GrowingState : CropState
    {
        public GrowingState(Crop crop) : base(crop) { }

        public override void StateEnter() { }

        public override void StateExit() { }

        public override void StateUpdate()
        {
            // 행동
            Grow();

            // 상태 전이
            if (crop._elapsedTime >= crop._growthTime)
            {
                crop.photonView.RPC(nameof(crop.ChangeState), RpcTarget.All, E_CropState.GrowCompleted);
            }

            if (!crop.CheckGrowthCondition())
            {
                crop.photonView.RPC(nameof(crop.ChangeState), RpcTarget.All, E_CropState.GrowStopped);
            }
        }

        private void Grow()
        {
            // 성장 시간 누적
            crop._elapsedTime += Time.deltaTime;

            // 성장치에 따른 외형 변화
            if (crop._elapsedTime >= crop._growthTime * (crop._curGrowthStep + 1) / crop._maxGrowthStep)
            {
                crop._GFXs[crop._curGrowthStep].SetActive(false);
                crop._GFXs[++crop._curGrowthStep].SetActive(true);
            }
        }
    }

    private class GrowStoppedState : CropState
    {
        public GrowStoppedState(Crop crop) : base(crop) { }

        public override void StateEnter() { }

        public override void StateExit() { }

        public override void StateUpdate()
        {
            // 상태 전이
            if (crop.CheckGrowthCondition())
            {
                crop.photonView.RPC(nameof(crop.ChangeState), RpcTarget.All, E_CropState.Growing);
            }
        }
    }

    private class GrowCompletedState : CropState
    {
        public GrowCompletedState(Crop crop) : base(crop) { }

        public override void StateEnter()
        {
            crop._GFXs[crop._maxGrowthStep - 1].SetActive(false);
            crop._GFXs[crop._maxGrowthStep].SetActive(true);
        }

        public override void StateExit() { }

        public override void StateUpdate() { }
    }

    private class WasteState : CropState
    {
        public bool isHigh;

        public WasteState(Crop crop) : base(crop) { }

        public override void StateEnter()
        {
            if (isHigh)
            {
                crop._GFXs[crop._curGrowthStep].GetComponent<Renderer>().material.color = Color.black;
            }
            else
            {
                crop._GFXs[crop._curGrowthStep].GetComponent<Renderer>().material.color = Color.cyan;
            }
        }

        public override void StateExit() { }

        public override void StateUpdate() { }
    }
    #endregion

    #region TestCode
    public void CompleteGrowth()
    {
        ChangeState(E_CropState.GrowCompleted);
    }
    #endregion
}
