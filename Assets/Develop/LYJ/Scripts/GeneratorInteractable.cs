using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;
using System.Collections;

public class GeneratorInteractable : XRGrabInteractable
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

    [Header("Linked Components")]
    [SerializeField] private XRKnob knob;
    [SerializeField] private XRLever lever;

    [Header("Lighting")]
    [Tooltip("정전 시 켜질 헤드라이트")]
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

    private void Start()
    {
        knob = transform.root.GetComponentInChildren<XRKnob>();
        lever = transform.root.GetComponentInChildren<XRLever>();

        knob.onValueChange.AddListener(OnKnobValueChanged);
        lever.onLeverActivate.AddListener(OnLeverActivate);
        lever.onLeverDeactivate.AddListener(OnLeverDeactivate);

        if (cordObject != null)
        {
            initialCordPosition = cordObject.position;
            initialCordRotation = cordObject.rotation;
            initialCordScale = cordObject.localScale;
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
        isLeverDown = true;
        Debug.Log("레버가 내려갔습니다.");

        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
            warningCoroutine = null;
            Debug.Log("레버가 내려가서 고장 방지됨.");
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
        isBeingPulled = true;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        isBeingPulled = false;
        ResetCordPosition();
    }

    private void Update()
    {
        if (isBeingPulled && cordObject != null && cordStartPosition != null && cordEndPosition != null)
        {
            float pullDistance = Vector3.Distance(cordObject.position, cordStartPosition.position);

            Vector3 newScale = initialCordScale;
            newScale.y = initialCordScale.y + pullDistance;
            cordObject.localScale = newScale;
        }

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
                Light[] lights = FindObjectsOfType<Light>();
                foreach (Light light in lights)
                {
                    if (light.type == LightType.Directional)
                    {
                        light.enabled = true;
                        Debug.Log("정전 해제");
                    }
                }

                if (headLight != null)
                {
                    headLight.DisableHeadlight();
                    Debug.Log("헤드라이트 꺼짐");
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
            isGeneratorRunning = false;

            Light[] lights = FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    light.enabled = false;
                    Debug.Log("정전 발생");
                }
            }

            if (headLight != null)
            {
                headLight.EnableHeadlight();
                Debug.Log("헤드라이트 켜짐");
            }
            else
            {
                Debug.Log("headLight가 없습니다.");
            }
        }

        warningCoroutine = null;
    }
}
