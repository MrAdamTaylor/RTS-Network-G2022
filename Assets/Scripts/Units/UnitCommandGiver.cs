 using System;
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{
    [SerializeField] private UnitSelectionHandler _unitSelectionHandler = null;

    private Camera _mainCamera;
    [SerializeField] private LayerMask _layerMask;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!Mouse.current.rightButton.wasPressedThisFrame)
        {
            return;
        }
        
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask))
        {
            return;
        }

        if (hit.collider.TryGetComponent<Targetable>(out Targetable targetable))
        {
            if (targetable.isOwned)
            {
                TryMove(hit.point);
                return;
            }

            TryTarget(targetable);
            return;
        }

        TryMove(hit.point);

    }

    private void TryTarget(Targetable target)
    {
        for (int i = 0; i < _unitSelectionHandler.SelectedUnits.Count; i++)
        {
            Unit unit = _unitSelectionHandler.SelectedUnits[i];
            unit.GetTargeter().CmdSetTarget(target);
        }
    }

    private void TryMove(Vector3 hitInfoPoint)
    {
        for (int i = 0; i < _unitSelectionHandler.SelectedUnits.Count; i++)
        {
            Unit unit = _unitSelectionHandler.SelectedUnits[i];
            
            PlayerUnityMovement playerUnityMovement = unit.GetUnitMovement();
            playerUnityMovement.MoveUnit(hitInfoPoint);
        }
    }
}
