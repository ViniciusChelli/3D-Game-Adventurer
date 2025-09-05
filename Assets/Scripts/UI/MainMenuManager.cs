using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Painéis de UI")]
    public GameObject mainPanel;      // painel com botões: Novo Jogo / Opções / Sair
    public GameObject optionsPanel;   // painel de opções (sliders etc.)

    public void NewGame()
    {
        SceneManager.LoadScene("customization");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenOptions()
    {
        if (mainPanel) mainPanel.SetActive(false);
        if (optionsPanel) optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        if (optionsPanel) optionsPanel.SetActive(false);
        if (mainPanel) mainPanel.SetActive(true);
    }
}
