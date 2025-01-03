using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class GameStartManager : MonoBehaviour
{
    public GameObject introPanel;
    public TMP_Text introText;

    private int _currentStep = 0;
    private bool _isButtonPressed = false;

    [SerializeField] private string[] gameInstructions = new string[]
    {
        "Welcome VR World! Press B key",
        "This is Intro Text.",
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
        introText.text = gameInstructions[_currentStep];
    }

    private void Update()
    {
        InputDevice rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // 버튼이 눌린 상태인지 확인
        if (rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isPressed))
        {
            if (isPressed && !_isButtonPressed) // 눌림이 시작되었을 때만 처리
            {
                _isButtonPressed = true; // 버튼 눌림 상태 기록
                ShowNextInstruction();
            }
            else if (!isPressed) // 버튼이 떼어졌을 때 상태 초기화
            {
                _isButtonPressed = false;
            }
        }
    }

    private void ShowNextInstruction()
    {
        _currentStep++;
        if (_currentStep < gameInstructions.Length)
        {
            introText.text = gameInstructions[_currentStep];
        }
        else
        {
            Debug.Log("튜토리얼 씬으로 이동.");
            SceneManager.LoadScene("02_TutorialScene");
        }
    }
}
