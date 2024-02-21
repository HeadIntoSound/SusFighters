using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using System.Linq;

// Controls data only available on the server
public class ServerController : NetworkBehaviour
{
    public static ServerController Instance;    // Singleton
    public Dictionary<int, PlayerController> players = new Dictionary<int, PlayerController>(); // Dictionary with the player slot and the player controller

    // Creates the singleton instance
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    // Deletes the object if on the client
    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        if (!base.IsServer)
        {
            Destroy(this.gameObject);
        }
    }

    // Avoids having more than 2 connections, initializes each player's data on connection 
    public override void OnSpawnServer(NetworkConnection connection)
    {
        base.OnSpawnServer(connection);

        if (players.Count >= 2)
        {
            connection.Disconnect(true);
            return;
        }

        players.Add(players.Count, connection.FirstObject.GetComponent<PlayerController>());

        foreach (var p in players)
        {
            if (players.Count > 1)
                RpcSetTarget(p.Value, players[p.Key == 0 ? 1 : 0]);
            RpcChangeColor(p.Value, p.Key);
            RpcSetUI(p.Value, p.Key);
        }
    }

    // Tells each client to change the color of the player 2
    [ObserversRpc]
    private void RpcChangeColor(PlayerController player, int playerSlot)
    {
        player.skinController.SetColor(playerSlot);
    }

    // Sets the target of each player
    [ObserversRpc]
    void RpcSetTarget(PlayerController player, PlayerController target)
    {
        player.target = target;
    }

    // Initializes UI
    [ObserversRpc]
    void RpcSetUI(PlayerController player, int playerSlot)
    {
        player.InitializeUI(playerSlot);
    }

    // Removes disconnected players
    public override void OnDespawnServer(NetworkConnection connection)
    {
        // Looks for the disconnected player and removes it from the dictionary
        foreach (var p in players)
        {
            if (p.Value == connection.FirstObject.GetComponent<PlayerController>())
            {
                players.Remove(p.Key);
                break;
            }
        }
        base.OnDespawnServer(connection);
        print(players.Count);
    }

}
