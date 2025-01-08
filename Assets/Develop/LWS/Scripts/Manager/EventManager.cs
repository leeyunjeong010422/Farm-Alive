using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GameData;

public class EventManager : MonoBehaviour
{
    [Header("이벤트 계산 주기")]
    [SerializeField] float checkInterval = 1.0f;

    // 이벤트가 진행 중인가
    private bool _isEventPlaying = false;

    [Header("이벤트 발생/종료 알림")]
    [SerializeField] UnityEvent onEventStarted;
    [SerializeField] UnityEvent onEventEnded;

    private void Start()
    {
        StartCoroutine(EventRoutine());
    }

    private IEnumerator EventRoutine()
    {
        // csv 다운로드가 완료될때까지 기다리기.
        while (!CSVManager.Instance.downloadCheck)
        {
            yield return null;
        }

        List<EVENT> events = CSVManager.Instance.Events;

        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            // 이벤트 진행중이면 패스
            if (_isEventPlaying)
                continue;

            float sumWeight = 0f;
            foreach(var ev in events)
            {
                float finalChance = ev.event_occurPercent + ev.event_occurPlusPercent;
                sumWeight += finalChance;
            }

            // 0퍼센트면 패스
            if (sumWeight <= 0f)
                continue;

            float rand = Random.value * sumWeight;

            float cumulative = 0f;
            EVENT selectedevent = default;
            bool found = false;

            foreach (var ev in events)
            {
                float finalChance = ev.event_occurPercent + ev.event_occurPlusPercent;
                cumulative += finalChance;
                if (rand <= cumulative)
                {
                    selectedevent = ev;
                    found = true;
                    break;
                }
            }

            if(found)
            {
                // TODO : 이벤트 실행
            }
        }
    }

    // TODO : 이벤트 실행 메서드

    // TODO : 해결 메서드

    // TODO : 이벤트 해결 알리기
}
