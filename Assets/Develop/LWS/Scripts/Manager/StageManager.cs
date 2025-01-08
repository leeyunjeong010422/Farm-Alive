using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

public class StageManager : MonoBehaviour
{
    [SerializeField] int _curStageID = 511;
    [SerializeField] TimeManager _timeManager;
    [SerializeField] EventManager _eventManager;

    private void Start()
    {
        StartCoroutine(SetupStageRoutine());

        _timeManager = GetComponent<TimeManager>();
        _eventManager = GetComponent<EventManager>();
    }

    private IEnumerator SetupStageRoutine()
    {
        // csv 다운로드가 완료될때까지 기다리기.
        while (!CSVManager.Instance.downloadCheck)
        {
            yield return null;
        }

        _curStageID = PunManager.Instance.selectedStage;

        // 끝났으면, 리스트에서 해당 스테이지 정보 가져오기.
        var stages = CSVManager.Instance.Stages;
        STAGE stageData = new STAGE();
        for (int i = 0; i < stages.Count; i++)
        {
            if (stages[i].stage_ID == _curStageID)
            {
                stageData = stages[i];
                break;
            }
        }

        yield return 10f;

        _timeManager.SetTimeLimit(360f);
        _timeManager.StartStageTimer();

        Debug.Log($"현재 스테이지={stageData.stage_ID}, 날씨={stageData.stage_seasonID}");

        //TODO : 날씨에 따른 로직.
    }
}