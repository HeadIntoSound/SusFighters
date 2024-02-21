using UnityEngine;

// Controls the blast zone to trigger an event if respawn is needed
// The blast zone is the area a player has to collide with in order to die
public class BlastZoneController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.PLAYER))
            EventManager.Instance.OnBlastZoneCollision.Invoke(other.transform);
    }
}
