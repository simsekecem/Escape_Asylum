using UnityEngine;
using TMPro;

public class DoorUIManager : MonoBehaviour
{
    public static DoorUIManager Instance { get; private set; }

    [Header("UI Bileþenleri")]
    public GameObject textCanvasObject; 
    public TextMeshProUGUI infoText;    

    [Header("VR Takip Ayarlarý")]
    public float distanceFromCamera = 1.5f; 
    public float heightOffset = 0f;          
    private float hideTimer = 0f;
    private bool isTextActive = false;
    private Transform vrCameraTransform;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (textCanvasObject != null) textCanvasObject.SetActive(false);
    }

    void Start()
    {
        if (Camera.main != null)
        {
            vrCameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        if (isTextActive)
        {
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0f)
            {
                HideText();
            }
        }
    }

    public void ShowMessage(string message, float duration = 2.5f)
    {
        if (infoText == null || textCanvasObject == null || vrCameraTransform == null) return;

        infoText.text = message;

        Vector3 targetPosition = vrCameraTransform.position + (vrCameraTransform.forward * distanceFromCamera);
        targetPosition.y += heightOffset;

        textCanvasObject.transform.position = targetPosition;

        
        textCanvasObject.transform.LookAt(vrCameraTransform);
        textCanvasObject.transform.Rotate(0, 180, 0);

        textCanvasObject.SetActive(true);
        hideTimer = duration;
        isTextActive = true;
    }

    public void HideText()
    {
        if (textCanvasObject != null) textCanvasObject.SetActive(false);
        isTextActive = false;
    }
}