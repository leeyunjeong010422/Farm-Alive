using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;
using UnityEngine;
using Fusion;

public class PlayerInfo : NetworkBehaviour
{
    [Header("UI References")]
    public TMP_Text nickNameText;
    public TMP_Text stageText;
    public GameObject infoPanel;

    [Networked] public string PlayerNickName { get; set; }
    [Networked] public string HighStage { get; set; }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            InitializePlayerInfo();
        }
        UpdateUI();
    }

    private void InitializePlayerInfo()
    {
        PlayerNickName = FirebaseManager.Instance.GetNickName();
        HighStage = FirebaseManager.Instance.GetHighStage();
    }

    public void UpdateUI()
    {
        if (nickNameText)
            nickNameText.text = $"{PlayerNickName}";

        if (stageText)
            stageText.text = $"Stage: {HighStage}";
    }

    public void ShowInfo()
    {
        if (infoPanel)
            infoPanel.SetActive(true);
    }

    public void HideInfo()
    {
        if (infoPanel)
            infoPanel.SetActive(false);
    }
}
