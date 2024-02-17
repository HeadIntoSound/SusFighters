using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;
using System;
using System.Linq;

public class GameManager : NetworkBehaviour
{
    public override void OnSpawnServer(NetworkConnection connection)
    {
        base.OnSpawnServer(connection);
    }
}
