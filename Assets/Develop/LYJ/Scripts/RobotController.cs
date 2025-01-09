using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RobotController : MonoBehaviour
{
    PhotonView photonView;

    private Transform _targetPlayer; // 로봇이 따라갈 유저
    private int targetPhotonViewID = -1; // 따라갈 유저의 PhotonViewID
    private bool isFollowing = false; // 현재 추적 중인지 확인하는 플래그

    private Vector3 initialPosition; // 로봇의 처음 위치 저장

    [SerializeField] private float _followDistance = 3.0f; // 따라갈 최소 거리

    private NavMeshAgent navMeshAgent; // NavMeshAgent 컴포넌트

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        // 로봇의 초기 위치 저장
        initialPosition = transform.position;

        // 버튼 컴포넌트 찾기 및 클릭 이벤트 등록
        Button moveButton = GetComponentInChildren<Button>();
        if (moveButton != null)
        {
            moveButton.onClick.AddListener(OnMoveButtonClicked);
        }
    }

    private void Update()
    {
        // 추적 대상이 없으면 아무것도 하지 않음
        if (_targetPlayer == null)
            return;

        // 로봇과 플레이어 사이의 거리 계산
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

    public void OnMoveButtonClicked()
    {
        // 로컬 플레이어의 PhotonViewID 가져오기
        PhotonView localPlayerPhotonView = GetLocalPlayerPhotonView();
        if (localPlayerPhotonView == null)
        {
            Debug.LogError("로컬 플레이어의 PhotonView를 찾을 수 없습니다!");
            return;
        }

        int photonViewID = localPlayerPhotonView.ViewID;
        Debug.Log($"버튼을 누른 플레이어의 PhotonViewID: {photonViewID}");

        // 버튼을 누른 로컬 플레이어의 PhotonViewID를 서버로 전달
        photonView.RPC(nameof(SyncTargetPlayer), RpcTarget.All, photonViewID);
    }

    [PunRPC]
    private void SyncTargetPlayer(int photonViewID, PhotonMessageInfo info)
    {
        // 이미 추적 중인 플레이어가 한 번 더 버튼을 누른 경우 초기 위치로 돌아감
        if (targetPhotonViewID == photonViewID && isFollowing)
        {
            _targetPlayer = null; // 추적 대상 해제
            targetPhotonViewID = -1; // 대상 ID 초기화
            isFollowing = false; // 추적 상태 비활성화
            navMeshAgent.SetDestination(initialPosition); // 초기 위치로 이동
            Debug.Log("로봇이 초기 위치로 돌아갑니다.");
            return;
        }

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

    private PhotonView GetLocalPlayerPhotonView()
    {
        // 로컬 플레이어에 해당하는 PhotonView 가져오기
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            PhotonView view = player.GetComponent<PhotonView>();
            if (view != null && view.IsMine) // 로컬 플레이어 확인
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
