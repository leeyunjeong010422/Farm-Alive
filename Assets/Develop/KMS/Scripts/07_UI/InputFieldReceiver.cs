using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;

public class InputFieldReceiver : MonoBehaviour
{
    [Tooltip("XR 키보드")]
    [SerializeField] private XRKeyboardDisplay _keyboardDisplay;
    [Tooltip("InputFiled 세팅")]
    [SerializeField] private TMP_InputField _inputField;
    [Tooltip("방 생성 정보")]
    [SerializeField] private TMP_Text _text;
    [Tooltip("부모 오브젝트")]
    [SerializeField] private GameObject _gameObject;

    private string _roomName;

    private void OnEnable()
    {
        if (_keyboardDisplay)
        {
            // 키보드 엔터를 쳤을떄 동작하는 이벤트
            _keyboardDisplay.onTextSubmitted.AddListener(OnKeyboardInputSubmitted);
        }

        if(_inputField)
        {
            // TODO : 파이어베이스에서 받아온 캐릭터의 닉네임 + Room으로 기본 이름 생성하기.
            _roomName = "Player" + "'s Room";
            _inputField.text = _roomName;
        }

        if(_text)
        {
            _text.transform.localScale = Vector3.one * 0.7f;
            _text.text = $"게임 모드 : {PunManager.Instance.GetGameMode()}\n스테이지 : {PunManager.Instance.GetStageNumber()}";
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

        if (inputText == "")
        {
            inputText = _roomName;
            Debug.Log(_roomName);
            _inputField.text = _roomName;
        }
        _roomName = inputText;
    }

    public void CreateRoom()
    {
        _gameObject.SetActive(false);
        PunManager.Instance.SetRoomName(_roomName);
        PunManager.Instance.CreateAndMoveToPunRoom();
    }

    public void ReturnRoom()
    {
        _gameObject.SetActive(false);
    }
}
