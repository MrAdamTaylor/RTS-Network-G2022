using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField] private RectTransform _unitSelectionArea = null;
    
    [SerializeField] private LayerMask _layerMask;
    
    public List<Unit> SelectedUnits = new List<Unit>(); 

    private Camera _mainCamera;
    private RTSPlayer _player;
    private Vector2 _startPosition;
    
    private void Start()
    {
        _mainCamera = Camera.main;
        _player = NetworkClient.connection?.identity?.GetComponent<RTSPlayer>();
    }
    
    private void Update()
    {
        if (_player == null)
        {
            _player = NetworkClient.connection?.identity?.GetComponent<RTSPlayer>();
            if (_player == null) return; 
        }
        
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleLeftClick();
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }
    }

    private void UpdateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        
        float areaWidth = mousePosition.x - _startPosition.x;
        float areaHeight = mousePosition.y - _startPosition.y;
        
        _unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
        _unitSelectionArea.anchoredPosition = _startPosition + new Vector2(areaWidth / 2, areaHeight / 2);
    }

    private void StartSelectionArea()
    {
        _unitSelectionArea.gameObject.SetActive(true);
        _startPosition = Mouse.current.position.ReadValue();
    }

    private void HandleLeftClick() 
    {
        
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask) &&
            hit.collider.TryGetComponent<Unit>(out Unit unit) &&
            unit.isOwned)  
        {
            if (SelectedUnits.Contains(unit))
            {
                unit.Deselect();
                SelectedUnits.Remove(unit);
            }
            else
            {
                if (!Keyboard.current.leftCtrlKey.isPressed)  
                    ClearSelection();  
        
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