using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairStateUpdater : MonoBehaviour
{
    [SerializeField] private int _facilityID;
    [SerializeField] private GameObject _symptom;
    [SerializeField] private GameObject _broken;

    private void Start()
    {
        SubscribeFacility();
    }

    private void SubscribeFacility()
    {
        Repair[] gameObjects = FindObjectsOfType<Repair>();
        foreach (Repair repair in gameObjects)
        {
            if (_facilityID == repair.ID)
            {
                Debug.Log($"{CSVManager.Instance.Facilities[repair.ID].facility_name}와 태블릿 UI 연결");
                repair.OnSymptomRaised.AddListener(ShowSymptom);
                repair.OnSymptomSolved.AddListener(HideSymptom);
                repair.OnBrokenRaised.AddListener(HideSymptom);
                repair.OnBrokenRaised.AddListener(ShowBroken);
                repair.OnBrokenSolved.AddListener(HideBroken);
            }
        }
    }

    private void ShowSymptom() => _symptom.SetActive(true);
    private void HideSymptom() => _symptom?.SetActive(false);
    private void ShowBroken() => _broken.SetActive(true);
    private void HideBroken() => _broken.SetActive(false);
}