using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace InventoryFramework
{
    public class VRItemPickup : MonoBehaviour
    {
        [Header("Item Config")]
        public Item itemData;
        public int amount = 1;

        [Header("Item Türü Belirleme (Cache İçin)")]
        public bool isFloppyDisk = false;
        public bool isRewardCard = false;
        public bool isRustyKey = false; 

        [Header("VR Input Actions (Right Hand)")]
        public InputAction rightHandAButton =
            new InputAction(binding: "<XRController>{RightHand}/primaryButton"); 

        public InputAction rightHandBButton =
            new InputAction(binding: "<XRController>{RightHand}/secondaryButton"); 

        private XRGrabInteractable grabInteractable;
        private IXRSelectInteractor holdingInteractor;
        private bool isGrabbed = false;

        void Awake()
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
        }

        void OnEnable()
        {
            if (grabInteractable != null)
            {
                grabInteractable.selectEntered.AddListener(OnGrabbed);
                grabInteractable.selectExited.AddListener(OnReleased);
            }

            rightHandAButton.Enable();
            rightHandBButton.Enable();
        }

        void OnDisable()
        {
            if (grabInteractable != null)
            {
                grabInteractable.selectEntered.RemoveListener(OnGrabbed);
                grabInteractable.selectExited.RemoveListener(OnReleased);
            }

            rightHandAButton.Disable();
            rightHandBButton.Disable();
        }

        void Start()
        {
            
            if (GameCache.Instance != null)
            {
                
                if (isFloppyDisk && (GameCache.Instance.GetMissionState("has_floppy") || GameCache.Instance.GetMissionState("pc_completed")))
                {
                    Destroy(gameObject);
                    return;
                }

                
                if (isRewardCard && GameCache.Instance.GetMissionState("has_card"))
                {
                    Destroy(gameObject);
                    return;
                }

                
                if (isRustyKey && GameCache.Instance.GetMissionState("has_rusty_key"))
                {
                    Destroy(gameObject);
                    return;
                }

                
                if (isRewardCard && !GameCache.Instance.GetMissionState("pc_completed"))
                {
                    gameObject.SetActive(false);
                }
            }
        }

        void Update()
        {
            if (isGrabbed)
            {
                
                if (rightHandAButton.WasPressedThisFrame())
                {
                    Debug.Log("A Tuşuna basıldı, envantere ekleme deneniyor...");
                    TryAddToInventoryDirectly();
                }

                
                if (rightHandBButton.WasPressedThisFrame())
                {
                    Debug.Log("B Tuşuna basıldı, terminale ışın atılıyor...");
                    TryTriggerMechanism();
                }
            }
        }

        private void OnGrabbed(SelectEnterEventArgs args)
        {
            isGrabbed = true;
            holdingInteractor = args.interactorObject;
        }

        private void OnReleased(SelectExitEventArgs args)
        {
            isGrabbed = false;
            holdingInteractor = null;
        }

        private void TryAddToInventoryDirectly()
        {
            if (itemData == null) return;

            
            Transform camTransform = Camera.main.transform;
            Transform inventoryTransform = camTransform.Find("Inventory");

            if (inventoryTransform != null)
            {
                Inventory targetInventory = inventoryTransform.GetComponent<Inventory>();
                InventoryUI iUI = inventoryTransform.GetComponent<InventoryUI>();

                if (targetInventory != null && iUI != null)
                {
                    bool success = targetInventory.AddItem(itemData, amount);

                    if (success)
                    {
                        Debug.Log($"[BAŞARILI] {itemData.itemName} başarıyla Inventory'e eklendi!");
                        iUI.RefreshUI();

                        
                        HotbarUI hUI = inventoryTransform.GetComponent<HotbarUI>();
                        if (hUI != null) hUI.RefreshUI();

                        
                        if (GameCache.Instance != null)
                        {
                            if (isFloppyDisk)
                            {
                                GameCache.Instance.SaveMissionState("has_floppy", true);
                                Debug.Log("[CACHE] Mavi Disket toplandı olarak kaydedildi.");
                            }
                            else if (isRewardCard)
                            {
                                GameCache.Instance.SaveMissionState("has_card", true);
                                Debug.Log("[CACHE] Kırmızı Kart toplandı olarak kaydedildi.");
                            }
                            
                            else if (isRustyKey)
                            {
                                GameCache.Instance.SaveMissionState("has_rusty_key", true);
                                Debug.Log("[CACHE] Paslı Anahtar toplandı olarak kaydedildi.");
                            }
                        }

                        if (grabInteractable != null && holdingInteractor != null)
                        {
                            grabInteractable.interactionManager.SelectExit(holdingInteractor, grabInteractable);
                        }

                        
#if UNITY_EDITOR
                        UnityEditor.Selection.activeObject = null;
#endif

                        Destroy(gameObject, 0.05f);
                    }
                    else
                    {
                        Debug.LogWarning("Envanter dolu olduğundan eşya eklenemedi!");
                    }
                }
            }
            else
            {
                Debug.LogError("Kritik Hata: Main Camera altında 'Inventory' adında bir obje bulunamadı!");
            }
        }

        private void TryTriggerMechanism()
        {
            
        }

        private void OnDestroy()
        {
            
            if (rightHandAButton != null) rightHandAButton.Dispose();
            if (rightHandBButton != null) rightHandBButton.Dispose();
        }
    }
}