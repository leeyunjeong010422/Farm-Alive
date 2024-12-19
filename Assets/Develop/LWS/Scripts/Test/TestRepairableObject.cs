using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRepairableObject : MonoBehaviour, IRepairable
{
    private int _repairCount = 0;
    public void Repair()
    {
        _repairCount++;
        Debug.Log($"수리 횟수: {_repairCount}");

        // TODO : 수리 횟수에 따라 고쳐질 로직 작성
    }
}
