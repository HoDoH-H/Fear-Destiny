using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GameState { FreeRoam, Battle, Dialog, Menu, PartyScreen, Bag, Cutscene, Paused}

public class GameController : MonoBehaviour
{
    [SerializeField] bool debug = false;

    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera playerCamera;
    [SerializeField] Material battleTransition;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryUI inventoryUI;

    GameState state;
    GameState stateBeforePause;

    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PrevScene { get; private set; }

    MenuController menuController;

    public static GameController Instance;

    string saveFileName = "1b3nzj7TLYXwj1Ml5Jw56jWxXV3UIwC81D8qhVNA40unU6NlBi";

    public string SaveFileName => saveFileName;

    private void Awake()
    {
        Instance = this;

        menuController = GetComponent<MenuController>();

        ConditionDB.Init();
        AnigmaDB.Init();
        MoveDB.Init();
    }

    private void Start()
    {
        battleTransition.SetFloat("_Cutoff", 0f);
        battleTransition.SetFloat("_Fade", 1);

        battleSystem.OnBattleOver += EndBattle;

        partyScreen.Init();

        DialogManager.Instance.OnShowDialog += () => {state = GameState.Dialog;};
        DialogManager.Instance.OnCloseDialog += () =>
        {
            if (state == GameState.Dialog)
            {
                state = GameState.FreeRoam;
            }
        };

        menuController.OnBack += () =>
        {
            state = GameState.FreeRoam;
        };
        menuController.OnMenuSelected += OnMenuSelected;

        if (!debug)
            SavingSystem.i.Load(saveFileName);
    }

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            stateBeforePause = state;
            state = GameState.Paused;
        }
        else
        {
            state = stateBeforePause;
        }
    }

    void EnablePlayerMovements(bool enable)
    {
        playerController.CanMove = enable;
    }

    public void StartBattle()
    {
        EnablePlayerMovements(false);
        if (battleTransition == null)
        {
            state = GameState.Battle;
            battleSystem.gameObject.SetActive(true);
            playerCamera.gameObject.SetActive(false);

            var playerParty = playerController.GetComponent<AnigmaParty>();
            var wildAnigma = CurrentScene.GetComponent<MapArea>().GetRandomWildAnigma();

            var wildAnigmaCopy = new Anigma(wildAnigma.Base, wildAnigma.Level);

            battleSystem.StartBattle(playerParty, wildAnigmaCopy);
        }
        else
            StartCoroutine(StartBattleWithTransition());
    }

    IEnumerator StartBattleWithTransition()
    {

        battleTransition.SetFloat("_Fade", 0);
        battleTransition.SetFloat("_Cutoff", 1);
        battleTransition.SetColor("_Color", Color.white);
        var fade = 0f;
        for (int i = 0; i < 2; i++)
        {
            while (fade < 0.5f)
            {
                fade += Time.deltaTime;
                battleTransition.SetFloat("_Fade", fade);
                yield return new WaitForSeconds(0.0000000001f);
            }
            while (fade > 0f)
            {
                fade -= Time.deltaTime;
                battleTransition.SetFloat("_Fade", fade);
                yield return new WaitForSeconds(0.0000000001f);
            }
            yield return new WaitForSeconds(0.05f);
        }
        battleTransition.SetFloat("_Cutoff", 0);
        battleTransition.SetFloat("_Fade", 1);
        battleTransition.SetColor("_Color", new Color(0.08447679f, 0.08447679f, 0.08447679f, 1));

        yield return new WaitForSeconds(0.2f);
        var cutOff = 0f;
        while (true) 
        {
            cutOff += Time.deltaTime;
            if (cutOff > 1f)
            {
                battleTransition.SetFloat("_Cutoff", 1);
                break;
            }
            battleTransition.SetFloat("_Cutoff", cutOff);
            yield return new WaitForSeconds(0.00001f);
        }
        yield return new WaitForSeconds(0.5f);

        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        playerCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<AnigmaParty>();
        var wildAnigma = CurrentScene.GetComponent<MapArea>().GetRandomWildAnigma();

        var wildAnigmaCopy = new Anigma(wildAnigma.Base, wildAnigma.Level);

        battleSystem.StartBattle(playerParty, wildAnigmaCopy);
    }

    TrainerController trainer;

    public void StartTrainerBattle(TrainerController trainer)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        playerCamera.gameObject.SetActive(false);

        this.trainer = trainer;
        var playerParty = playerController.GetComponent<AnigmaParty>();
        var trainerParty = trainer.GetComponent<AnigmaParty>();

        battleSystem.StartTrainerBattle(playerParty, trainerParty);
    }

    public void OnEnterTrainersView(TrainerController trainer)
    {
        state = GameState.Cutscene;
        StartCoroutine(trainer.TriggerTrainerBattle(playerController));
    }

    void EndBattle(bool won)
    {
        if (trainer != null && won)
        {
            trainer.BattleLost();
            trainer = null;
        }

        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);
        StartCoroutine(EndBattleTransition(true));
    }

    IEnumerator EndBattleTransition(bool isMainToBattle)
    {
        if (isMainToBattle)
        {
            var fade = 1f;
            while (true)
            {
                fade -= Time.deltaTime;
                if (fade < 0f)
                {
                    battleTransition.SetFloat("_Fade", 0);
                    break;
                }
                battleTransition.SetFloat("_Fade", fade);
                yield return new WaitForSeconds(0.00005f);
            }
        }

        EnablePlayerMovements(true);

        battleTransition.SetFloat("_Cutoff", 0);
        battleTransition.SetFloat("_Fade", 100);
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                state = GameState.Menu;
                menuController.OpenMenu();
            }
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if(state == GameState.Dialog) 
        { 
            DialogManager.Instance.HandleUpdate();
        }
        else if (state == GameState.Menu)
        {
            menuController.HandleUpdate();
        }
        else if (state == GameState.PartyScreen)
        {
            Action onSelected = () =>
            {
                // TODO - Summary Screen
            };

            Action onBack = () =>
            {
                partyScreen.gameObject.SetActive(false);
                menuController.OpenMenu();
                state = GameState.Menu;
            };

            partyScreen.HandleUpdate(onSelected, onBack);
        }
        else if (state == GameState.Bag)
        {
            Action onSelected = () =>
            {
                // TODO - Use Item
            };

            Action onBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                menuController.OpenMenu();
                state = GameState.Menu;
            };

            inventoryUI.HandleUpdate(onBack);
        }
    }

    public void SetCurrentScene(SceneDetails currScene)
    {
        PrevScene = CurrentScene;
        CurrentScene = currScene;
    }

    private void OnApplicationQuit()
    {
        battleTransition.SetColor("_Color", new Color(0.08447679f, 0.08447679f, 0.08447679f, 1));
        battleTransition.SetFloat("_Cutoff", 0);
        battleTransition.SetFloat("_Fade", 100);
    }

    void OnMenuSelected(int selectedItem)
    {
        if (selectedItem == 0)
        {
            // Anigmas
            partyScreen.gameObject.SetActive(true);
            partyScreen.SetPartyData(playerController.GetComponent<AnigmaParty>().Anigmas);
            state = GameState.PartyScreen;
        }
        else if (selectedItem == 1)
        {
            // Bag
            inventoryUI.gameObject.SetActive(true);
            inventoryUI.UpdateItemList();
            state = GameState.Bag;
        }
        else if (selectedItem == 2)
        {
            // Save
            SavingSystem.i.Save(saveFileName);
            state = GameState.FreeRoam;
        }
        else if (selectedItem == 3)
        {
            // Load
            SavingSystem.i.Load(saveFileName);
            state = GameState.FreeRoam;
        }
    }
}
