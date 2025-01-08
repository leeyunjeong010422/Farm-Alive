using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class NutrientBarrelRepair : BaseRepairable
{
    [SerializeField] private XRKnob _nutrientDial;

    protected override void Start()
    {
        base.Start();

        _nutrientDial = GetComponentInChildren<XRKnob>();
        if (_nutrientDial == null)
        {
            Debug.LogError("NutrientDial이 존재하지 않습니다.");
            return;
        }
    }
}
