using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR;

public class Intro : MonoBehaviour
{
    [Header("게임 스타트 오브젝트 세팅")]
    public GameObject introPanel;
    public TMP_Text introText;
    public Image skipGauge;

    [Header("영상 세팅")]
    [Header("VideoPlayer가 붙어 있는 GameObject")]
    public GameObject videoPlayerObject;
    public GameObject rawImage;
    [Tooltip("비디오 화면이 HMD에서 얼마나 떨어져 있는지 설정")]
    public float videoDistance = 2.0f;
    private VideoPlayer videoPlayer;
    
    [Header("닉네임 InputField 세팅")]
    public GameObject nickNameInputField;

    private bool _isButtonPressed = false;

    [Header("게임 스타트 Skip 세팅")]
    [Tooltip("버튼 누른 시간")]
    [SerializeField] private float _buttonPressDuration = 0.0f;
    [Tooltip("버튼을 눌러야 하는 시간")]
    [SerializeField] private float _requiredHoldTime = 2.0f;
    [Tooltip("Firebase 접속 이력 여부")]
    [SerializeField] private bool _isFirebaseUser = false;
    [Tooltip("영상 재생 확인 여부")]
    [SerializeField] private bool _isVideoPlaying = false;


    private void Start()
    {
        introPanel.SetActive(true);
        introText.text = "왼쪽 컨트롤러의 A 키를 눌러주세요";

        if (videoPlayerObject)
        {
            videoPlayer = videoPlayerObject.GetComponent<VideoPlayer>();
            videoPlayerObject.SetActive(false);
            rawImage.SetActive(false);
            videoPlayer.loopPointReached += OnVideoEnd;
        }

        CheckPlayUser();

        if (skipGauge)
        {
            skipGauge.fillAmount = 0.0f;
        }
    }

    private void Update()
    {
        InputDevice rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // 버튼이 눌린 상태인지 확인

        // A key
        if (rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool isAPressed)) // A 버튼 입력
        {
            if (isAPressed && !_isVideoPlaying && !nickNameInputField.activeSelf)
            {
                introText.text = "";
                introPanel.transform.position = new Vector3(introPanel.transform.position.x, introPanel.transform.position.y - 1, introPanel.transform.position.z);
                PlayVideo();
            }
        }

        // B key
        if (rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isPressed))
        {
            if (isPressed) // 눌림이 시작되었을 때만 처리
            {
                _buttonPressDuration += Time.deltaTime;

                if(FirebaseManager.Instance.GetNickName() != "")
                    UpdateSkipGauge();

                if (!_isButtonPressed)
                {
                    _isButtonPressed = true;
                }

                if (_buttonPressDuration >= _requiredHoldTime && FirebaseManager.Instance.GetNickName() != "")
                {
                    SkipToLobby();
                }
            }
            else if (!isPressed) // 버튼이 떼어졌을 때 상태 초기화
            {
                _isButtonPressed = false;
                _buttonPressDuration = 0f;
                skipGauge.fillAmount = 0f;
            }
        }

        if (videoPlayerObject.activeSelf)
        {
            FixHMD();

            if (FirebaseManager.Instance.GetNickName() != "")
                introText.text = $"{FirebaseManager.Instance.GetNickName()}님 왼쪽 컨트롤러의 B키를 1초동안 누르고 계시면 Skip이 가능합니다.";
        }


    }

    private void PlayVideo()
    {
#if UNITY_EDITOR
        Debug.Log("A키가 눌렸습니다. 영상을 재생합니다.");
#endif
        if (videoPlayerObject != null)
        {
            rawImage.SetActive(true);
            videoPlayerObject.SetActive(true);
            videoPlayer.Play();
            _isVideoPlaying = true;
            SoundManager.Instance.PlayBGM("Intro", 0.4f);
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
#if UNITY_EDITOR
        Debug.Log("MP4 영상 재생 완료.");
#endif
        if (videoPlayerObject)
        {
            introText.gameObject.SetActive(false);
            rawImage.SetActive(false);
            videoPlayerObject.SetActive(false);
        }

        if (nickNameInputField)
        {
            nickNameInputField.SetActive(true);
            nickNameInputField.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0.5f);
        }

        _isVideoPlaying = false;
    }

    private void FixHMD()
    {
        if (videoPlayerObject)
        {
            // HMD 위치를 기준으로 비디오 화면의 위치를 설정
            Vector3 cameraPosition = Camera.main.transform.position;
            Quaternion cameraRotation = Camera.main.transform.rotation;

            // 비디오 화면을 HMD 앞에 고정
            Vector3 offsetPosition = cameraPosition + cameraRotation * Vector3.forward * videoDistance;
            videoPlayerObject.transform.position = offsetPosition;

            // HMD 방향으로 화면이 항상 바라보도록 설정
            videoPlayerObject.transform.rotation = Quaternion.LookRotation(videoPlayerObject.transform.position - cameraPosition);
        }
    }

    private void SkipToLobby()
    {
        Debug.Log("한번 접속한 유저의 로비 씬으로 이동!");
        MessageDisplayManager.Instance.ShowMessage($"{FirebaseManager.Instance.GetNickName()}님 다시 접속해주셔서 감사합니다.", 0.5f, 3f);

        // 영상 재생 중이라면 중단 및 정리
        if (videoPlayer && videoPlayer.isPlaying)
        {
            introText.gameObject.SetActive(false);
            Debug.Log("영상 재생 중단 및 정리...");
            videoPlayer.Stop(); // 재생 중단
            SoundManager.Instance.StopBGM();
            rawImage.SetActive(false);
            videoPlayerObject.SetActive(false); // 오브젝트 비활성화
        }

        SceneLoader.LoadSceneWithLoading("03_Lobby");
    }

    private void UpdateSkipGauge()
    {
        skipGauge.fillAmount = Mathf.Clamp01(_buttonPressDuration / _requiredHoldTime);
    }

    private void CheckPlayUser()
    {
        string userId = FirebaseManager.Instance?.GetUserId();

        if (!string.IsNullOrEmpty(userId))
        {
            Debug.Log($"Firebase 유저 확인 완료: {userId}");
            _isFirebaseUser = true;
        }
        else
        {
            Debug.Log("Firebase 유저가 아닙니다.");
            _isFirebaseUser = false;
        }
    }
}
