using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GazeSelector : MonoBehaviour {
  [SerializeField] public XRGazeInteractor gazeInteractor;

  protected GameObject Selection;

  private void Start() {
    gazeInteractor.selectEntered.AddListener(OnSelectEntered);
    gazeInteractor.hoverEntered.AddListener(OnHoverEntered);
    gazeInteractor.selectExited.AddListener(OnSelectExited);
    gazeInteractor.hoverExited.AddListener(OnHoverExited);
  }

  protected void OnSelectEntered(SelectEnterEventArgs args) {
    Selection = args.interactableObject.transform.gameObject;
  }

  protected void OnHoverEntered(HoverEnterEventArgs args) {
    Selection = args.interactableObject.transform.gameObject;
  }

  protected void OnSelectExited(SelectExitEventArgs args) {
    Selection = null;
  }

  protected void OnHoverExited(HoverExitEventArgs args) {
    Selection = null;
  }
}