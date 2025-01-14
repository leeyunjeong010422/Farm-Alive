using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Voice.PUN;

public class CharacterVoice : MonoBehaviour
{
    private PhotonView _photonView;

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();

        // Speaker 찾기 및 등록
        var speaker = GetComponentInChildren<Speaker>();
        if (speaker != null)
        {
            var audioSource = speaker.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                SoundManager.Instance.RegisterPlayerVoice(_photonView, audioSource);
            }
            else
            {
                Debug.LogWarning("Speaker에 AudioSource가 없습니다!");
            }
        }
        else
        {
            Debug.LogWarning("Speaker가 이 캐릭터에 존재하지 않습니다!");
        }
    }

    private void OnDestroy()
    {
        if (_photonView != null)
        {
            SoundManager.Instance.UnregisterPlayerVoice(_photonView);
        }
    }
}
