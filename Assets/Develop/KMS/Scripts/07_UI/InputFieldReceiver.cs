using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;

public class InputFieldReceiver : MonoBehaviour
{
    [SerializeField] private XRKeyboardDisplay _keyboardDisplay;

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
        // TODO : 방 이름 넘겨주기. 
    }
}
