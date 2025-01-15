using Photon.Pun;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class ButtonInteractable_Ver0 : MonoBehaviourPunCallbacks
{
    [System.Serializable]
    public class ButtonPressedEvent : UnityEvent { }
    [System.Serializable]
    public class ButtonReleasedEvent : UnityEvent { }

    // 버튼이 움직일 축 지정 (기본적으로 y 아래 방향)
    public Vector3 _axis = new Vector3(0, -1, 0);

    // 버튼이 눌렀을 때 들어갈 거리 (위의 _axis 방향으로 움직임)
    public float _maxDistance;

    // 버튼이 원래 위치로 돌아가는 속도
    public float _returnSpeed = 10.0f;

    // 버튼 눌렀을 때 작용할 AudioClip
    // public AudioClip ButtonPressAudioClip;
    // public AudioClip ButtonReleaseAudioClip;

    // 버튼이 완전히 눌렸을 때와 떼어졌을 때 실행하는 이벤트
    public ButtonPressedEvent _onButtonPressed;
    public ButtonReleasedEvent _onButtonReleased;

    // 버튼의 시작위치 저장
    Vector3 _startPosition;
    Rigidbody _rigidbody;
    bool _pressed = false;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _startPosition = transform.position;

        // ownership transfer가 fixed여야 함
    }

    void FixedUpdate()
    {
        // 물리 연산 -> 소유자만 실행
        if (!photonView.IsMine)
            return;

        // 축을 월드방향으로
        Vector3 worldAxis = transform.TransformDirection(_axis);

        // 현재 버튼의 이동 위치
        float currentDistance = (transform.position - _startPosition).magnitude;
        float move = 0.0f;

        // SweepTest로 감지
        if (_rigidbody.SweepTest(-worldAxis, out RaycastHit hit, _returnSpeed * Time.deltaTime + 0.005f))
        {
            // 충돌이 있는 경우 : move를 양수로 하여 버튼을 더 눌러줌
            move = (_returnSpeed * Time.deltaTime) - hit.distance;
        }
        else
        {
            // 충돌이 없으면 move를 음수로 하여 버튼 복귀
            move -= _returnSpeed * Time.deltaTime;
        }

        // 새로운 이동거리 계산 및 적용
        float newDistance = Mathf.Clamp(currentDistance + move, 0, _maxDistance);

        _rigidbody.position = _startPosition + worldAxis * newDistance;

        // 눌렸을 때 이벤트 호출
        if (!_pressed && Mathf.Approximately(newDistance, _maxDistance))
        {
            _pressed = true;
            /*
            SFXPlayer.Instance.PlaySFX(ButtonPressAudioClip, transform.position, new SFXPlayer.PlayParameters()
            {
                Pitch = Random.Range(0.9f, 1.1f),
                SourceID = -1,
                Volume = 1.0f
            }, 0.0f);
            */
            photonView.RPC(nameof(RPC_OnButtonPressed), RpcTarget.All);
        }
        else if (_pressed && !Mathf.Approximately(newDistance, _maxDistance))
        {
            _pressed = false;
            /*
            SFXPlayer.Instance.PlaySFX(ButtonReleaseAudioClip, transform.position, new SFXPlayer.PlayParameters()
            {
                Pitch = Random.Range(0.9f, 1.1f),
                SourceID = -1,
                Volume = 1.0f
            }, 0.0f);
            */
            photonView.RPC(nameof(RPC_OnButtonReleased), RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_OnButtonPressed()
    {
        _onButtonPressed.Invoke();
    }

    [PunRPC]
    private void RPC_OnButtonReleased()
    {
        _onButtonReleased.Invoke();
    }
}