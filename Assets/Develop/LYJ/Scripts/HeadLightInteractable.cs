using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HeadLightInteractable : XRGrabInteractable
{
    private PhotonView photonView;
    [SerializeField] private Light _headLight;

    protected override void Awake()
    {
        base.Awake();

        photonView = GetComponent<PhotonView>();

        _headLight = GetComponentInChildren<Light>();
        if (_headLight == null)
        {
            Debug.LogError("HeadLight가 존재하지 않습니다.");
        }

        if (_headLight != null)
        {
            _headLight.enabled = false;
        }
    }

    public void TriggerBlackout()
    {
        if (_headLight != null)
        {
            _headLight.enabled = true;
        }
    }

    public void RecoverFromBlackout()
    {
        if (_headLight != null)
        {
            _headLight.enabled = false;
        }
    }
}
