using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;

public class NickNameInputFiledReceiver : MonoBehaviour
{
    [Tooltip("XR 키보드")]
    [SerializeField] private XRKeyboardDisplay _keyboardDisplay;
    [Tooltip("InputFiled 세팅")]
    [SerializeField] private TMP_InputField _inputField;
    [Tooltip("Test 출력")]
    [SerializeField] private TMP_Text _text; 
    private string _nickName;

    private void OnEnable()
    {
        if (_keyboardDisplay)
        {
            // 키보드 엔터를 쳤을떄 동작하는 이벤트
            _keyboardDisplay.onTextSubmitted.AddListener(OnKeyboardInputSubmitted);
        }
    }

    private void OnDisable()
    {
        if (_keyboardDisplay)
        {
            _keyboardDisplay.onTextSubmitted.RemoveListener(OnKeyboardInputSubmitted);
        }
    }

    // 키보드 입력값을 처리하는 메서드
    private void OnKeyboardInputSubmitted(string inputText)
    {
        Debug.Log($"입력 받은 텍스트 : {inputText}");

        _nickName = inputText;
    }

    public void CreateNickName()
    {

        _inputField.gameObject.SetActive(false);
        _text.text = "로비로 입장합니다!";
        FirebaseManager.Instance.SaveNickName( _nickName );

        SceneLoader.LoadSceneWithLoading("03_Lobby");
    }
}
