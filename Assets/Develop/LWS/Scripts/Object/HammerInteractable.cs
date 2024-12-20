using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerInteractable : MonoBehaviour
{
    private bool _isGrapped = false;

    public void OnGrab()
    {
        _isGrapped = true;
    }

    public void OnRelease()
    {
        _isGrapped = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_isGrapped) return;

        Repair repair = collision.collider.gameObject.GetComponent<Repair>();

        if (repair != null)
        {
            repair.PlusRepairCount();
        }
    }
}
