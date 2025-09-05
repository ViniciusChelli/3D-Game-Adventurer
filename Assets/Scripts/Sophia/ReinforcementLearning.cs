using UnityEngine;
using System.Collections.Generic;

public class ReinforcementLearning : MonoBehaviour
{
    private RelationshipSystem relationshipSystem;

    private Dictionary<string, float[]> hiddenState = new();
    private Dictionary<string, float[]> weights = new();

    private float learningRate = 0.05f;
    private readonly string[] actions = { "loving", "neutral", "cold" };

    void Start() { relationshipSystem = FindObjectOfType<RelationshipSystem>(); }

    public void ProcessResponse(string playerMessage)
    {
        string state = GetState();
        float[] input = MessageToVector(playerMessage);
        float[] output = RunRNN(state, input);
        int actionIndex = ArgMax(output);
        string action = actions[actionIndex];

        int reward = GetReward(playerMessage);
        TrainRNN(state, input, actionIndex, reward);

        if (action == "loving")      relationshipSystem?.AdjustLovePoints(+5);
        else if (action == "neutral") relationshipSystem?.AdjustLovePoints(0);
        else if (action == "cold")    relationshipSystem?.AdjustLovePoints(-5);
    }

    string GetState() => relationshipSystem ? relationshipSystem.GetRelationshipState() : "Neutro";

    float[] RunRNN(string state, float[] input)
    {
        if (!weights.ContainsKey(state))      weights[state] = new float[actions.Length * input.Length];
        if (!hiddenState.ContainsKey(state))  hiddenState[state] = new float[actions.Length];

        float[] output = new float[actions.Length];
        float[] w = weights[state];

        for (int i = 0; i < actions.Length; i++)
        {
            float sum = hiddenState[state][i];
            for (int j = 0; j < input.Length; j++)
                sum += w[i * input.Length + j] * input[j];
            output[i] = Mathf.Tan(Mathf.Clamp(sum, -3f, 3f)); // aproximação do tanh
        }

        hiddenState[state] = output;
        return output;
    }

    void TrainRNN(string state, float[] input, int actionIndex, int reward)
    {
        if (!weights.ContainsKey(state)) return;

        float[] output = hiddenState[state];
        float error = reward - output[actionIndex];
        float[] w = weights[state];

        for (int j = 0; j < input.Length; j++)
        {
            int index = actionIndex * input.Length + j;
            w[index] += learningRate * error * input[j];
        }
    }

    int ArgMax(float[] a)
    {
        int idx = 0; float m = a[0];
        for (int i = 1; i < a.Length; i++) if (a[i] > m) { m = a[i]; idx = i; }
        return idx;
    }

    float[] MessageToVector(string message)
    {
        message = (message ?? "").ToLower();
        return new float[] {
            (message.Contains("te amo") || message.Contains("linda") || message.Contains("gosto de você")) ? 1f : 0f,
            (message.Contains("oi") || message.Contains("como você está") || message.Contains("bom dia")) ? 1f : 0f,
            (message.Contains("não ligo") || message.Contains("me deixa") || message.Contains("irritante")) ? 1f : 0f
        };
    }

    int GetReward(string message)
    {
        message = (message ?? "").ToLower();
        if (message.Contains("te amo") || message.Contains("linda") || message.Contains("gosto de você")) return +5;
        if (message.Contains("oi") || message.Contains("como você está") || message.Contains("bom dia")) return 0;
        if (message.Contains("não ligo") || message.Contains("me deixa") || message.Contains("irritante")) return -5;
        return -1;
    }
}
