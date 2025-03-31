using UnityEngine;

public class TalkToBlackSmithStep : QuestStep
{
    [SerializeField] private string blackSmithName;

    private void OnEnable()
    {
        GameController.Instance.OnTalkStart += TalkToBlackSmith;
    }

    private void OnDisable()
    {
        GameController.Instance.OnTalkStart -= TalkToBlackSmith;
    }

    void TalkToBlackSmith(NPC_Controller npc)
    {
        if(npc.npcName == blackSmithName) FinishQuestStep();
    }
}
