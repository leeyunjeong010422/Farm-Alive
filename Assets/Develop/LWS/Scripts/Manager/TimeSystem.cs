using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSystem : MonoBehaviour
{
    // TODO : 게임 내에서 1초가 흐르는데 필요한 실제 시간 비율

    // 옵저버 목록
    // private List<ITimeObserver> observers = new List<ITimeObserver>();

    public void StartTime()
    {
        // TODO : 현재 시간을 어떻게 할지 결정 (리셋 등)
    }

    public void StopTime()
    {
        // TODO : 시간 멈춤 처리
    }

    public void RegisterObserver()
    {
        // TODO : 중복 등록 방지 등
    }

    public void UnregisterObserver()
    {
        // TODO : 구독해제
    }

    private void Update()
    {
        // TODO : timeScale을 이용해 게임 시간 진행.

        // 특정 이벤트에 옵저버들에게  알림 진행
    }

}
