using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repair : MonoBehaviour
{
    [Tooltip("몇 번 때려야 고쳐지는지")]
    [SerializeField] int _maxRepairCount;
    private int _curRepairCount = 0;

    public void PlusRepairCount()
    {
        _curRepairCount++;

        if (_curRepairCount == _maxRepairCount)
        {
            // TODO 수리 완료 INVOKE
        }
    }
}
