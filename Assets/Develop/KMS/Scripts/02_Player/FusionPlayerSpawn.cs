using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem.XR;

public class FusionPlayerSpawn : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{
    [Tooltip("로비 캐릭터")]
    [SerializeField] private GameObject _playerPrefab;
    [Tooltip("로비 캐릭터 스폰 위치")]
    [SerializeField] private Transform _playerSpawn;

    public void PlayerJoined(PlayerRef player)
    {
        if (player != Runner.LocalPlayer) return;

        Debug.Log("Fusion Player 참가!");
        Runner.Spawn(_playerPrefab, _playerSpawn.position, _playerSpawn.rotation, player);
    }

    public void PlayerLeft(PlayerRef player)
    {
        if(player != Runner.LocalPlayer) return;
        Debug.Log("Fusion Player 퇴장!");
    }
}
