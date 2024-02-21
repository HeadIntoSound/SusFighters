using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float startingDistance = 10f;
    [SerializeField] float maxDisplacement = 5f;
    [SerializeField] float speed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.OnLocalCameraSetup.AddListener((t) => { target = t; });
    }

    // Update is called once per frame
    void Update()
    {
        FollowPlayer();
    }

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
