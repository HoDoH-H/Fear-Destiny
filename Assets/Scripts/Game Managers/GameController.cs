using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GameState { FreeRoam, Battle, Dialog, Menu, PartyScreen, Bag, Cutscene, Paused, Morlenis, Shop}

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
    GameState prevState;
    GameState stateBeforeMorlenis;

    public PlayerController Player => playerController;

    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PrevScene { get; private set; }

    MenuController menuController;

    public static GameController Instance;

    string saveFileName = "1b3nzj7TLYXwj1Ml5Jw56gWxXV3UIiC81D8qhVNA40unU6NlBi";

    public string SaveFileName => saveFileName;

    private void Awake()
    {
        Instance = this;

        menuController = GetComponent<MenuController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ConditionDB.Init();
        BattlerDB.Init();
        MoveDB.Init();
        ItemDB.Init();
        QuestDB.Init();
    }

    private void Start()
    {
        battleTransition.SetFloat("_Cutoff", 0f);
        battleTransition.SetFloat("_Fade", 1);

        battleSystem.OnBattleOver += EndBattle;

        partyScreen.Init();

        DialogManager.Instance.OnShowDialog += () => 
        {
            prevState = state;
            state = GameState.Dialog;
        };

        DialogManager.Instance.OnDialogFinished += () =>
        {
            if (state == GameState.Dialog)
            {
                state = prevState;
            }
        };

        menuController.OnBack += () =>
        {
            state = GameState.FreeRoam;
        };
        menuController.OnMenuSelected += OnMenuSelected;

        MorlenisManager.Instance.OnMorlenisStart += () => 
        {
            stateBeforeMorlenis = state;
            state = GameState.Morlenis;
        };
        MorlenisManager.Instance.OnMorlenisCompleted += () =>  
        {
            partyScreen.SetPartyData();
            state = stateBeforeMorlenis;

            AudioManager.Instance.PlayMusic(CurrentScene.SceneMusic, fade: true);
        };

        ShopController.Instance.OnStart += () => state = GameState.Shop;
        ShopController.Instance.OnFinish += () => state = GameState.FreeRoam;

        if (!debug)
            SavingSystem.i.Load(saveFileName);
    }

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            prevState = state;
            state = GameState.Paused;
        }
        else
        {
            state = prevState;
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

            var playerParty = playerController.GetComponent<BattlerParty>();
            var wildAnigma = CurrentScene.GetComponent<MapArea>().GetRandomWildAnigma();

            var wildAnigmaCopy = new Battler(wildAnigma.Base, wildAnigma.Level);

            battleSystem.StartBattle(playerParty, wildAnigmaCopy);
        }
        else
            StartCoroutine(StartBattleWithTransition());
    }

    IEnumerator StartBattleWithTransition()
    {
        AudioManager.Instance.PlayMusic(battleSystem.DefaultWildMusic);

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

        var playerParty = playerController.GetComponent<BattlerParty>();
        var wildAnigma = CurrentScene.GetComponent<MapArea>().GetRandomWildAnigma();

        var wildAnigmaCopy = new Battler(wildAnigma.Base, wildAnigma.Level);

        battleSystem.StartBattle(playerParty, wildAnigmaCopy);
    }

    TrainerController trainer;

    public void StartTrainerBattle(TrainerController trainer)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        playerCamera.gameObject.SetActive(false);

        this.trainer = trainer;
        var playerParty = playerController.GetComponent<BattlerParty>();
        var trainerParty = trainer.GetComponent<BattlerParty>();

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

        partyScreen.SetPartyData();

        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);
        StartCoroutine(EndBattleTransition(true));

        var playerParty = playerController.GetComponent<BattlerParty>();
        bool hasMorlenis = playerParty.CheckForMorlen();

        if (hasMorlenis)
            StartCoroutine(playerParty.RunMorlenis());
        else
            AudioManager.Instance.PlayMusic(CurrentScene.SceneMusic, fade: true);
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
                StartCoroutine(menuController.OpenMenu(true));
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
                AudioManager.Instance.PlaySFX(AudioId.UIDenied);
            };

            Action onBack = () =>
            {
                AudioManager.Instance.PlaySFX(AudioId.UIBack);
                partyScreen.gameObject.SetActive(false);
                StartCoroutine(menuController.OpenMenu());
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
                StartCoroutine(menuController.OpenMenu());
                state = GameState.Menu;
            };

            inventoryUI.HandleUpdate(onBack);
        }
        else if (state == GameState.Shop)
        {
            ShopController.Instance.HandleUpdate();
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
            StartCoroutine(menuController.CloseMenu());

            // Anigmas
            partyScreen.gameObject.SetActive(true);
            state = GameState.PartyScreen;
        }
        else if (selectedItem == 1)
        {
            StartCoroutine(menuController.CloseMenu());

            // Bag
            inventoryUI.gameObject.SetActive(true);
            inventoryUI.UpdateItemList();
            state = GameState.Bag;
        }
        else if (selectedItem == 2)
        {
            StartCoroutine(menuController.CloseMenu(true));

            // Save
            SavingSystem.i.Save(saveFileName);
            state = GameState.FreeRoam;
        }
        else if (selectedItem == 3)
        {
            StartCoroutine(menuController.CloseMenu(true));

            // Load
            SavingSystem.i.Load(saveFileName);
            state = GameState.FreeRoam;
        }
    }

    public int RotateSelection(int index, int max)
    {
        if (index < 0)
            return index + max + 1;
        else if (index > max)
            return index - max - 1;

        return index;
    }

    public IEnumerator MoveCamera(Vector2 moveOffset, bool needFade = false, bool waitForCompletion = false)
    {
        if (needFade)
        {
            yield return Fader.Instance.FadeIn(0.25f);

            playerCamera.transform.localPosition += new Vector3(moveOffset.x, moveOffset.y, -10);

            if (waitForCompletion)
                yield return Fader.Instance.FadeOut(0.25f);
            else
                StartCoroutine(Fader.Instance.FadeOut(0.25f));
        }
        else
        {
            Vector3 newPos = new Vector3(playerCamera.transform.localPosition.x + moveOffset.x, playerCamera.transform.localPosition.y + moveOffset.y, -10);
            if (waitForCompletion)
                playerCamera.transform.DOLocalMove(newPos, 0.3f).WaitForCompletion();
            else
                playerCamera.transform.DOLocalMove(newPos, 0.3f);
        }
    }

    public GameState State => state;
}
