using System.Collections.Generic;
using UnityEngine;

namespace InventoryFramework
{
    public class GameCache : MonoBehaviour
    {
        public static GameCache Instance { get; private set; }

        [Header("Cache Verileri")]
        private List<InventorySlot> cachedInventorySlots = new List<InventorySlot>();
        private Dictionary<string, bool> missionStates = new Dictionary<string, bool>();

        public Vector3 SavedPlayerPosition { get; set; }
        public bool IsInventoryCached => cachedInventorySlots.Count > 0;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadFromDisk();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            Invoke(nameof(AutoLoadInventoryOnStart), 0.1f);
        }

        private void AutoLoadInventoryOnStart()
        {
            Transform inventoryTransform = Camera.main.transform.Find("Inventory");
            if (inventoryTransform != null)
            {
                Inventory playerInventory = inventoryTransform.GetComponent<Inventory>();
                InventoryUI inventoryUI = inventoryTransform.GetComponent<InventoryUI>();

                if (playerInventory != null && cachedInventorySlots.Count > 0)
                {
                    for (int i = 0; i < playerInventory.slots.Count; i++)
                    {
                        if (i < cachedInventorySlots.Count)
                        {
                            playerInventory.slots[i].item = cachedInventorySlots[i].item;
                            playerInventory.slots[i].count = cachedInventorySlots[i].count;
                        }
                    }

                    if (inventoryUI != null) inventoryUI.RefreshUI();

                    HotbarUI hUI = inventoryTransform.GetComponent<HotbarUI>();
                    if (hUI != null) hUI.RefreshUI();

                    Debug.Log("[LOAD] Envanter arayüzü baþarýyla eski eþyalarla dolduruldu!");
                }
            }
        }

        public void SaveMissionState(string missionKey, bool isCompleted)
        {
            if (missionStates.ContainsKey(missionKey))
                missionStates[missionKey] = isCompleted;
            else
                missionStates.Add(missionKey, isCompleted);

            Debug.Log($"[CACHE] Durum Güncellendi -> {missionKey}: {isCompleted}");
            SaveToDisk();
        }

        public bool GetMissionState(string missionKey)
        {
            if (missionStates.ContainsKey(missionKey))
                return missionStates[missionKey];

            return false;
        }

        public void SaveToDisk()
        {
            PlayerPrefs.SetInt("Save_has_floppy", GetMissionState("has_floppy") ? 1 : 0);
            PlayerPrefs.SetInt("Save_pc_completed", GetMissionState("pc_completed") ? 1 : 0);
            PlayerPrefs.SetInt("Save_has_card", GetMissionState("has_card") ? 1 : 0);
            PlayerPrefs.SetInt("Save_has_rusty_key", GetMissionState("has_rusty_key") ? 1 : 0);
            PlayerPrefs.SetInt("Save_Card_Gate_Red_Door_unlocked", GetMissionState("Card_Gate_Red_Door_unlocked") ? 1 : 0);

            // --- OYUN BÝTTÝ DURUMU ÝÇÝN EKLENDÝ ---
            PlayerPrefs.SetInt("Save_game_completed", GetMissionState("game_completed") ? 1 : 0);

            Transform inventoryTransform = Camera.main.transform.Find("Inventory");
            if (inventoryTransform != null)
            {
                Inventory playerInventory = inventoryTransform.GetComponent<Inventory>();
                if (playerInventory != null && playerInventory.slots != null)
                {
                    cachedInventorySlots.Clear();
                    for (int i = 0; i < playerInventory.slots.Count; i++)
                    {
                        var slot = playerInventory.slots[i];
                        cachedInventorySlots.Add(new InventorySlot { item = slot.item, count = slot.count });

                        if (slot != null && !slot.IsEmpty && slot.item != null)
                        {
                            PlayerPrefs.SetInt($"Inv_Slot_ID_{i}", slot.item.id);
                            PlayerPrefs.SetInt($"Inv_Slot_Count_{i}", slot.count);
                        }
                        else
                        {
                            PlayerPrefs.SetInt($"Inv_Slot_ID_{i}", -1);
                            PlayerPrefs.SetInt($"Inv_Slot_Count_{i}", 0);
                        }
                    }
                    PlayerPrefs.SetInt("Inv_Total_Size", playerInventory.slots.Count);
                }
            }

            PlayerPrefs.Save();
            Debug.Log("[SAVE] Tüm ilerleme ve Oyun Bitiþ durumu kalýcý hafýzaya kaydedildi.");
        }

        private void LoadFromDisk()
        {
            missionStates.Clear();
            cachedInventorySlots.Clear();

            bool hasFloppy = PlayerPrefs.GetInt("Save_has_floppy", 0) == 1;
            bool pcCompleted = PlayerPrefs.GetInt("Save_pc_completed", 0) == 1;
            bool hasCard = PlayerPrefs.GetInt("Save_has_card", 0) == 1;
            bool hasRustyKey = PlayerPrefs.GetInt("Save_has_rusty_key", 0) == 1;
            bool doorUnlocked = PlayerPrefs.GetInt("Save_Card_Gate_Red_Door_unlocked", 0) == 1;

            // --- OYUN BÝTTÝ DURUMU ÝÇÝN EKLENDÝ ---
            bool gameCompleted = PlayerPrefs.GetInt("Save_game_completed", 0) == 1;

            missionStates.Add("has_floppy", hasFloppy);
            missionStates.Add("pc_completed", pcCompleted);
            missionStates.Add("has_card", hasCard);
            missionStates.Add("has_rusty_key", hasRustyKey);
            missionStates.Add("Card_Gate_Red_Door_unlocked", doorUnlocked);

            // --- OYUN BÝTTÝ DURUMU ÝÇÝN EKLENDÝ ---
            missionStates.Add("game_completed", gameCompleted);

            int totalSize = PlayerPrefs.GetInt("Inv_Total_Size", 0);
            if (totalSize > 0)
            {
                Item[] allItems = Resources.LoadAll<Item>("");

                for (int i = 0; i < totalSize; i++)
                {
                    int itemId = PlayerPrefs.GetInt($"Inv_Slot_ID_{i}", -1);
                    int itemCount = PlayerPrefs.GetInt($"Inv_Slot_Count_{i}", 0);

                    InventorySlot loadedSlot = new InventorySlot();

                    if (itemId != -1)
                    {
                        foreach (var item in allItems)
                        {
                            if (item.id == itemId)
                            {
                                loadedSlot.item = item;
                                loadedSlot.count = itemCount;
                                break;
                            }
                        }
                    }
                    cachedInventorySlots.Add(loadedSlot);
                }
            }

            Debug.Log($"[LOAD] Diskten tüm ilerleme ve oyun sonu verileri yüklendi.");
        }

        public void ClearAllData()
        {
            PlayerPrefs.DeleteAll();
            missionStates.Clear();
            cachedInventorySlots.Clear();
            Debug.Log("[CACHE] Tüm oyun kayýtlarý tamamen sýfýrlandý!");
        }
    }
}