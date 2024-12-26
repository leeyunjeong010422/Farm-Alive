using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageDisplayManager : MonoBehaviour
{
    public static MessageDisplayManager Instance { get; private set; }

    [SerializeField] private Canvas canvas;              // 텍스트를 띄울 캔버스
    [SerializeField] private TextMeshProUGUI messageText; // 텍스트 컴포넌트
    [SerializeField] private float displayTime = 2f;     // 텍스트 표시 시간
    [SerializeField] private float distanceFromCamera = 2f; // 텍스트가 카메라 앞에 나타날 거리

    private Transform playerCamera;
    private float timer;
    private bool isShowing;

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (canvas != null)
        {
            canvas.enabled = false; // 초기 상태는 비활성화
        }
        else
        {
            Debug.Log("Canvas를 설정해야 합니다.");
        }

        if (messageText == null)
        {
            Debug.Log("Text를 설정해야 합니다.");
        }
    }

    private void Start()
    {
        FindPlayerCamera();
    }

    private void Update()
    {
        // 메시지 타이머 업데이트
        if (isShowing)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                HideMessage();
            }
        }

        // 캔버스 위치를 플레이어 카메라 정면으로 업데이트
        if (playerCamera != null && canvas.enabled)
        {
            UpdateCanvasPosition();
        }
    }

    private void FindPlayerCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            playerCamera = mainCamera.transform;
        }
        else
        {
            Debug.Log("Camera를 찾을 수 없습니다.");
        }
    }

    /// <summary>
    /// 메시지를 표시
    /// </summary>
    /// <param name="message">플레이어에게 표시할 메시지</param>
    public void ShowMessage(string message)
    {
        if (playerCamera == null)
        {
            FindPlayerCamera(); // 카메라가 설정되지 않았다면 다시 탐색
        }

        if (messageText != null && canvas != null)
        {
            messageText.text = message;
            canvas.enabled = true;
            timer = displayTime;
            isShowing = true;

            // 메시지가 활성화될 때 캔버스 위치 업데이트
            UpdateCanvasPosition();
        }
    }

    private void HideMessage()
    {
        if (canvas != null)
        {
            canvas.enabled = false;
            isShowing = false;
        }
    }

    // 캔버스를 플레이어의 정면에 위치시
    private void UpdateCanvasPosition()
    {
        if (playerCamera == null || canvas == null) return;

        // 플레이어 카메라의 위치와 회전
        Vector3 cameraPosition = playerCamera.position;
        Quaternion cameraRotation = playerCamera.rotation;

        // 카메라 정면 앞 거리만큼 캔버스 위치를 설정
        Vector3 canvasPosition = cameraPosition + cameraRotation * Vector3.forward * distanceFromCamera;
        canvas.transform.position = canvasPosition;

        // 캔버스가 항상 카메라를 향하도록 회전
        canvas.transform.rotation = Quaternion.LookRotation(canvas.transform.position - cameraPosition);
    }
}
