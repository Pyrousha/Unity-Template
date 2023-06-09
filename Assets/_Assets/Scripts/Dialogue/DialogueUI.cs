﻿using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class DialogueUI : Singleton<DialogueUI>
{
    [SerializeField] private DialogueObject currDialogueObject;
    public DialogueEvent[] dialogueEvents;


    private GameObject interactableObj;
    [Space(10)]
    [SerializeField] private TMP_Text textLabel;
    [SerializeField] private TMP_Text nameLabel;
    [SerializeField] private TMP_Text nameSpacing;
    [SerializeField] private Image speakerImage;

    [SerializeField] private GameObject dialogueParent;
    [SerializeField] private Animator anim;

    public bool isOpen { get; private set; }

    private TypewriterEffect typewriterEffect;
    private ResponseHandler responseHandler;

    private void Awake()
    {
        typewriterEffect = GetComponent<TypewriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();
    }

    public void ShowDialogue(DialogueObject dialogueObject, GameObject newInteractableObj)
    {
        currDialogueObject = dialogueObject;

        if (newInteractableObj != null)
            interactableObj = newInteractableObj;

        foreach (DialogueResponseEvents responseEvents in interactableObj.GetComponents<DialogueResponseEvents>())
        {
            if (responseEvents.DialogueObject == currDialogueObject)
            {
                AddResponseEvents(responseEvents.Events);
                break;
            }
        }

        dialogueEvents = null;

        foreach (DialogueEvents dialogueEvents in interactableObj.GetComponents<DialogueEvents>())
        {
            if (dialogueEvents.DialogueObject == currDialogueObject)
            {
                AddDialogueEvents(dialogueEvents.Events);
                break;
            }
        }

        isOpen = true;

        dialogueParent.SetActive(true);

        if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "FadeIn")
            anim.SetTrigger("FadeIn");

        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    public void SetCurrDialogueObject(DialogueObject newDO)
    {
        currDialogueObject = newDO;
    }
    public void AddResponseEvents(ResponseEvent[] responseEvents)
    {
        responseHandler.AddResponseEvents(responseEvents);
    }

    public void AddDialogueEvents(DialogueEvent[] events)
    {
        dialogueEvents = events;
    }

    public void ClearEvents()
    {
        responseHandler.AddResponseEvents(null);
        dialogueEvents = null;
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        for (int i = 0; i < dialogueObject.GetDialogue().Length; i++)
        {
            //Set speaker labels + icon
            AudioClip voiceClip = null;
            CharacterObject newSpeaker = dialogueObject.Characters[i];
            if (newSpeaker != null)
            {
                nameLabel.text = newSpeaker.CharacterName;
                nameSpacing.text = newSpeaker.CharacterName;
                speakerImage.sprite = newSpeaker.PortraitSprite;
                voiceClip = newSpeaker.Voice;
            }
            else
            {
                Debug.LogError("No speaker set for DialogueObject \"" + dialogueObject.name + "\", on dialogue line " + i);
            }

            //show text
            string dialogue = dialogueObject.GetDialogue()[i];
            yield return RunTypingEffect(dialogue, voiceClip);
            textLabel.color = typewriterEffect.TextColor;

            //if responses exist, don't let player close text box
            if ((i == dialogueObject.GetDialogue().Length - 1) && (dialogueObject.HasResponses))
                break;

            //Wait for input to show next slide
            yield return null;
            yield return new WaitUntil(() => InputHandler.Instance.Interact.Down);

            //Handle Dialogue Events if they exist
            if ((dialogueEvents != null) && (dialogueEvents.Length > 0) && (i < dialogueEvents.Length))
            {
                dialogueEvents[i].AfterTextSpoken?.Invoke();
            }
        }

        if (dialogueObject.HasResponses)
        {
            responseHandler.ShowResponses(dialogueObject.Responses);
        }
        else
        {
            if (dialogueObject.nextDialogueObject != null)
            {
                ShowDialogue(dialogueObject.nextDialogueObject, null);
            }
            else
            {
                CloseDialogueBox();
            }
        }
    }

    private IEnumerator RunTypingEffect(string dialogue, AudioClip voiceClip)
    {
        typewriterEffect.Run(dialogue, textLabel, voiceClip);

        while (typewriterEffect.isRunning)
        {
            yield return null;

            if (InputHandler.Instance.Interact.Down)
            {
                typewriterEffect.Stop();
            }
        }
    }

    public void CloseDialogueBox()
    {
        currDialogueObject = null;

        anim.SetTrigger("FadeOut");

        textLabel.text = string.Empty;

        isOpen = false;
    }

    public void DisableDialogueBox()
    {
        dialogueParent.SetActive(false);

        //This shouldn't be needed, but Unity be Unity sometimes...
        currDialogueObject = null;
        textLabel.text = string.Empty;
        isOpen = false;
    }
}
