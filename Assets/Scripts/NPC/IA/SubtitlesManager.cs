using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// Gera legendas acessíveis (CC) sincronizadas com o áudio.
/// Estratégia: divide por frases/pontuação e distribui o tempo proporcional ao tamanho.
/// Também pode exportar SRT.
/// </summary>
public class SubtitlesManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text captionsText;           // Texto de legenda (Canvas)
    public CanvasGroup captionsBackground;  // Opcional: fundo com alpha para contraste

    [Header("Timings")]
    public float minChunkTime = 0.6f;       // tempo mínimo por pedaço
    public float leadIn = 0.05f;            // pré-atraso antes da primeira legenda
    public float leadOut = 0.1f;            // pós

    private Coroutine running;

    public void ShowForClip(string fullText, AudioSource src, bool exportSrt = false)
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(RunCaptions(fullText, src, exportSrt));
    }

    private IEnumerator RunCaptions(string fullText, AudioSource src, bool exportSrt)
    {
        if (!src || !src.clip || string.IsNullOrWhiteSpace(fullText))
            yield break;

        // Quebra em frases (pontos, exclamação, interrogação) mantendo pontuação
        var sentences = SplitSentences(fullText);
        float totalLen = sentences.Sum(s => s.Length);
        float clipDur = src.clip.length;

        // Opcional: exporta SRT
        StringBuilder srt = exportSrt ? new StringBuilder() : null;
        float tCursor = 0f;
        int idx = 1;

        yield return new WaitForSeconds(leadIn);

        foreach (var s in sentences)
        {
            float weight = s.Length / Mathf.Max(1f, totalLen);
            float chunkTime = Mathf.Max(minChunkTime, clipDur * weight);

            // UI
            if (captionsText) captionsText.text = s.Trim();
            if (captionsBackground) captionsBackground.alpha = 0.75f;

            // SRT
            if (exportSrt)
            {
                float start = tCursor;
                float end = Mathf.Min(clipDur + leadOut, tCursor + chunkTime);
                srt.AppendLine(idx.ToString());
                srt.AppendLine($"{Fmt(start)} --> {Fmt(end)}");
                srt.AppendLine(s.Trim());
                srt.AppendLine();
            }

            yield return new WaitForSeconds(chunkTime);
            tCursor += chunkTime;
            idx++;
        }

        // Limpa UI
        if (captionsText) captionsText.text = "";
        if (captionsBackground) captionsBackground.alpha = 0f;

        // Salva SRT
        if (exportSrt)
        {
            string path = Path.Combine(Application.persistentDataPath, $"sophia_{System.DateTime.Now:yyyyMMdd_HHmmss}.srt");
            File.WriteAllText(path, srt.ToString(), Encoding.UTF8);
            Debug.Log($"📝 SRT salvo em: {path}");
        }
    }

    private static string[] SplitSentences(string text)
    {
        // Divide por . ! ? e também vírgulas longas; mantém a pontuação no chunk
        var list = new System.Collections.Generic.List<string>();
        StringBuilder cur = new StringBuilder();
        foreach (char c in text)
        {
            cur.Append(c);
            if (c == '.' || c == '!' || c == '?' )
            {
                list.Add(cur.ToString());
                cur.Clear();
            }
        }
        if (cur.Length > 0) list.Add(cur.ToString());
        return list.Where(s => s.Trim().Length > 0).ToArray();
    }

    private static string Fmt(float t)
    {
        int h = (int)(t / 3600f);
        int m = (int)((t % 3600f) / 60f);
        int s = (int)(t % 60f);
        int ms = (int)((t - Mathf.Floor(t)) * 1000f);
        return $"{h:00}:{m:00}:{s:00},{ms:000}";
    }
}
