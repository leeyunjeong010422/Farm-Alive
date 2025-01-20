using UnityEngine;
using Fusion;

public class FusionPlayerSpawn : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{
    [Tooltip("로비 캐릭터")]
    [SerializeField] private GameObject _playerPrefab;
    [Tooltip("로비 캐릭터 스폰 위치")]
    [SerializeField] private Transform _playerSpawn;

    private NetworkObject _spawnedPlayer;

    public void PlayerJoined(PlayerRef player)
    {
        if (player != Runner.LocalPlayer) return;

        Debug.Log("Fusion Player 참가!");
        _spawnedPlayer = Runner.Spawn(_playerPrefab, _playerSpawn.position, _playerSpawn.rotation, player);

        var playerInfo = _spawnedPlayer.GetComponent<PlayerInfo>();
        if (playerInfo != null && Runner.IsServer)
        {
            playerInfo.InitializePlayerInfo();
            playerInfo.UpdateUI();
        }

    }

    public void PlayerLeft(PlayerRef player)
    {
        if(player != Runner.LocalPlayer) return;
        Debug.Log("Fusion Player 퇴장!");

        if (_spawnedPlayer != null)
        {
            Runner.Despawn(_spawnedPlayer);
        }
    }
}
