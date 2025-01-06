using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.SocialPlatforms;

public class TimeSystem : MonoBehaviour
{
    /// <summary>
    /// 스테이지별 제한시간과 관련된 타임 시스템
    /// 일반적으로 타이쿤 게임에는 배속기능, 일시정지 기능을 사용하기 위해 필요하며
    /// 추가적으로 필요해질 경우에만 사용하고 이 스크립트는 현재 스테이지별 제한시간과 남은시간만 사용함.
    /// </summary>

    [Header("스테이지 시간 관련 설정")]
    [Tooltip("스테이지 제한 시간(초 단위)")]
    [SerializeField] float _stageTimeLimit = 360f; // 디폴트값. 6분

    [Tooltip("게임 내부 시간 배속 사용 시 조절")]
    [SerializeField] float _timeScale = 1f; // = 실제 속도

    [Header("타이머 종료 이벤트")]
    [Tooltip("스테이지 시간이 모두 소진되었을 때 호출할 이벤트")]
    // 신 전환 등
    [SerializeField] UnityEvent _onStageTimeOver;

    // 경과 시간
    private float _curStageTime = 0f;
    private bool _isTimerRunnig = false;
    private bool _isPaused = false;

    // 시간변경 통지받을 스크립트 등록
    // ex) UI등에서 구독 후 사용, 비활성화 될 경우 해제 필요
    public event Action<float, float> onTimeUpdate;

    /// <summary>
    /// stageManager에서 스크립터블 오브젝트를 읽어와 제한시간을 변경하기 위한 public 함수
    /// timeSystem.SetTimeLimit(stageDataSO.timeLimit);
    /// timeSystem.StartStageTimer();
    /// </summary>
    /// <param name="seconds"></param>
    public void SetTimeLimit(float seconds)
    {
        _stageTimeLimit = seconds;
    }

    /// <summary>
    /// 카운트 시작 함수
    /// </summary>
    public void StartStageTimer()
    {
        _curStageTime = 0F;
        _isTimerRunnig = true;
        _isPaused = false;
    }

    /// <summary>
    /// 일시정지 함수 (필요 시 사용할 것)
    /// </summary>
    public void PauseTimer()
    {
        _isPaused = true;
    }

    public void ResumeTimer()
    {
        _isPaused = false;
    }

    private void Update()
    {
        if (!_isTimerRunnig)
            return;
        if (_isPaused)
            return;

        // 실제 deltaTime에 배속 비율만큼
        float delta = Time.deltaTime * _timeScale;

        // 시간 경과
        _curStageTime += delta;

        // 구독통지
        onTimeUpdate?.Invoke(_curStageTime, delta);

        // 제한시간 도달했을 경우
        if (_curStageTime >= _stageTimeLimit)
        {
            _isTimerRunnig = false;
            _onStageTimeOver?.Invoke(); // 스테이지 종료 이벤트 호출
        }
    }

    public float GetRemainTime()
    {
        // 남은 시간
        return Mathf.Max(0, _stageTimeLimit - _curStageTime);
    }
}