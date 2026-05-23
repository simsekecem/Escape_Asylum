using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace InventoryFramework
{
    public class MissionTerminal : MonoBehaviour
    {
        [Header("Gereksinimler")]
        public Item requiredItem; 

        [Header("ÖDÜL AYARLARI")]
        public GameObject rewardCard; 

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
            
            if (GameCache.Instance != null)
            {
                
                isCompleted = GameCache.Instance.GetMissionState("pc_completed");

                
                bool cardPickedUp = GameCache.Instance.GetMissionState("has_card");

                if (isCompleted)
                {
                    
                    
                    if (rewardCard != null)
                    {
                        rewardCard.SetActive(!cardPickedUp);
                    }
                    Debug.Log($"[CACHE] Terminal durumu geri yüklendi. Görev Tamamlanmış mı: {isCompleted}, Kart Toplanmış mı: {cardPickedUp}");
                }
            }
        }

        private void Update()
        {
            if (!isCompleted && isHandInside && rightHandBButton.WasPressedThisFrame())
            {
                Debug.Log("Bilgisayar alanında B tuşuna basıldı! Envanter taranıyor...");
                TryProcessMissionFromInventory();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.name.Contains("Hand") || other.name.Contains("Controller") || other.GetComponent<XRDirectInteractor>() != null)
            {
                isHandInside = true;
                Debug.Log("VR Eli bilgisayar etkileşim alanına girdi. B tuşuna basılabilir.");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") || other.name.Contains("Hand") || other.name.Contains("Controller") || other.GetComponent<XRDirectInteractor>() != null)
            {
                isHandInside = false;
                Debug.Log("VR Eli bilgisayar etkileşim alanından çıktı.");
            }
        }

        private void TryProcessMissionFromInventory()
        {
            Transform camTransform = Camera.main.transform;
            Transform inventoryTransform = camTransform.Find("Inventory");

            if (inventoryTransform == null)
            {
                Debug.LogError("Terminal: Main Camera altında 'Inventory' objesi bulunamadı!");
                return;
            }

            Inventory playerInventory = inventoryTransform.GetComponent<Inventory>();
            InventoryUI inventoryUI = inventoryTransform.GetComponent<InventoryUI>();

            if (playerInventory != null && playerInventory.slots != null)
            {
                bool itemFound = false;

                for (int i = 0; i < playerInventory.slots.Count; i++)
                {
                    var slot = playerInventory.slots[i];

                    if (slot != null && !slot.IsEmpty && slot.item != null && slot.item.id == requiredItem.id)
                    {
                        slot.count--;
                        itemFound = true;

                        if (slot.count <= 0)
                        {
                            slot.item = null;
                            slot.count = 0;
                        }
                        break;
                    }
                }

                if (itemFound)
                {
                    Debug.Log("[BAŞARILI] Envanter slotlarında Mavi Disket bulundu ve düşüldü!");

                    if (inventoryUI != null) inventoryUI.RefreshUI();

                    HotbarUI hUI = inventoryTransform.GetComponent<HotbarUI>();
                    if (hUI != null) hUI.RefreshUI();

                    
                    if (GameCache.Instance != null)
                    {
                        
                        GameCache.Instance.SaveMissionState("has_floppy", false);
                        
                        GameCache.Instance.SaveMissionState("pc_completed", true);
                    }

                    CompleteMission();
                }
                else
                {
                    Debug.LogWarning("Envanter slotlarında gerekli olan 'Floppy_Blue' bulunamadı!");
                }
            }
        }

        private void CompleteMission()
        {
            isCompleted = true;

            if (rewardCard != null)
            {
                rewardCard.SetActive(true);
                Debug.Log($"[ÖDÜL] {rewardCard.name} başarıyla görünür yapıldı!");
            }
            else
            {
                Debug.LogError("Terminal: Reward Card referansı boş!");
            }
        }
    }
}