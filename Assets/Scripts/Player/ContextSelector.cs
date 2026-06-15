using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class ContextSelector : MonoBehaviour
{
    private ESelectionType selectionType;
    [SerializeField] private InputActionReference clickInput;
    [SerializeField] private InputActionReference mousePositionInput;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private GameObject spawnObject;
    private Camera playerCamera;

    private void Start()
    {
        playerCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (clickInput.action.WasPressedThisFrame())
        {
            Vector2 mousePosition = mousePositionInput.action.ReadValue<Vector2>();
            Ray ray = playerCamera.ScreenPointToRay(mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                Instantiate(spawnObject, hit.point, Quaternion.identity);
            }
        }
    }
}

public enum ESelectionType { None, Character, Room }