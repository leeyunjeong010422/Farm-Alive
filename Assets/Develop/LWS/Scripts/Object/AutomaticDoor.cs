using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
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
        if (!other.gameObject.CompareTag("Player"))
            return;

        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);

        photonView.RPC(nameof(RPC_MoveDoor), RpcTarget.All, true);

    }

    [PunRPC]
    private void RPC_MoveDoor(bool isOpen)
    {
        Vector3 targetPos = isOpen? _destinationPos.position : _initPos;

        _moveCoroutine = StartCoroutine(MoveDoor(targetPos));
    }

    private IEnumerator MoveDoor(Vector3 targetPos)
    {
        while ((transform.position - _destinationPos.position).sqrMagnitude > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, _moveSpeed * Time.deltaTime);
            yield return null;
        }

        // 이동 완료
        _moveCoroutine = null;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);

        photonView.RPC(nameof(RPC_CloseDoor), RpcTarget.All);

    }

    [PunRPC]
    private void RPC_CloseDoor()
    {
        _moveCoroutine = StartCoroutine(WaitAndCloseDoor());
    }

    private IEnumerator WaitAndCloseDoor()
    {
        yield return new WaitForSeconds(_delay);

        photonView.RPC(nameof(RPC_MoveDoor), RpcTarget.All, false);
    }
}