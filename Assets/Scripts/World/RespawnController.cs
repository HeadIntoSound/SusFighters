using FishNet.Object;
using UnityEngine;

// Controls respawning when a player hits the blast zone
// The blast zone is the area a player has to collide with in order to die
public class RespawnController : NetworkBehaviour
{
    [SerializeField] Transform respawnPlatform;     // A temporal platform is placed to hold the respawning player

    void Start()
    {
        EventManager.Instance.OnBlastZoneCollision.AddListener(OnBlastZoneCollision);
    }

    // Respawns the player that hit the blast zone, awards points to the other
    void OnBlastZoneCollision(Transform player)
    {
        foreach (var p in ServerController.Instance.players.Values)
        {
            if (p.transform == player)
            {
                RpcRespawnPlayer(p);
                p.health = 0;
            }
            else
            {
                p.points++;
            }
        }
    }

    // Coordinates respawn between all clients
    [ObserversRpc]
    void RpcRespawnPlayer(PlayerController player)
    {
        if (player.IsOwner)
        {
            Vector3 respawnPoint = respawnPlatform.gameObject.activeSelf ? Vector3.zero : respawnPlatform.position;
            player.transform.position = respawnPoint + Vector3.up;
        }
        respawnPlatform.gameObject.SetActive(true);
    }

    void OnDestroy()
    {
        EventManager.Instance.OnBlastZoneCollision.RemoveListener(OnBlastZoneCollision);
    }
}
