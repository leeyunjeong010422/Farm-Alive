using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShovelInteractor : MonoBehaviour
{
    private bool _isShovelTouchingGround = false;

    private void OnTriggerEnter(Collider other)
    {
        // 삽이 DisappearGround와 닿으면
        if (other.CompareTag("DisappearGround"))
        {
            _isShovelTouchingGround = true; // 삽이 땅에 닿음
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 삽이 DisappearGround에서 떨어지면
        if (other.CompareTag("DisappearGround"))
        {
            if (_isShovelTouchingGround)
            {
                Destroy(other.gameObject); // DisappearGround 오브젝트 삭제
                _isShovelTouchingGround = false; // 상태 초기화
            }
        }
    }
}
