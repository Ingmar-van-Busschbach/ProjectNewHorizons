using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
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
    [SerializeField] private InputActionReference mouseScrollInput;

    [Header("Core Configuration - Do not touch")]
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private SpriteRenderer draggedObjectPrefab;
    [SerializeField] private Transform bottomLeftBounds;
    [SerializeField] private Transform topRightBounds;
    [SerializeField] private Vector2 zoomBounds;
    [SerializeField] private RectTransform roomUnlockMenu;
    [SerializeField] private RectTransform ratInfoMenu;
    [SerializeField] private RectTransform roomInfoMenu;

    [Header("Settings")]
    [Tooltip("The offset for the drag highlight. Should be a positive numner between 0 and 5.")]
    [SerializeField] private float dragDistanceOffset = 1;
    [Tooltip("Camera drag speed when touching the screen.")]
    [SerializeField] private float cameraDragSpeed = 0.01f;
    [SerializeField] private float touchZoomSpeed = 0.01f;
    [SerializeField] private float mouseZoomSpeed = 0.5f;
    [SerializeField] private float menuAnimationSpeed = 0.5f;

    // Internal Parameters
    private ESelectionType selectionType;
    private float rayHitDistance;
    private bool isHeld;
    private float lastMultiTouchDistance;
    private Vector2 roomUnlockMenuPosition;
    private Vector2 ratInfoMenuPosition;
    private Vector2 roomInfoMenuPosition;

    // Components
    private GameObject selectedObject;
    private SpriteRenderer draggedObject;
    private Camera playerCamera;
    private Coroutine roomUnlockMenuAnimation;
    private Coroutine ratInfoMenuAnimation;
    private Coroutine roomInfoMenuAnimation;
    private PointerEventData pointerEventData;

#if UNITY_WEBGL
    private void Awake()
    {
        EnhancedTouchSupport.Enable();
    }
    #endif
    private void Start()
    {
        playerCamera = GetComponent<Camera>();
        roomUnlockMenuPosition = roomUnlockMenu.anchoredPosition;
        ratInfoMenuPosition = ratInfoMenu.anchoredPosition;
        roomInfoMenuPosition = roomInfoMenu.anchoredPosition;
    }

    private void Update()
    {
        Vector2 mousePosition = pointerPositionInput.action.ReadValue<Vector2>();

        #if UNITY_EDITOR
        // On click/touch start
        if (clickInput.action.WasPressedThisFrame())
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
        else if (clickInput.action.WasReleasedThisFrame())
        {
            isHeld = false;
            if(selectionType == ESelectionType.Character)
            {
                ReleaseCharacter(mousePosition);
            }
        }
#elif UNITY_WEBGL
        if (Touch.activeFingers.Count == 1 && !isHeld)
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
        else if (Touch.activeFingers.Count == 0 && isHeld)
        {
            isHeld = false;
            if (selectionType == ESelectionType.Character)
            {
                ReleaseCharacter(mousePosition);
            }
        }
#endif
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
        
        float mouseScroll = mouseScrollInput.action.ReadValue<float>();
        if (mouseScroll != 0)
        {
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                transform.localPosition.y,
                Mathf.Clamp(transform.localPosition.z + mouseScroll * mouseZoomSpeed, zoomBounds.x, zoomBounds.y)
                );
        }
    }
    private void SelectCharacter(RaycastHit hit)
    {
        selectionType = ESelectionType.Character;
        selectedObject = hit.collider.gameObject;
        if (selectedObject.TryGetComponent(out Character character))
        {
            AnimateRoomUnlockMenu(false);
            AnimateRatInfoMenu(true);
            AnimateRoomInfoMenu(false);
            if (ratInfoMenu.TryGetComponent(out RatStatDisplay ratStatDisplay))
            {
                ratStatDisplay.DisplayStats(character.stats, character.statPlugs, character.name);
            }
        }
    }
    private void SelectRoom(RaycastHit hit)
    {
        selectionType = ESelectionType.Room;
        selectedObject = hit.collider.gameObject;
        if(selectedObject.TryGetComponent(out Room room))
        {
            AnimateRatInfoMenu(false);
            if (!room.unlockedRoom)
            {
                AnimateRoomUnlockMenu(true);
                if (roomUnlockMenu.TryGetComponent(out UnlockMenuHandler unlockMenuHandler))
                {
                    unlockMenuHandler.SetRoomToUnlock(room);
                }
            }
            else
            {
                AnimateRoomInfoMenu(true);
                if(roomInfoMenu.TryGetComponent(out RoomInfoHandler roomInfoHandler))
                {
                    roomInfoHandler.DisplayInfo(room);
                }
            }
        }
        
    }
    private void Deselect()
    {
        selectionType = ESelectionType.None;
        selectedObject = null;
    }

    public void AnimateRoomUnlockMenu(bool onScreen)
    {
        if (roomUnlockMenuAnimation != null)
        {
            StopCoroutine(roomUnlockMenuAnimation);
        }
        roomUnlockMenuAnimation = StartCoroutine(AnimateMenu(roomUnlockMenu, roomUnlockMenuPosition * new Vector2(1, onScreen ? -1 : 1)));
    }

    public void AnimateRatInfoMenu(bool onScreen)
    {
        if (ratInfoMenuAnimation != null)
        {
            StopCoroutine(ratInfoMenuAnimation);
        }
        ratInfoMenuAnimation = StartCoroutine(AnimateMenu(ratInfoMenu, ratInfoMenuPosition * new Vector2(onScreen ? -1 : 1, 1)));
    }

    public void AnimateRoomInfoMenu(bool onScreen)
    {
        if (roomInfoMenuAnimation != null)
        {
            StopCoroutine(roomInfoMenuAnimation);
        }
        roomInfoMenuAnimation = StartCoroutine(AnimateMenu(roomInfoMenu, roomInfoMenuPosition * new Vector2(onScreen ? -1 : 1, 1)));
    }
    private IEnumerator AnimateMenu(RectTransform menu, Vector2 position)
    {
        float distance = Vector3.Distance(menu.anchoredPosition, position);
        while (Vector3.Distance(menu.anchoredPosition, position) > 0)
        {
            menu.anchoredPosition = Vector3.MoveTowards(menu.anchoredPosition, position, menuAnimationSpeed * distance * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
    }
    private void CameraMove()
    {
        if(Touch.activeFingers.Count == 2)
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
                Mathf.Clamp(transform.localPosition.z + (newMultiTouchDistance - lastMultiTouchDistance) * touchZoomSpeed, zoomBounds.x, zoomBounds.y)
                );

            lastMultiTouchDistance = newMultiTouchDistance;
        }
        else
        {
            Vector2 mouseDelta = pointerDeltaInput.action.ReadValue<Vector2>();
            mouseDelta *= cameraDragSpeed;
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x - mouseDelta.x, bottomLeftBounds.position.x, topRightBounds.position.x),
                Mathf.Clamp(transform.position.y - mouseDelta.y, bottomLeftBounds.position.y, topRightBounds.position.y),
                transform.position.z
                );
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