using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class SophiaAnimatorDriver : MonoBehaviour
{
    [Header("Refs")]
    public NavMeshAgent agent;

    [Header("Locomoção por parâmetro INT")]
    public string locomotionParam = "transition"; // 0 idle, 1 walk, 2 run
    public int locomotionIdle = 0;
    public int locomotionWalk = 1;
    public int locomotionRun  = 2;
    public float runSpeedThreshold = 2.7f;
    public float walkSpeedThreshold = 0.05f;

    [Header("Bools opcionais")]
    public string boolIsRoll   = "isRoll";
    public string boolCasting  = "isCasting";
    public string boolHammer   = "hammering";
    public string boolHit      = "hit";

    [Header("States (nomes no Animator)")]
    public string stIdle="idle", stWalk="walking", stRun="run", stDig="dig",
                  stCut="cutting", stWater="watering", stHammer="hammering",
                  stCasting="casting", stRoll="roll", stHit="hit";

    [Header("Duração default (s)")]
    public float durCut=1.2f, durWater=1.1f, durHarvestOrDig=1.0f, durCraft=1.4f, durCast=1.0f, durRoll=0.6f, durHit=0.5f;

    private Animator anim;
    private float busyUntil = 0f;

    void Awake()
    {
        anim = GetComponent<Animator>();
        if (!agent) agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (!agent) return;
        if (Time.time < busyUntil) return;

        float spd = agent.velocity.magnitude;
        if (HasParamInt(locomotionParam))
        {
            if (spd > runSpeedThreshold)       anim.SetInteger(locomotionParam, locomotionRun);
            else if (spd > walkSpeedThreshold) anim.SetInteger(locomotionParam, locomotionWalk);
            else                                anim.SetInteger(locomotionParam, locomotionIdle);
        }
        else
        {
            if (spd > runSpeedThreshold)       Cross(stRun);
            else if (spd > walkSpeedThreshold) Cross(stWalk);
            else                                Cross(stIdle);
        }
    }

    public void TriggerChop()       { ActionState(stCut, durCut);   SetBoolFor(boolHammer, true, durCut); }
    public void TriggerWater()      { ActionState(stWater, durWater); }
    public void TriggerHarvest()    { ActionState(stDig, durHarvestOrDig); }
    public void TriggerCraft()      { ActionState(stHammer, durCraft); SetBoolFor(boolHammer, true, durCraft); }
    public void TriggerEnterHouse() { ActionState(stCasting, durCast);   SetBoolFor(boolCasting, true, durCast); }
    public void TriggerRoll()       { ActionState(stRoll, durRoll);  SetBoolFor(boolIsRoll, true, durRoll); }
    public void TriggerHit()        { ActionState(stHit, durHit);    SetBoolFor(boolHit, true, durHit); }

    private void ActionState(string stateName, float dur)
    {
        if (!string.IsNullOrEmpty(stateName) && HasState(stateName)) Cross(stateName);
        busyUntil = Time.time + Mathf.Max(0.05f, dur);
    }
    private void Cross(string stateName, float fade=0.06f) { anim.CrossFadeInFixedTime(Animator.StringToHash(stateName), fade); }

    private bool HasParamInt(string name)
    {
        if (string.IsNullOrEmpty(name)) return false;
        foreach (var p in anim.parameters) if (p.name == name && p.type == AnimatorControllerParameterType.Int) return true;
        return false;
    }
    private bool HasState(string state) { return anim.HasState(0, Animator.StringToHash(state)); }

    private void SetBoolFor(string param, bool value, float seconds)
    {
        if (string.IsNullOrEmpty(param)) return;
        foreach (var p in anim.parameters) if (p.name == param && p.type == AnimatorControllerParameterType.Bool)
        {
            StopCoroutine("CoResetBool");
            anim.SetBool(param, value);
            StartCoroutine(CoResetBool(param, seconds));
            return;
        }
    }
    private IEnumerator CoResetBool(string param, float seconds)
    { yield return new WaitForSeconds(seconds); anim.SetBool(param, false); }
}
