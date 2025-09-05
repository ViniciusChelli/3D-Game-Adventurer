using System.Collections.Generic;
using UnityEngine;

public class NPCInventory : MonoBehaviour
{
    [System.Serializable]
    public class Stack { public string type; public int count; }

    public int capacity = 20;
    public List<Stack> slots = new List<Stack>();

    public bool IsFull()
    {
        int total = 0; foreach (var s in slots) total += s.count;
        return total >= capacity;
    }

    public bool CanAccept(string type, int amount)
    {
        int total = 0; foreach (var s in slots) total += s.count;
        return total + amount <= capacity;
    }

    public void Add(string type, int amount)
    {
        var stack = slots.Find(s => s.type == type);
        if (stack == null) { stack = new Stack { type = type, count = 0 }; slots.Add(stack); }
        stack.count += amount;
    }

    public int RemoveAllOfType(string type)
    {
        var stack = slots.Find(s => s.type == type);
        if (stack == null) return 0;
        int n = stack.count; stack.count = 0; return n;
    }

    public Dictionary<string, int> DumpAll()
    {
        var dict = new Dictionary<string, int>();
        foreach (var s in slots)
        {
            if (s.count <= 0) continue;
            if (!dict.ContainsKey(s.type)) dict[s.type] = 0;
            dict[s.type] += s.count;
            s.count = 0;
        }
        return dict;
    }
}
