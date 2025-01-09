using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRepairable
{
    void Symptom();         // 전조 증상 처리
    bool Broken();          // 고장 발생 처리
    void SolveSymptom();    // 전조 증상 해결
    void SolveBroken();     // 고장 해결
    bool IsBroken();        // 고장 상태 반환
}
