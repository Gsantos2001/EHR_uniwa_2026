using UnityEngine;
using UnityEngine.InputSystem;

public class DialKnobController : MonoBehaviour
{
    public static DialKnobController ActiveDial { get; private set; }

    [Header("Hotspot Settings")]
    public string hotspotId = "hs_nasal_cannula";

    [Header("Rotation Settings")]
    public Vector3 rotationAxis = Vector3.forward; 
    public float minAngle = 0f;                   
    public float maxAngle = 180f;                 
    public float dragSensitivity = 1.5f;

    [Header("Dependencies")]
    public ScenarioEngine scenarioEngine;
    public HotspotCameraController cameraController;

    public bool IsDragging { get; private set; } = false;

    private float currentAngle = 0f;
    private bool hasTriggered = false;
    private Vector2 lastMousePosition;
    private Quaternion initialLocalRotation;

    private void Start()
    {
        initialLocalRotation = transform.localRotation;
        currentAngle = minAngle;

        if (cameraController == null)
            cameraController = Camera.main.GetComponent<HotspotCameraController>();

        if (scenarioEngine == null)
            scenarioEngine = FindFirstObjectByType<ScenarioEngine>();
    }

    private void Update()
    {
        if (!IsDragging) return;

        if (!Mouse.current.leftButton.isPressed)
        {
            StopDragging();
            return;
        }

        UpdateDrag();
    }

    public void StartDragging()
    {
        ResetKnob(); 

        IsDragging = true;
        ActiveDial = this;
        lastMousePosition = Mouse.current.position.ReadValue();
    }

    public void StopDragging()
    {
        IsDragging = false;
        if (ActiveDial == this) ActiveDial = null;
    }

    private void UpdateDrag()
    {
        Vector2 currentMousePosition = Mouse.current.position.ReadValue();
        Vector2 delta = currentMousePosition - lastMousePosition;
        lastMousePosition = currentMousePosition;

        float deltaAngle = (delta.x + delta.y) * dragSensitivity;
        currentAngle = Mathf.Clamp(currentAngle + deltaAngle, minAngle, maxAngle);

        transform.localRotation = initialLocalRotation * Quaternion.AngleAxis(currentAngle, rotationAxis);

        if (currentAngle >= maxAngle - 1f && !hasTriggered)
        {
            hasTriggered = true;
            StopDragging();
            OnDialMaxed();
        }
    }

    private void OnDialMaxed()
    {
        if (scenarioEngine != null)
        {
            scenarioEngine.TrySelectOptionByHotspot(hotspotId);
        }
        
        if (cameraController != null && cameraController.IsFocused)
        {
            cameraController.ExitFocus();
        }
    }

    public void ResetKnob()
    {
        currentAngle = minAngle;
        hasTriggered = false; 
        IsDragging = false;
        transform.localRotation = initialLocalRotation;
    }
}