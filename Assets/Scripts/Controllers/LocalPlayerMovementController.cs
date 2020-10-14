using SpiritWorld.Inventories.Items;
using SpiritWorld.World.Entities.Creatures;
using UnityEngine;

namespace SpiritWorld.Controllers {
  [RequireComponent(typeof(CharacterController))]
  [RequireComponent(typeof(CapsuleCollider))]
  public class LocalPlayerMovementController : PlayerController {

    /// <summary>
    /// The character's head, for mouselook
    /// </summary>
    public GameObject headObject;

    /// <summary>
    /// The move speed of the player
    /// </summary>
    public float moveSpeed = 10;

    /// <summary>
    /// The jump speed of the player
    /// </summary>
    public float jumpHeight = 1;

    /// <summary>
    /// How much to multiply the walk speed for for sprinting
    /// </summary>
    public float sprintMultiplier = 1.5f;

    /// <summary>
    /// The strength of gravity
    /// </summary>
    public float gravityStrength = -9.81f;

    /// <summary>
    /// The mouselook clamp
    /// </summary>
    Vector2 clampInDegrees = new Vector2(360, 180);

    /// <summary>
    /// Sensitivity vector for mouselook
    /// </summary>
    Vector2 sensitivity = new Vector2(2, 2);

    /// <summary>
    /// Smoothing vector for mouselook
    /// </summary>
    Vector2 smoothing = new Vector2(3, 3);

    /// <summary>
    /// direction the camera is facing
    /// </summary>
    Vector2 facingDirection;

    /// <summary>
    /// Direction the character is facing.
    /// </summary>
    Vector2 targetCharacterDirection;

    /// <summary>
    /// The character controller unity component, used for movement.
    /// </summary>
    CharacterController movementController;

    /// <summary>
    /// The absolute mouse mosition
    /// </summary>
    Vector2 mouseAbsolute;

    /// <summary>
    /// The smooth mouse position
    /// </summary>
    Vector2 smoothMouse;

    /// <summary>
    /// player velocity for movement
    /// </summary>
    Vector3 playerVelocity;

    /// <summary>
    /// The original slope limit set for the movement contorller
    /// </summary>
    float startingSlopeLimit;

    /// <summary>
    /// The original slope limit set for the movement contorller
    /// </summary>
    float startingStepOffset;

    /// <summary>
    /// Whether to lock cursor for mouselook
    /// </summary>
    bool lockCursor;

    void Awake() {
      movementController = GetComponent<CharacterController>();
      startingSlopeLimit = movementController.slopeLimit;
      startingStepOffset = movementController.stepOffset;
      // Set target direction to the camera's initial orientation.
      facingDirection = headObject.transform.localRotation.eulerAngles;

      // Ensure the cursor is always locked when set
      if (lockCursor) {
        Cursor.lockState = CursorLockMode.Locked;
      }
    }

    // Update is called once per frame
    void Update() {
      move();
      look();
    }

    /// <summary>
    /// Player movement management
    /// </summary>
    void move() {
      bool isGrounded = movementController.isGrounded;
      if (isGrounded) {
        // prevent gravity from pulling you into the ground
        if (playerVelocity.y < 0) {
          playerVelocity.y = 0f;
        }
        movementController.slopeLimit = startingSlopeLimit;
        movementController.stepOffset = startingStepOffset;
      } else {
        // prevent the player from jumping up walls
        movementController.slopeLimit = 0;
        movementController.stepOffset = 0;
      }

      /// X, Z movement along ground
      Vector3 horizontalMovement = headObject.transform.forward * Input.GetAxis("Vertical");
      Vector3 verticalMovement = headObject.transform.right * Input.GetAxis("Horizontal");
      Vector3 movement2D = horizontalMovement + verticalMovement;
      float currentSpeed = moveSpeed;
      // sprint?
      if (Input.GetButton("Sprint Lock")) {
        currentSpeed *= sprintMultiplier;
      }
      movementController.Move(movement2D * Time.deltaTime * currentSpeed);

      // face the directon you're moving in
      /*if (move != Vector3.zero) {
        gameObject.transform.forward = move;
      }*/

      /// Jump?
      if (isGrounded && Input.GetButtonDown("Jump")) {
        playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityStrength);
      }
      playerVelocity.y += gravityStrength * Time.deltaTime;
      movementController.Move(playerVelocity * Time.deltaTime);
    }

    /// <summary>
    /// Player mouselook management
    /// </summary>
    void look() {
      if (Input.GetButton("Rotate Camera Lock")) {
        lockCursor = true;
        // Allow the script to clamp based on a desired target value.
        Quaternion targetOrientation = Quaternion.Euler(targetCharacterDirection);

        // Get raw mouse input for a cleaner reading on more sensitive mice.
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        // Scale input against the sensitivity setting and multiply that against the smoothing value.
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

        // Interpolate mouse movement over time to apply smoothing delta.
        smoothMouse.x = Mathf.Lerp(smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
        smoothMouse.y = Mathf.Lerp(smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

        // Find the absolute mouse movement value from point zero.
        mouseAbsolute += smoothMouse;

        // Clamp and apply the local x value first, so as not to be affected by world transforms.
        if (clampInDegrees.x < 360) {
          mouseAbsolute.x = Mathf.Clamp(mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);
        }

        // Then clamp and apply the global y value.
        if (clampInDegrees.y < 360) {
          mouseAbsolute.y = Mathf.Clamp(mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);
        }

        // Set the new look rotations
        headObject.transform.localRotation = Quaternion.AngleAxis(-mouseAbsolute.y, targetOrientation * Vector3.right) * targetOrientation;
        Quaternion yRotation = Quaternion.AngleAxis(mouseAbsolute.x, headObject.transform.InverseTransformDirection(Vector3.up));
        headObject.transform.localRotation *= yRotation;
        facingDirection = headObject.transform.localRotation.eulerAngles;
      } else if (lockCursor == true) {
        lockCursor = false;
      }
    }
  }
}