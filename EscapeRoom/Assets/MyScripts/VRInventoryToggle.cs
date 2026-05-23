using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class VRInventoryToggle : MonoBehaviour
{
    [Header("Inventory Canvas")]
    public GameObject inventoryCanvas;

    [Header("Main Menu Panel")]
    public GameObject mainMenuPanel;

    [Header("VR Etkilesim Bileşenleri")]
    [Tooltip("Sağ ve Sol eldeki XR Ray Interactor bileşenleri")]
    public XRRayInteractor[] rayInteractors;

    [Tooltip("Sağ ve Sol eldeki XR Direct Interactor bileşenleri")]
    public XRDirectInteractor[] directInteractors;

    [Header("Grip/Trigger Girdilerini Engellemek İçin")]
    [Tooltip("Sağ ve Sol eldeki Action Based Controller bileşenleri")]
    public XRBaseController[] handControllers;

    [Header("VR Input Tuş Ayarları")]
    public InputAction leftHandXButton = new InputAction(binding: "<XRController>{LeftHand}/primaryButton");
    public InputAction leftHandYButton = new InputAction(binding: "<XRController>{LeftHand}/secondaryButton");

    private void OnEnable()
    {
        leftHandXButton.Enable();
        leftHandYButton.Enable();
    }

    private void OnDisable()
    {
        leftHandXButton.Disable();
        leftHandYButton.Disable();
    }

    private void Start()
    {
        ForceDisableRaysOnStart();
    }

    private void Update()
    {
        if (leftHandXButton.WasPressedThisFrame())
        {
            HandleMenuToggle();
        }

        if (leftHandYButton.WasPressedThisFrame())
        {
            HandleInventoryToggle();
        }
    }

    private void ForceDisableRaysOnStart()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (inventoryCanvas != null) inventoryCanvas.SetActive(false);

        UpdateInteractorsState(false);
    }

    private void HandleMenuToggle()
    {
        if (mainMenuPanel == null)
            return;

        bool menuActive = mainMenuPanel.activeSelf;
        bool inventoryActive = inventoryCanvas != null && inventoryCanvas.activeSelf;

        if (menuActive)
        {
            mainMenuPanel.SetActive(false);
            UpdateInteractorsState(false);
            return;
        }

        if (inventoryActive)
        {
            inventoryCanvas.SetActive(false);
        }

        OpenMenu();
    }

    private void OpenMenu()
    {
        mainMenuPanel.SetActive(true);
        Transform cam = Camera.main.transform;
        mainMenuPanel.transform.position = cam.position + cam.forward * 1.2f;
        mainMenuPanel.transform.rotation = Quaternion.LookRotation(mainMenuPanel.transform.position - cam.position);

        UpdateInteractorsState(true);
    }

    private void HandleInventoryToggle()
    {
        if (inventoryCanvas == null)
            return;

        if (mainMenuPanel != null && mainMenuPanel.activeSelf)
            return;

        bool isActive = inventoryCanvas.activeSelf;

        inventoryCanvas.SetActive(!isActive);

        if (!isActive)
        {
            Transform invTransform = inventoryCanvas.transform.Find("Inv");
            if (invTransform != null)
            {
                invTransform.gameObject.SetActive(true);
            }
        }

        if (!isActive)
        {
            Transform cam = Camera.main.transform;
            inventoryCanvas.transform.position = cam.position + cam.forward * 1.2f;
            inventoryCanvas.transform.rotation = Quaternion.LookRotation(inventoryCanvas.transform.position - cam.position);
        }

        UpdateInteractorsState(!isActive);
    }

    private void UpdateInteractorsState(bool isAnyMenuOpen)
    {
        bool rayEnabled = isAnyMenuOpen;
        bool directEnabled = !isAnyMenuOpen;

        if (rayInteractors != null)
        {
            foreach (var ray in rayInteractors)
            {
                if (ray != null)
                {
                    ray.enabled = rayEnabled;
                    var lineVisual = ray.GetComponent<XRInteractorLineVisual>();
                    if (lineVisual != null) lineVisual.enabled = rayEnabled;
                    var lineRenderer = ray.GetComponent<LineRenderer>();
                    if (lineRenderer != null) lineRenderer.enabled = rayEnabled;
                }
            }
        }


        if (directInteractors != null)
        {
            foreach (var direct in directInteractors)
            {
                if (direct != null)
                {

                    direct.enabled = directEnabled;
                }
            }
        }

        if (handControllers != null)
        {
            foreach (var controller in handControllers)
            {
                if (controller != null)
                {
                    controller.enableInputActions = true;
                }
            }
        }
    }
}