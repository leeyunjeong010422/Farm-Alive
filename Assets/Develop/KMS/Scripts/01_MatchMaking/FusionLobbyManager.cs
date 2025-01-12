using Photon.Pun;
using UnityEngine;
using System.Threading.Tasks;
using Fusion;
using Unity.VisualScripting;
using UnityEngine.UI;

public class FusionLobbyManager : MonoBehaviour
{
    public NetworkRunner networkRunnerPrefab;
    public string lobbyName = "FusionLobby";

    private async void Start()
    {
        if(!networkRunnerPrefab)
        {
            Debug.Log("networkRunnerPrefab이 존재하지 않습니다.");
            Debug.Log("networkRunnerPrefab을 찾고 있는중입니다.");
            networkRunnerPrefab = FindObjectOfType<NetworkRunner>();
        }

        // Fusion 로비 시작
        await StartFusionLobby();
    }

    private async Task StartFusionLobby()
    {
        networkRunnerPrefab.ProvideInput = true;

        var startResult = await networkRunnerPrefab.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = lobbyName
        });

        if (startResult.Ok)
        {
            Debug.Log("Fusion 로비 시작 성공");
        }
        else
        {
            Debug.LogError($"Fusion 로비 시작 실패: {startResult.ShutdownReason}");
        }
    }
}
