using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera playerCamera;

    GameState state;

    private void Awake()
    {
        ConditionDB.Init();
    }

    private void Start()
    {
        playerController.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        DialogManager.Instance.OnShowDialog += () => {state = GameState.Dialog;};
        DialogManager.Instance.OnCloseDialog += () =>
        {
            if (state == GameState.Dialog)
                state = GameState.FreeRoam;
        };
    }

    void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        playerCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<AnigmaParty>();
        var wildAnigma = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildAnigma();

        battleSystem.StartBattle(playerParty, wildAnigma);
    }

    void EndBattle(bool won)
    {
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if(state == GameState.Dialog) 
        { 
            DialogManager.Instance.HandleUpdate();
        }
    }
}
