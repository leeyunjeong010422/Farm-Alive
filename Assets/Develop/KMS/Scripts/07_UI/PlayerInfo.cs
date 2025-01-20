using Fusion;
using System;
using TMPro;
using UnityEngine;

public class PlayerInfo : NetworkBehaviour
{
    [Header("UI References")]
    public TMP_Text nickNameText;
    public TMP_Text stageText;
    public TMP_Text starText;

    [Networked] public string PlayerNickName { get; set; }
    [Networked] public string HighStage { get; set; }
    [Networked] public int Stars { get; set; }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            InitializePlayerInfo();
        }
        UpdateUI();
    }

    /// <summary>
    /// 플레이어 정보를 초기화
    /// </summary>
    private void InitializePlayerInfo()
    {
        PlayerNickName = FirebaseManager.Instance.GetNickName();
        HighStage = FirebaseManager.Instance.GetHighStage();

        if (Enum.TryParse(HighStage, out E_StageMode stageMode))
        {
            int stageID = (int)stageMode;
            var stageData = FirebaseManager.Instance.GetCachedStageData(stageID);

            Stars = stageData != null ? stageData.stars : 0;
        }
        else
        {
            Debug.LogWarning($"HighStage '{HighStage}'를 파싱할 수 없습니다.");
            Stars = 0;
        }
    }

    /// <summary>
    /// UI를 업데이트.
    /// </summary>
    public void UpdateUI()
    {
        if (nickNameText)
            nickNameText.text = $"{PlayerNickName}";

        if (stageText)
            stageText.text = $"최고 스테이지: {HighStage}";

        if (starText)
            starText.text = $"스타 점수: {Stars}";
    }
}
