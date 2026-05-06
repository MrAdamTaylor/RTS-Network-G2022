using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerTerminal :NetworkBehaviour
{
    private static PlayerTerminal _instance = null;

    public static PlayerTerminal Instance { get { return _instance; } }

    private void Start()
    {
        if (isClient && isOwned)
        {
            _instance = this;
            CmdSpawnPLayer();
        }
    }

    [Command]
    public void CmdSpawnPLayer(NetworkConnectionToClient sender = null)
    {
        int id = sender.connectionId;
        var prefab = NetworkManager.singleton.spawnPrefabs[0];
        var player = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        NetworkServer.Spawn(player, player);
    }
}
