using UnityEngine;

// Controls the blast zone to trigger an event if respawn is needed
public class BlastZoneController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.PLAYER))
            EventManager.Instance.OnBlastZoneCollision.Invoke(other.transform);
    }
}
