using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class VRMenuController : MonoBehaviour
{
    [Header("UI Elemanlari")]
    public GameObject menuIcon;
    public GameObject mainMenuPanel;

    [Header("Ses Ayarlari")]
    public Slider volumeSlider;
    public AudioSource backgroundMusic;

    [Header("VR Input Ayari")]
    public InputActionProperty menuButtonAction;

    void Start()
    {
        CloseMenu();

        if (backgroundMusic != null && volumeSlider != null)
        {
            volumeSlider.value = backgroundMusic.volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    void Update()
    {
        if (menuButtonAction.action != null && menuButtonAction.action.WasPressedThisFrame())
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        if (mainMenuPanel != null)
        {
            bool isMenuActive = mainMenuPanel.activeSelf;
            if (isMenuActive)
            {
                CloseMenu();
            }
            else
            {
                OpenMenu();
            }
        }
    }

    public void OpenMenu()
    {
        if (menuIcon != null) menuIcon.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void CloseMenu()
    {
        if (menuIcon != null) menuIcon.SetActive(true);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
    }

    public void SetVolume(float value)
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = value;
        }
    }

    public void NewGame()
    {
        Debug.Log("[NEW GAME] Tüm veriler sýfýrlanýyor ve sahne yeniden yükleniyor...");

        if (InventoryFramework.GameCache.Instance != null)
        {
            InventoryFramework.GameCache.Instance.ClearAllData();
        }

        CloseMenu();

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void QuitGame()
    {
        Debug.Log("Oyun kapatiliyor...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}