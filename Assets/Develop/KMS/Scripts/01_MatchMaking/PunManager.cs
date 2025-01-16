using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using Fusion;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PunManager : MonoBehaviourPunCallbacks
{
    [Tooltip("테스트를 위한 방 넘버 설정.")]
    public int RoomNum = 0;
    public int maxPlayer;

    [Tooltip("스테이지 ID")]
    public int selectedStage = (int)E_StageMode.Stage1;
    public string roomName;

    [Tooltip("대기방 씬 이름")]
    public string waittingRoomName = "04_Waiting Room";

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
    private E_GameMode _gameMode = E_GameMode.Normal;

    public static PunManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// FirebaseManager가 초기화 완료되면 ConnectToPhoton() 호출.
    /// VR의 특성상 키보드를 사용하기에 어려움이 있다고 생각이 들어서
    /// 게임이 시작하자마자 바로 로그인을 진행.
    /// </summary>
    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        // 호출의 순서가 FirebaseManager가 초기화 완료되고 나서
        // ConnectToPhoton() 호출해야하기에 이벤트로 연결.
        Debug.Log("Firebase 이벤트 등록");
        FirebaseManager.Instance.OnFirebaseInitialized += ConnectToPhoton;
        //FirebaseManager.Instance.NotifyInitializationComplete();
    }

    /// <summary>
    /// Photon서버에 연결하는 메서드.
    /// 
    /// UserId가 중복으로 방에 입장시 네트워크에서 두번째로 들어온 플레이어는 입장을 제한한다.
    /// 고유의 Id이기에 이에 대해서 네트워크에서 배제가 되는 현상인듯하다.
    /// (로비까지는 들어가지지만 방에는 입장이 안된다.)
    /// UUID를 이용해서도 시도했는데 해당 부분의 기기의 고유번호인 IMEI가 노출되는 보안상 위험이 있기에
    /// Firebase의 익명 로그인으로 대체하기로 함.
    /// </summary>
    private void ConnectToPhoton()
    {
        string userId = FirebaseManager.Instance.GetUserId();

        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("UserId를 이용할 수 없다..포톤 연결 실패!.");
            return;
        }

        // PhotonNetwork에서 고유의 UserID를 가져와서 인증을 받음.
        // 테스트시에는 userId를 불러올시 ParrelSync가 동작이 안되기에 Random.Range로 진행.
        PhotonNetwork.AuthValues = new AuthenticationValues { UserId = userId /*Random.Range(1000, 10000).ToString() */ };

        Debug.Log($"ConnectToPhoton {userId}");
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("포톤에 연결 중...");
    }

    /// <summary>
    /// Photon에 연결되자마자 로비로 이동.
    /// </summary>
    public override void OnConnectedToMaster()
    {
        if (!PhotonNetwork.InLobby)
        {
            Debug.Log("0. Photon Master Server와 연결!");
            PhotonNetwork.LocalPlayer.NickName = FirebaseManager.Instance.GetNickName();
            PhotonNetwork.JoinLobby();
        }
        else
        {
            Debug.Log("Master Server 로비에 연결 중!");
        }
    }

    /// <summary>
    /// 로비입장 메서드.
    /// </summary>
    public override void OnJoinedLobby()
    {
        Debug.Log("1. PUN 로비 입장!");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        if (SceneManager.GetActiveScene().name != "03_Lobby" && PhotonNetwork.InLobby)
        {
            Debug.Log("로딩 씬으로 이동...");
            SceneLoader.LoadSceneWithLoading("03_Lobby");
        }
    }

    /// <summary>
    /// Fusion 네트워크 종료 및 Pun 방 생성 후 이동
    /// </summary>
    public void CreateAndMoveToPunRoom()
    {
        Debug.Log("방생성 시작!");

        // 5초 카운트다운 및 방 생성 시작
        StartCoroutine(PunRoomCountdown(5f));
    }

    public void SetGameMode(E_GameMode eGameMode)
    {
        // 현재는 게임 모드는 Normal만 존재하기에
        if (eGameMode != E_GameMode.Normal)
        {
            Debug.LogWarning($"선택된 {eGameMode} 모드는 아직 없는 모드입니다. \nNormal 모드로 변경 합니다.");
            eGameMode = E_GameMode.Normal;
        }
        _gameMode = eGameMode;
    }

    public string GetGameMode() { return _gameMode.ToString(); }

    public void SetStageNumber(E_StageMode eStageMode)
    {
        // 현재는 게임 모드는 Normal만 존재하기에
        if (eStageMode == E_StageMode.None || eStageMode == E_StageMode.SIZE_MAX)
        {
            Debug.LogWarning($"선택된 {eStageMode}은 잘못 입력 되었습니다. \nStage 1 로 변경 합니다.");
            eStageMode = E_StageMode.Stage1;
        }
        selectedStage = (int)eStageMode;
    }

    public string GetStageNumber() 
    {
        return ((E_StageMode)selectedStage).ToString(); 
    }

    public void SetRoomName(string sRoomName)
    {
        roomName = sRoomName;
        Debug.Log($"방 이름은 {roomName} \n방 모드{_gameMode.ToString()} \n스테이지 번호{selectedStage}");
    }

    /// <summary>
    /// Coroutine으로 5초 동안 카운트다운 메시지를 갱신하고 방 생성 및 이동
    /// </summary>
    private IEnumerator PunRoomCountdown(float countdown)
    {
        float remainingTime = countdown;

        while (remainingTime > 0)
        {
            // 메시지 갱신
            MessageDisplayManager.Instance.ShowMessage($"{(int)remainingTime} 초 후 , 방에 입장 합니다..", 1f, 3f);
            Debug.Log($"After {(int)remainingTime} seconds, you enter the room.");
            yield return new WaitForSeconds(1f);
            remainingTime--;
        }

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = maxPlayer,
            IsVisible = true,
            IsOpen = true,
            CustomRoomProperties = new PhotonHashtable
            {
                { "gameMode", _gameMode},
                { "selectedStage", selectedStage }
            },
            CustomRoomPropertiesForLobby = new string[] { "gameMode", "selectedStage" }
        };

        Debug.Log("방 생성 시도 중...");
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"방 생성 성공!");
    }

    /// <summary>
    /// 방 입장 성공
    /// </summary>
    public override void OnJoinedRoom()
    {
        // Pun 이동
        Debug.Log($"방 입장 성공: {PhotonNetwork.CurrentRoom.Name}");
        PhotonNetwork.AutomaticallySyncScene = true;

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out object gameModeValue))
        {
            _gameMode = (E_GameMode)gameModeValue;
            Debug.Log($"방의 gameMode 값 동기화: {_gameMode}");
        }

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("selectedStage", out object stageValue))
        {
            selectedStage = (int)stageValue;
            Debug.Log($"방의 selectedStage 값 동기화: {selectedStage}");
        }

        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        SceneManager.LoadScene("04_Waiting Room"); // 대기실 씬으로 이동
    }

    /// <summary>
    /// 방 목록 업데이트
    /// </summary>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
            {
                // 삭제된 방 제거
                cachedRoomList.Remove(room.Name);
            }
            else
            {
                // 추가 또는 갱신된 방 업데이트
                cachedRoomList[room.Name] = room;
            }
        }
    }

    /// <summary>
    /// 방 입장 실패 관련 메서드.
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("방 입장 실패: " + message);
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Pun 방을 나갔습니다. 게임서버에서 마스터 서버로 교체!");
    }

    /// <summary>
    /// 방 목록 가져오기.
    /// </summary>
    /// <returns></returns>
    public List<RoomInfo> GetRoomList()
    {
        return new List<RoomInfo>(cachedRoomList.Values);
    }

    /// <summary>
    /// 방제로 방 입장.
    /// </summary>
    /// <param name="roomName"></param>
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
}
