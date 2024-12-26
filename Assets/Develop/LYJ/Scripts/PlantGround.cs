using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlantGround : MonoBehaviourPun
{
    [SerializeField] private int _digCount; // 필요 삽질 횟수
    private int _currentDigCount = 0; // 현재 삽질 횟수
    private bool _isInteractable = true; // 상호작용 가능 여부
    private XRSocketInteractor _socketInteractor;

    private void Awake()
    {
        _socketInteractor = GetComponentInChildren<XRSocketInteractor>();

        if (_socketInteractor != null)
        {
            _socketInteractor.enabled = false; // 초기 상태 비활성화
            _socketInteractor.hoverEntered.AddListener(OnHoverEntered); // 아이템 삽입 이벤트 연결
        }
        else
        {
            Debug.LogWarning("XRSocketInteractor를 찾을 수 없습니다.");
        }
    }

    /// <summary>
    /// 삽질 관리 메소드
    /// </summary>
    public void Dig()
    {
        if (!_isInteractable) return;

        // Dig() 동작을 네트워크에서 동기화
        photonView.RPC(nameof(SyncDig), RpcTarget.AllBuffered);
    }

    /// <summary>
    /// 삽질 관리 메소드
    /// </summary>
    [PunRPC]
    public void SyncDig()
    {
        if (!_isInteractable) return;

        _currentDigCount++;
        Debug.Log($"삽질 횟수: {_currentDigCount} / {_digCount}");

        if (_currentDigCount >= _digCount)
        {
            Transform disappearGround = transform.Find("DisappearingGround");
            if (disappearGround != null)
            {
                Destroy(disappearGround.gameObject);
                Debug.Log("DisappearingGround가 삭제되었습니다.");
            }

            _isInteractable = false; // 추가 삽질 방지

            // 소켓 인터렉터 활성화 (조건 검사는 OnHoverEntered에서 수행)
            if (_socketInteractor != null)
            {
                _socketInteractor.enabled = true;
                _socketInteractor.showInteractableHoverMeshes = true;
                Debug.Log("소켓 인터렉터가 활성화되었습니다.");
            }
        }
    }

    // 오브젝트를 땅에 가져다 댔을 때 조건 검사
    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        // 가져다댄 오브젝트에서 PlantDigCount 컴포넌트를 가져옴
        // TODO: 식물 스크립트로 코드를 옮긴다면 밑에 코드도 수정해 주어야 함
        Crop plant = args.interactableObject.transform.GetComponent<Crop>();

        if (plant == null)
        {
            _socketInteractor.enabled = false;
            return;
        }

        // 조건 검사: 땅의 _digCount와 식물의 _plantDigCount가 같아야 함
        if (!CanPlant(plant))
        {
            _socketInteractor.enabled = false;
        }
        else
        {
            _socketInteractor.enabled = true;
        }
    }

    /// <summary>
    /// 현재 땅이 특정 식물이 심어질 수 있는지 확인
    /// </summary>
    public bool CanPlant(Crop plant)
    {
        if (plant == null) return false;

        // 식물의 요구 삽질 횟수와 땅의 DigCount 비교
        return plant.DigCount == _digCount;
    }
}
