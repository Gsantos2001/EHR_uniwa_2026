
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HotspotCameraController : MonoBehaviour
{
    [Header("Scripts to Disable on Focus")]
    public PlayerMovement playerMovement;
    public PlayerCam playerCam;
    public HotspotCameraController hotspotCameraController;
    public Interactor interactor;

    [Header("UI & Visual Elements to Hide on Focus")]
    public GameObject handCamera;
    public GameObject crosshairUI;

    [Header("Transition Settings")]
    public float transitionSpeed = 8f;

    [Header("Online Help")]
    public OnlineHelpUI onlineHelpUI;

   

    private Transform originalParent;
    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;

    private bool isFocused = false;
    private Coroutine activeRoutine;

    public bool IsFocused => isFocused;

    private void Start()
    {
        originalParent = transform.parent;
        originalLocalPos = transform.localPosition;
        originalLocalRot = transform.localRotation;

        if (playerCam == null)
            playerCam = GetComponent<PlayerCam>();

        if (interactor == null)
            interactor = GetComponent<Interactor>();

        if (onlineHelpUI == null)
            onlineHelpUI = FindObjectOfType<OnlineHelpUI>();
    }

   /* private void Update()
    {
        if (isFocused &&
            ((Keyboard.current != null &&
              Keyboard.current.escapeKey.wasPressedThisFrame) ||
             (Mouse.current != null &&
              Mouse.current.rightButton.wasPressedThisFrame)))
        {
            ExitFocus();
        }
    }*/

    public void FocusOnTarget(Transform focusTarget)
    {
        if (isFocused || focusTarget == null)
            return;

        isFocused = true;

        TogglePlayerControls(false);

        if (activeRoutine != null)
            StopCoroutine(activeRoutine);

        activeRoutine = StartCoroutine(
            MoveCameraToTarget(
                focusTarget.position,
                focusTarget.rotation
            )
        );
    }

    public void ExitFocus()
    {
        if (!isFocused)
            return;

        EHRUI ehrUI = FindObjectOfType<EHRUI>();

        if (ehrUI != null)
        {
            ehrUI.CloseEHR();
        }

        
        if (onlineHelpUI != null &&
            onlineHelpUI.IsHelpVisible)
        {
            onlineHelpUI.HideHelp();
        }

        if (activeRoutine != null)
            StopCoroutine(activeRoutine);

        activeRoutine = StartCoroutine(
            ReturnCameraToPlayer()
        );
    }

    private IEnumerator MoveCameraToTarget(
        Vector3 targetPos,
        Quaternion targetRot)
    {
        transform.SetParent(null);

        while (Vector3.Distance(transform.position, targetPos) > 0.001f ||
               Quaternion.Angle(transform.rotation, targetRot) > 0.05f)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                targetPos,
                Time.deltaTime * transitionSpeed
            );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                Time.deltaTime * transitionSpeed
            );

            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
    }

    private IEnumerator ReturnCameraToPlayer()
    {
        transform.SetParent(originalParent);

        while (Vector3.Distance(transform.localPosition, originalLocalPos) > 0.001f ||
               Quaternion.Angle(transform.localRotation, originalLocalRot) > 0.1f)
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                originalLocalPos,
                Time.deltaTime * transitionSpeed
            );

            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                originalLocalRot,
                Time.deltaTime * transitionSpeed
            );

            yield return null;
        }

        transform.localPosition = originalLocalPos;
        transform.localRotation = originalLocalRot;

        SyncPlayerCamRotation();

        TogglePlayerControls(true);

        isFocused = false;

        if (interactor != null)
        {
            interactor.ClearInteractedHotspot();
        }
    }

    private void SyncPlayerCamRotation()
    {
        if (playerCam == null)
            return;

        Vector3 euler = transform.eulerAngles;

        float xRot = euler.x;

        if (xRot > 180f)
            xRot -= 360f;

        float yRot = euler.y;

        var fieldX = typeof(PlayerCam).GetField(
            "xRotation",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance
        );

        var fieldY = typeof(PlayerCam).GetField(
            "yRotation",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance
        );

        if (fieldX != null)
            fieldX.SetValue(playerCam, xRot);

        if (fieldY != null)
            fieldY.SetValue(playerCam, yRot);
    }

    private void TogglePlayerControls(bool enable)
    {
        if (playerMovement != null)
            playerMovement.enabled = enable;

        if (playerCam != null)
            playerCam.enabled = enable;

        if (interactor != null)
            interactor.enabled = true;

        if (handCamera != null)
            handCamera.SetActive(enable);

        if (crosshairUI != null)
            crosshairUI.SetActive(enable);

        if (enable)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}