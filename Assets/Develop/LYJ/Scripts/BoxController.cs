using Photon.Voice.Unity.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger Enter 발생");

        if (other.GetComponent<Crop>() != null)
        {
            Debug.Log($"{other.name}이 바구니에 들어왔습니다.");
            other.transform.SetParent(transform);
            return;
        }

        else if (other.CompareTag("Tool"))
        {
            Debug.Log($"{other.name}이 바구니에 들어왔습니다.");
            other.transform.SetParent(transform);
            return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Crop>() != null)
        {
            Debug.Log($"{other.name}이 바구니에서 나갔습니다.");
            other.transform.SetParent(null);
            return;
        }

        else if (other.CompareTag("Tool"))
        {
            Debug.Log($"{other.name}이 바구니에서 나갔습니다.");
            other.transform.SetParent(null);
            return;
        }
    }
}
