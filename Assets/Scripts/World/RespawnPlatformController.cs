using System.Collections;
using UnityEngine;

// The respawn platform is temporal, this turns it off after some seconds
public class RespawnPlatformController : MonoBehaviour
{
    [SerializeField] float activeTime;
    void OnEnable()
    {
        StartCoroutine(DisableCount());
    }

    IEnumerator DisableCount()
    {
        yield return new WaitForSeconds(activeTime);
        gameObject.SetActive(false);
    }
}
