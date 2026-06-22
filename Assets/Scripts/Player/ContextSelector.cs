using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UIElements;
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
    [SerializeField] private Vector2 zoomBounds;
    [SerializeField] private RectTransform unlockMenu;

    [Header("Settings")]
    [Tooltip("The offset for the drag highlight. Should be a positive numner between 0 and 5.")]
    [SerializeField] private float dragDistanceOffset = 1;
    [Tooltip("Camera drag speed when touching the screen.")]
    [SerializeField] private float cameraDragSpeed = 0.01f;
    [SerializeField] private float zoomSpeed = 0.01f;
    [SerializeField] private float menuAnimationSpeed = 100;

    // Internal Parameters
    private ESelectionType selectionType;
    private float rayHitDistance;
    private bool isHeld;
    private float lastMultiTouchDistance;

    // Components
    private GameObject selectedObject;
    private SpriteRenderer draggedObject;
    private Camera playerCamera;
    private Coroutine unlockMenuAnimation;

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
                        SelectRoom(hit);
                        break;
                    case 6: // Character layer
                        SelectCharacter(hit);
                        break;
                    default:
                        Deselect();
                        break;
                }
            }
            else // Fallback to not selecting anything if no object was hit with the raycast.
            {
                Deselect();
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
    private void SelectCharacter(RaycastHit hit)
    {
        selectionType = ESelectionType.Character;
        selectedObject = hit.collider.gameObject;
    }
    private void SelectRoom(RaycastHit hit)
    {
        selectionType = ESelectionType.Room;
        selectedObject = hit.collider.gameObject;
        if(selectedObject.TryGetComponent(out Room room))
        {
            if (!room.unlockedRoom)
            {
                if (unlockMenuAnimation != null)
                {
                    StopCoroutine(unlockMenuAnimation);
                }
                unlockMenuAnimation = StartCoroutine(AnimateUnlockMenu(new Vector2(0, 50)));
                if(unlockMenu.TryGetComponent(out UnlockMenuHandler unlockMenuHandler))
                {
                    unlockMenuHandler.SetRoomToUnlock(room);
                }
            }
        }
        
    }
    private void Deselect()
    {
        selectionType = ESelectionType.None;
        selectedObject = null;
        if (unlockMenuAnimation != null)
        {
            StopCoroutine(unlockMenuAnimation);
        }
        unlockMenuAnimation = StartCoroutine(AnimateUnlockMenu(new Vector2(0, -50)));
    }
    private IEnumerator AnimateUnlockMenu(Vector2 position)
    {
        while (Vector3.Distance(unlockMenu.anchoredPosition, position) > 0)
        {
            unlockMenu.anchoredPosition = Vector3.MoveTowards(unlockMenu.anchoredPosition, position, menuAnimationSpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
    }
    private void CameraMove()
    {
        if(Touch.activeFingers.Count == 1)
        {
            Vector2 mouseDelta = pointerDeltaInput.action.ReadValue<Vector2>();
            mouseDelta *= cameraDragSpeed;
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x - mouseDelta.x, bottomLeftBounds.position.x, topRightBounds.position.x),
                Mathf.Clamp(transform.position.y - mouseDelta.y, bottomLeftBounds.position.y, topRightBounds.position.y),
                transform.position.z
                );
        }
        else if(Touch.activeFingers.Count == 2)
        {
            Touch firstTouch = Touch.activeTouches[0];
            Touch secondTouch = Touch.activeTouches[1];
            if (firstTouch.phase == TouchPhase.Began || secondTouch.phase == TouchPhase.Began)
            {
                lastMultiTouchDistance = Vector2.Distance(firstTouch.screenPosition, secondTouch.screenPosition);
            }
            if (firstTouch.phase != TouchPhase.Moved || secondTouch.phase != TouchPhase.Moved)
            {
                return;
            }
            float newMultiTouchDistance = Vector2.Distance(firstTouch.screenPosition, secondTouch.screenPosition);

            transform.localPosition = new Vector3(
                transform.localPosition.x,
                transform.localPosition.y,
                Mathf.Clamp(transform.localPosition.z + (newMultiTouchDistance - lastMultiTouchDistance) * zoomSpeed, zoomBounds.x, zoomBounds.y)
                );

            lastMultiTouchDistance = newMultiTouchDistance;
        }
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
        //draggedObject.flipX = Vector3.Dot(selectedObject.transform.right, Vector3.right) < 0;
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
                    Room oldRoom = character.currentRoom;
                    Transform location = room.AssignCharacter(character);
                    character.MoveToLocation(location);
                    if(location != character.gameObject.transform)
                    {
                        if (character.gameObject.transform.GetChild(0).TryGetComponent(out RatHats ratHats))
                        {
                            ratHats.AssignHat(room);
                        }
                        if (oldRoom != null)
                        {
                            oldRoom.UnassignCharacter(character);
                        }
                    }
                }
            }
        }
    }
}