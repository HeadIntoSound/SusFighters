using UnityEngine;

//  Controls the camera movement when the player is slightly off the stage
public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;                      // A reference to the player
    [SerializeField] float startingDistance = 11f;          // How much the player has to move from the center to trigger camera's movement
    [SerializeField] float maxDisplacement = 6.5f;          // How much the camera can be moved
    [SerializeField] float speed = 1.15f;                   // Camera's movement speed  

    void Start()
    {
        EventManager.Instance.OnLocalCameraSetup.AddListener((t) => { target = t; });
    }

    void Update()
    {
        FollowPlayer();
    }

    // Only moving in the X-axis is needed, so this moves the camera a little to the right or to the left
    void FollowPlayer()
    {
        if (target == null)
            return;
        if (Mathf.Abs(target.position.x) >= startingDistance)
        {
            float dir = target.position.x / Mathf.Abs(target.position.x);
            float x = Mathf.Lerp(transform.position.x, maxDisplacement * dir, speed * Time.deltaTime);
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }
        else if (transform.position.x != 0)
        {
            float x = Mathf.Lerp(transform.position.x, 0, speed * Time.deltaTime);
            x = Mathf.Abs(x) < 0.001f ? 0 : x;
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }
    }
}
