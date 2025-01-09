using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GameData;
using System;

/// <summary>
/// 시스템 기획서 상 같은 색 동시에 일어나지 않는 로직 추가 필요
/// 스테이지 매니저의 현재 날씨 받아와서 occurPlusPercent 설정 필요
/// </summary>
public class EventManager : MonoBehaviour
{
    [Header("이벤트 계산 주기(초)")]
    [SerializeField] private float _checkInterval = 1.0f;

    public UnityEvent<EVENT> OnEventStarted;
    public UnityEvent<EVENT> OnEventEnded;

    // 이벤트 발생확률 테스트용 조정
    [SerializeField] Dictionary<int, EVENT> events;

    // 현재 진행 중인 이벤트 목록
    private List<int> _activeEventsID = new List<int>();


    private (int, int)[] _conflictPairs = new (int, int)[]
    {
        (421,441),
        (431,442),
    };

    private void Start()
    {
        StartCoroutine(EventRoutine());
    }

    private IEnumerator EventRoutine()
    {
        // CSV 다운로드가 끝날 때까지 대기
        while (!CSVManager.Instance.downloadCheck)
            yield return null;

        events = CSVManager.Instance.Events;


        var eventDict = CSVManager.Instance.Events;
        var seasonDict = CSVManager.Instance.Events_Seasons;

        while (true)
        {
            yield return new WaitForSeconds(_checkInterval);

            // 당첨된 이벤트 리스트
            List<int> triggered = new List<int>();

            int seasonID = StageManager.Instance.WeatherID;

            foreach (var kv in eventDict)
            {
                int eventID = kv.Key;
                EVENT ev = kv.Value;

                // 이미 진행 중인 같은 이벤트는 스킵
                if (_activeEventsID.Contains(eventID))
                    continue;

                // 기본 확률
                float finalRate = ev.event_occurPercent;

                // 이벤트가 현재 계절이면,
                if (CheckSeasonMatch(eventID, seasonID, seasonDict))
                {
                    finalRate += ev.event_occurPlusPercent;
                }

                // 확률 검사
                if (ProbabilityHelper.Draw(finalRate))
                {
                    triggered.Add(eventID);
                }
            }

            if (triggered.Count == 0)
                continue;

            ResolveConflicts(triggered);

            // 나머지 이벤트 모두 동시 발생 가능
            foreach (var evID in triggered)
            {
                StartEvent(evID);
            }
        }
    }

    // 계절 확인, true반환
    private bool CheckSeasonMatch(int eventID, int seasonID, Dictionary<int, EVENT_SEASON> seasonDict)
    {
        if (!seasonDict.ContainsKey(eventID))
            return false;

        EVENT_SEASON es = seasonDict[eventID];
        for (int i = 0; i < es.event_seasonID.Length; i++)
        {
            if (es.event_seasonID[i] == seasonID)
                return true;
        }

        return false;
    }

    private void ResolveConflicts(List<int> triggered)
    {
        foreach (var pair in _conflictPairs)
        {
            // 동시에 일어나면 안되는 쌍이 동시에 존재하는지 확인
            int AeventID = pair.Item1;
            int BeventID = pair.Item2;
            bool ifHasA = triggered.Contains(AeventID);
            bool ifHasB = triggered.Contains(BeventID);

            // 동시에 존재하면 하나 삭제
            if (ifHasA && ifHasB)
            {
                int r = UnityEngine.Random.Range(0, 2);
                if ( r == 0)
                {
                    triggered.Remove(BeventID);
                }
                else
                {
                    triggered.Remove(AeventID);
                }
            }
        }
    }

    private void StartEvent(int eventID)
    {
        var eventDict = CSVManager.Instance.Events;
 
        EVENT evData = eventDict[eventID];

        _activeEventsID.Add(eventID);

        OnEventStarted?.Invoke(evData);

        // 자동 종료
        if (evData.event_continueTime > 0)
        {
            StartCoroutine(AutoEndRoutine(eventID, evData.event_continueTime));
        }
    }

    private IEnumerator AutoEndRoutine(int eventID, float dur)
    {
        yield return new WaitForSeconds(dur);
        ResolveEvent(eventID);
    }

    /// <summary>
    /// 이벤트 종료 조건 달성시 호출 부탁드립니다
    /// </summary>
    public void ResolveEvent(int eventID)
    {
        if (_activeEventsID.Remove(eventID))
        {
            EVENT evDATA = CSVManager.Instance.Events[eventID];

            OnEventEnded?.Invoke(evDATA);
        }
    }
}