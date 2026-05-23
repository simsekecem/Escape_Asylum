using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

namespace InventoryFramework
{
    public class MainMenuEscape : MonoBehaviour
    {
        [Header("Gereksinim")]
        public Item requiredKey;

        [Header("UI Ayarlarý (Sadece Yazý)")]
        [Tooltip("Hiyerarþideki o her þeyin önünde parlayan 'KilitYazisi' benzeri TextMeshProUGUI objen")]
        public TextMeshProUGUI congratulationsText;

        [Header("Zamanlama Ayarý")]
        [Tooltip("Yazý ekranda kaç saniye dursun? (10 saniye)")]
        public float restartDelay = 5.0f;

        [Header("VR Input Action (Right Hand B)")]
        public InputAction rightHandBButton = new InputAction(binding: "<XRController>{RightHand}/secondaryButton");

        private bool isCompleted = false;
        private bool isHandInside = false;

        private void OnEnable()
        {
            rightHandBButton.Enable();
        }

        private void OnDisable()
        {
            rightHandBButton.Disable();
        }

        private void Start()
        {
            if (congratulationsText != null)
            {
                congratulationsText.gameObject.SetActive(false);
            }

            if (GameCache.Instance != null)
            {
                isCompleted = GameCache.Instance.GetMissionState("game_completed");
                if (isCompleted)
                {
                    ShowEndText();
                }
            }
        }

        private void Update()
        {
            if (!isCompleted && isHandInside && rightHandBButton.WasPressedThisFrame())
            {
                Debug.Log("Ana Kapýda B tuþuna basýldý! Rusty Key aranýyor...");
                TryEscapeWithKey();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.name.Contains("Hand") || other.name.Contains("Controller") || other.GetComponent<XRDirectInteractor>() != null)
            {
                isHandInside = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") || other.name.Contains("Hand") || other.name.Contains("Controller") || other.GetComponent<XRDirectInteractor>() != null)
            {
                isHandInside = false;
            }
        }

        private void TryEscapeWithKey()
        {
            Transform camTransform = Camera.main.transform;
            Transform inventoryTransform = camTransform.Find("Inventory");

            if (inventoryTransform == null) return;

            Inventory playerInventory = inventoryTransform.GetComponent<Inventory>();
            InventoryUI inventoryUI = inventoryTransform.GetComponent<InventoryUI>();

            if (playerInventory != null && playerInventory.slots != null)
            {
                bool keyFound = false;

                for (int i = 0; i < playerInventory.slots.Count; i++)
                {
                    var slot = playerInventory.slots[i];
                    if (slot != null && !slot.IsEmpty && slot.item != null && slot.item.id == requiredKey.id)
                    {
                        keyFound = true;
                        break;
                    }
                }

                if (keyFound)
                {
                    Debug.Log("[KAÇIÞ BAÞARILI] Rusty Key doðrulandý! Hafýza temizleniyor...");

                    if (GameCache.Instance != null)
                    {
                        GameCache.Instance.ClearAllData();
                        GameCache.Instance.SaveMissionState("game_completed", true);
                    }

                    if (inventoryUI != null) inventoryUI.RefreshUI();
                    HotbarUI hUI = inventoryTransform.GetComponent<HotbarUI>();
                    if (hUI != null) hUI.RefreshUI();

                    isCompleted = true;
                    ShowEndText();
                }
                else
                {
                    Debug.LogWarning("Bu kapýyý açmak için 'Rusty Key' gerekiyor!");
                }
            }
        }

        private void ShowEndText()
        {
            if (congratulationsText != null)
            {
                congratulationsText.gameObject.SetActive(true);


                if (Camera.main != null)
                {
                    Transform cam = Camera.main.transform;
                    congratulationsText.transform.position = cam.position + (cam.forward * 0.5f);
                    congratulationsText.transform.LookAt(cam);
                    congratulationsText.transform.Rotate(0, 180, 0); 
                }

                congratulationsText.text = "Congrats!\nYou escaped from the asylum.";
            }

            Invoke(nameof(StartNewGame), restartDelay);
        }

        public void StartNewGame()
        {
            if (GameCache.Instance != null)
            {
                GameCache.Instance.ClearAllData();
            }

            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }
}