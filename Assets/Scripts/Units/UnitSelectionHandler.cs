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
    private bool _isDragging; // Флаг, что выделение областью активно

    private void Start()
    {
        _mainCamera = Camera.main;
        _player = NetworkClient.connection?.identity?.GetComponent<RTSPlayer>();
    }

    private void Update()
    {
        // Безопасное получение игрока (без ошибок в момент инициализации)
        if (_player == null)
        {
            _player = NetworkClient.connection?.identity?.GetComponent<RTSPlayer>();
            if (_player == null) return;
        }

        // Начало выделения (зажата ЛКМ)
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartSelectionArea();
        }
        // Рисование области (удерживается ЛКМ)
        else if (Mouse.current.leftButton.isPressed && _isDragging)
        {
            UpdateSelectionArea();
        }
        // Завершение выделения (отпущена ЛКМ)
        else if (Mouse.current.leftButton.wasReleasedThisFrame && _isDragging)
        {
            EndSelectionArea();
        }
    }

    // Активация области и подготовка выделения
    private void StartSelectionArea()
    {
        _isDragging = true;
        _unitSelectionArea.gameObject.SetActive(true);
        _startPosition = Mouse.current.position.ReadValue();

        // Если Ctrl не зажат – очищаем текущее выделение
        if (!Keyboard.current.leftCtrlKey.isPressed)
        {
            ClearSelection();
        }
    }

    // Обновление размеров и позиции прямоугольника
    private void UpdateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        float areaWidth = mousePosition.x - _startPosition.x;
        float areaHeight = mousePosition.y - _startPosition.y;

        _unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
        _unitSelectionArea.anchoredPosition = _startPosition + new Vector2(areaWidth / 2, areaHeight / 2);
    }

    // Завершение выделения: либо одиночный клик, либо сбор юнитов из области
    private void EndSelectionArea()
    {
        _isDragging = false;
        _unitSelectionArea.gameObject.SetActive(false);

        // Если область почти нулевая – считаем это кликом
        if (_unitSelectionArea.sizeDelta.magnitude == 0)
        {
            HandleLeftClick();
            return;
        }

        // Собираем юниты, попавшие в область
        Vector2 min = _unitSelectionArea.anchoredPosition - (_unitSelectionArea.sizeDelta / 2);
        Vector2 max = _unitSelectionArea.anchoredPosition + (_unitSelectionArea.sizeDelta / 2);

        foreach (Unit unit in _player.GetMyUnits())
        {
            Vector3 screenPosition = _mainCamera.WorldToScreenPoint(unit.transform.position);
            if (screenPosition.x > min.x && screenPosition.x < max.x &&
                screenPosition.y > min.y && screenPosition.y < max.y)
            {
                // Избегаем дубликатов (на случай, если юнит уже выделен)
                if (!SelectedUnits.Contains(unit))
                {
                    SelectedUnits.Add(unit);
                    unit.Select();
                }
            }
        }

        // Если не было захвачено ни одного юнита и Ctrl не зажат – выделение уже очищено в StartSelectionArea.
        // Для Ctrl – просто ничего не добавляем, старые юниты остаются.
    }

    // Обработка клика по одному юниту (с учётом Ctrl)
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