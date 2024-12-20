using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repair : MonoBehaviour
{
    public int _maxRepairCount;
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
