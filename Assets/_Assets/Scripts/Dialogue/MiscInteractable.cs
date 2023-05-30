using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Similar to DialogueActivator, but for arbitrary events instead of dialogue
/// </summary>
public class MiscInteractable : MonoBehaviour, IInteractable
{
    public int Priority { get; set; }

    private bool played = false;
    private bool playOnce = false;

    [SerializeField] private bool playWithoutInput;
    [SerializeField] private DialogueActivator.PlayOptions playOption;

    [SerializeField] private UnityEvent interactEvent;

    [Header("Optional References For Events")]
    [SerializeField] private List<GameObject> objs;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out HeroDialogueInteract player))
        {
            player.AddInteractable(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out HeroDialogueInteract player))
        {
            player.RemoveInteractable(this);
        }
    }

    public void TryInteract(HeroDialogueInteract player)
    {
        if (played && playOnce)
            return;

        if (playWithoutInput || InputHandler.Instance.Interact.Down)
        {
            interactEvent.Invoke();

            switch (playOption)
            {
                case DialogueActivator.PlayOptions.playOnce:
                    {
                        played = true;

                        playOnce = true;
                        break;
                    }

                case DialogueActivator.PlayOptions.playOnceIfSucceeds:
                    {
                        break;
                    }

                case DialogueActivator.PlayOptions.playAgainWithInput:
                    {
                        played = true;

                        playWithoutInput = false;
                        break;
                    }
            }
        }
    }
}

