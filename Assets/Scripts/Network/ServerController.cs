using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;

// Controls data only available on the server
public class ServerController : NetworkBehaviour
{
    public static ServerController Instance;                                // Singleton
    public List<PlayerController> players = new List<PlayerController>();   // A list of all connected players
    
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

        players.Add(connection.FirstObject.GetComponent<PlayerController>());

        for (int i = 0; i < players.Count; i++)
        {
            int nextIndex = i > 0 ? 0 : 1;
            if (players.Count > 1)
                RpcSetTarget(players[i], players[nextIndex]);
            RpcChangeColor(players[i], i);
            RpcSetUI(players[i], i);
        }
    }

    // Tells each client to change the color of the player 2
    [ObserversRpc]
    private void RpcChangeColor(PlayerController player, int index)
    {
        player.skinController.SetColor(index);
    }

    // Sets the target of each player
    [ObserversRpc]
    void RpcSetTarget(PlayerController player, PlayerController target)
    {
        player.target = target;
    }

    // Initializes UI
    [ObserversRpc]
    void RpcSetUI(PlayerController player, int index)
    {
        player.InitializeUI(index);
    }

    // Removes disconnected players
    public override void OnDespawnServer(NetworkConnection connection)
    {
        base.OnDespawnServer(connection);
        players.RemoveAt(connection.ClientId % 2 == 0 ? 0 : 1);
    }

}
