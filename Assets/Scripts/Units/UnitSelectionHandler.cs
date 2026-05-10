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
    private bool _isDragging; 

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
            StartSelectionArea();
        }
        else if (Mouse.current.leftButton.isPressed && _isDragging)
        {
            UpdateSelectionArea();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame && _isDragging)
        {
            EndSelectionArea();
        }
    }
    
    private void StartSelectionArea()
    {
        _isDragging = true;
        _unitSelectionArea.gameObject.SetActive(true);
        _startPosition = Mouse.current.position.ReadValue();
        
        if (!Keyboard.current.leftCtrlKey.isPressed)
        {
            ClearSelection();
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
    
    private void EndSelectionArea()
    {
        _isDragging = false;
        _unitSelectionArea.gameObject.SetActive(false);
        
        if (_unitSelectionArea.sizeDelta.magnitude == 0)
        {
            HandleLeftClick();
            return;
        }
        
        Vector2 min = _unitSelectionArea.anchoredPosition - (_unitSelectionArea.sizeDelta / 2);
        Vector2 max = _unitSelectionArea.anchoredPosition + (_unitSelectionArea.sizeDelta / 2);

        foreach (Unit unit in _player.GetMyUnits())
        {
            Vector3 screenPosition = _mainCamera.WorldToScreenPoint(unit.transform.position);
            if (screenPosition.x > min.x && screenPosition.x < max.x &&
                screenPosition.y > min.y && screenPosition.y < max.y)
            {
                if (!SelectedUnits.Contains(unit))
                {
                    SelectedUnits.Add(unit);
                    unit.Select();
                }
            }
        }
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

    // Полная очистка выделения
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