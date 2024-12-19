using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlantGround : MonoBehaviour
{
    [SerializeField] private int _digCount; // 필요 삽질 횟수
    private int _currentDigCount = 0; // 현재 삽질 횟수
    private bool _isInteractable = true; // 상호작용 가능 여부
    private XRSocketInteractor _socketInteractor;

    private void Awake()
    {
        _socketInteractor = GetComponentInChildren<XRSocketInteractor>();

        if (_socketInteractor != null)
        {
            _socketInteractor.enabled = false;
        }
        else
        {
            Debug.Log("XRSocketInteractor를 찾을 수 없습니다.");
        }
    }

    /// <summary>
    /// 삽질 관리 메소드
    /// </summary>
    public void Dig()
    {
        if (!_isInteractable) return;

        _currentDigCount++;
        Debug.Log($"삽질 횟수: {_currentDigCount} / {_digCount}");

        if (_currentDigCount >= _digCount)
        {
            GameObject disappearGround = GameObject.FindWithTag("DisappearingGround");
            if (disappearGround != null)
            {
                Destroy(disappearGround);
                Debug.Log("DisappearingGround이 삭제되었습니다.");
            }

            if (_socketInteractor != null)
            {
                _socketInteractor.enabled = true;
            }


            _isInteractable = false;
        }
    }
}
