using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects {

public interface IInteractable {
  public bool IsDisplay { get; set; }
  public void SetUpUI();
  public void CloseUI();
  public string InteractionPrompt { get; }
  public bool Interact(Interactor interactor);
}

public class Interactor : MonoBehaviour {
  [SerializeField]
  Transform interactionPoint;
  [SerializeField]
  float interactionRadius = 0.5f;
  [SerializeField]
  LayerMask interactableMask;

  private readonly Collider[] colliders = new Collider[1];
  IInteractable interactable;

  // Update is called once per frame
  void Update() {
    int numFound = Physics.OverlapSphereNonAlloc(interactionPoint.position,
                                                 interactionRadius, colliders,
                                                 interactableMask);
    if (numFound > 0) {
      interactable = colliders[0].GetComponent<IInteractable>();
      if (interactable != null) {
        if (!interactable.IsDisplay) {
          interactable.SetUpUI();
        }
        if (Input.GetKey(KeyCode.E)) {
          interactable.Interact(this);
        }
      }
    } else {
      if (interactable != null) {
        interactable.CloseUI();
      }
    }
  }
}

}
