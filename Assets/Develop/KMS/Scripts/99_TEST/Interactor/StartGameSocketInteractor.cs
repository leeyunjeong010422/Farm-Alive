using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class StartGameSocketInteractor : XRSocketInteractor
{
    private bool isSelected;

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        if (PhotonNetwork.IsMasterClient && !isSelected)
        {
            isSelected = true;
            StartCoroutine(GameCountdown(3f));
        }
    }

    private IEnumerator GameCountdown(float countdown)
    {
        float remainingTime = countdown;

        while (remainingTime > 0)
        {
            // 메시지 갱신
            MessageDisplayManager.Instance.ShowMessage($"After {(int)remainingTime} seconds, you enter the Game.", 1f, 3f);
            Debug.Log($"After {(int)remainingTime} seconds, you enter the room.");
            yield return new WaitForSeconds(1f);
            remainingTime--;
        }

        Debug.Log("게임 씬 이동 중...");
        SceneLoader.LoadNetworkSceneWithLoading("AssetScene");
    }
}
