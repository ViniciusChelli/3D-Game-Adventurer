using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class SophiaController : MonoBehaviour
{
    [Header("Configurações de rede e áudio")]
    public string mistralUrl = "http://localhost:11434/api/generate";
    public string coquiTTSUrl = "http://localhost:5002/api/tts";
    public AudioSource audioSource;

    [Header("Componentes da IA")]
    public SophiaMistral chatLocal;
    private RelationshipSystem relationshipSystem;
    private ReinforcementLearning reinforcementLearning;
    private SophiaMemory sophiaMemory;
    private SophiaPerception sophiaPerception;
    private SophiaNavMeshModule navModule;
    public SophiaControllerLLMApi llmApi = SophiaControllerLLMApi.OllamaGenerate;

    // Helper: 2D(x,y) -> 3D(XZ) mantendo Y atual
    private Vector3 XZ(Vector2 v) => new Vector3(v.x, transform.position.y, v.y);

    [Header("Legendas (Acessibilidade)")]
    public SubtitlesManager subtitles;

    [Header("UI - Diálogo")]
    public TMP_Text dialogueText;
    public GameObject loadingIndicator;
    public ScrollRect dialogueScrollRect;
    [Header("TTS")]
    public bool useEmbeddedTTS = true;
    public PiperTTS piperTTS; // arraste o componente PiperTTS da cena

    void Start()
    {
        if (chatLocal == null) chatLocal = FindObjectOfType<SophiaMistral>();
        if (relationshipSystem == null) relationshipSystem = FindObjectOfType<RelationshipSystem>();
        if (reinforcementLearning == null) reinforcementLearning = FindObjectOfType<ReinforcementLearning>();
        if (sophiaMemory == null) sophiaMemory = FindObjectOfType<SophiaMemory>();
        if (sophiaPerception == null) sophiaPerception = FindObjectOfType<SophiaPerception>();
        if (navModule == null) navModule = FindObjectOfType<SophiaNavMeshModule>();

        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        if (loadingIndicator != null) loadingIndicator.SetActive(false);
        if (dialogueScrollRect == null && dialogueText != null)
            dialogueScrollRect = dialogueText.GetComponentInParent<ScrollRect>();
        if (dialogueText != null) dialogueText.text = "";
    }

    public void ReceiveMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;
        Debug.Log($"📩 Mensagem recebida: {message}");

        reinforcementLearning?.ProcessResponse(message);

        string prompt = GeneratePrompt(message);

        if (dialogueText != null)
            dialogueText.text += $"\n<b><color=lime>Você:</color></b> {message}\nSophia está pensando...";
        if (loadingIndicator != null) loadingIndicator.SetActive(true);

        StartCoroutine(SendToMistralAndSpeak(prompt));
    }

    public string GeneratePrompt(string playerMessage)
    {
        string relationshipState = relationshipSystem ? relationshipSystem.GetRelationshipState() : "Desconhecido";
        string detectedObjects = sophiaPerception ? sophiaPerception.GetDetectedObjects() : null;
        string memoryContext = sophiaMemory ? sophiaMemory.GetMemoryContext() : null;

        string prompt = "Você é Sophia, uma NPC emocional em um jogo.";
        if (!string.IsNullOrWhiteSpace(relationshipState))
            prompt += $" Relacionamento: {relationshipState}.";
        if (!string.IsNullOrWhiteSpace(detectedObjects) && !detectedObjects.StartsWith("Nada"))
            prompt += $" Objetos visíveis: {detectedObjects}.";
        if (!string.IsNullOrWhiteSpace(memoryContext) && memoryContext.ToLower() != "nenhuma memória")
        {
            if (memoryContext.Trim().EndsWith(":") || memoryContext.Contains(": .") || memoryContext.Contains(".."))
                memoryContext = "Sem dados relevantes no momento";
            else
                memoryContext = memoryContext.Replace("..", ".").Replace(": .", ": nenhum dado");
            prompt += $" Memórias: {memoryContext}.";
        }
        prompt += $" O jogador disse: \"{playerMessage}\". Como você responde?";
        return prompt;
    }

    private IEnumerator SendToMistralAndSpeak(string prompt)
{
    string url;
    string jsonBody;

    if (llmApi == SophiaControllerLLMApi.OllamaGenerate)
    {
        url = mistralUrl.TrimEnd('/');
        // ex: http://127.0.0.1:11434/api/generate
        jsonBody = "{\"model\":\"mistral\",\"prompt\":\"" + EscapeJson(prompt) + "\",\"stream\":false}";
    }
    else // LlamaCpp
    {
        // vamos usar /v1/completions (formato OpenAI-like do llama.cpp server)
        url = mistralUrl.TrimEnd('/') + "/v1/completions";
        jsonBody = "{\"model\":\"mistral\",\"prompt\":\"" + EscapeJson(prompt) + "\",\"max_tokens\":256,\"temperature\":0.7}";
    }

    using (UnityWebRequest req = new UnityWebRequest(url, "POST"))
    {
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        req.uploadHandler   = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (loadingIndicator != null) loadingIndicator.SetActive(false);

        if (req.result == UnityWebRequest.Result.Success)
        {
            string result = req.downloadHandler.text;
            string response = ParseLLMResponse(result);
            if (string.IsNullOrWhiteSpace(response)) response = "Desculpe, não consegui responder.";

            sophiaMemory?.AddToMemory(response, false);

            if (dialogueText != null)
            {
                string currentText = dialogueText.text;
                int i = currentText.LastIndexOf("Sophia está pensando...");
                if (i != -1) currentText = currentText.Remove(i);
                dialogueText.text = currentText + $"\n<b><color=yellow>Sophia:</color></b> {response}";
            }

            // Fala
            StartCoroutine(SpeakWithTTS(response));

            // Comandos “vá até…”
            DetectAndHandleGoCommands(response);
        }
        else
        {
            Debug.LogError($"❌ Erro LLM: {req.error}\n{req.downloadHandler.text}");
            if (dialogueText != null)
            {
                string currentText = dialogueText.text;
                int i = currentText.LastIndexOf("Sophia está pensando...");
                if (i != -1) currentText = currentText.Remove(i);
                dialogueText.text = currentText + $"\n<b><color=red>Erro:</color></b> Falha ao obter resposta do LLM.";
            }
        }
    }
    ScrollToBottom();
}
// CHAMADOR ÚNICO — sempre use isso
private IEnumerator SpeakWithTTS(string texto)
{
    if (useEmbeddedTTS && piperTTS != null)
        yield return SpeakWithPiper(texto);
    else
        yield return SpeakWithCoquiTTS(texto); // fallback, se você ainda usa o Coqui
}

// Piper offline (recomendado)
private IEnumerator SpeakWithPiper(string texto)
{
    bool done = false;
    AudioClip clip = null;

    if (piperTTS == null)
    {
        Debug.LogWarning("[TTS] PiperTTS não setado. (useEmbeddedTTS=true)");
        yield break;
    }

    yield return piperTTS.SynthesizeToClip(texto, c => { clip = c; done = true; });
    while (!done) yield return null;

    if (clip != null)
    {
        audioSource.clip = clip;
        audioSource.Play();

        if (subtitles != null)
            subtitles.ShowForClip(texto, audioSource, exportSrt: false);
    }
}

// Se você não quer mais Coqui, deixe apenas um stub que cai no Piper:
private IEnumerator SpeakWithCoquiTTS(string texto)
{
    Debug.LogWarning("[TTS] Coqui desativado. Usando Piper como fallback.");
    yield return SpeakWithPiper(texto);
}

private string EscapeJson(string s) => s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n");

private string ParseLLMResponse(string raw)
{
    try
    {
        if (llmApi == SophiaControllerLLMApi.OllamaGenerate)
        {
            // {"response":"..."}
            return JsonUtility.FromJson<MistralResponse>(raw).response;
        }
        else
        {
            // llama.cpp /v1/completions -> {"choices":[{"text":"..."}]}
            var o = JsonUtility.FromJson<LlamaCompletion>(raw);
            if (o != null && o.choices != null && o.choices.Length > 0) return o.choices[0].text;
        }
    } catch { }
    return raw; // fallback
}

[Serializable] private class LlamaCompletion { public Choice[] choices; }
[Serializable] private class Choice { public string text; }

    public void PlayAudio(string resposta) => StartCoroutine(SpeakWithCoquiTTS(resposta));

    string ExtrairTexto(string raw)
    {
        try { return JsonUtility.FromJson<MistralResponse>(raw).response; }
        catch { Debug.LogWarning("❌ Erro ao decodificar JSON do Mistral."); return "Desculpe, houve um erro ao processar a resposta."; }
    }

    
    void DetectAndHandleGoCommands(string response)
    {
        if (navModule == null || sophiaPerception == null || string.IsNullOrEmpty(response)) return;

        string r = response.ToLower();
        bool wantsGo = r.Contains("vá até") || r.Contains("va até") || r.Contains("va ate") ||
                       r.Contains("ir até") || r.Contains("ir ate") || r.Contains("vá para") || r.Contains("va para") ||
                       r.Contains("vou até") || r.Contains("indo até");
        if (!wantsGo) return;

        string target = null;
        if (r.Contains("árvore") || r.Contains("arvore")) target = "árvore";
        else if (r.Contains("casa")) target = "casa";
        else if (r.Contains("lago")) target = "lago";
        if (target == null) return;

        if (sophiaPerception.TryGetNearest(target, out Vector2 p2))
        {
            navModule.GoTo(XZ(p2));
            Debug.Log($"🧭 Indo até {target} em {XZ(p2)}");
        }
        else if (dialogueText != null)
        {
            dialogueText.text += $"\n<b><i>Não encontrei {target} por perto.</i></b>";
        }
    }

    void ScrollToBottom()
    {
        if (dialogueScrollRect != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueScrollRect.content);
            dialogueScrollRect.verticalNormalizedPosition = 0f;
        }
    }

    [Serializable] public class MistralResponse { public string response; }
}
