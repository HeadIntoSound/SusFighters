#if UNITY_EDITOR
using UnityEngine;
using FishNet.Transporting.Tugboat;
using FishNet;
using FishNet.Transporting;

// For testing only, controls the server and client connections
public class ConnectionController : MonoBehaviour
{
    [SerializeField] Tugboat tugboat;

    void OnEnable()
    {
        InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnectionState;
    }



    void OnDisable()
    {
        InstanceFinder.ClientManager.OnClientConnectionState -= OnClientConnectionState;
    }

    void Start()
    {
        if (ParrelSync.ClonesManager.IsClone())
        {
            tugboat.StartConnection(false);
        }
        else
        {
            tugboat.StartConnection(true);
            tugboat.StartConnection(false);
        }
    }
    private void OnClientConnectionState(ClientConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Stopping)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }
}
#endif