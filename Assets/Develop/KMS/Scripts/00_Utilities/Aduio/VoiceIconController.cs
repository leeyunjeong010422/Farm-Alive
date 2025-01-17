using Photon.Pun;
using Photon.Voice.PUN;
using UnityEngine;

public class VoiceIconController : MonoBehaviourPun
{
    public GameObject speakingIcon;  // 말할 때 표시될 아이콘
    public GameObject listeningIcon; // 들을 때 표시될 아이콘

    private PhotonVoiceView _photonVoiceView;

    void Start()
    {
        _photonVoiceView = GetComponent<PhotonVoiceView>();
        if (_photonVoiceView == null)
        {
            Debug.LogError("PhotonVoiceView가 설정되지 않았습니다.");
            return;
        }

        if (speakingIcon) speakingIcon.SetActive(false);
        if (listeningIcon) listeningIcon.SetActive(false);
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            if (_photonVoiceView.IsRecording)
            {
                // 로컬 플레이어가 말을 하고 있을 때 speakingIcon 활성화
                SetIconState(speakingIcon, true);
            }
            else
            {
                // 말을 멈췄을 때 speakingIcon 비활성화
                SetIconState(speakingIcon, false);
            }
        }
        else
        {
            if (_photonVoiceView.IsSpeaking)
            {
                // 다른 플레이어의 음성을 듣고 있을 때 listeningIcon 활성화
                SetIconState(listeningIcon, true);
            }
            else
            {
                // 듣는 상태가 아니면 listeningIcon 비활성화
                SetIconState(listeningIcon, false);
            }
        }
    }

    // 아이콘 활성화/비활성화 처리 메서드
    private void SetIconState(GameObject icon, bool state)
    {
        if (icon && icon.activeSelf != state)
        {
            icon.SetActive(state);
        }
    }
}