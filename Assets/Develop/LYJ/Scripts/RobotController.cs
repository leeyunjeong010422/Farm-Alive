using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RobotController : MonoBehaviour
{
    PhotonView photonView;

    private Transform _targetPlayer; // 로봇이 따라갈 유저
    private int targetPhotonViewID = -1; // 따라갈 유저의 PhotonViewID
    private int lastPhotonViewID = -1; // 마지막으로 추적했던 PhotonViewID
    private bool isFollowing = false; // 현재 추적 중인지에 대한 여부
    private bool isReturning = false; // 초기 위치로 돌아가는 중인지에 대한 여부

    private Vector3 initialPosition; // 로봇의 처음 위치 저장
    private Quaternion initialRotation; // 로봇의 처음 회전값 저장

    [SerializeField] private float _followDistance = 3.0f; // 따라갈 최소 거리
    [SerializeField] private float _returnDistance = 0.1f; // 초기 위치 근처로 돌아온 거리
    [SerializeField] private float cooldownTime = 3.0f; // 쿨다운 시간

    private NavMeshAgent navMeshAgent;
    private float lastButtonClickTime = -Mathf.Infinity; // 마지막으로 버튼이 눌린 시간

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
        // 이 로봇이 내가 조종하는 로봇이 아니라면 리턴
        if (!photonView.IsMine)
        {
            return;
        }

        // 로봇이 원래 위치로 돌아가고 있다면
        if (isReturning)
        {
            // 초기 위치로 이동 중일 때
            float distanceToInitial = Vector3.Distance(transform.position, initialPosition);
            if (distanceToInitial <= _returnDistance)
            {
                CompleteReturnToInitial();
            }
        }

        // 로봇이 따라갈 대상이 있다면
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

    // 로봇 버튼을 클릭했을 때
    public void OnMoveButtonClicked()
    {
        if (Time.time - lastButtonClickTime < cooldownTime)
        {
            Debug.Log("버튼이 너무 빨리 눌렸습니다. 쿨다운 중입니다.");
            return;
        }

        lastButtonClickTime = Time.time;

        // 로컬 플레이어의 PhotonViewID 가져오기
        PhotonView localPlayerPhotonView = GetLocalPlayerPhotonView();
        if (localPlayerPhotonView == null)
        {
            Debug.LogError("로컬 플레이어의 PhotonView를 찾을 수 없습니다!");
            return;
        }

        int photonViewID = localPlayerPhotonView.ViewID;
        Debug.Log($"버튼을 누른 플레이어의 PhotonViewID: {photonViewID}");

        photonView.RPC(nameof(RPC_ButtonClick), RpcTarget.MasterClient, photonViewID);
    }

    [PunRPC]
    private void RPC_ButtonClick(int photonViewID, PhotonMessageInfo info)
    {
        if (photonViewID == targetPhotonViewID && photonViewID == lastPhotonViewID)
        {
            photonView.RPC(nameof(StartReturnToInitial), RpcTarget.AllBuffered);
        }
        else
        {
            photonView.TransferOwnership(info.Sender.ActorNumber);
            photonView.RPC(nameof(SyncTargetPlayer), RpcTarget.AllBuffered, photonViewID);
        }

        lastPhotonViewID = photonViewID;
    }

    // 마스터 클라이언트에게 소유권 요청
    [PunRPC]
    private void RequestOwnership(int photonViewID, PhotonMessageInfo info)
    {
        // 마스터 클라이언트에서 요청한 플레이어에게 소유권 이전
        photonView.TransferOwnership(info.Sender.ActorNumber);

        // 소유권 이전 후 모든 플레이어에게 로봇 타겟 플레이어 대상 동기화
        photonView.RPC(nameof(SyncTargetPlayer), RpcTarget.AllBuffered, photonViewID);
    }

    [PunRPC]
    private void SyncTargetPlayer(int photonViewID, PhotonMessageInfo info)
    {
        //나를 타켓으로 삼고 있는 로봇이 아니라면
        if (!photonView.IsMine)
        {
            return;
        }

        //// 이미 추적 중인 플레이어가 한 번 더 버튼을 누른 경우 초기 상태로 복원
        //if (targetPhotonViewID == photonViewID && isFollowing)
        //{
        //    StartReturnToInitial();
        //    Debug.Log("로봇이 초기 위치로 복원 중입니다.");
        //    return;
        //}

        // 새로운 PhotonViewID를 기반으로 해당 플레이어의 Transform 가져오기
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

        // 초기 위치와 회전값으로 설정
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
            if (view != null && view.IsMine) // 내 플레이어라면 PhotonView 반환
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
            if (view != null && view.ViewID == photonViewID) // photonViewID 가 같다면
            {
                return player;
            }
        }

        return null;
    }
}