using System.Collections.Generic;
using UnityEngine;

public class NPC_Dialogue3D : MonoBehaviour
{
    public float dialogueRange = 2f;
    public LayerMask playerMask;
    public DialogueSettings dialogue;

    bool playerNear;
    readonly List<string> sentences = new();
    readonly List<string> actorName = new();
    readonly List<Sprite> actorSprite = new();

    void Start() => GetNPCInfo();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerNear)
            DialogueControl.instance.Speech(sentences.ToArray(), actorName.ToArray(), actorSprite.ToArray());
    }

    void FixedUpdate()
    {
        playerNear = Physics.CheckSphere(transform.position, dialogueRange, playerMask);
    }

    void GetNPCInfo()
    {
        for (int i = 0; i < dialogue.dialogues.Count; i++)
        {
            switch (DialogueControl.instance.language)
            {
                case DialogueControl.idiom.pt:  sentences.Add(dialogue.dialogues[i].sentence.portuguese); break;
                case DialogueControl.idiom.eng: sentences.Add(dialogue.dialogues[i].sentence.english); break;
                case DialogueControl.idiom.spa: sentences.Add(dialogue.dialogues[i].sentence.spanish); break;
            }
            actorName.Add(dialogue.dialogues[i].actorName);
            actorSprite.Add(dialogue.dialogues[i].profile);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dialogueRange);
    }
}
