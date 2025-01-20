using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class PlantGround : MonoBehaviourPun
{
    public UnityEvent<int, Crop.E_CropState> OnMyPlantUpdated;

    public int section;
    public int ground;

    [SerializeField] private int _digCount; // 필요 삽질 횟수
    private int _currentDigCount = 0; // 현재 삽질 횟수
    private bool _isInteractable = true; // 상호작용 가능 여부

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
        //Debug.Log($"삽질 횟수: {_currentDigCount} / {_digCount}");

        if (_currentDigCount >= _digCount)
        {
            Transform disappearGround = transform.Find("DisappearingGround");
            if (disappearGround != null)
            {
                Destroy(disappearGround.gameObject);
                //Debug.Log("DisappearingGround가 삭제되었습니다.");
            }

            _isInteractable = false; // 추가 삽질 방지
        }
    }

    /// <summary>
    /// 현재 땅이 특정 식물이 심어질 수 있는지 확인
    /// </summary>
    public bool CanPlant(Crop plant)
    {
        if (plant == null) return false;

        // 식물의 요구 삽질 횟수와 땅의 DigCount 비교
        return plant.DigCount <= _digCount;
    }
}
