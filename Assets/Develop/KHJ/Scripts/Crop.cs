using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Crop : MonoBehaviourPun
{
    public enum E_CropState
    {
        Seeding, Growing, GrowStopped, GrowCompleted, SIZE
    }

    [Header("작물 외형")]
    [Tooltip("성장 단계에 따라 변화하는 외형")]
    [SerializeField] private GameObject[] _GFXs;

    [Header("작물의 현재 상태")]
    [SerializeField] private E_CropState _curState;

    [Header("수치")]
    [Tooltip("밭에 심어지기 위해 밭이 경작돼야하는 횟수")]
    [SerializeField] private int _digCount;
    [Tooltip("수확가능 상태로 변경될 때까지 성장가능 상태에서 머물러야 하는 시간")]
    [SerializeField] private float _growthTime;
    [Tooltip("성장가능 상태에서 머무른 시간")]
    [SerializeField] private float _elapsedTime;
    [Tooltip("성장가능 상태가 되기 위해 필요한 수분")]
    [SerializeField] private int _maxMoisture;
    [Tooltip("작물의 현재 수분")]
    [SerializeField] private int _curMoisture;
    [Tooltip("성장가능 상태가 되기 위해 필요한 영양분")]
    [SerializeField] private int _maxNutrient;
    [Tooltip("작물의 현재 영양분")]
    [SerializeField] private int _curNutrient;

    private BaseState[] _states = new BaseState[(int)E_CropState.SIZE];
    private CropInteractable _cropInteractable;
    private int _maxGrowthStep;

    public E_CropState CurState { get { return _curState; } }
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

        _states[(int)E_CropState.Seeding] = new SeedingState(this);
        _states[(int)E_CropState.Growing] = new GrowingState(this);
        _states[(int)E_CropState.GrowStopped] = new GrowStoppedState(this);
        _states[(int)E_CropState.GrowCompleted] = new GrowCompletedState(this);

        _cropInteractable = GetComponent<CropInteractable>();
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
        private int curGrowthStep = 0;

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
        }

        private void Grow()
        {
            // 성장 시간 누적
            crop._elapsedTime += Time.deltaTime;

            // 성장치에 따른 외형 변화
            if (crop._elapsedTime >= crop._growthTime * (curGrowthStep + 1) / crop._maxGrowthStep)
            {
                crop._GFXs[curGrowthStep].SetActive(false);
                crop._GFXs[++curGrowthStep].SetActive(true);
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
            if (CheckGrowthCondition())
            {
                crop.photonView.RPC(nameof(crop.ChangeState), RpcTarget.All, E_CropState.Growing);
            }
        }

        private bool CheckGrowthCondition()
        {
            return CheckMoisture() && CheckNutrient();
        }

        private bool CheckMoisture() => crop._curMoisture >= crop._maxMoisture;
        private bool CheckNutrient() => crop._curNutrient >= crop._maxNutrient;
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
    #endregion

    #region TestCode
    public void CompleteGrowth()
    {
        ChangeState(E_CropState.GrowCompleted);
    }
    #endregion
}
