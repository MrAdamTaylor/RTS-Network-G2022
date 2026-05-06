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
    [SerializeField] private PlayerUnityMovement _unitMovement = null; //Modify

    public PlayerUnityMovement GetUnitMovement() //Modify
    {
        return _unitMovement;
    }

    #region Client

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
