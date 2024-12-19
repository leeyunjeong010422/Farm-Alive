using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUpgradableObject : MonoBehaviour, IUpgradable
{
    private int _upgradeCount = 0;
    public void Upgrade()
    {
        _upgradeCount++;
        Debug.Log($"업그레이드 횟수 : {_upgradeCount}");

        // TODO : 업그레이드 횟수에 따라 진행할 로직 작성
    }
}
