using System;
using TMPro;
using UnityEngine;
using Fusion;

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
            RpcSetPlayerInfo(PlayerNickName, HighStage, Stars);
        }
        else
        {
            RpcRequestPlayerInfo();
        }
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

        UpdateUI();
    }

    /// <summary>
    /// RPC를 사용하여 플레이어 정보를 동기화
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcSetPlayerInfo(string playerName, string highStage, int stars)
    {
        PlayerNickName = playerName;
        HighStage = highStage;
        Stars = stars;

        UpdateUI();
    }

    /// <summary>
    /// 서버에 플레이어 정보를 요청
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcRequestPlayerInfo()
    {
        RpcSendPlayerInfo(PlayerNickName, HighStage, Stars);
    }

    /// <summary>
    /// 클라이언트들에게 플레이어 정보 전송
    /// </summary>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcSendPlayerInfo(string playerName, string highStage, int stars)
    {
        PlayerNickName = playerName;
        HighStage = highStage;
        Stars = stars;

        RpcSetPlayerInfo(PlayerNickName, HighStage, Stars);
    }

    /// <summary>
    /// UI를 업데이트
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