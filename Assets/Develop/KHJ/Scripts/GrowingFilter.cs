using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;

public class GrowingFilter : XRBaseTargetFilter
{
    public override void Process(IXRInteractor interactor, List<IXRInteractable> targets, List<IXRInteractable> results)
    {
        // Interactor가 감지한 모든 Interactable들을 포함하는 targets(필터링 전) 순회
        foreach (var target in targets)
        {
            Crop crop = target.transform.GetComponent<Crop>();
            if (crop == null)
                continue;

            // 성장도 검사
            if (crop.CurState != Crop.E_CropState.GrowCompleted)
            {
                // 다 자란 작물이 아닐 시 results(필터링 후)에 추가
                results.Add(target);
            }
        }
    }
}
