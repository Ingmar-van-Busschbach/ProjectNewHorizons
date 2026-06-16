using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

[RequireComponent(typeof(Camera))]
public class ContextSelector : MonoBehaviour
{
    [SerializeField] private InputActionReference clickInput;
    [SerializeField] private InputActionReference mousePositionInput;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private float dragDistanceOffset;
    [SerializeField] private GameObject testObject;
    private ESelectionType selectionType;
    private GameObject selectedObject;
    private GameObject draggedObject;
    private Camera playerCamera;
    private float rayHitDistance;
    private bool isHeld;

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
        Vector2 mousePosition = mousePositionInput.action.ReadValue<Vector2>();
        if (clickInput.action.WasPressedThisFrame() || (Touch.activeFingers.Count == 1 && !isHeld))
        {
            isHeld = true;
            Ray ray = playerCamera.ScreenPointToRay(mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                rayHitDistance = hit.distance - dragDistanceOffset;
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
            else
            {
                selectionType = ESelectionType.None;
                selectedObject = null;
            }
        }
        else if(clickInput.action.WasReleasedThisFrame() || (Touch.activeFingers.Count == 0 && isHeld))
        {
            isHeld = false;
            if(selectionType == ESelectionType.Character)
            {
                ReleaseCharacter(mousePosition);
            }
        }
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
        //Debug.Log("Moving Camera");
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
        //Debug.Log("Dragging Character");
    }
    private void StartDraggingCharacter(Vector3 location)
    {
        draggedObject = Instantiate(selectedObject.transform.GetChild(0).gameObject, location, selectedObject.transform.rotation);
        if (draggedObject.TryGetComponent<Renderer>(out Renderer renderer))
        {
            renderer.material = highlightMaterial;
        }
        if (draggedObject.TryGetComponent<Collider>(out Collider collider))
        {
            collider.enabled = false;
        }
    }
    private void ReleaseCharacter(Vector2 mousePosition)
    {
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
        Destroy(draggedObject);
    }
}

public enum ESelectionType { None, Character, Room }