using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SophiaPerception : MonoBehaviour
{
    [Header("Percepção 3D (XZ)")]
    public float detectionRadius = 6f;
    public LayerMask detectionLayer;

    [Header("Palavras-chave")]
    [SerializeField] string[] houseKeys = { "house","casa","home","hut" };
    [SerializeField] string[] lakeKeys  = { "lake","lago","water","pond","rio" };
    [SerializeField] string[] treeKeys  = { "tree","árvore","arvore","pine","oak" };

    private readonly Dictionary<string, Vector3> nearestByCat = new();

    void Update() { Scan(); }

    void Scan()
    {
        nearestByCat.Clear();
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);
        Vector3 origin = transform.position;

        foreach (var h in hits)
        {
            if (!h) continue;
            var go = h.attachedRigidbody ? h.attachedRigidbody.gameObject : h.gameObject;
            if (go == gameObject) continue;

            string cat = Classify(go.name, go.tag);
            if (cat == null) continue;

            Vector3 p = go.transform.position;
            if (!nearestByCat.ContainsKey(cat) ||
                (p - origin).sqrMagnitude < (nearestByCat[cat] - origin).sqrMagnitude)
                nearestByCat[cat] = p;
        }
    }

    string Classify(string name, string tag)
    {
        string s = (name + " " + tag).ToLower();
        bool Has(string x, string[] keys) { foreach (var k in keys) if (s.Contains(k.ToLower())) return true; return false; }
        if (Has(s, houseKeys)) return "casa";
        if (Has(s, lakeKeys))  return "lago";
        if (Has(s, treeKeys))  return "árvore";
        return null;
    }

    public bool TryGetNearest(string categoria, out Vector2 pos)
    {
        pos = Vector2.zero;
        string key = NormalizeCat(categoria);
        if (nearestByCat.TryGetValue(key, out Vector3 p))
        {
            pos = new Vector2(p.x, p.z);
            return true;
        }
        return false;
    }

    public string GetDetectedObjects()
    {
        if (nearestByCat.Count == 0) return "Nada interessante ao redor.";
        Vector3 origin = transform.position;
        var frases = nearestByCat.Select(kv => $"{kv.Key} {Cardinal(kv.Value - origin)}");
        return "Vejo " + string.Join(", ", frases) + ".";
    }

    string NormalizeCat(string c)
    {
        c = (c ?? "").ToLower();
        if (c.StartsWith("arv")) return "árvore";
        if (c.StartsWith("lag")) return "lago";
        if (c.StartsWith("cas")) return "casa";
        return c;
    }

    string Cardinal(Vector3 v)
    {
        float ang = Mathf.Atan2(v.z, v.x) * Mathf.Rad2Deg; if (ang < 0) ang += 360f;
        if (ang < 22.5f || ang >= 337.5f) return "à direita";
        if (ang < 67.5f)  return "a sudeste";
        if (ang < 112.5f) return "abaixo";
        if (ang < 157.5f) return "a sudoeste";
        if (ang < 202.5f) return "à esquerda";
        if (ang < 247.5f) return "a noroeste";
        if (ang < 292.5f) return "acima";
        return "a nordeste";
    }
}
