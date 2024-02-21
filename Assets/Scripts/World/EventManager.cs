using UnityEngine;
using UnityEngine.Events;

// To communicate data between behaviours anonymously, I like to use events, all handled from a singleton
public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    public class LocalCameraSetup : UnityEvent<Transform> { }
    public LocalCameraSetup OnLocalCameraSetup = new LocalCameraSetup();

    // Used to update UI when a player's health changes its value
    public class HealthChange : UnityEvent<float, int> { }
    public HealthChange OnHealthChange = new HealthChange();

    // Used to update UI when a player scores a point
    public class PointsChange : UnityEvent<int, int> { }
    public PointsChange OnPointsChange = new PointsChange();

    // Used to respawn the player that was thrown off the stage
    public class BlastZoneCollision : UnityEvent<Transform> { }
    public BlastZoneCollision OnBlastZoneCollision = new BlastZoneCollision();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
}
