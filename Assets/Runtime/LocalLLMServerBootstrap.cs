// Assets/Runtime/LocalLLMServerBootstrap.cs
using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Diagnostics; // Process, ProcessStartInfo

public class LocalLLMServerBootstrap : MonoBehaviour
{
    public enum Backend { LlamaCppServer }

    [Header("Backend")]
    public Backend backend = Backend.LlamaCppServer;

    [Header("Arquivos (relativos a StreamingAssets)")]
    public string llamaExeWin   = "Tools/llama/win/llama-server.exe";
    public string llamaExeLinux = "Tools/llama/linux/llama-server";
    public string llamaExeMac   = "Tools/llama/mac/llama-server";
    public string ggufModelRel  = "Models/mistral-7b-instruct.Q4_K_M.gguf";

    [Header("Servidor")]
    public int port = 8080;
    public int contextTokens = 4096;

    [Header("Integração com a Sophia")]
    public SophiaController targetController;

    private Process _proc;
    private string _exePath;
    private string _modelPath;

    private void Start()
    {
        string sa = Application.streamingAssetsPath;
        string pp = Application.persistentDataPath;

        _exePath   = CopyToPersistent(ResolveExePath(sa), pp);
        _modelPath = CopyToPersistent(Path.Combine(sa, ggufModelRel), pp);

#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        MakeExecutable(_exePath);
#endif
        StartCoroutine(CoStartLLM());
    }

    private IEnumerator CoStartLLM()
    {
        // inicia o servidor
        string args = $"--model \"{_modelPath}\" -c {contextTokens} --host 127.0.0.1 --port {port}";
        _proc = StartProcess(_exePath, args);
        UnityEngine.Debug.Log($"[LLM] Start: {_exePath} {args}");

        // health check (sem try/catch para não misturar com yield)
        string url = $"http://127.0.0.1:{port}/health";
        float t = 0f; bool ok = false;

        while (t < 25f)
        {
            using (var req = UnityWebRequest.Get(url))
            {
                yield return req.SendWebRequest();
                if (req.result == UnityWebRequest.Result.Success)
                {
                    ok = true;
                    break;
                }
            }
            t += 1f;
            yield return new WaitForSeconds(1f);
        }

        if (!ok) UnityEngine.Debug.LogWarning("[LLM] health-check falhou (o server pode estar ok mesmo assim).");

        // pluga na Sophia
        if (targetController != null)
        {
            targetController.llmApi = SophiaControllerLLMApi.LlamaCpp;
            targetController.mistralUrl = $"http://127.0.0.1:{port}";
            UnityEngine.Debug.Log("[LLM] SophiaController → llama.cpp server.");
        }
    }

    private void OnDestroy()
{
    try
    {
        if (_proc != null && !_proc.HasExited)
        {
            // tenta fechar “limpo” (quase sempre não tem janela, então vai direto pro Kill)
            try { _proc.CloseMainWindow(); } catch { }
            _proc.Kill();        // <- sem o (true)
            _proc.Dispose();
        }
    }
    catch { }
}


    private Process StartProcess(string exe, string args)
    {
        var psi = new ProcessStartInfo(exe, args)
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError  = true,
            CreateNoWindow = true,
            WorkingDirectory = Path.GetDirectoryName(exe)
        };
        var p = new Process { StartInfo = psi, EnableRaisingEvents = true };
        p.OutputDataReceived += (_, e) => { if (!string.IsNullOrEmpty(e.Data)) UnityEngine.Debug.Log("[llama] " + e.Data); };
        p.ErrorDataReceived  += (_, e) => { if (!string.IsNullOrEmpty(e.Data)) UnityEngine.Debug.LogWarning("[llama-err] " + e.Data); };
        p.Start();
        p.BeginOutputReadLine();
        p.BeginErrorReadLine();
        return p;
    }

    private string ResolveExePath(string streamingAssets)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        return Path.Combine(streamingAssets, llamaExeWin);
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        return Path.Combine(streamingAssets, llamaExeMac);
#else
        return Path.Combine(streamingAssets, llamaExeLinux);
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
            return dst;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogWarning("[LLM] Copy fail: " + e.Message + " | usando direto do StreamingAssets.");
            return srcFull; // fallback
        }
    }

    private void MakeExecutable(string path)
    {
        try
        {
            var psi = new ProcessStartInfo("/bin/chmod", $"+x \"{path}\"")
            {
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process.Start(psi);
        }
        catch (Exception e) { UnityEngine.Debug.LogWarning("[LLM] chmod falhou: " + e.Message); }
    }
}
