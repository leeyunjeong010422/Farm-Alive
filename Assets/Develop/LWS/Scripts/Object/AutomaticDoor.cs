using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

public class AutomaticDoor : MonoBehaviourPun
{
    [Tooltip("문이 열릴 위치")]
    [SerializeField] Transform _destinationPos;
    [SerializeField] float _moveSpeed = 5f;

    [Tooltip("문이 열리고 대기시간")]
    [SerializeField] float _delay = 2f;

    private Vector3 _initPos;
    private bool _isOpen = false;
    private int _playerInside = 0;

    private Coroutine _moveCoroutine;
    private Coroutine _closeDoorCoroutine;

    private void Start()
    {
        _initPos = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (other.CompareTag("Player"))
        {
            _playerInside++;

            if (!_isOpen)
            {
                if (_closeDoorCoroutine != null)
                {
                    StopCoroutine(_closeDoorCoroutine);
                    _closeDoorCoroutine = null;
                }

                photonView.RPC(nameof(RPC_OpenDoor), RpcTarget.All);
            }
        }
    }

    [PunRPC]
    private void RPC_OpenDoor()
    {
        if (_isOpen)
            return;

        _isOpen = true;

        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);

        _moveCoroutine = StartCoroutine(MoveDoor(_destinationPos.position));
    }

    private IEnumerator MoveDoor(Vector3 targetPos)
    {
        while ((transform.position - targetPos).sqrMagnitude > 0.001f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                _moveSpeed * Time.deltaTime
            );
            yield return null;
        }
        _moveCoroutine = null;
    }

    private void OnTriggerExit(Collider other)
    {
        if(!PhotonNetwork.IsMasterClient)
            return;

        if (other.CompareTag("Player"))
        {
            _playerInside--;

            if ( _playerInside < 0 )
                _playerInside = 0;

            if (_playerInside <= 0 )
            {
                if (_closeDoorCoroutine != null)
                {
                    StopCoroutine(_closeDoorCoroutine);
                    _closeDoorCoroutine = null;
                }
                _closeDoorCoroutine = StartCoroutine(WaitAndCloseDoor());
            }
        }
    }

    [PunRPC]
    private void RPC_CloseDoor()
    {
        if (!_isOpen) return;
        _isOpen = false;

        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);

        _moveCoroutine = StartCoroutine(MoveDoor(_initPos));
    }

    private IEnumerator WaitAndCloseDoor()
    {
        yield return new WaitForSeconds(_delay);

        if (_playerInside <= 0 && _isOpen)
        {
            photonView.RPC(nameof(RPC_CloseDoor), RpcTarget.All);
        }
        _closeDoorCoroutine = null;
    }
}