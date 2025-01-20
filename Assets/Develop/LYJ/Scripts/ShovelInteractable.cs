using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ShovelInteractable : XRGrabInteractable
{
    private PlantGround _currentGround;
    private int _groundTriggerCount = 0;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        SoundManager.Instance.PlaySFX("SFX_ShovelSelected");
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        
        SoundManager.Instance.PlaySFX("SFX_ShovelExited");
    }

    private void OnTriggerEnter(Collider other)
    {
        PlantGround ground = other.GetComponentInParent<PlantGround>();
        if (ground != null)
        {
            if (_currentGround == null)
            {
                _currentGround = ground;
                //Debug.Log($"참조 성공");
            }
            else
            {
                Debug.Log("참조 실패: PlantGround 스크립트가 없음");
            }

            if (ground == _currentGround)
            {
                _groundTriggerCount++;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlantGround ground = other.GetComponentInParent<PlantGround>();
        if (ground != null && ground == _currentGround)
        {
            _groundTriggerCount--;

            if (_groundTriggerCount <= 0)
            {
                SoundManager.Instance.PlaySFX("SFX_ShovelDig");
                _currentGround.Dig();
                _currentGround = null;
                _groundTriggerCount = 0;
                //Debug.Log("참조 해제");
            }
        }
    }
}
