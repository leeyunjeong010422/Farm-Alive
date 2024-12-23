using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystem : MonoBehaviour
{
    [SerializeField] TimeSystem _timeSystem;

    private void Start()
    {
        // TODO : 초기 이벤트 등록/ 세팅
    }

    public void RegisterEvent()
    {
        // TODO : 이벤트 등록
    }

    public void UnregisterEvent()
    {
        // TODO : 이벤트 제거
    }

    public void TriggerEvent()
    {
        // TODO : 이벤트 트리거용 함수
    }

    // 이벤트 컨디션을 검사할 코루틴
}
