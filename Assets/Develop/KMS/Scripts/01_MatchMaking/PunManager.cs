using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PunManager : MonoBehaviourPunCallbacks
{
    [Tooltip("테스트를 위한 방 넘버 설정.")]
    public int RoomNum = 0;

    // 버튼 프리팹 동적 참조
    public GameObject RoomMakeButtonPrefab;
    private GameObject instantiatedRoomMakeButton;

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

        // Resources 폴더에서 프리팹 동적 로드
        RoomMakeButtonPrefab = Resources.Load<GameObject>("Room Make Button");

        if (!RoomMakeButtonPrefab)
        {
            Debug.LogError("RoomMakeButtonPrefab이 Resources 폴더에 없습니다!");
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
        FirebaseManager.Instance.OnFirebaseInitialized += ConnectToPhoton;
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
        PhotonNetwork.AuthValues = new AuthenticationValues { UserId = /*userId*/ Random.Range(1000, 10000).ToString() };

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
        if (SceneManager.GetActiveScene().name != "03_FusionLobby" && PhotonNetwork.InLobby)
        {
            Debug.Log("로딩 씬으로 이동...");
            SceneManager.LoadScene("LoadingScene");
        }
    }

    /// <summary>
    /// 방 생성 및 방 목록 보기 버튼 생성
    /// </summary>
    public void CreateDynamicButtons()
    {
        Debug.Log("버튼 생성");
        Canvas canvas = FindObjectOfType<Canvas>();

        if (!canvas)
        {
            Debug.LogError("Canvas가 씬에 존재하지 않습니다!");
            return;
        }

        // 기존 버튼 삭제
        if (instantiatedRoomMakeButton)
        {
            Destroy(instantiatedRoomMakeButton);
        }

        // 방 생성 버튼 생성
        if (RoomMakeButtonPrefab)
        {
            instantiatedRoomMakeButton = Instantiate(RoomMakeButtonPrefab, canvas.transform);
            Button makeButton = instantiatedRoomMakeButton.GetComponent<Button>();
            if (makeButton != null)
            {
                TMP_Text buttonText = instantiatedRoomMakeButton.GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                {
                    buttonText.text = "Create Room";
                }

                makeButton.onClick.AddListener(() =>
                {
                    Debug.Log("Room Make 버튼 클릭됨!");
                    CreateAndMoveToPunRoom();
                });
            }
        }
    }

    /// <summary>
    /// Fusion 네트워크 종료 및 Pun 방 생성 후 이동
    /// </summary>
    public void CreateAndMoveToPunRoom()
    {
        // Pun 방 생성
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = true,
            IsOpen = true
        };

        PhotonNetwork.JoinOrCreateRoom($"PunRoom_{RoomNum}", roomOptions, TypedLobby.Default);
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
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        PhotonNetwork.LoadLevel("04_PunWaitingRoom"); // 대기실 씬으로 이동
    }

    /// <summary>
    /// 방 목록 업데이트
    /// </summary>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            Debug.Log($"방 이름: {room.Name}, 플레이어: {room.PlayerCount}/{room.MaxPlayers}");
            Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
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
}
