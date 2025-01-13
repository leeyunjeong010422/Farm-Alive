using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR;

public class GameStartManager : MonoBehaviour
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

    private int _currentStep = 0;
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

    [SerializeField] private string[] _gameInstructions = new string[]
    {
        "Press B key",
        "한번이라도 접속 하셨다면 \n B key를 1초동안 눌러주세요. \nIf you have logged in even once, \npress the B key for 1 seconds. ",
        "10",
        "9",
        "8",
        "7",
        "6",
        "5",
        "4",
        "3",
        "2",
        "1",
        "Next stage in Press B Button..."
    };

    private void Start()
    {
        introPanel.SetActive(true);
        introText.text = _gameInstructions[_currentStep];

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
            if (isAPressed && !_isVideoPlaying)
            {
                PlayVideo();
            }
        }

        // B key
        if (rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isPressed))
        {
            if (isPressed) // 눌림이 시작되었을 때만 처리
            {
                _buttonPressDuration += Time.deltaTime;

                if(_isFirebaseUser)
                    UpdateSkipGauge();

                if (!_isButtonPressed)
                {
                    _isButtonPressed = true; // 버튼 눌림 상태 기록
                    ShowNextInstruction();
                }

                if (_buttonPressDuration >= _requiredHoldTime && _isFirebaseUser)
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

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.A) && !_isVideoPlaying)
        {
            PlayVideo();
        }

        if (Input.GetKey(KeyCode.B))
        {
            isPressed = true;
            if (isPressed)
            {
                _buttonPressDuration += Time.deltaTime;

                if (_isFirebaseUser)
                    UpdateSkipGauge();

                if (!_isButtonPressed)
                {
                    _isButtonPressed = true;
                    ShowNextInstruction();
                }

                if (_buttonPressDuration >= _requiredHoldTime && _isFirebaseUser)
                {
                    SkipToLobby();
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.B))
        {
            isPressed = false;
            if (!isPressed)
            {
                _isButtonPressed = false;
                _buttonPressDuration = 0f;
                skipGauge.fillAmount = 0f;
            }
        }
#endif

        if (videoPlayerObject.activeSelf)
        {
            FollowHMD();
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
        }

        // 안내 텍스트 숨기기
        introPanel.SetActive(false);
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
#if UNITY_EDITOR
        Debug.Log("MP4 영상 재생 완료.");
#endif
        if (videoPlayerObject)
        {
            rawImage.SetActive(false);
            videoPlayerObject.SetActive(false);
        }

        if (nickNameInputField)
        {
            nickNameInputField.SetActive(true);
        }

        _isVideoPlaying = false;
    }

    // HMD를 따라오도록 설정
    private void FollowHMD()
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

    private void ShowNextInstruction()
    {
        _currentStep++;
        if (_currentStep < _gameInstructions.Length)
        {
            introText.text = _gameInstructions[_currentStep];
        }
        else
        {
            Debug.Log("튜토리얼 씬으로 이동.");
            SceneManager.LoadScene("02_Tutorial");
        }
    }

    private void SkipToLobby()
    {
        Debug.Log("한번 접속한 유저의 로비 씬으로 이동!");

        // 영상 재생 중이라면 중단 및 정리
        if (videoPlayer && videoPlayer.isPlaying)
        {
            Debug.Log("영상 재생 중단 및 정리...");
            videoPlayer.Stop(); // 재생 중단
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
