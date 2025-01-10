using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class SectionRepair : BaseRepairable
{
    [SerializeField] private XRLever _sectionLever;
    [SerializeField] private SectionMover _sectionMover;

    protected override void Start()
    {
        base.Start();

        _sectionLever = GetComponentInChildren<XRLever>();
        if (_sectionLever == null)
        {
            Debug.LogError("SectionLever가 존재하지 않습니다.");
            return;
        }

        _sectionMover = GetComponentInChildren<SectionMover>();
        if ( _sectionMover == null)
        {
            Debug.Log("SectionMover가 존재하지 않습니다.");
            return;
        }

        _sectionLever.onLeverActivate.AddListener(OnLeverActivated);
    }

    // 레버를 움직여 전조 증상을 해결
    private void OnLeverActivated()
    {
        if (_repair == null || !_repair.IsSymptom) // 전조 증상이 없으면 처리하지 않음
            return;

        SolveSymptom();
    }

    public override bool Broken()
    {
        // 이미 고장이 발생한 상태라면 아무 작업도 하지 않음
        if (_sectionMover.isBroken)
            return false;

        bool isBroken = base.Broken();

        if (isBroken && _sectionMover != null)
        {
            _sectionMover.DisableMovement(); // 섹션 움직임 비활성화
        }

        return isBroken;
    }

    public override void SolveBroken()
    {
        base.SolveBroken();

        if (_sectionMover != null)
        {
            _sectionMover.EnableMovement(); // 고장 수리 후 섹션의 움직임 허용
        }
    }
}
