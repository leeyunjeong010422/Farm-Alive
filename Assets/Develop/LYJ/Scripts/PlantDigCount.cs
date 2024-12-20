using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantDigCount : MonoBehaviour
{
    [SerializeField] private int _plantDigCount; // 식물이 심어질 땅에 필요한 삽질 횟수

    /// <summary>
    /// 이 식물이 땅에 심어질 수 있는지 groundDigCount 로 확인
    /// </summary>
    public bool CanPlant(int groundDigCount)
    {
        // 땅의 삽질 횟수와 식물의 필요 삽질 횟수를 비교
        return groundDigCount == _plantDigCount;
    }
}
