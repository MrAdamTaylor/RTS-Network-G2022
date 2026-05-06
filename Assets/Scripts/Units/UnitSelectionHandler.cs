using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    
    [SerializeField] private LayerMask _layerMask;
    
    public List<Unit> SelectedUnits = new List<Unit>(); 

    private Camera _mainCamera;
    
    private void Start()
    {
        _mainCamera = Camera.main;
    }
    
    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleLeftClick();
        }
    }
    
    private void HandleLeftClick() //Modify 
    {
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask) &&
            hit.collider.TryGetComponent<Unit>(out Unit unit) &&
            unit.isOwned)  //Modify 
        {
            if (SelectedUnits.Contains(unit))
            {
                unit.Deselect();
                SelectedUnits.Remove(unit);
            }
            else
            {
                if (!Keyboard.current.leftCtrlKey.isPressed) //Modify  
                    ClearSelection();  //Modify 
        
                SelectedUnits.Add(unit);
                unit.Select();
            }
        }
        else
        {
            ClearSelection();
        }
    }
    
    private void ClearSelection()
    {
        foreach (Unit unit in SelectedUnits)
        {
            if (unit != null)
                unit.Deselect();
        }
        SelectedUnits.Clear();
    }
}