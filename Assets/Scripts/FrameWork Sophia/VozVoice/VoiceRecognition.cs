using UnityEngine;
using UnityEngine.Windows.Speech;
using System;

/// <summary>
/// Módulo de reconhecimento de voz da Sophia usando DictationRecognizer (somente Windows).
/// Captura a fala do jogador e envia para análise emocional e resposta.
/// </summary>
public class VoiceRecognition : MonoBehaviour
{
    private DictationRecognizer dictationRecognizer; // Componente do Windows para ditado
    private SophiaEmotionResponse sophiaEmotionResponse; // Sistema que processa a fala emocionalmente
    private bool isRecognizerActive = false; // Flag de estado de escuta

    private void Start()
    {
        // Verifica se o sistema suporta reconhecimento de fala
        if (!CheckSpeechSystemStatus())
        {
            Debug.LogError("🚨 Erro: O reconhecimento de fala não está ativado ou suportado no sistema!");
            return;
        }

        // Busca o sistema de resposta emocional
        sophiaEmotionResponse = FindObjectOfType<SophiaEmotionResponse>();

        // Instancia o reconhecedor
        dictationRecognizer = new DictationRecognizer();

        // Liga eventos do reconhecedor
        dictationRecognizer.DictationResult += OnSpeechRecognized;
        dictationRecognizer.DictationComplete += OnDictationComplete;
        dictationRecognizer.DictationError += OnDictationError;

        // Aguarda 2s antes de ativar (evita falhas de inicialização)
        Invoke("StartListening", 2f);
    }

    /// <summary>
    /// Inicia o reconhecimento de fala.
    /// </summary>
    public void StartListening()
    {
        if (dictationRecognizer == null)
        {
            Debug.LogError("🚨 Erro: DictationRecognizer não inicializado corretamente!");
            return;
        }

        if (isRecognizerActive)
        {
            Debug.Log("🎤 Sophia já está ouvindo...");
            return;
        }

        try
        {
            dictationRecognizer.Start();
            isRecognizerActive = true;
            Debug.Log("🎤 Sophia está ouvindo...");
        }
        catch (Exception e)
        {
            Debug.LogError($"🚨 Erro ao iniciar reconhecimento de fala: {e.Message}");
        }
    }

    /// <summary>
    /// Evento disparado quando algo é reconhecido com sucesso.
    /// </summary>
    private void OnSpeechRecognized(string text, ConfidenceLevel confidence)
    {
        Debug.Log($"🗣️ Reconhecido: {text}");

        if (sophiaEmotionResponse != null)
        {
            sophiaEmotionResponse.ReceivePlayerMessage(text); // Passa a fala para o módulo emocional
        }
        else
        {
            Debug.LogWarning("⚠️ SophiaEmotionResponse não foi encontrado.");
        }

        RestartRecognizer(); // Reinicia escuta para continuar fluidez
    }

    /// <summary>
    /// Evento disparado quando a escuta finaliza por timeout ou erro.
    /// </summary>
    private void OnDictationComplete(DictationCompletionCause cause)
    {
        isRecognizerActive = false;

        if (cause == DictationCompletionCause.TimeoutExceeded)
            Debug.LogWarning("⚠️ Tempo de reconhecimento excedido, reiniciando...");
        else if (cause != DictationCompletionCause.Complete)
            Debug.LogWarning($"⚠️ DictationRecognizer finalizado inesperadamente: {cause}");

        RestartRecognizer();
    }

    /// <summary>
    /// Evento disparado em caso de erro durante reconhecimento de fala.
    /// </summary>
    private void OnDictationError(string error, int hresult)
    {
        Debug.LogError($"🚨 Erro de reconhecimento: {error} (Código: {hresult})");
        isRecognizerActive = false;
        RestartRecognizer();
    }

    /// <summary>
    /// Encerra o reconhecedor ao destruir o objeto (boas práticas de liberação).
    /// </summary>
    private void OnDestroy()
    {
        if (dictationRecognizer != null)
        {
            if (dictationRecognizer.Status == SpeechSystemStatus.Running)
                dictationRecognizer.Stop();

            dictationRecognizer.Dispose();
        }
    }

    /// <summary>
    /// Reinicia o reconhecedor após término ou erro.
    /// </summary>
    private void RestartRecognizer()
    {
        if (dictationRecognizer != null && dictationRecognizer.Status != SpeechSystemStatus.Running)
        {
            Debug.Log("🔄 Reiniciando DictationRecognizer...");
            Invoke("StartListening", 1f); // Pequeno atraso para segurança
        }
    }

    /// <summary>
    /// Tenta instanciar um DictationRecognizer temporário para testar suporte.
    /// </summary>
    private bool CheckSpeechSystemStatus()
    {
        try
        {
            using (var tempRecognizer = new DictationRecognizer())
            {
                return tempRecognizer.Status != SpeechSystemStatus.Failed;
            }
        }
        catch
        {
            return false;
        }
    }
}
