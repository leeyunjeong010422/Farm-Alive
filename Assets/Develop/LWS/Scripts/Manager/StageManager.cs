using System.Collections;
using Photon.Pun;
using UnityEngine;
using GameData;
using UnityEngine.Events;

public class StageManager : MonoBehaviourPunCallbacks
{
    [SerializeField] int _curStageID;

    private bool _isMapSetted = false;
    public UnityEvent OnGameStarted;

    [Header("스테이지 시간 속성")]
    [SerializeField] float _stageTimeLimit = 0;
    [SerializeField] float _curStageTime = 0;
    public float CurStageTime { get { if (PhotonNetwork.IsMasterClient) return _curStageTime; else return 0f; } }
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

    [Tooltip("소환할 플레이어 프리팹")]
    public GameObject newcharacterPrefab;
    public Vector3 PlayerSpawn;

    // 계절별 파티클 / 오브젝트 등.
    [Tooltip("계절별 스카이박스 메테리얼 (순서대로 계절)")]
    [SerializeField] Material[] _materials;
    //
    //

    public static StageManager Instance { get; private set; }

    private void Awake()
    {
        _curStageID = PunManager.Instance.selectedStage;

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

        _stageTimeLimit = 400f;


        while (!ParticleManager.Instance.isAllParticleStoped)
            yield return null; // 모든 파티클 정보 저장, 재생 중단 후 진행

        SetSeason();

        while (!_isMapSetted)
            yield return null; // 맵 세팅 후 진행

        SpawnPlayer();

        yield return new WaitForSeconds(5f);

        OnGameStarted?.Invoke();
        
        yield return new WaitForSeconds(5f);
        
        if (PhotonNetwork.IsMasterClient)
            StartStageTimer();
    }

    private void Update()
    {
        if (!_isTimerRunning)
            return;

        _curStageTime += Time.deltaTime;

        while (_stageTimeLimit - _curStageTime <= 60f)
        {
            SoundManager.Instance.PlaySFXLoop("BGM_StageOneMinute");

            if (_stageTimeLimit == _curStageTime)
                SoundManager.Instance.StopSFXLoop("BGM_StageOneMinute");
                break;
        }

        if (_stageTimeLimit > 0 && _curStageTime >= _stageTimeLimit)
        {
            photonView.RPC(nameof(EndStage), RpcTarget.All);
        }
    }

    private void SetSeason()
    {
        // 맵별 파티클 setactive false

        switch (_weatherID)
        {
            case 0: // 봄
                 RenderSettings.skybox = _materials[0];
                break;
            case 1: // 여름
                 RenderSettings.skybox = _materials[1];
                break;
            case 2: // 가을
                 RenderSettings.skybox = _materials[2];
                break;
            case 3: // 겨울
                 RenderSettings.skybox = _materials[3];
                break;
        }

        _isMapSetted = true;
    }

    private void SpawnPlayer()
    {
        GameObject Player = PhotonNetwork.Instantiate(newcharacterPrefab.name, PlayerSpawn, Quaternion.identity);
    }

    public void StartStageTimer()
    {
        QuestManager.Instance.FirstStart(_curStageID);
        SoundManager.Instance.PlayBGM(_curStageID.ToString());
        Debug.Log($"{_curStageID.ToString()} BGM 시작!");
        _curStageTime = 0f;
        _isTimerRunning = true;
    }

    [PunRPC]
    /// <summary>
    /// 퀘스트가 모두 종료되었을 때, 호출할 함수.
    /// </summary>
    public void EndStage()
    {
        _isTimerRunning = false;

        int star = EvaluateStar();
        float playTime = _curStageTime;

        FirebaseManager.Instance.SaveStageResult(_curStageID, _curStageTime, star);

        StartCoroutine(ReturnToFusion());
    }

    public IEnumerator ReturnToFusion()
    {
        yield return new WaitForSeconds(3f);

        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Pun 방 나가기...");
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            Debug.LogWarning($"현재 상태에서는 LeaveRoom을 호출할 수 없습니다: {PhotonNetwork.NetworkClientState}");
        }


        Debug.Log("서버 교체 중...");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"PhotonNetwork.NetworkClientState = {PhotonNetwork.NetworkClientState}");
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