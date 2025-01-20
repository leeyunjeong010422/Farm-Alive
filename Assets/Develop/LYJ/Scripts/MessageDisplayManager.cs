using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageDisplayManager : MonoBehaviour
{
    public static MessageDisplayManager Instance { get; private set; }

    [SerializeField] private Canvas _messageCanvas;
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private float _displayTime = 3f;        // 텍스트 표시 시간
    [SerializeField] private float _distanceFromCamera = 3f; // 텍스트가 카메라 앞에 나타날 거리

    private Transform _playerCamera;
    private float _timer;
    private bool _isShowing;

    private void Awake()
    {
        _messageCanvas = GetComponentInChildren<Canvas>();
        _messageText = GetComponentInChildren<TextMeshProUGUI>();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (_messageCanvas != null)
        {
            _messageCanvas.enabled = false;
        }
        else
        {
            Debug.Log("Canvas를 설정해야 합니다.");
        }

        if (_messageText == null)
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
        if (_isShowing)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                HideMessage();
            }
        }

        // 캔버스 위치를 플레이어 카메라 정면으로 업데이트
        if (_playerCamera != null && _messageCanvas.enabled)
        {
            UpdateCanvasPosition();
        }
    }

    private void FindPlayerCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            _playerCamera = mainCamera.transform;
        }
        else
        {
            //Debug.Log("Camera를 찾을 수 없습니다.");
        }
    }

    /// <summary>
    /// 메시지를 표시
    /// </summary>
    /// <param name="message">플레이어에게 표시할 메시지</param>
    /// <param name="displayTime">메시지 표시 시간 (선택적)</param>
    /// <param name="distanceFromCamera">메시지가 카메라에서 떨어진 거리 (선택적)</param>
    public void ShowMessage(string message, float? displayTime = null, float? distanceFromCamera = null)
    {
        if (_playerCamera == null)
        {
            FindPlayerCamera(); // 카메라가 설정되지 않았다면 다시 탐색
        }

        if (_messageText != null && _messageCanvas != null)
        {
            _messageText.text = message;

            _timer = displayTime ?? _displayTime;
            _distanceFromCamera = distanceFromCamera ?? _distanceFromCamera;

            _messageCanvas.enabled = true;
            _isShowing = true;

            // 메시지가 활성화될 때 캔버스 위치 업데이트
            UpdateCanvasPosition();
        }
    }

    private void HideMessage()
    {
        if (_messageCanvas != null)
        {
            _messageCanvas.enabled = false;
            _isShowing = false;
        }
    }

    // 캔버스를 플레이어의 정면에 위치시
    private void UpdateCanvasPosition()
    {
        if (_playerCamera == null || _messageCanvas == null) return;

        // 플레이어 카메라의 위치와 회전
        Vector3 cameraPosition = _playerCamera.position;
        Quaternion cameraRotation = _playerCamera.rotation;

        // 카메라 정면 앞 거리만큼 캔버스 위치를 설정
        Vector3 canvasPosition = cameraPosition + cameraRotation * Vector3.forward * _distanceFromCamera;
        _messageCanvas.transform.position = canvasPosition;

        // 캔버스가 항상 카메라를 향하도록 회전
        _messageCanvas.transform.rotation = Quaternion.LookRotation(_messageCanvas.transform.position - cameraPosition);
    }
}
