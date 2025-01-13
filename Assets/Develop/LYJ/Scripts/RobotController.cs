using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RobotController : MonoBehaviour
{
    PhotonView photonView;

    private Transform _targetPlayer; // 로봇이 따라갈 유저
    private int targetPhotonViewID = -1; // 따라갈 유저의 PhotonViewID
    private bool isFollowing = false; // 현재 추적 중인지에 대한 여부
    private bool isReturning = false; // 초기 위치로 돌아가는 중인지에 대한 여부

    private Vector3 initialPosition; // 로봇의 처음 위치 저장
    private Quaternion initialRotation; // 로봇의 처음 회전값 저장

    [SerializeField] private float _followDistance = 3.0f; // 따라갈 최소 거리
    [SerializeField] private float _returnDistance = 0.1f; // 초기 위치 근처로 돌아온 거리

    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        // 로봇의 초기 위치와 회전값 저장
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        Button moveButton = GetComponentInChildren<Button>();
        if (moveButton != null)
        {
            moveButton.onClick.AddListener(OnMoveButtonClicked);
        }
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (isReturning)
        {
            // 초기 위치로 이동 중일 때
            float distanceToInitial = Vector3.Distance(transform.position, initialPosition);
            if (distanceToInitial <= _returnDistance)
            {
                CompleteReturnToInitial();
            }
        }
        else if (_targetPlayer != null)
        {
            // 추적 대상이 있으면 거리 계산
            float distance = Vector3.Distance(transform.position, _targetPlayer.position);

            // 최소 거리보다 멀리 있을 때만 이동
            if (distance > _followDistance)
            {
                navMeshAgent.SetDestination(_targetPlayer.position);
            }
            else
            {
                navMeshAgent.ResetPath();
            }
        }
    }

    public void OnMoveButtonClicked()
    {
        PhotonView localPlayerPhotonView = GetLocalPlayerPhotonView();
        if (localPlayerPhotonView == null)
        {
            Debug.LogError("로컬 플레이어의 PhotonView를 찾을 수 없습니다!");
            return;
        }

        int photonViewID = localPlayerPhotonView.ViewID;
        Debug.Log($"버튼을 누른 플레이어의 PhotonViewID: {photonViewID}");

        // 소유권 요청
        photonView.RPC(nameof(RequestOwnership), RpcTarget.MasterClient, photonViewID);
    }

    [PunRPC]
    private void RequestOwnership(int photonViewID, PhotonMessageInfo info)
    {
        // 마스터 클라이언트에서 소유권 이전
        photonView.TransferOwnership(info.Sender.ActorNumber);

        // 소유권 이전 후 대상 동기화
        photonView.RPC(nameof(SyncTargetPlayer), RpcTarget.AllBuffered, photonViewID);
    }

    [PunRPC]
    private void SyncTargetPlayer(int photonViewID)
    {
        // 로봇의 소유자만 실행
        if (!photonView.IsMine)
        {
            return;
        }

        // 이미 추적 중인 플레이어가 버튼을 다시 누른 경우 초기 상태로 복원
        if (targetPhotonViewID == photonViewID && isFollowing)
        {
            StartReturnToInitial();
            Debug.Log("로봇이 초기 위치로 복원 중입니다.");
            return;
        }

        GameObject targetObject = GetPlayerGameObjectByPhotonViewID(photonViewID);

        if (targetObject != null)
        {
            _targetPlayer = targetObject.transform; // 추적 대상 갱신
            targetPhotonViewID = photonViewID; // 추적 대상 ID 갱신
            isFollowing = true; // 추적 상태 활성화
            Debug.Log($"로봇이 플레이어 {photonViewID} 를 따라갑니다.");
        }
        else
        {
            Debug.LogError($"해당 PhotonViewID {photonViewID} 를 가진 플레이어를 찾을 수 없습니다!");
        }
    }

    private void StartReturnToInitial()
    {
        _targetPlayer = null; // 추적 대상 해제
        targetPhotonViewID = -1; // 대상 ID 초기화
        isFollowing = false; // 추적 상태 비활성화
        isReturning = true; // 초기 위치로 돌아가는 상태 활성화

        // 초기 위치로 이동
        navMeshAgent.SetDestination(initialPosition);
    }

    private void CompleteReturnToInitial()
    {
        isReturning = false; // 초기 위치로 돌아오는 상태 비활성화

        // NavMeshAgent 경로 초기화
        navMeshAgent.ResetPath();

        // 정확한 초기 위치와 회전값으로 설정
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        Debug.Log("로봇이 초기 상태로 복원되었습니다.");
    }

    private PhotonView GetLocalPlayerPhotonView()
    {
        // 로컬 플레이어에 해당하는 PhotonView 가져오기
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            PhotonView view = player.GetComponent<PhotonView>();
            if (view != null && view.IsMine)
            {
                return view;
            }
        }

        return null;
    }

    private GameObject GetPlayerGameObjectByPhotonViewID(int photonViewID)
    {
        // PhotonViewID로 GameObject 찾기
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            PhotonView view = player.GetComponent<PhotonView>();
            if (view != null && view.ViewID == photonViewID)
            {
                return player;
            }
        }

        return null;
    }
}
