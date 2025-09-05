using UnityEngine;

/// <summary>
/// Responsável por analisar a fala reconhecida e derivar uma "emoção" que pode ser passada para o sistema de TTS ou expressão facial.
/// Esta classe serve de ponte entre o reconhecimento de voz e a resposta emocional da Sophia.
/// </summary>
public class SophiaEmotionResponse : MonoBehaviour
{
    public enum Emotion
    {
        Neutra,
        Feliz,
        Triste,
        Brava,
        Assustada,
        Amorosa
    }

    [Header("Referências")]
    public SophiaController sophiaController; // ponte com TTS e IA

    private void Start()
    {
        if (sophiaController == null)
            sophiaController = FindObjectOfType<SophiaController>();
    }

    /// <summary>
    /// Processa uma frase para detectar emoção e acionar respostas apropriadas.
    /// </summary>
    public void ReceivePlayerMessage(string frase)
    {
        Emotion detectedEmotion = DetectEmotion(frase);
        Debug.Log($"✨ Emoção detectada: {detectedEmotion}");

        // Em vez de repetir o que o jogador disse, repassamos para a IA gerar uma resposta
        sophiaController?.ReceiveMessage(frase);
    }

    private Emotion DetectEmotion(string frase)
    {
        frase = frase.ToLower();

        if (frase.Contains("feliz") || frase.Contains("alegre") || frase.Contains("legal"))
            return Emotion.Feliz;
        if (frase.Contains("triste") || frase.Contains("chateada") || frase.Contains("sozinha"))
            return Emotion.Triste;
        if (frase.Contains("raiva") || frase.Contains("brava") || frase.Contains("nervosa"))
            return Emotion.Brava;
        if (frase.Contains("medo") || frase.Contains("assustada") || frase.Contains("estranho"))
            return Emotion.Assustada;
        if (frase.Contains("amor") || frase.Contains("gosto de você") || frase.Contains("apaixonada"))
            return Emotion.Amorosa;

        return Emotion.Neutra;
    }
}
