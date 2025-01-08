using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabletUIController : UIBinder
{
    public enum E_PanelName
    {
        Main, Tutorial, Control, Schedule, SIZE
    }

    [SerializeField] private GameObject[] _panels;

    protected override void Awake()
    {
        base.Awake();

        _panels = new GameObject[((int)E_PanelName.SIZE)];
    }

    private void Start()
    {
        _panels[(int)E_PanelName.Main] = GetUI(E_PanelName.Main.ToString());
        _panels[(int)E_PanelName.Tutorial] = GetUI(E_PanelName.Tutorial.ToString());
        _panels[(int)E_PanelName.Control] = GetUI(E_PanelName.Control.ToString());
        _panels[(int)E_PanelName.Schedule] = GetUI(E_PanelName.Schedule.ToString());

        ReturnToMainPanel();

        // Main Panel
        GetUI<Button>("Main_TutorialButton").onClick.AddListener(OnTutorialButtonClicked);
        GetUI<Button>("Main_ControlButton").onClick.AddListener(OnControlButtonClicked);
        GetUI<Button>("Main_ScheduleButton").onClick.AddListener(OnScheduleButtonClicked);

        // Tutorial Panel
        GetUI<Button>("Tutorial_CloseButton").onClick.AddListener(ReturnToMainPanel);

        // Control Panel
        GetUI<Button>("Control_CloseButton").onClick.AddListener(ReturnToMainPanel);

        // Schedule Panel
        GetUI<Button>("Schedule_CloseButton").onClick.AddListener(ReturnToMainPanel);

    }

    private void ChangePanel(E_PanelName panelName)
    {
        foreach (GameObject panel in _panels)
        {
            panel.SetActive(panel.name == panelName.ToString());
        }
    }

    private void ReturnToMainPanel() => ChangePanel(E_PanelName.Main);

    #region Main Panel
    private void OnTutorialButtonClicked() => ChangePanel(E_PanelName.Tutorial);
    private void OnControlButtonClicked() => ChangePanel(E_PanelName.Control);
    private void OnScheduleButtonClicked() => ChangePanel(E_PanelName.Schedule);
    #endregion

    #region Tutorial Panel
    #endregion

    #region Schedule Panel
    #endregion

    #region Control Panel
    #endregion
}
