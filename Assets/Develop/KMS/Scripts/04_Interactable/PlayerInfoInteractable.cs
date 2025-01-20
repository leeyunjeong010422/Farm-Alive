using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerInfoInteractable : XRGrabInteractable
{
    private PlayerInfo _playerInfo;

    protected override void Awake()
    {
        base.Awake();
        _playerInfo = GetComponent<PlayerInfo>();
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);

        if (_playerInfo != null)
        {
            _playerInfo.ShowInfo();
        }
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);

        if (_playerInfo != null)
        {
            _playerInfo.HideInfo();
        }
    }
}
