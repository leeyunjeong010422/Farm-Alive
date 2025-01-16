using GameData;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CropInteractable : XRGrabInteractable
{
    public PlantGround Ground { 
        get {
            if (interactorsSelecting.Count <= 0)
                return null;
            PlantGround plantGround = interactorsSelecting[0].transform.GetComponent<PlantGround>();
            return plantGround != null ? plantGround : null;
        } 
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        Crop crop = GetComponent<Crop>();
        if (crop.CurState == Crop.E_CropState.Waste)
        {
            gameObject.SetActive(false);
            return;
        }

        Debug.Log("CropInteractable OnSelectEntered");
        base.OnSelectEntered(args);

        PlantGround plantGround = args.interactorObject.transform.GetComponent<PlantGround>();
        if (plantGround != null)
        {
            Debug.Log("planted into ground");

            SectionManager.Instance.Sections[SectionManager.Instance.CurSection, plantGround.ground] = crop;
            crop.ChangeState(Crop.E_CropState.GrowStopped);
            crop.ReactToEvents();
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        PlantGround plantGround = args.interactorObject.transform.GetComponent<PlantGround>();
        if (plantGround != null)
        {
            SectionManager.Instance.Sections[SectionManager.Instance.CurSection, plantGround.ground] = null;
            plantGround.OnMyPlantUpdated?.Invoke(0, Crop.E_CropState.SIZE);
        }
    }

    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        if (interactor is XRSocketInteractor)
            return base.IsSelectableBy(interactor);

        Crop crop = GetComponent<Crop>();
        if (crop.CurState == Crop.E_CropState.Seeding || crop.CurState == Crop.E_CropState.GrowCompleted || crop.CurState == Crop.E_CropState.Waste)
            return base.IsSelectableBy(interactor);
        else
            return false;
    }
}
