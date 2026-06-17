using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

[RequireComponent(typeof(Camera))]
public class ContextSelector : MonoBehaviour
{
    [Header("Inputs - Do not touch")]
    [SerializeField] private InputActionReference clickInput;
    [SerializeField] private InputActionReference pointerPositionInput;
    [SerializeField] private InputActionReference pointerDeltaInput;

    [Header("Core Configuration - Do not touch")]
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private SpriteRenderer draggedObjectPrefab;
    [SerializeField] private Transform bottomLeftBounds;
    [SerializeField] private Transform topRightBounds;

    [Header("Settings")]
    [Tooltip("The offset for the drag highlight. Should be a positive numner between 0 and 5.")]
    [SerializeField] private float dragDistanceOffset = 1;
    [Tooltip("Camera drag speed when touching the screen.")]
    [SerializeField] private float cameraDragSpeed = 0.010f;

    // Internal Parameters
    private ESelectionType selectionType;
    private float rayHitDistance;
    private bool isHeld;

    // Components
    private GameObject selectedObject;
    private SpriteRenderer draggedObject;
    private Camera playerCamera;


    private void Awake()
    {
        EnhancedTouchSupport.Enable();
    }
    private void Start()
    {
        playerCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        Vector2 mousePosition = pointerPositionInput.action.ReadValue<Vector2>();

        // On click/touch start
        if (clickInput.action.WasPressedThisFrame() || (Touch.activeFingers.Count == 1 && !isHeld))
        {
            isHeld = true;

            Ray ray = playerCamera.ScreenPointToRay(mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                rayHitDistance = hit.distance - dragDistanceOffset; // Distance at which the highlight object will be placed during the drag event.
                switch (hit.collider.gameObject.layer)
                {
                    case 3: // Room layer
                        selectionType = ESelectionType.Room;
                        selectedObject = hit.collider.gameObject;
                        break;
                    case 6: // Character layer
                        selectionType = ESelectionType.Character;
                        selectedObject = hit.collider.gameObject;
                        break;
                    default:
                        selectionType = ESelectionType.None;
                        selectedObject = null;
                        break;
                }
            }
            else // Fallback to not selecting anything if no object was hit with the raycast.
            {
                selectionType = ESelectionType.None; 
                selectedObject = null;
            }
        }

        // On click/touch end
        else if (clickInput.action.WasReleasedThisFrame() || (Touch.activeFingers.Count == 0 && isHeld))
        {
            isHeld = false;
            if(selectionType == ESelectionType.Character)
            {
                ReleaseCharacter(mousePosition);
            }
        }

        // On click/touch continuous
        if (isHeld)
        {
            Touch activeTouch = Touch.activeFingers[0].currentTouch;
            if (selectionType == ESelectionType.Character)
            {
                DragCharacter(mousePosition, rayHitDistance);
            }
            else
            {
                CameraMove();
            }
        }
    }

    private void CameraMove()
    {
        Vector2 mouseDelta = pointerDeltaInput.action.ReadValue<Vector2>();
        mouseDelta *= cameraDragSpeed;
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x - mouseDelta.x, bottomLeftBounds.position.x, topRightBounds.position.x),
            Mathf.Clamp(transform.position.y - mouseDelta.y, bottomLeftBounds.position.y, topRightBounds.position.y),
            transform.position.z
            );
    }

    private void DragCharacter(Vector2 mousePosition, float distance)
    {
        Vector3 location = playerCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, distance));
        if (draggedObject == null)
        {
            StartDraggingCharacter(location);
        }
        Ray ray = playerCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            rayHitDistance = hit.distance - dragDistanceOffset;
        }
        draggedObject.transform.position = location;
    }
    private void StartDraggingCharacter(Vector3 location)
    {
        draggedObject = Instantiate(draggedObjectPrefab, location, Quaternion.identity);
        draggedObject.flipX = Vector3.Dot(selectedObject.transform.right, Vector3.right) < 0;
    }
    private void ReleaseCharacter(Vector2 mousePosition)
    {
        Destroy(draggedObject.gameObject);
        Ray ray = playerCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            if(hit.collider.gameObject.TryGetComponent(out Room room))
            {
                if(selectedObject.TryGetComponent(out Character character))
                {
                    if (character.currentRoom != null)
                    {
                        character.currentRoom.UnassignCharacter(character);
                    }
                    Transform location = room.AssignCharacter(character);
                    character.MoveToLocation(location);
                }
            }
        }
    }
}

public enum ESelectionType { None, Character, Room }