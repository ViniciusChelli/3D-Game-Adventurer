using UnityEngine;

public class RelationshipSystem : MonoBehaviour
{
    [Header("Parâmetros do Relacionamento")]
    public int lovePoints = 50;
    public int maxLovePoints = 100;
    public int minLovePoints = 0;

    [Header("Estados")]
    public string currentState;

    [Header("Configurações")]
    public bool debugLogs = true;

    void Start() { UpdateState(); }

    public void AdjustLovePoints(int value)
    {
        lovePoints = Mathf.Clamp(lovePoints + value, minLovePoints, maxLovePoints);
        UpdateState();
        if (debugLogs) Debug.Log($"❤️ Amor Atual: {lovePoints} | Estado: {currentState}");
    }

    void UpdateState()
    {
        if (lovePoints >= 70) currentState = "Amoroso";
        else if (lovePoints >= 40) currentState = "Neutro";
        else currentState = "Frio";
    }

    public string GetRelationshipState() => currentState;

    public void SetRelationshipState(string newState)
    {
        currentState = newState;
        switch (newState)
        {
            case "Amoroso": lovePoints = Mathf.Max(lovePoints, 70); break;
            case "Neutro":  lovePoints = Mathf.Clamp(lovePoints, 40, 69); break;
            case "Frio":    lovePoints = Mathf.Min(lovePoints, 39); break;
        }
        if (debugLogs) Debug.Log($"🔧 Estado forçado para: {newState} (Pontos: {lovePoints})");
    }
}
