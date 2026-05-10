using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private UnityEvent _onSelected;
    [SerializeField] private UnityEvent _onDeselected;
    [SerializeField] private PlayerUnityMovement _unitMovement = null;
    [SerializeField] private Targeter _targeter;
    
    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;


    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;

    public Targeter GetTargeter()
    {
        return _targeter;
    }

    public PlayerUnityMovement GetUnitMovement() 
    {
        return _unitMovement;
    }

    #region Server

    public override void OnStartServer()
    {
        ServerOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnUnitDespawned?.Invoke(this);
    }


    #endregion
    
    #region Client

    public override void OnStartClient()
    {
        if (!isClientOnly || !isOwned) { return; }

        AuthorityOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!isClientOnly || !isOwned) { return; }

        AuthorityOnUnitDespawned?.Invoke(this);
    }

    
    [Client]
    public void Select()
    {
        if (!isOwned) return;  
        _onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!isOwned) return;   
        _onDeselected?.Invoke();
    }

    #endregion
}
