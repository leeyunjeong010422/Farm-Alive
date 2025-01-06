using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class RobotController : MonoBehaviour
{
    PhotonView photonView;

    private Canvas _canvas;
    private Button _moveButton;

    private Transform _targetPlayer; // 로봇이 따라갈 유저
    private int targetPhotonViewID = -1; // 따라갈 유저의 PhotonViewID
    [SerializeField] private float followSpeed; // 따라가는 속도
    [SerializeField] private float followDistance; // 따라갈 최소 거리

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

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
            if (distance > followDistance)
            {
                // 로봇이 타겟 플레이어 방향으로 일정 속도로 이동
                transform.position = Vector3.MoveTowards(transform.position, _targetPlayer.position, followSpeed * Time.deltaTime);

                // 로봇이 항상 플레이어를 바라보도록 회전
                Vector3 direction = (_targetPlayer.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f);
            }
        }
    }

    private void OnMoveButtonClicked()
    {
        // 로컬 플레이어의 GameObject를 가져오기
        GameObject localPlayerObject = GameObject.FindWithTag("Player");
        PhotonView localPlayerPhotonView = localPlayerObject.GetComponent<PhotonView>();
        int photonViewID = localPlayerPhotonView.ViewID;

        // PhotonViewID를 RPC로 모든 클라이언트와 동기화
        photonView.RPC(nameof(SyncTargetPlayer), RpcTarget.AllBuffered, photonViewID);
    }

    [PunRPC]
    private void SyncTargetPlayer(int photonViewID)
    {
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
                break;
            }
        }
    }
}
