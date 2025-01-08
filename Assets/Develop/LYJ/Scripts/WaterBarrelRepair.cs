using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class WaterBarrelRepair : BaseRepairable
{
    [SerializeField] private XRKnob _waterDial;

    protected override void Start()
    {
        base.Start();

        _waterDial = GetComponentInChildren<XRKnob>();
        if (_waterDial == null)
        {
            Debug.LogError("WaterDial이 존재하지 않습니다.");
            return;
        }

    }
}
