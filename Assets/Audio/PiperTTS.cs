using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using UnityEngine;
using Debug = UnityEngine.Debug;
public class PiperTTS : MonoBehaviour
{
    [Header("Arquivos (StreamingAssets → copia p/ persistent)")]
    public string piperExeWin   = "Tools/piper/win/piper.exe";
    public string piperExeLinux = "Tools/piper/linux/piper";
    public string piperExeMac   = "Tools/piper/mac/piper";
    public string voiceModelRel = "Voices/pt-br.onnx"; // troque p/ a voz que você embutir

    [Header("Config")]
    public float sentenceSilence = 0.2f;    // pausa entre frases
    public bool  keepWavOnDisk = false;     // debug

    private string _exePath;
    private string _modelPath;

    private void Awake()
    {
        string sa = Application.streamingAssetsPath;
        string pp = Application.persistentDataPath;

        _exePath   = CopyToPersistent(ResolveExe(sa), pp);
        _modelPath = CopyToPersistent(Path.Combine(sa, voiceModelRel), pp);

#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        MakeExecutable(_exePath);
#endif
    }

    public IEnumerator SynthesizeToClip(string text, Action<AudioClip> onDone)
    {
        if (string.IsNullOrWhiteSpace(text)) { onDone?.Invoke(null); yield break; }

        string outPath = Path.Combine(Application.persistentDataPath, "piper_out.wav");
        if (File.Exists(outPath)) { try { File.Delete(outPath); } catch { } }

        // piper lê do stdin. Vamos chamar: piper -m model.onnx -f out.wav --sentence_silence 0.2
        var psi = new ProcessStartInfo(_exePath,
            $"-m \"{_modelPath}\" -f \"{outPath}\" --sentence_silence {sentenceSilence.ToString(System.Globalization.CultureInfo.InvariantCulture)}")
        {
            UseShellExecute = false,
            RedirectStandardInput  = true,
            RedirectStandardError  = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
            WorkingDirectory = Path.GetDirectoryName(_exePath)
        };

        var p = new Process { StartInfo = psi, EnableRaisingEvents = true };
        p.Start();

        // Manda o texto
        using (var sw = p.StandardInput)
        {
            sw.Write(text);
        }
        // Espera finalizar
        while (!p.HasExited) yield return null;

        if (!File.Exists(outPath))
        {
            UnityEngine.Debug.LogError("[Piper] WAV não foi gerado.");
            onDone?.Invoke(null);
            yield break;
        }

        byte[] wav = File.ReadAllBytes(outPath);
        if (!keepWavOnDisk) { try { File.Delete(outPath); } catch { } }

        try
        {
            AudioClip clip = WAVReader.ToAudioClip(wav, "Piper_TTS");
            onDone?.Invoke(clip);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("[Piper] Falha ao ler WAV: " + e.Message);
            onDone?.Invoke(null);
        }
    }

    // --- util ---
    private string ResolveExe(string sa)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        return Path.Combine(sa, piperExeWin);
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        return Path.Combine(sa, piperExeMac);
#else
        return Path.Combine(sa, piperExeLinux);
#endif
    }

    private string CopyToPersistent(string srcFull, string persistentRoot)
    {
        string dst = Path.Combine(persistentRoot, Path.GetFileName(srcFull));
        try
        {
            if (!File.Exists(dst))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dst));
                File.Copy(srcFull, dst, true);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("[Piper] Copy fail: " + e.Message + " | src=" + srcFull);
            return srcFull;
        }
        return dst;
    }

    private void MakeExecutable(string path)
    {
        try
        {
            var p = new ProcessStartInfo("/bin/chmod", $"+x \"{path}\"") { UseShellExecute=false, CreateNoWindow=true };
            Process.Start(p);
        }
        catch (Exception e) { Debug.LogWarning("[Piper] chmod falhou: " + e.Message); }
    }
}
