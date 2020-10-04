using SpiritWorld.Managers;
using SpiritWorld.World.Terrain.Features;
using SpiritWorld.World.Terrain.TileGrid;
using System;
using UnityEngine;

namespace SpiritWorld.Controllers {
  [RequireComponent(typeof(CharacterController))]
  [RequireComponent(typeof(CapsuleCollider))]
  public class PlayerController : MonoBehaviour {
    /// <summary>
    /// The minimum time a key must be held down in order to get a hold action instead of a click action
    /// </summary>
    const float MinimumHoldDownTime = 0.5f;

    /// <summary>
    /// The character's head, for mouselook
    /// </summary>
    public GameObject headObject;

    /// <summary>
    /// The object used to hilight the selected tile
    /// </summary>
    public GameObject SelectedTileHilight;

    /// <summary>
    /// The move speed of the player
    /// </summary>
    public float moveSpeed = 10;

    /// <summary>
    /// The mouselook clamp
    /// </summary>
    public Vector2 clampInDegrees = new Vector2(360, 180);

    /// <summary>
    /// Whether to lock cursor for mouselook
    /// </summary>
    public bool lockCursor;

    /// <summary>
    /// Sensitivity vector for mouselook
    /// </summary>
    public Vector2 sensitivity = new Vector2(2, 2);

    /// <summary>
    /// Smoothing vector for mouselook
    /// </summary>
    public Vector2 smoothing = new Vector2(3, 3);

    /// <summary>
    /// The strength of gravity
    /// </summary>
    public float gravityStrength = 5;

    /// <summary>
    /// direction the camera is facing
    /// </summary>
    public Vector2 facingDirection;

    /// <summary>
    /// Direction the character is facing.
    /// </summary>
    public Vector2 targetCharacterDirection;

    /// <summary>
    /// The character controller unity component, used for movement.
    /// </summary>
    CharacterController movementController;

    /// <summary>
    /// The currently selected tile
    /// </summary>
    Tile selectedTile;

    /// <summary>
    /// A timer for performing an action
    /// </summary>
    float actionTimer = 0.0f;

    /// <summary>
    /// The absolute mouse mosition
    /// </summary>
    Vector2 mouseAbsolute;

    /// <summary>
    /// The smooth mouse position
    /// </summary>
    Vector2 smoothMouse;

    void Awake() {
      movementController = GetComponent<CharacterController>();
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
      selectHoveredTile();
      tryToActOnSelectedTile();
    }

    /// <summary>
    /// Player movement management
    /// </summary>
    void move() {
      // apply gravity
      Vector3 gravity = new Vector3(0, -gravityStrength, 0);
      Vector3 move = gravity;


      // movement left and right and forward and back
      if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0) {
        Vector3 forwardMovement = headObject.transform.forward * Input.GetAxis("Vertical") * moveSpeed;
        Vector3 rightMovement = headObject.transform.right * Input.GetAxis("Horizontal") * moveSpeed;
        // get the total vector and check if we're moving
        move += forwardMovement + rightMovement;
      }

      // apply it with the movement controller
      if (move.magnitude > 0) {
        // move character
        movementController.SimpleMove(move);
      }
    }

    /// <summary>
    /// Player mouselook management
    /// </summary>
    void look() {
      if (Input.GetButton("Rotate Camera Lock")) {
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
      }
    }

    /// <summary>
    /// Select and hilight the tile the mouse is hovering over
    /// </summary>
    void selectHoveredTile() {
      // we only want to change the selected tile when we're not acting on a tile atm
      if (!Input.GetButton("Act")) {
        /// if the mouse is pointing at the tileboard, use a ray get data about the tile
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 25, Color.red);
        if (Physics.Raycast(ray, out RaycastHit hit, 25) && hit.collider.gameObject.CompareTag("TileBoard")) {
          // get the place we hit, plus a little forward in case we hit the side of a column
          Vector3 gridHitPosition = hit.point + (hit.normal * 0.1f);
          // zero out the y, we dont' account for height in the coords.
          gridHitPosition.y = 0;
          // get which tile we hit and the chunk it's in
          selectedTile = Universe.ActiveBoardManager.activeBoard.get(gridHitPosition);

          /// Move the selected tile hilight to the correct location
          if (SelectedTileHilight != null) {
            SelectedTileHilight.transform.position = new Vector3(
              selectedTile.worldLocation.x,
              selectedTile.height * Universe.StepHeight,
              selectedTile.worldLocation.z
            );
          }
        }
      }
    }

    /// <summary>
    /// Act on the selected tile when appropriate
    /// </summary>
    void tryToActOnSelectedTile() {
      // if we're holding the button
      if (Input.GetButton("Act")) {
        actionTimer += Time.deltaTime;
        // if we've been holding it for the minimum hold time at least, act via hold action
        if (actionTimer >= MinimumHoldDownTime) {
          holdDownActionOnSelectedTile();
        }
      }
      // if we've let go of the button
      if (Input.GetButtonUp("Act")) {
        // if we haven't gone over minimum hold time, do the single click action on the tile
        if (actionTimer < MinimumHoldDownTime) {
          clickActionOnSelectedTile();
        }
        // reset the action timer
        actionTimer = 0;
      }
    }

    /// <summary>
    /// Use an equiped tool on the tile's feature
    /// </summary>
    /// <param name="actionTimer"></param>
    void holdDownActionOnSelectedTile() {
      // check if the tile has a resource. If it does, we'll try to mine it
      // @todo: replace this with a messaging system and listeners
      FeaturesByLayer features = Universe.ActiveBoardManager.activeBoard.getFeaturesFor(selectedTile);
      if (features != null && features.TryGetValue(TileFeature.Layer.Resource, out TileFeature resource)) {
        TileFeature beforeResourceValues = resource;
        resource.interact(actionTimer);
        Universe.ActiveBoardManager.activeBoard.update(selectedTile, resource);
        if (beforeResourceValues.mode != resource.mode) {
          actionTimer = 0;
          Universe.ActiveBoardManager.updateModeForFeature(selectedTile, resource);
        }
      }
    }

    /// <summary>
    /// display info about the tile
    /// </summary>
    void clickActionOnSelectedTile() {

    }
  }
}