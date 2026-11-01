using UnityEngine;

public class MenuPanelController : MonoBehaviour
{
    [Header("Menu Panel")]
    [SerializeField] private GameObject menuPanel;

    private void Awake()
    {
        if (menuPanel != null)
            menuPanel.SetActive(false);
    }

    public void ToggleMenu()
    {
        if (menuPanel == null) return;

        menuPanel.SetActive(!menuPanel.activeSelf);
    }

    public void OpenMenu()
    {
        if (menuPanel == null) return;

        menuPanel.SetActive(true);
    }

    public void CloseMenu()
    {
        if (menuPanel == null) return;

        menuPanel.SetActive(false);
    }
}