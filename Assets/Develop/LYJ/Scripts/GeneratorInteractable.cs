using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GeneratorInteractable : XRGrabInteractable
{
    [Header("Generator Settings")]
    [Tooltip("시동이 걸리기까지 필요한 시도 횟수")]
    [SerializeField] private int startAttemptsRequired = 3; // m값 (필요한 시도 횟수)

    [Header("Cord Settings")]
    [Tooltip("시동줄의 고정 시작 위치")]
    [SerializeField] private Transform cordStartPosition;
    [Tooltip("시동줄의 최대 끝 위치")]
    [SerializeField] private Transform cordEndPosition;
    [Tooltip("시동줄 오브젝트")]
    [SerializeField] private Transform cordObject;

    private Vector3 initialCordPosition; // 줄의 초기 위치
    private Quaternion initialCordRotation; // 줄의 초기 회전
    private int currentAttempts = 0; // 현재 시도 횟수
    private bool isBeingPulled = false; // 줄이 당겨지고 있는 상태인지
    private bool hasTriggered = false; // 중복 처리를 방지하기 위한 플래그

    private void Start()
    {
        // 줄의 초기 위치 및 초기 회전 설정
        if (cordObject != null)
        {
            initialCordPosition = cordObject.position;
            initialCordRotation = cordObject.rotation;
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        isBeingPulled = true;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        isBeingPulled = false;
        ResetCordPosition();
    }

    private void OnTriggerEnter(Collider other)
    {
        // cordObject가 cordEndPosition에 닿았고, 중복 처리가 안 된 경우
        if (other.transform == cordEndPosition && !hasTriggered)
        {
            hasTriggered = true; // 중복 처리 방지
            currentAttempts++; // 시도 횟수 증가

            Debug.Log($"발전기 시동 시도: {currentAttempts}/{startAttemptsRequired}");

            if (currentAttempts >= startAttemptsRequired)
            {
                Debug.Log("발전기 시동 성공!");
                currentAttempts = 0; // 성공 후 초기화
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // cordObject가 cordEndPosition에서 벗어났을 때 중복 처리를 다시 허용
        if (other.transform == cordEndPosition)
        {
            hasTriggered = false; // 플래그 초기화
        }
    }

    private void ResetCordPosition()
    {
        // 줄을 초기 위치와 초기 회전으로 되돌리기
        if (cordObject != null)
        {
            cordObject.position = initialCordPosition;
            cordObject.rotation = initialCordRotation;
        }
    }
}
