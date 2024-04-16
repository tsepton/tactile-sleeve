using System;
using System.Collections;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class CarMenuInteraction : GazeSelector {
  [SerializeField] public GameObject reducer;

  [SerializeField] public GameObject menu;

  [SerializeField] public GameObject menuCursor;

  private Canvas _canvas;

  private RectTransform _canvasRect;

  private Vector2 _touchStartPos;

  private Collider _selectedMenuItem;

  private Cursor _menuCursor;

  private RectTransform _menuCursorRect;

  private void OnEnable() {
    Touchpad.OnTouch += OnTouch;
    Touchpad.OnRelease += OnRelease;

    _canvas = menu.GetComponentInChildren<Canvas>();
    _canvasRect = menu.GetComponentInChildren<RectTransform>();

    _menuCursor = menuCursor.GetComponent<Cursor>();
    _menuCursorRect = menuCursor.GetComponentInChildren<RectTransform>();

    _menuCursor.OnSelectionUpdate += UpdateMaterial;
  }

  private void OnDestroy() {
    // Unsubscribe from the OnSelectionUpdate event to avoid memory leaks
    Touchpad.OnTouch -= OnTouch;
    Touchpad.OnRelease -= OnRelease;
    _menuCursor.OnSelectionUpdate -= UpdateMaterial;
  }

  private void OnTouch(TouchPoint touch) {
    if (Selection == null && menu.activeSelf) {
      _menuCursorRect.anchoredPosition = (touch.coordinates - _touchStartPos) * 500;
    }
    else if (Selection != null && Selection.gameObject == reducer && !menu.activeSelf) {
      _touchStartPos = touch.coordinates;
      reducer.SetActive(false);
      menu.SetActive(true);
    }
  }

  private void OnRelease(int touchId) {
    if (touchId != 1) return;

    if (_selectedMenuItem == null) {
      reducer.SetActive(true);
      menu.SetActive(false);
    } // TODO -> Execute the linked action

    if (_menuCursor.Selection != null) _menuCursor.Selection.GetComponent<Renderer>().material.color = Color.grey;
  }

  private void UpdateMaterial([CanBeNull] GameObject newGo, [CanBeNull] GameObject oldGo) {
    if (oldGo != null) oldGo.GetComponent<Renderer>().material.color = Color.grey;
    if (newGo != null) newGo.GetComponent<Renderer>().material.color = Color.green;
  }
}