using System.Collections.Generic;
using UnityEngine;

public static class ProbabilityHelper
{
    /// <summary>
    /// 당첨 여부 확인 함수
    /// </summary>
    /// <param name="percentage">당첨 확률</param>
    /// <returns>당첨 여부</returns>
    public static bool Draw(int percentage)
    {
        return Random.Range(0, 100) < percentage;
    }

    /// <summary>
    /// 리스트 내부의 임의의 원소 반환 함수
    /// </summary>
    /// <typeparam name="T">element type</typeparam>
    /// <param name="list">list</param>
    /// <returns>임의의 원소</returns>
    public static T Draw<T>(List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    /// <summary>
    /// 배열 내부의 임의의 원소 반환 함수
    /// </summary>
    /// <typeparam name="T">element type</typeparam>
    /// <param name="array">array</param>
    /// <returns>임의의 원소</returns>
    public static T Draw<T>(T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }
}
