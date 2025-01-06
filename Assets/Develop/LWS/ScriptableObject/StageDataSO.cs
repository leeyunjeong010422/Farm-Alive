using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Game/StageData")]
public class StageDataSO : ScriptableObject
{
    public int stageNum; // 스테이지 넘버
    public E_WeatherType weatherType; // 스테이지 별 계절
    public int questCount; // 거래처 수
    public float eventBaseChanse; // 이벤트 발생 확률
    public float timeLimit; // 스테이지 제한 시간
}