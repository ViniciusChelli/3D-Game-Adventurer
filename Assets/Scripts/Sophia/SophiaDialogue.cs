using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SophiaDialogue : MonoBehaviour
{
    [Header("Referências")]
    public SophiaController sophiaController;
    public SophiaMistral sophiaMistral;
    public TMP_InputField inputField;
    public TMP_Text dialogueText;
    public GameObject loadingIcon;

    void Start()
    {
        if (sophiaController == null) sophiaController = FindObjectOfType<SophiaController>();
        if (sophiaMistral == null) sophiaMistral = FindObjectOfType<SophiaMistral>();
        if (loadingIcon != null) loadingIcon.SetActive(false);
    }

    public void EnviarMensagem()
    {
        string msg = inputField.text;
        if (string.IsNullOrWhiteSpace(msg)) return;

        inputField.text = "";
        if (dialogueText) dialogueText.text = "Você: " + msg + "\nSophia está pensando...";
        if (loadingIcon != null) loadingIcon.SetActive(true);

        string prompt = sophiaController.GeneratePrompt(msg);
        sophiaMistral.SendMessageToMistral(prompt, OnRespostaRecebida);
    }

    void OnRespostaRecebida(string resposta)
    {
        if (dialogueText) dialogueText.text = "Sophia: " + resposta;
        sophiaController.PlayAudio(resposta);
        if (loadingIcon != null) loadingIcon.SetActive(false);
    }
}
