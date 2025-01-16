using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GameData;
using Photon.Pun;

public class EventManager : MonoBehaviourPunCallbacks
{
    [Header("이벤트 계산 주기(초)")]
    [SerializeField] private float _checkInterval = 1.0f;

    public UnityEvent<EVENT> OnEventStarted;
    public UnityEvent<EVENT> OnEventEnded;

    // 현재 진행 중인 이벤트 목록
    public List<int> _activeEventsID = new List<int>(); 

    private (int, int)[] _conflictPairs = new (int, int)[]
    {
        (421,441),
        (431,442),
    };

    public static EventManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(EventRoutine());
        }
    }

        private void TriggerManualEvent(int eventID)
    {
        int[] arr = new int[1]{ eventID };
        photonView.RPC(nameof(RPC_StartEvents), RpcTarget.All, arr);
    }

    private IEnumerator EventRoutine()
    {
        // CSV 다운로드가 끝날 때까지 대기
        while (!CSVManager.Instance.downloadCheck)
            yield return null;

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

                // 같이 발생하면 안되는 이벤트가 이미 진행중이면 스킵
                if (CheckConflictWithActive(eventID))
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

            photonView.RPC(nameof(RPC_StartEvents), RpcTarget.All, triggered.ToArray());
        }
    }

    [PunRPC]
    private void RPC_StartEvents(int[] eventIDs)
    {
        foreach (int eID in eventIDs)
        {
            StartEvent(eID);
        }
    }

    private void StartEvent(int eventID)
    {
        var eventDict = CSVManager.Instance.Events;

        Debug.Log($"{eventDict[eventID].event_name} 발생");
        if (!_activeEventsID.Contains(eventID))
        {
            _activeEventsID.Add(eventID);

            EVENT evData = eventDict[eventID];

            OnEventStarted?.Invoke(evData);
            
            ParticleManager.Instance.PlayParticle(eventID.ToString(), evData.event_continueTime);

            // MessageDisplayManager.Instance.ShowMessage(eventDict[eventID].event_name);

            // 자동 종료
            if (evData.event_continueTime > 0) 
            {
                StartCoroutine(AutoEndRoutine(eventID, evData.event_continueTime));
            }
        }
    }

    // 이미 진행중인 이벤트 충돌이면 true 반환
    private bool CheckConflictWithActive(int newEventID)
    {
        foreach (var pair in _conflictPairs)
        {
            int A = pair.Item1;
            int B = pair.Item2;
            if (newEventID == A && _activeEventsID.Contains(B))
                return true;
            if (newEventID == B && _activeEventsID.Contains(A))
                return true;
        }
        return false;
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
                int r = Random.Range(0, 2);
                if (r == 0)
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


    private IEnumerator AutoEndRoutine(int eventID, float dur)
    {
        yield return new WaitForSeconds(dur);

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RPC_ResolveEvent), RpcTarget.All, eventID);
        }
    }

    /// <summary>
    /// 이벤트 외부에서 종료
    /// </summary>
    /// <param name="eventID"></param>
    public void EndEvent(int eventID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RPC_ResolveEvent), RpcTarget.All, eventID);
        }
    }


    [PunRPC]
    private void RPC_ResolveEvent(int eventID)
    {
        ResolveEvent(eventID);

        // MessageDisplayManager.Instance.SendMessage($"{CSVManager.Instance.Events[eventID].event_name} 해결 완료");
    }

    public void ResolveEvent(int eventID)
    {
        if (_activeEventsID.Remove(eventID))
        {
            var evData = CSVManager.Instance.Events[eventID];

            OnEventEnded?.Invoke(evData);

            ParticleManager.Instance.StopParticle(eventID.ToString());
        }
    }
}