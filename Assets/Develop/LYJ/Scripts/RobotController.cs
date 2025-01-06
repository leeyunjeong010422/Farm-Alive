using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RobotController : MonoBehaviour
{
    PhotonView photonView;

    private Canvas _canvas;
    private Button _moveButton;

    private Transform _targetPlayer; // 로봇이 따라갈 유저
    private int targetPhotonViewID = -1; // 따라갈 유저의 PhotonViewID
    //[SerializeField] private float _followSpeed; // 따라가는 속도
    [SerializeField] private float _followDistance; // 따라갈 최소 거리

    private NavMeshAgent navMeshAgent; // NavMeshAgent 컴포넌트

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        _canvas = GetComponentInChildren<Canvas>();
        _moveButton = GetComponentInChildren<Button>();

        _moveButton.onClick.AddListener(OnMoveButtonClicked);
    }

    private void Update()
    {
        if (_targetPlayer != null)
        {
            // 로봇과 플레이어 사이의 거리 계산
            float distance = Vector3.Distance(transform.position, _targetPlayer.position);

            // 로봇이 플레이어와의 최소 거리보다 멀리 떨어져 있을 때만 이동
            if (distance > _followDistance)
            {
                navMeshAgent.SetDestination(_targetPlayer.position);
                //// 로봇이 타겟 플레이어 방향으로 일정 속도로 이동
                //transform.position = Vector3.MoveTowards(transform.position, _targetPlayer.position, _followSpeed * Time.deltaTime);

                //// 로봇이 항상 플레이어를 바라보도록 회전
                //Vector3 direction = (_targetPlayer.position - transform.position).normalized;
                //Quaternion lookRotation = Quaternion.LookRotation(direction);
                //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f);
            }
            else
            {
                // 플레이어와 가까워지면 멈춤
                navMeshAgent.ResetPath();
            }
        }
    }

    private void OnMoveButtonClicked()
    {
        Debug.Log("버튼이 눌렸습니다!");

        // 로컬 플레이어의 GameObject 가져오기
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            PhotonView view = player.GetComponent<PhotonView>();
            if (view != null && view.IsMine) // 로컬 플레이어 확인
            {
                int photonViewID = view.ViewID;

                Debug.Log($"로컬 플레이어 PhotonViewID: {photonViewID}");

                // 버튼을 누른 클라이언트가 PhotonViewID를 모든 클라이언트에 동기화
                photonView.RPC(nameof(SyncTargetPlayer), RpcTarget.AllBuffered, photonViewID);
                return;
            }
        }

        Debug.LogError("로컬 플레이어의 PhotonView를 찾을 수 없습니다!");
    }

    [PunRPC]
    private void SyncTargetPlayer(int photonViewID)
    {
        if (targetPhotonViewID == photonViewID)
        {
            // 이미 설정된 추적 대상이라면 중복 처리하지 않음
            Debug.Log($"로봇이 이미 플레이어 {photonViewID} 를 추적 중입니다.");
            return;
        }

        // 타겟 PhotonViewID 업데이트
        targetPhotonViewID = photonViewID;

        // PhotonViewID로 타겟 유저의 Transform 가져오기
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); // 모든 플레이어 검색 (Player 태그 필요)
        foreach (GameObject player in players)
        {
            PhotonView view = player.GetComponent<PhotonView>();
            if (view != null && view.ViewID == targetPhotonViewID)
            {
                _targetPlayer = player.transform;
                Debug.Log($"로봇이 플레이어 {targetPhotonViewID} 를 따라갑니다.");
                break;
            }
        }
    }
}
