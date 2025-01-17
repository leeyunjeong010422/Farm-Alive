using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticDoor : MonoBehaviourPun
{
    [Tooltip("문이 열릴 위치")]
    [SerializeField] Transform _destinationPos;

    private Vector3 _initPos;

    [SerializeField] float _moveSpeed = 5f;

    [Tooltip("문이 열리고 대기시간")]
    [SerializeField] float _delay = 2f;

    private Coroutine _moveCoroutine;

    private void Start()
    {
        _initPos = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);

            photonView.RPC(nameof(RPC_MoveDoor), RpcTarget.All, _destinationPos);
        }
    }

    [PunRPC]
    private void RPC_MoveDoor(Vector3 targetPos)
    {
        _moveCoroutine = StartCoroutine(MoveDoor(targetPos));
    }

    private IEnumerator MoveDoor(Vector3 targetPos)
    {
        while ((transform.position - targetPos).sqrMagnitude > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, _moveSpeed * Time.deltaTime);
            yield return null;
        }

        // 이동 완료
        _moveCoroutine = null;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);

            photonView.RPC(nameof(RPC_CloseDoor), RpcTarget.All, _destinationPos);
        }
    }

    [PunRPC]
    private void RPC_CloseDoor()
    {
        _moveCoroutine = StartCoroutine(WaitAndCloseDoor());
    }

    private IEnumerator WaitAndCloseDoor()
    {
        yield return new WaitForSeconds(_delay);

        _moveCoroutine = StartCoroutine(MoveDoor(_initPos));
    }
}