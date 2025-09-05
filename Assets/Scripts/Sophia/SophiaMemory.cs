using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleJSON;

public class SophiaMemory : MonoBehaviour
{
    public List<string> ShortTermMemory = new();
    public List<string> LongTermMemory = new();

    string memoryFilePath;

    void Start()
    {
        memoryFilePath = Path.Combine(Application.persistentDataPath, "SophiaMemory.json");
        LoadMemory();

        if (LongTermMemory.Count == 0)
        {
            AddToMemory("Meu nome é Sophia.", true);
            AddToMemory("Sou engenheira agrônoma, especialista em plantas e olericultura.", true);
            AddToMemory("Sou dona da Fazenda Pouso Feliz.", true);
            AddToMemory("Normalmente sou alegre, extrovertida e adoro conversar.", true);
        }
    }

    public void AddToMemory(string memory, bool isLongTerm = false)
    {
        if (isLongTerm) LongTermMemory.Add(memory);
        else
        {
            if (ShortTermMemory.Count >= 10) ShortTermMemory.RemoveAt(0);
            ShortTermMemory.Add(memory);
        }
        SaveMemory();
    }

    public string GetMemoryContext()
    {
        string curto = ShortTermMemory.Count > 0 ? string.Join(", ", ShortTermMemory) : "Nada registrado.";
        string longo = LongTermMemory.Count > 0 ? string.Join(", ", LongTermMemory) : "Nada registrado.";
        return $"Curto Prazo: {curto} | Longo Prazo: {longo}";
    }

    public string SerializeMemory()
    {
        JSONNode json = new JSONObject();
        var arrS = new JSONArray(); json["ShortTermMemory"] = arrS;
        var arrL = new JSONArray(); json["LongTermMemory"] = arrL;

        foreach (var m in ShortTermMemory) arrS.Add(m);
        foreach (var m in LongTermMemory) arrL.Add(m);

        return json.ToString();
    }

    public void DeserializeMemory(string jsonData)
    {
        if (string.IsNullOrEmpty(jsonData)) return;
        var json = JSON.Parse(jsonData);

        ShortTermMemory.Clear(); LongTermMemory.Clear();
        foreach (var m in json["ShortTermMemory"].AsArray) ShortTermMemory.Add(m.Value);
        foreach (var m in json["LongTermMemory"].AsArray) LongTermMemory.Add(m.Value);
    }

    public void SaveMemory()
    {
        string json = SerializeMemory();
        File.WriteAllText(memoryFilePath, json);
    }

    public void LoadMemory()
    {
        if (File.Exists(memoryFilePath))
        {
            string json = File.ReadAllText(memoryFilePath);
            DeserializeMemory(json);
        }
    }
}
