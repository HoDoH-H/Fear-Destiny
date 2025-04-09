using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Controller : MonoBehaviour, Interactable, ISavable
{
    [SerializeField] Dialog dialog;

    [Header("Quests")]
    [SerializeField] QuestBase questToStart;
    [SerializeField] QuestBase questToComplete;
    Quest activeQuest;

    [Header("Movements")]
    [SerializeField] List<MovementPattern> movementPattern;
    NPCState state;
    float idleTimer = 0f;
    int currentPattern = 0;
    int currentPatternList = 0;

    Character character;
    ItemGiver itemGiver;
    BattlerGiver battlerGiver;
    Healer healer;
    Merchant merchant;

    private void Awake()
    {
        character = GetComponent<Character>();
        itemGiver = GetComponent<ItemGiver>();
        battlerGiver = GetComponent<BattlerGiver>();
        healer = GetComponent<Healer>();
        merchant = GetComponent<Merchant>();
    }

    public IEnumerator Interact(Transform initiator)
    {
        if (state == NPCState.Idle)
        {
            state = NPCState.Dialog;
            character.LookTowards(initiator.position);

            if (questToComplete != null)
            {
                var quest = new Quest(questToComplete);
                yield return quest.CompleteQuest();

                questToComplete = null;
            }

            if (itemGiver != null && itemGiver.CanBeGiven())
            {
                yield return itemGiver.GiveItem(initiator.GetComponent<PlayerController>());
            }
            else if (itemGiver != null && itemGiver.CanBeGiven())
            {
                yield return battlerGiver.GiveBattler(initiator.GetComponent<PlayerController>());
            }
            else if (questToStart != null)
            {
                activeQuest = new Quest(questToStart);
                yield return activeQuest.StartQuest();
                questToStart = null;

                if (activeQuest.CanBeCompleted())
                {
                    yield return activeQuest.CompleteQuest();
                    activeQuest = null;
                }
                else
                {
                    yield return DialogManager.Instance.ShowDialog(activeQuest.Base.InProgressDialog);
                }
            }
            else if (activeQuest != null)
            {
                if (activeQuest.CanBeCompleted())
                {
                    yield return activeQuest.CompleteQuest();
                    activeQuest = null;
                }
                else
                {
                    yield return DialogManager.Instance.ShowDialog(activeQuest.Base.InProgressDialog);
                }
            }
            else if (healer != null)
            {
                yield return healer.Heal(initiator, dialog);
            }
            else if(merchant != null)
            {
                yield return merchant.Trade();
            }
            else
            {
                yield return DialogManager.Instance.ShowDialog(dialog);
            }

            idleTimer = 0f;
            state = NPCState.Idle;
        }
    }

    private void Update()
    {
        if (state == NPCState.Idle)
        {
            if (movementPattern.Count > 0)
            {
                idleTimer += Time.deltaTime;
                if (idleTimer > movementPattern[currentPatternList].patterns[currentPattern].timeBeforePattern)
                {
                    idleTimer = 0f;
                    if (movementPattern[currentPatternList].patterns.Count > 0)
                        StartCoroutine(Walk());
                }
            }
        }

        character.HandleUpdate();
    }

    IEnumerator Walk()
    {
        state = NPCState.Walking;

        var oldPos = transform.position;
        var oldDir = new Vector2(character.Animator.MoveX, character.Animator.MoveY);

        bool hasWalked = false;

        if (movementPattern[currentPatternList].patterns[currentPattern].directionOnly)
            character.LookTowards((Vector2)transform.position + movementPattern[currentPatternList].patterns[currentPattern].movement);
        else
        {
            yield return character.Move(movementPattern[currentPatternList].patterns[currentPattern].movement);
            hasWalked = true;
        }
            

        if (transform.position != oldPos && hasWalked || oldDir != new Vector2(character.Animator.MoveX, character.Animator.MoveY) && !hasWalked)
        {
            currentPattern = (currentPattern + 1) % movementPattern[currentPatternList].patterns.Count;
            if (currentPattern == 0)
            {
                currentPatternList = Random.Range(0, movementPattern.Count);
            }
        }
            

        state = NPCState.Idle;
    }

    public object CaptureState()
    {
        var saveData = new NPCQuestSaveData();
        saveData.activeQuest = activeQuest?.GetSaveData();

        if (questToStart != null)
            saveData.questToStart = (new Quest(questToStart)).GetSaveData();

        if (questToComplete != null)
            saveData.questToComplete = (new Quest(questToComplete)).GetSaveData();

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = state as NPCQuestSaveData;
        if (saveData != null)
        {
            activeQuest = (saveData.activeQuest != null)? new Quest(saveData.activeQuest) : null;

            questToStart = (saveData.questToStart != null) ? new Quest(saveData.questToStart).Base : null;

            questToComplete = (saveData.questToComplete != null) ? new Quest(saveData.questToComplete).Base : null;
        }
    }
}

[System.Serializable]
public class NPCQuestSaveData
{
    public QuestSaveData activeQuest;
    public QuestSaveData questToStart;
    public QuestSaveData questToComplete;
}

public enum NPCState { Idle, Walking, Dialog}
