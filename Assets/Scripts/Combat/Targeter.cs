using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    [SyncVar]
    [SerializeField] private Targetable _target;

    #region Server
    [Command]
    public void CmdSetTarget(Targetable targetObject)
    {
        /*if (!targetObject.TryGetComponent<Targetable>(out Targetable newTarget)) 
        { return;}*/
        //_target = newTarget;
        _target = targetObject;
    }

    [Server]
    public void ClearTarget()
    {
        _target = null;
    }
    #endregion
    
    
    
    #region Client
        
    #endregion
}
