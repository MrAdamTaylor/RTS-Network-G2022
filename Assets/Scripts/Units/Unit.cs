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

    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;


    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;
    
    
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
        if (!isOwned) return; //Modify  
        _onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!isOwned) return;   //Modify
        _onDeselected?.Invoke();
    }

    #endregion
}
