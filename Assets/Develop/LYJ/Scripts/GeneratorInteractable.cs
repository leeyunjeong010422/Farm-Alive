using System.Collections;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class GeneratorInteractable : XRBaseInteractable
{
    [Header("Generator Settings")]
    [Tooltip("시동이 걸리기까지 필요한 시도 횟수")]
    [SerializeField] private int startAttemptsRequired = 3;

    [Tooltip("고장이 발생하기까지의 시간")]
    [SerializeField] private float breakdownWarningDuration = 5f;

    [Tooltip("시동줄의 고정 시작 위치")]
    [SerializeField] private Transform cordStartPosition;
    [Tooltip("시동줄의 최대 끝 위치")]
    [SerializeField] private Transform cordEndPosition;
    [Tooltip("시동줄 오브젝트")]
    [SerializeField] private Transform cordObject;

    [SerializeField] private XRKnobGenerator _knob;
    [SerializeField] private XRLever _lever;

    [SerializeField] private Repair repair;
    [SerializeField] private HeadLightInteractable headLight;

    private Vector3 initialCordPosition;
    private Quaternion initialCordRotation;
    private Vector3 initialCordScale;
    private int currentAttempts = 0;
    private bool isBeingPulled = false;
    private bool hasTriggered = false;

    private bool isGeneratorRunning = true;
    private Coroutine warningCoroutine = null;
    private bool isLeverDown = false;
    private float currentKnobValue = 0f;

    private Rigidbody rigid;
    private Vector3 startPos;

    private bool warningActive = false;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        startPos = transform.position;

        repair = GetComponentInParent<Repair>();
        repair.enabled = false;

        headLight = FindObjectOfType<HeadLightInteractable>();

        _knob = transform.root.GetComponentInChildren<XRKnobGenerator>();
        _lever = transform.root.GetComponentInChildren<XRLever>();

        _knob.onValueChange.AddListener(OnKnobValueChanged);
        _lever.onLeverActivate.AddListener(OnLeverActivate);
        _lever.onLeverDeactivate.AddListener(OnLeverDeactivate);

        if (cordObject != null)
        {
            initialCordPosition = cordObject.position;
            initialCordRotation = cordObject.rotation;
            initialCordScale = cordObject.localScale;
        }

        if (headLight == null)
        {
            Debug.LogWarning("HeadLightInteractable을 찾을 수 없습니다.");
        }
    }

    private void OnKnobValueChanged(float value)
    {
        currentKnobValue = value;

        if (currentKnobValue >= 1f && !isGeneratorRunning)
        {
            Debug.Log("휠이 최대 범위까지 돌아감!");
        }
    }

    private void OnLeverActivate()
    {
        // 전조 증상이 활성화된 경우에만 레버 내리기가 인정됨
        if (warningActive)
        {
            isLeverDown = true;
            Debug.Log("레버가 내려갔습니다.");

            if (warningCoroutine != null)
            {
                StopCoroutine(warningCoroutine);
                warningCoroutine = null;
                warningActive = false;
                Debug.Log("레버가 내려가서 고장 방지됨.");
            }
        }
        else
        {
            Debug.Log("전조 증상이 발생전 레버 동작으로 동작 인정 X");
        }
    }

    private void OnLeverDeactivate()
    {
        isLeverDown = false;
        Debug.Log("레버가 올라갔습니다.");
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        rigid.isKinematic = false;
        isBeingPulled = true;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        isBeingPulled = false;
        rigid.isKinematic = true;
        transform.position = startPos;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("전조 증상 테스트 시작");
            TriggerWarning();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == cordEndPosition && !hasTriggered)
        {
            hasTriggered = true;
            currentAttempts++;

            Debug.Log($"발전기 시동 시도: {currentAttempts}/{startAttemptsRequired}");

            if (currentAttempts >= startAttemptsRequired && currentKnobValue >= 1f)
            {
                Debug.Log("발전기 시동 성공!");
                isGeneratorRunning = true;
                currentAttempts = 0;

                // 정전 해제
                if (headLight != null)
                {
                    headLight.RecoverFromBlackout(); // 조명 복구
                }
                else
                {
                    Debug.LogWarning("HeadLightInteractable이 설정되지 않았습니다!");
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == cordEndPosition)
        {
            hasTriggered = false;
        }
    }

    private void ResetCordPosition()
    {
        if (cordObject != null)
        {
            cordObject.position = initialCordPosition;
            cordObject.rotation = initialCordRotation;
            cordObject.localScale = initialCordScale;
        }
    }

    public void TriggerWarning()
    {
        if (warningCoroutine == null)
        {
            warningActive = true; // 전조 증상 활성화
            warningCoroutine = StartCoroutine(BreakdownWarning());
        }
    }

    private IEnumerator BreakdownWarning()
    {
        Debug.Log("전조 증상! 레버를 내려 고장을 방지하세요!!!");
        yield return new WaitForSeconds(breakdownWarningDuration);

        if (!isLeverDown)
        {
            Debug.Log("고장이 발생했습니다!");
            repair.enabled = true;
            isGeneratorRunning = false;

            if (headLight != null)
            {
                headLight.TriggerBlackout(); // 정전 발생
            }
            else
            {
                Debug.LogWarning("HeadLightInteractable이 설정되지 않았습니다!");
            }
        }

        warningActive = false; // 전조 증상이 더 이상 진행되지 않음
        warningCoroutine = null;
    }
}
