using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabletUIController : UIBinder
{
    public enum E_PanelName
    {
        Main, Guide, Section, Schedule, SIZE
    }

    [SerializeField] private GameObject[] _panels;

    protected override void Awake()
    {
        base.Awake();

        _panels = new GameObject[((int)E_PanelName.SIZE)];
    }

    private void Start()
    {
        StageManager.Instance.OnGameStarted.AddListener(ReturnToMainPanel);

        _panels[(int)E_PanelName.Main] = GetUI(E_PanelName.Main.ToString());
        _panels[(int)E_PanelName.Guide] = GetUI(E_PanelName.Guide.ToString());
        _panels[(int)E_PanelName.Section] = GetUI(E_PanelName.Section.ToString());
        _panels[(int)E_PanelName.Schedule] = GetUI(E_PanelName.Schedule.ToString());

        // Main Panel
        GetUI<Button>("Main_GuideButton").onClick.AddListener(OnGuideButtonClicked);
        GetUI<Button>("Main_SectionButton").onClick.AddListener(OnSectionButtonClicked);
        GetUI<Button>("Main_ScheduleButton").onClick.AddListener(OnScheduleButtonClicked);

        // Guide Panel
        GetUI<Button>("Guide_CloseButton").onClick.AddListener(ReturnToMainPanel);
        GetUI<Button>("CropTab_FacilityTabButton").onClick.AddListener(OnFacilityTabButtonClicked);
        GetUI<Button>("FacilityTab_CropTabButton").onClick.AddListener(OnCropTabButtonClicked);

        // Section Panel
        GetUI<Button>("Section_CloseButton").onClick.AddListener(ReturnToMainPanel);

        // Schedule Panel
        GetUI<Button>("Schedule_CloseButton").onClick.AddListener(ReturnToMainPanel);
    }

    public void ReturnToMainPanel() => ChangePanel(E_PanelName.Main);

    private void ChangePanel(E_PanelName panelName)
    {
        foreach (GameObject panel in _panels)
        {
            panel.SetActive(panel.name == panelName.ToString());
        }
    }

    #region Main Panel
    private void OnGuideButtonClicked() => ChangePanel(E_PanelName.Guide);
    private void OnSectionButtonClicked() => ChangePanel(E_PanelName.Section);
    private void OnScheduleButtonClicked() => ChangePanel(E_PanelName.Schedule);
    #endregion

    #region Guide Panel
    private void OnCropTabButtonClicked()
    {
        GetUI("CropTab").SetActive(true);
        GetUI("FacilityTab").SetActive(false);
    }

    private void OnFacilityTabButtonClicked()
    {
        GetUI("FacilityTab").SetActive(true);
        GetUI("CropTab").SetActive(false);
    }
    #endregion

    #region Schedule Panel
    #endregion

    #region Section Panel
    #endregion
}
