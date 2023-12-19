using UnityEngine;
using UI;

namespace FPSController {

public class FirstPersonLook : MonoBehaviour {
  [SerializeField]
  Transform character;
  public float sensitivity = 2;
  public float smoothing = 1.5f;
  public CrosshairHandler crosshairHandler;

  Vector2 velocity;
  Vector2 frameVelocity;
  Vector3 fireDirection;
  Vector3 firePoint;

  void Reset() {
    // Get the character from the FirstPersonMovement in parents.
    character = GetComponentInParent<FirstPersonMovement>().transform;
  }

  void Start() {
    // Lock the mouse cursor to the game screen.
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }

  void Update() {
    // Get smooth velocity.
    if (Time.timeScale != 0) {
      Vector2 mouseDelta =
          new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
      Vector2 rawFrameVelocity =
          Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
      frameVelocity =
          Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
      velocity += frameVelocity;
      velocity.y = Mathf.Clamp(velocity.y, -90, 90);

      // Rotate camera up-down and controller left-right from velocity.
      transform.localRotation =
          Quaternion.AngleAxis(-velocity.y, Vector3.right);
      character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
    }
  }
  void Hit() {
    // Raycasting variables:
    RaycastHit hit;
    fireDirection = transform.TransformDirection(Vector3.forward) * 10;
    firePoint = transform.position;

    // Do raycasting:
    if (Physics.Raycast(firePoint, (fireDirection), out hit, Mathf.Infinity)) {
      // Change the color based on what object is under the crosshair:
      if (hit.transform.name == "Enemy") {
        crosshairHandler.ChangeColor(Color.red);
      } else {
        crosshairHandler.ChangeColor(Color.white);
      }
    } else {
      crosshairHandler.ChangeColor(Color.white);
    }

    // Debug the ray out in the editor:
    Debug.DrawRay(firePoint, fireDirection, Color.green);
  }
}
}
