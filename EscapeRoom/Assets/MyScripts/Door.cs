using InventoryFramework; 
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace DoorScript
{
    [RequireComponent(typeof(AudioSource))]
    public class Door : MonoBehaviour
    {
        [Header("State")]
        public bool open = false;
        public bool startLocked = false; 

        private bool isDoorMovingLocked = false; 
        private bool actualKeyLocked = false;    
        private bool playerInside = false;
        private bool isHandInside = false;       

        [Header("Gereksinimler")]
        public Item requiredCardItem; 
        public string doorUniqueSaveKey = "Card_Gate_Red_Door"; 

        [Header("VR Input Action (Right Hand B)")]
        public InputAction rightHandBButton = new InputAction(binding: "<XRController>{RightHand}/secondaryButton");

        [Header("Timing")]
        public float closeDelay = 1.5f;
        private float closeTimer = 0f;

        [Header("Movement")]
        public float smooth = 2.0f;
        public float defaultOpenAngle = 90.0f;
        public float closeAngle = 0.0f;

        [Header("Audio")]
        public AudioClip openDoorSound;
        public AudioClip closeDoorSound;
        public AudioClip lockedDoorSound; 

        private AudioSource audioSource;
        private float dynamicOpenAngle;
        private Transform currentHandTransform; 

        private void OnEnable() => rightHandBButton.Enable();
        private void OnDisable() => rightHandBButton.Disable();

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            transform.localRotation = Quaternion.Euler(0, closeAngle, 0);

            
            if (GameCache.Instance != null)
            {
                if (startLocked)
                {
                    bool isUnlockedBefore = GameCache.Instance.GetMissionState(doorUniqueSaveKey + "_unlocked");
                    actualKeyLocked = !isUnlockedBefore;
                }
                else
                {
                    actualKeyLocked = false;
                }
            }
            else
            {
                actualKeyLocked = startLocked;
            }
        }

        void Update()
        {
            
            if (actualKeyLocked && isHandInside && rightHandBButton.WasPressedThisFrame())
            {
                Debug.Log("Kilitli kapıda B tuşuna basıldı! Kart kontrol ediliyor...");
                TryUnlockAndOpenWithCard();
            }
            
            else if (!actualKeyLocked && isHandInside && rightHandBButton.WasPressedThisFrame())
            {
                ToggleDoorState();
            }

            
            float targetAngle = open ? dynamicOpenAngle : closeAngle;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

            transform.localRotation =
                Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * smooth);

            
            if (playerInside && open)
            {
                closeTimer = 0f;
            }
            else if (open)
            {
                closeTimer += Time.deltaTime;

                if (closeTimer >= closeDelay)
                {
                    CloseDoor();
                }
            }
        }

        private void ToggleDoorState()
        {
            if (open)
                CloseDoor();
            else
                OpenDoor(currentHandTransform != null ? currentHandTransform.position : transform.position + transform.forward);
        }

        private void TryUnlockAndOpenWithCard()
        {
            Transform camTransform = Camera.main.transform;
            Transform inventoryTransform = camTransform.Find("Inventory");

            if (inventoryTransform == null) return;

            Inventory playerInventory = inventoryTransform.GetComponent<Inventory>();

            if (playerInventory != null && playerInventory.slots != null)
            {
                bool cardFound = false;

                
                for (int i = 0; i < playerInventory.slots.Count; i++)
                {
                    var slot = playerInventory.slots[i];
                    if (slot != null && !slot.IsEmpty && slot.item != null && slot.item.id == requiredCardItem.id)
                    {
                        cardFound = true;
                        break;
                    }
                }

                if (cardFound)
                {
                    actualKeyLocked = false; 
                    Debug.Log("[BAŞARILI] Kart bulundu, kilit açılıyor ve kapı açılıyor!");

                    
                    if (GameCache.Instance != null)
                    {
                        GameCache.Instance.SaveMissionState(doorUniqueSaveKey + "_unlocked", true);
                    }

                    
                    if (DoorUIManager.Instance != null)
                    {
                        DoorUIManager.Instance.ShowMessage("The door is unlocked!", 1.5f);
                    }

                    
                    OpenDoor(currentHandTransform != null ? currentHandTransform.position : transform.position + transform.forward);
                }
                else
                {
                    Debug.LogWarning("Kart envanterde yok! Kapı açılamaz.");
                    PlaySound(lockedDoorSound);

                    if (DoorUIManager.Instance != null)
                    {
                        DoorUIManager.Instance.ShowMessage("You need the card! (There is no card in your inventory)", 2.0f);
                    }
                }
            }
        }

        public void OpenDoor(Vector3 triggerPosition)
        {
            if (open || isDoorMovingLocked || actualKeyLocked) return;

            isDoorMovingLocked = true;

            Vector3 directionToPlayer = triggerPosition - transform.position;
            float dotForward = Vector3.Dot(directionToPlayer.normalized, transform.forward);
            float dotRight = Vector3.Dot(directionToPlayer.normalized, transform.right);

            if (Mathf.Abs(dotForward) > Mathf.Abs(dotRight))
            {
                dynamicOpenAngle = (dotForward > 0) ? -defaultOpenAngle : defaultOpenAngle;
            }
            else
            {
                dynamicOpenAngle = (dotRight > 0) ? -defaultOpenAngle : defaultOpenAngle;
            }

            open = true;
            PlaySound(openDoorSound);

            Invoke(nameof(UnlockMoving), 0.2f);
        }

        public void CloseDoor()
        {
            if (!open || isDoorMovingLocked) return;

            isDoorMovingLocked = true;
            open = false;
            PlaySound(closeDoorSound);

            Invoke(nameof(UnlockMoving), 0.2f);
        }

        void UnlockMoving() => isDoorMovingLocked = false;

        private void PlaySound(AudioClip clip)
        {
            if (clip != null && audioSource != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("PlayerHand") || other.name.Contains("Hand") || other.name.Contains("Controller") || other.GetComponent<XRDirectInteractor>() != null)
            {
                isHandInside = true;
                playerInside = true;
                currentHandTransform = other.transform;

                
                if (actualKeyLocked)
                {
                    // Eğer kapı kilitliyse ASLA OTOMATİK AÇMA, sadece ekrana yazı yazdır!
                    Debug.Log("Kilitli kapıya el değdi. Otomatik açılma engellendi.");

                    if (DoorUIManager.Instance != null)
                    {
                        DoorUIManager.Instance.ShowMessage("Door is Locked! You need the card to open up.", 3.5f);
                    }
                }
                else
                {
                    
                    OpenDoor(other.transform.position);
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("PlayerHand") || other.name.Contains("Hand") || other.name.Contains("Controller") || other.GetComponent<XRDirectInteractor>() != null)
            {
                isHandInside = false;
                playerInside = false;
                currentHandTransform = null;

                
                if (DoorUIManager.Instance != null)
                {
                    DoorUIManager.Instance.HideText();
                }
            }
        }
    }
}