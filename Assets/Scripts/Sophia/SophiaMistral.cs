using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using TMPro;

public class SophiaMistral : MonoBehaviour
{
    [Header("Configuração do Servidor")]
    public string serverUrl = "http://localhost:11434/api/generate";

    [Header("Componentes de UI")]
    public TextMeshProUGUI mistralText;
    public GameObject loadingIndicator;

    public void SendMessageToMistral(string prompt, Action<string> onResponse)
    {
        StartCoroutine(SendRequestCoroutine(prompt, onResponse));
    }

    IEnumerator SendRequestCoroutine(string prompt, Action<string> onResponse)
    {
        string jsonBody = "{\"model\": \"mistral\", \"prompt\": \"" + prompt.Replace("\"", "\\\"") + "\", \"stream\": false}";

        using (UnityWebRequest request = new UnityWebRequest(serverUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            if (loadingIndicator) loadingIndicator.SetActive(true);
            yield return request.SendWebRequest();
            if (loadingIndicator) loadingIndicator.SetActive(false);

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("❌ Erro ao se comunicar com o Mistral: " + request.error);
                onResponse?.Invoke("Erro ao obter resposta.");
                yield break;
            }

            string result = request.downloadHandler.text;
            try
            {
                string respostaFinal = JsonUtility.FromJson<MistralMsg>(result).response;
                if (mistralText) StartCoroutine(TypeResponse(respostaFinal));
                onResponse?.Invoke(respostaFinal);
            }
            catch (Exception e)
            {
                Debug.LogWarning("❌ Erro ao processar resposta JSON: " + e.Message);
                onResponse?.Invoke("Erro ao processar resposta.");
            }
        }
    }

    IEnumerator TypeResponse(string response)
    {
        if (!mistralText) yield break;
        mistralText.text = "";
        foreach (char c in response)
        {
            mistralText.text += c;
            yield return new WaitForSeconds(0.02f);
        }
    }

    [Serializable] private class MistralMsg { public string response; }
}
