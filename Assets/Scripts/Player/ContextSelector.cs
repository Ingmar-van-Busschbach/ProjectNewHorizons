using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class ContextSelector : MonoBehaviour
{
    [SerializeField] private InputActionReference clickInput;
    [SerializeField] private InputActionReference mousePositionInput;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private float dragDistanceOffset;
    private ESelectionType selectionType;
    private GameObject selectedObject;
    private GameObject draggedObject;
    private Camera playerCamera;
    private float rayHitDistance;

    private void Start()
    {
        playerCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        Vector2 mousePosition = mousePositionInput.action.ReadValue<Vector2>();
        if (clickInput.action.WasPressedThisFrame())
        {
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
        }
        else if(clickInput.action.WasReleasedThisFrame())
        {
            if(selectionType == ESelectionType.Character)
            {
                ReleaseCharacter(mousePosition);
            }
        }
        if (clickInput.action.IsPressed())
        {
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
        Debug.Log("Moving Camera");
    }

    private void DragCharacter(Vector2 mousePosition, float distance)
    {
        Vector3 location = playerCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, distance));
        if (draggedObject == null)
        {
            StartDraggingCharacter(location);
        }
        draggedObject.transform.position = location;
        Debug.Log("Dragging Character");
    }
    private void StartDraggingCharacter(Vector3 location)
    {
        draggedObject = Instantiate(selectedObject, location, selectedObject.transform.rotation);
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
        Destroy(draggedObject);
        Ray ray = playerCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            if(hit.collider.gameObject.layer == 3) // Room layer
            {
                Debug.Log("Entering room!");
            }
        }
    }
}

public enum ESelectionType { None, Character, Room }