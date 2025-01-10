using System.Collections;
using Photon.Pun;
using UnityEngine;
using GameData;

public class StageManager : MonoBehaviourPunCallbacks
{
    [SerializeField] int _curStageID;

    [Header("스테이지 시간 속성")]
    [SerializeField] float _stageTimeLimit = 0;
    [SerializeField] float _curStageTime = 0;
    [SerializeField] bool _isTimerRunning = false;

    private STAGE _curStageData;
    private int _weatherID;
    public int WeatherID { get { return _weatherID; } }

    private int _maxBrokenMachineCount;
    private int _maxDamagedCropCount = 0; // 0으로 설정 (현재 데이터 테이블에서는 성공여부만 따짐)

    // 기계가 고장난 횟수 (다른곳에서 고장나면 ++필요)
    public int brokenMachineCount = 0;
    // 작물이 손상된 횟수 (다른곳에서 손상되면 ++필요)
    public int damagedCropCount = 0;

    // 계절별 파티클 / 오브젝트 등.
    // 
    //
    //

    public static StageManager Instance { get; private set; }

    private void Awake()
    {
        //_curStageID = PunManager.Instance.selectedStage;

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private IEnumerator Start()
    {
        // CSV 다운로드 끝날 때까지 대기
        while (!CSVManager.Instance.downloadCheck)
            yield return null;

        var stageDict = CSVManager.Instance.Stages;

        _curStageData = stageDict[_curStageID];
        _weatherID = _curStageData.stage_seasonID;
        _maxBrokenMachineCount = _curStageData.stage_allowSymptomFacilityCount;

        _stageTimeLimit = 360f;

        StartStageTimer();
    }

    private void Update()
    {
        if (!_isTimerRunning)
            return;

        _curStageTime += Time.deltaTime;

        if (_stageTimeLimit > 0 && _curStageTime >= _stageTimeLimit)
        {
            EndStage();
        }
    }

    private void SetSeason()
    {
        // 맵별 파티클 setactive false

        switch (_weatherID)
        {
            case 0: // 봄
                break;
            case 1: // 여름
                break;
            case 2: // 가을
                break;
            case 3: // 겨울
                break;
        }
    }

    private void SpawnPlayer()
    {
        // GameObject Player = PhotonNetwork.Instantiate(newcharacterPrefab.name, PlayerSpawn, Quaternion.identity);
    }

    public void StartStageTimer()
    {
        // QuestManager.Instance.FirstStart(_curStageID);

        _curStageTime = 0f;
        _isTimerRunning = true;
    }

    /// <summary>
    /// 퀘스트가 모두 종료되었을 때, 호출할 함수.
    /// </summary>
    public void EndStage()
    {
        _isTimerRunning = false;

        int star = EvaluateStar();
        float playTime = _curStageTime;

        FirebaseManager.Instance.SaveStageResult(_curStageID, _curStageTime, star);
    }

    private int EvaluateStar()
    {
        int star = 0;

        int successCount = QuestManager.Instance.clearQuestCount;
        int totalDealer = QuestManager.Instance.totalQuestCount;

        if (totalDealer == 3)
        {
            if (successCount == 3) star = 3;
            else if (successCount == 2) star = 2;
            else if (successCount == 1) star = 1;
            else star = 0;
        }
        else if (totalDealer == 2)
        {
            if (successCount == 2) star = 3;
            else if (successCount == 1) star = 1;
            else star = 0;
        }
        else if (totalDealer == 1)
        {
            // 1개 스테이지 => (1=>3star, 0=>0star)
            if (successCount == 1) star = 3;
            else star = 0;
        }

        if (damagedCropCount <= _maxDamagedCropCount)
            star += 1;

        if (brokenMachineCount <= _maxBrokenMachineCount)
            star += 1;

        // 최대 5개
        if (star > 5) star = 5;
        return star;
    }
}