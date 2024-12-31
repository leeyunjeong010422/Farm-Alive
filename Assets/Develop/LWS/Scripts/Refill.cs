using UnityEngine;
using Photon.Pun;

public class Refill : MonoBehaviourPun
{
    [Header("생성할 프리팹 & 설정")]
    [SerializeField] GameObject _refillPrefab; // 재생성할 프리팹
    [SerializeField] int _maxCount = 10;       // 최대 생성 횟수
    [SerializeField] string _triggerZoneName;


    private int _curCount = 0;      // 현재 몇 번 생성했는지
    private Vector3 _originalPos;       // 시작 위치

    private void Start()
    {
        // 오브젝트의 초기 위치
        _originalPos = transform.position;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("트리거 탈출");
        if (other.gameObject.CompareTag(_triggerZoneName))
        {
            Debug.Log("트리거 존 탈출");
            if (!PhotonNetwork.IsMasterClient)
                return;

            int objectID = GetInstanceID();

            TrySpawnRefill();
        }
    }

    private void TrySpawnRefill()
    {
        
        // 10회 이상이면 재생성 x
        if (_curCount >= _maxCount)
        {
            Debug.Log("추가 소환 x");
            return;
        }
        _curCount++;

        GameObject NewObject = PhotonNetwork.Instantiate(_refillPrefab.name, _originalPos, Quaternion.identity);
        Refill refill = NewObject.GetComponent<Refill>();
        refill._curCount = _curCount;

        Debug.Log($"리필 횟수 {_curCount} / 맥스 횟수 {_maxCount}");
    }
}