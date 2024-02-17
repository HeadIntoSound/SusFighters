using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class ServerController : NetworkBehaviour
{
    public List<int> playerList = new List<int>();
    [SerializeField] List<PlayerController> playerControllers = new List<PlayerController>();
    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        if (!base.IsServer)
        {
            Destroy(this.gameObject);
        }
    }

    public override void OnSpawnServer(NetworkConnection connection)
    {
        base.OnSpawnServer(connection);

        // Checks if there's more than 2 players, automatically kicks any new connection if true
        if (playerList.Count >= 2)
        {
            connection.Disconnect(true);
            return;
        }

        playerList.Add(connection.ClientId);
        var player = connection.FirstObject.GetComponent<PlayerController>();
        playerControllers.Add(player);
        // Sets player 2 color
        if (playerList.Count > 1)
        {
            RpcChangeColor(player);
            for (int i = 0; i < playerControllers.Count; i++)
            {
                int nextIndex = i > 0 ? 0 : 1;
                RpcSetTarget(playerControllers[i], playerControllers[nextIndex].transform);
            }
        }
    }

    // Tells each client to change the color of the player 2
    [ObserversRpc]
    private void RpcChangeColor(PlayerController player)
    {
        player.skinController.SetColor(1);
    }

    [ObserversRpc]
    void RpcSetTarget(PlayerController player, Transform target)
    {
        player.target = target;
    }

    public override void OnDespawnServer(NetworkConnection connection)
    {
        base.OnDespawnServer(connection);
        playerList.Remove(connection.ClientId);
    }

}
