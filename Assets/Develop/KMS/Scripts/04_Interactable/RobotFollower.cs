using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RobotFollower : MonoBehaviour
{
    [Header("로봇 이동 세팅")]
    [Tooltip("추적할 플레이어")]
    public Transform targetPlayer;

    [Tooltip("플레이어와 유지할 거리")]
    public float followDistance = 2.0f;

    [Tooltip("로봇의 이동 속도")]
    public float speed = 3.5f;

    [Tooltip("로봇의 초기 위치")]
    public Transform initialPosition;

    [Tooltip("허용된 이동 영역")]
    public Collider allowedArea;

    [Tooltip("추적여부")]
    [SerializeField] private bool _isFollowing = false;
    private NavMeshAgent _agent;
    private PhotonView _photonView;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _photonView = GetComponent<PhotonView>();

        if (!_photonView)
        {
            Debug.LogError("PhotonView가 이 오브젝트에 없습니다. PhotonView를 추가하세요.");
        }

        _agent.speed = speed;
    }

    private void Update()
    {
        if (_isFollowing && targetPlayer)
        {
            Vector3 targetPosition = new Vector3(targetPlayer.position.x, transform.position.y, targetPlayer.position.z);
            float distance = Vector3.Distance(transform.position, targetPosition);

            if (allowedArea && !allowedArea.bounds.Contains(targetPosition))
            {
                StopFollowingAndReturnToInitial();
                return;
            }

            if (distance > followDistance)
            {
                SetDestination(targetPosition);
            }
            else
            {
                SetDestination(transform.position);
            }
        }
    }

    [PunRPC]
    public void StartFollowingRPC(int playerViewID)
    {
        PhotonView playerPhotonView = PhotonView.Find(playerViewID);
        if (playerPhotonView)
        {
            GameObject player = playerPhotonView.gameObject;
            targetPlayer = player.transform;
            _isFollowing = true;
            Debug.Log($"로봇이 {player.name}을(를) 따라가기 시작합니다.");
        }
    }

    [PunRPC]
    public void StopFollowingRPC()
    {
        _isFollowing = false;
        targetPlayer = null;
        SetDestination(transform.position);
        Debug.Log("로봇이 추적을 멈췄습니다.");
    }

    private void StopFollowingAndReturnToInitial()
    {
        _isFollowing = false;
        targetPlayer = null;

        if (initialPosition)
        {
            SetDestination(initialPosition.position);
            Debug.Log("플레이어가 허용된 구역을 벗어났습니다. 로봇이 초기 위치로 돌아갑니다.");
        }
        else
        {
            Debug.LogWarning("초기 위치가 설정되지 않았습니다.");
        }
    }

    private void SetDestination(Vector3 destination)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _agent.SetDestination(destination);
            _photonView.RPC("SyncDestination", RpcTarget.Others, destination);
        }
    }

    [PunRPC]
    private void SyncDestination(Vector3 destination)
    {
        _agent.SetDestination(destination);
    }
}
