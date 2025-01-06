using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

public class GameStartManager : MonoBehaviour
{
    [Header("게임 스타트 오브젝트 세팅")]
    public GameObject introPanel;
    public TMP_Text introText;
    public Image skipGauge;

    private int _currentStep = 0;
    private bool _isButtonPressed = false;

    [Header("게임 스타트 Skip 세팅")]
    [Tooltip("버튼 누른 시간")]
    [SerializeField] private float _buttonPressDuration = 0.0f;
    [Tooltip("버튼을 눌러야 하는 시간")]
    [SerializeField] private float _requiredHoldTime = 2.0f;
    [Tooltip("Firebase 접속 이력 여부")]
    [SerializeField] private bool _isFirebaseUser = false;


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

#if UNITY_INCLUDE_TESTS
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
            SceneManager.LoadScene("02_TutorialScene");
        }
    }

    private void SkipToLobby()
    {
        Debug.Log("한번 접속한 유저의 로비 씬으로 이동!");
        SceneLoader.LoadSceneWithLoading("03_FusionLobby");
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
