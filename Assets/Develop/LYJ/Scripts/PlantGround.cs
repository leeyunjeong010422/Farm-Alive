using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGround : MonoBehaviour
{
    [SerializeField] private int _digCount; // 필요 삽질 횟수
    private int _currentDigCount = 0; // 현재 삽질 횟수

    /// <summary>
    /// 삽질 관리 메소드
    /// </summary>
    public void Dig()
    {
        _currentDigCount++;
        Debug.Log($"삽질 횟수: {_currentDigCount} / {_digCount}");

        if (_currentDigCount >= _digCount)
        {
            GameObject disappearGround = GameObject.FindWithTag("DisappearGround");
            if (disappearGround != null)
            {
                Destroy(disappearGround);
                Debug.Log("DisappearGround가 삭제되었습니다.");
            }
        }
    }
}
