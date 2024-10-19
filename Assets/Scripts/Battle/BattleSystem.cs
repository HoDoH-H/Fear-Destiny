using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { Start, ActionSelection, MoveSelection, RunningTurn, Busy, PartyScreen, Bag, AboutToUse, MoveToForget, BattleOver }
public enum BattleAction { Move, SwitchAnigma, UseItem, Run}

public class BattleSystem : MonoBehaviour
{
    #region Variables
    // All needed variables

    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit opponentUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;
    [SerializeField] GameObject ringSprite;
    [SerializeField] MoveSelectionUI moveSelectionUI;
    [SerializeField] InventoryUI inventoryUI;

    public event Action<bool> OnBattleOver;

    BattleState state;


    int currentAction;
    int currentMove;
    bool aboutToUseChoice = true;

    BattlerParty playerParty;
    BattlerParty trainerParty;
    Battler wildAnigma;

    public Field Field { get; private set; }

    bool isTrainerBattle = false;
    PlayerController player;
    TrainerController trainer;

    int escapeAttemps;
    MoveBase moveToLearn;

    BattleUnit firstToMove;

    List<Vector3> ringAnimVectors = new List<Vector3>() { new Vector3(0f, 0.10f), new Vector3(0.10f, 0.10f), new Vector3(0.10f, 0f), new Vector3(0.10f, -0.10f), new Vector3(0f, -0.10f), new Vector3(-0.10f, -0.10f), new Vector3(-0.10f, 0f), new Vector3(-0.10f, 0.10f) };

    DamageDetails lastDamageDetails = new DamageDetails();

    #endregion Variables

    #region Start Battle
    // Pack of functions used to start a battle (preparing anigmas, etc)

    public void StartBattle(BattlerParty playerParty, Battler wildAnigma)
    {
        isTrainerBattle=false;
        this.playerParty = playerParty;
        this.wildAnigma = wildAnigma;
        player = playerParty.GetComponent<PlayerController>();
        StartCoroutine(SetupBattle());
    }

    public void StartTrainerBattle(BattlerParty playerParty, BattlerParty trainerParty)
    {
        this.playerParty = playerParty;
        this.trainerParty = trainerParty;

        isTrainerBattle = true;
        player = playerParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();

        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Clear();
        opponentUnit.Clear();

        escapeAttemps = 0;

        if (!isTrainerBattle)
        {
            // Wild Anigma Battle
            playerUnit.Setup(playerParty.GetHealthyBattler());
            opponentUnit.Setup(wildAnigma);

            dialogBox.SetMoveNames(playerUnit.Anigma.Moves);

            yield return dialogBox.TypeDialog($"A wild {opponentUnit.Anigma.Base.Name} appeared.");
        }
        else
        {
            // Trainer Battle
            playerUnit.gameObject.SetActive(false);
            opponentUnit.gameObject.SetActive(false);

            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);
            playerImage.sprite = player.Unit.BackSprite;
            trainerImage.sprite = trainer.Sprite;

            yield return dialogBox.TypeDialog($"{trainer.Name} wants to battle!");

            // Send out first anigma of the trainer
            trainerImage.gameObject.SetActive(false);
            opponentUnit.gameObject.SetActive(true);
            var opponentAnigma = trainerParty.GetHealthyBattler();
            opponentUnit.Setup(opponentAnigma);
            yield return dialogBox.TypeDialog($"{trainer.Name} send out {opponentAnigma.Base.Name}!");

            // Send out first anigma of the player
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            var playerAnigma = playerParty.GetHealthyBattler();
            playerUnit.Setup(playerAnigma);
            yield return dialogBox.TypeDialog($"{playerAnigma.Base.Name} go!");
            dialogBox.SetMoveNames(playerUnit.Anigma.Moves);
        }

        Field = new Field();

        // Use to set a default weather.
        //Field.SetWeather(ConditionID.sandstorm);
        //yield return dialogBox.TypeDialog(Field.Weather.StartMessage);

        partyScreen.Init();
        ActionSelection();
    }

    #endregion Start Battle

    #region UI Managers
    // Pack of function used by UIs (Update menus, health bar, etc)

    void BattleOver(bool isWon)
    {
        state = BattleState.BattleOver;
        playerParty.Battlers.ForEach(p => p.OnBattleOver());
        playerUnit.Hud.ClearData();
        opponentUnit.Hud.ClearData();
        OnBattleOver(isWon);

        if (!isWon)
        {
            SavingSystem.i.Load(GameController.Instance.SaveFileName);
        }
    }

    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogBox.SetDialog("Choose an action");
        dialogBox.EnableActionSelector(true);
        dialogBox.EnableDialogText(true);
        dialogBox.EnableMoveSelector(false);
        partyScreen.gameObject.SetActive(false);
    }

    void OpenPartyScreen()
    {
        partyScreen.CalledFrom = state;
        state = BattleState.PartyScreen;
        partyScreen.gameObject.SetActive(true);
    }

    void OpenBag()
    {
        state = BattleState.Bag;
        inventoryUI.gameObject.SetActive(true);
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.SetDialog("");
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator ChooseMoveToForget(Battler anigma, MoveBase newMove)
    {
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog($"Do you want that {anigma.Base.Name} forget a move to learn this new one?");
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(anigma.Moves.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;

        state = BattleState.MoveToForget;
    }

    IEnumerator AboutToUse(Battler newAnigma)
    {
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog($"{trainer.Name} is about to send another anigma. Do you want to change yours?");

        state = BattleState.AboutToUse;
        dialogBox.EnableChoiceBox(true);
    }

    #endregion UI Managers

    # region Battle
    // Pack of function used to make the battle logics (Turns, Use moves, Take damage, etc)

    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;

        if (playerAction == BattleAction.Move)
        {
            playerUnit.Anigma.CurrentMove = playerUnit.Anigma.Moves[currentMove];
            opponentUnit.Anigma.CurrentMove = opponentUnit.Anigma.GetRandomMove();

            int playerMovePriority = playerUnit.Anigma.CurrentMove.Base.Priority;
            int opponentMovePriority = opponentUnit.Anigma.CurrentMove.Base.Priority;

            // Check who goes first
            bool playerGoesFirst = true;
            if (opponentMovePriority > playerMovePriority)
                playerGoesFirst=false;
            else if (opponentMovePriority == playerMovePriority)
                playerGoesFirst = playerUnit.Anigma.Speed >= opponentUnit.Anigma.Speed;

            var firstUnit = (playerGoesFirst ? playerUnit : opponentUnit);
            var secondUnit = (playerGoesFirst ? opponentUnit : playerUnit);

            var secondAnigma = secondUnit.Anigma;

            firstToMove = firstUnit;

            // First turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.Anigma.CurrentMove);
            yield return RunAfterTurn(firstUnit, lastDamageDetails);
            if (state == BattleState.BattleOver) yield break;

            if (secondAnigma.HP > 0)
            {
                // Second turn
                yield return RunMove(secondUnit, firstUnit, secondUnit.Anigma.CurrentMove);
                yield return RunAfterTurn(secondUnit, lastDamageDetails);
                if (state == BattleState.BattleOver) yield break;
            }
        }
        else
        {
            if (playerAction == BattleAction.SwitchAnigma)
            {
                var selectedAnigma = partyScreen.SelectedMember;
                state = BattleState.Busy;
                yield return SwitchAnigma(selectedAnigma);
            }
            else if (playerAction == BattleAction.UseItem)
            {
                // Inventory is handled from item screen, so do nothing and skip to opponent move.
                dialogBox.EnableActionSelector(false);
            }
            else if (playerAction == BattleAction.Run)
            {
                if (isTrainerBattle)
                {
                    yield return dialogBox.TypeDialog($"You can't run from trainer battles!");
                    ActionSelection();
                    yield break;
                }
                else
                    yield return TryToEscape();
            }

            // Opponent Turn
            var opponentMove = opponentUnit.Anigma.GetRandomMove();
            yield return RunMove( opponentUnit, playerUnit,opponentMove);
            yield return RunAfterTurn(opponentUnit, lastDamageDetails);
            if (state == BattleState.BattleOver) yield break;
        }

        // If there's a field weather
        if (Field.Weather != null)
        {
            yield return dialogBox.TypeDialog(Field.Weather.EffectMessage);

            // For player unit
            var beforeWeatherHp = playerUnit.Anigma.HP;
            Field.Weather.OnWeather?.Invoke(playerUnit.Anigma);
            if (beforeWeatherHp != playerUnit.Anigma.HP) playerUnit.PlayHitAnimation();

            yield return ShowStatusChanges(playerUnit.Anigma);
            if (playerUnit.Anigma.HP <= 0)
            {
                yield return dialogBox.TypeDialog($"{playerUnit.Anigma.Base.Name} fainted.");
                playerUnit.PlayFaintAnimation();

                yield return new WaitForSeconds(1.5f);

                CheckForBattleOver(playerUnit);
                yield break;
            }



            // For opponent unit
            beforeWeatherHp = opponentUnit.Anigma.HP;
            Field.Weather.OnWeather?.Invoke(opponentUnit.Anigma);
            if (beforeWeatherHp != opponentUnit.Anigma.HP) opponentUnit.PlayHitAnimation();

            yield return ShowStatusChanges(opponentUnit.Anigma);
            if (opponentUnit.Anigma.HP <= 0)
            {
                yield return dialogBox.TypeDialog($"{opponentUnit.Anigma.Base.Name} fainted.");
                opponentUnit.PlayFaintAnimation();

                yield return new WaitForSeconds(1.5f);

                CheckForBattleOver(opponentUnit);
                yield break;
            }

            if (Field.WeatherDuration != null)
            {
                Field.WeatherDuration--;
                if (Field.WeatherDuration <= 0)
                {
                    Field.Weather = null;
                    Field.WeatherDuration = null;
                    yield return dialogBox.TypeDialog("The weather has turned back to normal.");
                }
            }
        }

        if (state != BattleState.BattleOver)
        {
            ActionSelection();
        }
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canRunMove = sourceUnit.Anigma.OnBeforeMove();

        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Anigma);
            if (sourceUnit.Anigma.VolatileStatus?.Name == "Confusion")
            {
                sourceUnit.PlayAttackAnimation();
                yield return new WaitForSeconds(0.1f);
                sourceUnit.Anigma.DecreaseHP(sourceUnit.Anigma.MaxHp / 8);
            }
            yield return targetUnit.Hud.WaitForHPUpdate();

            if (sourceUnit.Anigma.HP <= 0)
            {
                yield return dialogBox.TypeDialog($"{sourceUnit.Anigma.Base.Name} fainted.");
                sourceUnit.PlayFaintAnimation();

                yield return new WaitForSeconds(1.5f);

                CheckForBattleOver(sourceUnit);
            }
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Anigma);

        move.UP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Anigma.Base.Name} used {move.Base.Name}.");


        if (CheckIfMoveHits(move, sourceUnit.Anigma, targetUnit.Anigma))
        {
            var hitTime = move.Base.GetHitTimes();

            float typeEffectiveness = 1f;
            int hit = 1;

            for (int i = 1; i <= hitTime; i++)
            {
                sourceUnit.PlayAttackAnimation();
                yield return new WaitForSeconds(0.5f);
                targetUnit.PlayHitAnimation();

                if (move.Base.Category == AttackCategory.Status)
                {
                    yield return RunMoveEffect(move.Base.Effects, sourceUnit.Anigma, targetUnit.Anigma, move.Base.Target);
                }
                else
                {
                    lastDamageDetails = targetUnit.Anigma.TakeDamage(move, sourceUnit.Anigma, Field.Weather);
                    yield return targetUnit.Hud.WaitForHPUpdate();
                    yield return ShowDamageDetail(lastDamageDetails);
                    typeEffectiveness = lastDamageDetails.TypeEffectiveness;
                }

                if (move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && targetUnit.Anigma.HP > 0)
                {
                    foreach (var secondary in move.Base.Secondaries)
                    {
                        var rnd = UnityEngine.Random.Range(1, 101);
                        if (rnd <= secondary.Chance)
                        {
                            yield return RunMoveEffect(secondary, sourceUnit.Anigma, targetUnit.Anigma, secondary.Target);
                        }
                    }
                }

                hit = i;
                if (targetUnit.Anigma.HP <= 0)
                {
                    break;
                }
            }

            yield return ShowTypeEffectiveness(typeEffectiveness, targetUnit.Anigma);

            if (hitTime > 1)
                yield return dialogBox.TypeDialog($"Hit {hit} times!");

            if (targetUnit.Anigma.HP <= 0)
            {
                yield return HandleAnigmaFainted(targetUnit);
            }
        }
        else
        {
            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(0.5f);
            yield return dialogBox.TypeDialog($"{sourceUnit.Anigma.Base.Name}'s attack missed...");
        }
    }

    IEnumerator RunAfterTurn(BattleUnit sourceUnit, DamageDetails damageDetails)
    {
        if (state == BattleState.BattleOver) yield break;
        yield return new WaitUntil(() => state == BattleState.RunningTurn);

        // Statuses like burn, poison or recoil will hurt the anigma after the turn
        sourceUnit.Anigma.OnAfterTurn(damageDetails.DamageDealt);
        yield return ShowStatusChanges(sourceUnit.Anigma);
        yield return sourceUnit.Hud.WaitForHPUpdate();

        if (sourceUnit.Anigma.HP <= 0)
        {
            yield return HandleAnigmaFainted(sourceUnit);
            yield return new WaitUntil(() => state == BattleState.RunningTurn);
        }
    }

    bool CheckIfMoveHits(Move move, Battler source, Battler target)
    {
        if (move.Base.AlwaysHits)
            return true;

        float moveAccuracy = move.Base.Accuracy;

        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasiveness = target.StatBoosts[Stat.Evasiveness];

        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        if (accuracy > 0)
        {
            moveAccuracy *= boostValues[accuracy];
        }
        else
        {
            moveAccuracy /= boostValues[-accuracy];
        }

        if (evasiveness > 0)
        {
            moveAccuracy /= boostValues[accuracy];
        }
        else
        {
            moveAccuracy *= boostValues[-accuracy];
        }

        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }

    IEnumerator ShowStatusChanges(Battler anigma)
    {
        while(anigma.StatusChanges.Count > 0)
        {
            var message = anigma.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    IEnumerator HandleAnigmaFainted(BattleUnit faintedUnit)
    {
        yield return dialogBox.TypeDialog($"{faintedUnit.Anigma.Base.Name} fainted.");
        faintedUnit.PlayFaintAnimation();

        yield return new WaitForSeconds(1.5f);

        if (!faintedUnit.IsPlayerUnit)
        {
            // Exp Gain
            int expYield = faintedUnit.Anigma.Base.ExpYield;
            int opponentLevel = faintedUnit.Anigma.Level;
            float trainerBonus = (isTrainerBattle) ? 1.5f : 1;

            int expGain = Mathf.FloorToInt((expYield * opponentLevel) / 7 * trainerBonus);
            playerUnit.Anigma.Exp += expGain;
            yield return dialogBox.TypeDialog($"{playerUnit.Anigma.Base.Name} gained {expGain} exp.");
            yield return playerUnit.Hud.SetExpSmooth();

            // Check Level Up
            while (playerUnit.Anigma.CheckForLevelUp())
            {
                playerUnit.Hud.SetLevel();
                yield return dialogBox.TypeDialog($"{playerUnit.Anigma.Base.Name} ascended to level {playerUnit.Anigma.Level}!");

                // Try to learn a new move
                var newMove = playerUnit.Anigma.GetLearnableMoveAtCurrLevel();
                if (newMove != null)
                {
                    if (playerUnit.Anigma.Moves.Count < BattlerBase.MaxNumOfMoves)
                    {
                        playerUnit.Anigma.LearnMove(newMove.Base);
                        yield return dialogBox.TypeDialog($"{playerUnit.Anigma.Base.Name} learned {newMove.Base.Name}!");
                        dialogBox.SetMoveNames(playerUnit.Anigma.Moves);
                    }
                    else
                    {
                        // Need to forget a move to learn a new one
                        yield return dialogBox.TypeDialog($"{playerUnit.Anigma.Base.Name} is attempting to acquire {newMove.Base.Name}...");
                        yield return dialogBox.TypeDialog($"But an Anigma cannot learn more than {BattlerBase.MaxNumOfMoves} moves!");
                        yield return ChooseMoveToForget(playerUnit.Anigma, newMove.Base);
                        yield return new WaitUntil(() => state != BattleState.MoveToForget);
                        yield return new WaitForSeconds(2f);
                    }
                }

                yield return playerUnit.Hud.SetExpSmooth(true);
            }
        }

        yield return new WaitForSeconds(0.75f);
        CheckForBattleOver(faintedUnit);
    }

    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextAnigma = playerParty.GetHealthyBattler();
            if (nextAnigma != null)
            {

                OpenPartyScreen();
            }
            else
            {
                BattleOver(false);
            }
        }
        else
        {
            if (!isTrainerBattle)
            {
                BattleOver(true);
            }
            else
            {
                var nextAnigma = trainerParty.GetHealthyBattler();
                if (nextAnigma != null)
                {
                    StartCoroutine(AboutToUse(nextAnigma));
                }
                else
                {
                    BattleOver(true);
                }
            }
        }
    }

    IEnumerator RunMoveEffect(MoveEffects effect, Battler source, Battler target, MoveTarget moveTarget)
    {
        // Stat Boosting
        if (effect.Boosts?.Count != 0)
        {
            if (moveTarget == MoveTarget.Self)
            {
                source.ApplyBoosts(effect.Boosts);
            }
            else
            {
                target.ApplyBoosts(effect.Boosts);
            }
        }
        else if (effect.DoRandomStatBoost)
        {
            var n = UnityEngine.Random.Range(0, 7);
            var boost = new List<StatBoost>() { new() { stat = (Stat)n, boost = effect.BoostValue } };
            if (moveTarget == MoveTarget.Self)
            {
                source.ApplyBoosts(boost);
            }
            else
            {
                target.ApplyBoosts(boost);
            }
        }

        // Status Condition
        if (effect.Status != ConditionID.None)
        {
            if (moveTarget == MoveTarget.Self)
            {
                source.SetStatus(effect.Status);
            }
            else
            {
                target.SetStatus(effect.Status);
            }
        }

        // Volatile Status Condition
        if (effect.VolatileStatus != ConditionID.None)
        {
            if (effect.VolatileStatus == ConditionID.flinch && firstToMove.Anigma != source)
            {
                yield break;
            }

            if (moveTarget == MoveTarget.Self)
            {
                source.SetVolatileStatus(effect.VolatileStatus);
            }
            else
            {
                target.SetVolatileStatus(effect.VolatileStatus);
            }
        }

        // Weather Condition
        if (effect.Weather != ConditionID.None)
        {
            Field.SetWeather(effect.Weather);
            Field.WeatherDuration = 5;
            yield return dialogBox.TypeDialog(Field.Weather.StartMessage);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    IEnumerator ShowDamageDetail(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
        {
            yield return dialogBox.TypeDialog("A critical hit!");
        }
    }

    IEnumerator ShowTypeEffectiveness(float effectiveness, Battler battler)
    {
        if (effectiveness > 2f)
        {
            yield return dialogBox.TypeDialog("It's tremendously effective!");
        }
        else if (effectiveness > 1f)
        {
            yield return dialogBox.TypeDialog("It's super effective!");
        }
        else if (effectiveness < 1f)
        {
            yield return dialogBox.TypeDialog("It's not very effective!");
        }
        else if (effectiveness < 0.5f)
        {
            yield return dialogBox.TypeDialog("It's really uneffective!");
        }
        else if (effectiveness == 0)
        {
            yield return dialogBox.TypeDialog($"It doesn't affect {battler.Base.Name}!");
        }
    }

    IEnumerator SwitchAnigma(Battler newAnigma, bool isTrainerAboutToUse=false)
    {
        if (playerUnit.Anigma.HP > 0)
        {
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Anigma.Base.Name}!");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1.5f);
        }

        playerUnit.Setup(newAnigma);

        dialogBox.SetMoveNames(newAnigma.Moves);

        yield return dialogBox.TypeDialog($"{newAnigma.Base.Name} go!");

        if (isTrainerAboutToUse)
            StartCoroutine(SendNextTrainerAnigma());
        else
            state = BattleState.RunningTurn;
    }

    IEnumerator SendNextTrainerAnigma()
    {
        state = BattleState.Busy;
        var nextAnigma = trainerParty.GetHealthyBattler();
        opponentUnit.Setup(nextAnigma);
        yield return dialogBox.TypeDialog($"{trainer.Name} send out {nextAnigma.Base.Name}!");

        state = BattleState.RunningTurn;
    }

    IEnumerator OnItemUsed(ItemBase usedItem)
    {
        state = BattleState.Busy;
        inventoryUI.gameObject.SetActive(false);

        if (usedItem is RingItem)
        {
            yield return TriggerRing((RingItem)usedItem);
        }

        StartCoroutine(RunTurns(BattleAction.UseItem));
    }

    IEnumerator TriggerRing(RingItem ringItem)
    {
        state = BattleState.Busy;
        dialogBox.EnableBigDialogBox(true);
        dialogBox.EnableActionSelector(false);

        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog($"You can't steal trainers anigmas!");
            state = BattleState.RunningTurn;
            yield break;
        }

        yield return dialogBox.TypeDialog($"{player.Name} triggered a {ringItem.Name}!");

        var ringObj = Instantiate(ringSprite, playerUnit.transform.position - new Vector3(2, 0), Quaternion.identity);
        var ring = ringObj.GetComponent<SpriteRenderer>();
        ring.sprite = ringItem.Icon;

        // Animations
        yield return ring.transform.DOJump(opponentUnit.transform.position + new Vector3(0, 2.5f), 2, 1, 1f).WaitForCompletion();
        yield return opponentUnit.PlayCaptureAnimation();
        yield return ring.transform.DOMoveY(opponentUnit.transform.position.y - 0.5f, 1f).WaitForCompletion();

        int shakeCount = TryToCatchAnigma(opponentUnit.Anigma, ringItem);

        for (int i = 0; i < Mathf.Min(shakeCount, 4); i++)
        {
            var sequence = DOTween.Sequence();
            int selectedAnim = UnityEngine.Random.Range(0, ringAnimVectors.Count);
            sequence.Append(ring.transform.DOPunchPosition(ringAnimVectors[selectedAnim], 0.8f));
            sequence.Join(ring.DOColor(new Color(1, 0.6622641f, 0.6622641f), 0.8f));
            sequence.Append(ring.DOColor(new Color(1, 1, 1), 0.8f));
            yield return sequence.WaitForCompletion();
            yield return new WaitForSeconds(0.1f);
        }

        if (shakeCount == 5)
        {
            // Anigma is caught
            yield return dialogBox.TypeDialog($"{opponentUnit.Anigma.Base.Name} was caught!");
            yield return ring.DOFade(0f, 1.5f).WaitForCompletion();

            playerParty.AddBattler(opponentUnit.Anigma);
            yield return dialogBox.TypeDialog($"{opponentUnit.Anigma.Base.Name} has been added to your party.");

            Destroy(ring);
            BattleOver(true);
        }
        else
        {
            // Anigma broke out
            yield return new WaitForSeconds(0.65f);
            var sequence = DOTween.Sequence();
            sequence.Append(ring.transform.DOPunchPosition(new Vector3(0.12f, 0), 0.8f));
            sequence.Join(ring.transform.DOPunchPosition(new Vector3(0, 0.12f), 0.8f));
            sequence.Join(ring.DOColor(new Color(1, 0.5f, 0.5f), 0.8f));
            sequence.Append(ring.transform.DOScale(new Vector3(0.3f, 0.3f), 0.2f));
            sequence.Join(ring.DOFade(0f, 0.2f));
            yield return new WaitForSeconds(0.7f);
            yield return opponentUnit.PlayBreakOutAnimation();

            if (shakeCount < 2)
                yield return dialogBox.TypeDialog($"{opponentUnit.Anigma.Base.Name} broke free easily!");
            else if (shakeCount < 3)
                yield return dialogBox.TypeDialog($"{opponentUnit.Anigma.Base.Name} broke free!");
            else
                yield return dialogBox.TypeDialog($"Almost caught it!");

            Destroy(ring);
            state = BattleState.RunningTurn;
        }
    }

    int TryToCatchAnigma(Battler anigma, RingItem ringUsed)
    {
        float a = (3 * anigma.MaxHp - 2 * anigma.HP) * anigma.Base.CatchRate * ringUsed.CatchRateModifier * ConditionDB.GetStatusBonus(anigma.Status) / (3 * anigma.MaxHp);

        if (a >= 255 || ringUsed.AlwaysCatch)
            return 5;

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;
        while (shakeCount < 5)
        {
            if (UnityEngine.Random.Range(0, 65535) >= b)
                break;

            ++shakeCount;
        }
        return shakeCount;
    }

    IEnumerator TryToEscape()
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog($"You can't run from trainer battles!");
            state = BattleState.RunningTurn;
            yield break;
        }

        ++escapeAttemps;

        int playerSpeed = playerUnit.Anigma.Speed;
        int opponentSpeed = opponentUnit.Anigma.Speed;

        if (playerSpeed > opponentSpeed)
        {
            yield return dialogBox.TypeDialog($"You ran away safely!");
            BattleOver(true);
        }
        else
        {
            float f = (playerSpeed * 128) / opponentSpeed + 30 * escapeAttemps;
            f = f % 256;

            if (UnityEngine.Random.Range(0, 256) < f)
            {
                yield return dialogBox.TypeDialog($"You ran away safely!");
                BattleOver(true);
            }
            else
            {
                if (escapeAttemps < 3)
                    yield return dialogBox.TypeDialog($"You couldn't escape...");
                else
                    yield return dialogBox.TypeDialog($"Unfortunately you couldn't escape...");
                state = BattleState.RunningTurn;
            }
        }
    }

    #endregion Battle

    #region Battle States Handlers
    // Pack of function that redirect user's inputs to certain UIs (Action selection, move selection, bag, etc)

    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
        else if (state == BattleState.AboutToUse)
        {
            HandleAboutToUse();
        }
        else if (state == BattleState.Bag)
        {
            Action onBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                state = BattleState.ActionSelection;
            };

            Action<ItemBase> onItemUsed = (ItemBase usedItem) => 
            {
                StartCoroutine(OnItemUsed(usedItem));
            };

            inventoryUI.HandleUpdate(onBack, onItemUsed);
        }
        else if (state == BattleState.MoveToForget)
        {
            Action<int> onMoveSelected = (moveIndex) =>
            {
                moveSelectionUI.gameObject.SetActive(false);
                if (moveIndex == BattlerBase.MaxNumOfMoves)
                {
                    // Don't learn the new move
                    StartCoroutine(dialogBox.TypeDialog($"{playerUnit.Anigma.Base.Name} did not acquired {moveToLearn.Name}"));
                }
                else
                {
                    // Forget the selected move and learn the new one
                    var selectedMove = playerUnit.Anigma.Moves[moveIndex].Base;

                    playerUnit.Anigma.Moves[moveIndex] = new Move(moveToLearn);
                    StartCoroutine(dialogBox.TypeDialog($"{playerUnit.Anigma.Base.Name} forgot {selectedMove.Name} and successfully acquired {moveToLearn.Name}"));
                }

                moveToLearn = null;
                state = BattleState.RunningTurn;
            };
            
            moveSelectionUI.HandleMoveSelection(onMoveSelected);
        }
    }

    void HandleActionSelection()
    {
        if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Right))
        {
            if (currentAction == 0 || currentAction == 2)
            {
                ++currentAction;
            }
            else
            {
                --currentAction;
            }
        }
        else if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Left))
        {
            if (currentAction == 1 || currentAction == 3)
            {
                --currentAction;
            }
            else
            {
                ++currentAction;
            }
        }
        else if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Up))
        {
            if (currentAction == 2 || currentAction == 3)
            {
                currentAction = currentAction - 2;
            }
            else
            {
                currentAction = currentAction + 2;
            }
        }
        else if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Down))
        {
            if (currentAction == 0 || currentAction == 1)
            {
                currentAction = currentAction + 2;
            }
            else
            {
                currentAction = currentAction - 2;
            }
        }

        dialogBox.UpdateActionSelection(currentAction);

        if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Enter))
        {
            if (currentAction == 0)
            {
                //Fight
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                //Open party screen
                OpenPartyScreen();
            }
            else if (currentAction == 2)
            {
                // Open bag
                OpenBag();
            }
            else if (currentAction == 3)
            {
                // Run away
                StartCoroutine(RunTurns(BattleAction.Run));
            }
        }
    }

    void HandlePartySelection()
    {
        Action onSelected = () =>
        {
            var selectedMember = partyScreen.SelectedMember;
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText("You can't send out a fainted anigma...");
                return;
            }
            else if (selectedMember == playerUnit.Anigma)
            {
                partyScreen.SetMessageText("You can't switch with the same anigma!");
                return;
            }

            partyScreen.gameObject.SetActive(false);

            if (partyScreen.CalledFrom == BattleState.ActionSelection)
            {
                StartCoroutine(RunTurns(BattleAction.SwitchAnigma));
            }
            else
            {
                state = BattleState.Busy;
                bool isTrainerAboutToUse = partyScreen.CalledFrom == BattleState.AboutToUse;
                StartCoroutine(SwitchAnigma(selectedMember, isTrainerAboutToUse));
            }

            partyScreen.CalledFrom = null;
        };

        Action onBack = () =>
        {
            if (playerUnit.Anigma.HP <= 0)
            {
                partyScreen.SetMessageText("You have to choose another anigma to continue.");
                return;
            }

            partyScreen.gameObject.SetActive(false);

            //Get back to action selection
            if (partyScreen.CalledFrom == BattleState.AboutToUse)
            {
                StartCoroutine(SendNextTrainerAnigma());
            }
            else
                ActionSelection();

            partyScreen.CalledFrom = null;
        };

        partyScreen.HandleUpdate(onSelected, onBack);
    }

    void HandleAboutToUse()
    {
        if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Up) || GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Down))
            aboutToUseChoice = !aboutToUseChoice;

        dialogBox.UpdateChoiceBox(aboutToUseChoice);

        if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Enter))
        {
            dialogBox.EnableChoiceBox(false);
            if (aboutToUseChoice)
            {
                // yes
                OpenPartyScreen();
            }
            else
            {
                // no
                StartCoroutine(SendNextTrainerAnigma());
            }
        }
        else if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Back))
        {
            // no
            dialogBox.EnableChoiceBox(false);
            StartCoroutine(SendNextTrainerAnigma());
        }
    }

    void HandleMoveSelection()
    {
        if (currentMove >= playerUnit.Anigma.Moves.Count)
        {
            currentMove = 0;
        }

        if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Back))
        {
            //Get back to action selection
            ActionSelection();
        }

        if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Right))
        {
            if (currentMove == 0 && playerUnit.Anigma.Moves.Count > 1 || currentMove == 2 && playerUnit.Anigma.Moves.Count > 3)
            {
                ++currentMove;
            }
            else if (currentMove == 2 && playerUnit.Anigma.Moves.Count == 3)
            {
                --currentMove;
            }
            else
            {
                if (currentMove == 1 || currentMove == 3)
                    --currentMove;
            }
        }
        else if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Left))
        {
            if (currentMove == 1 || currentMove == 3)
            {
                --currentMove;
            }
            else
            {
                if (playerUnit.Anigma.Moves.Count > 1 && currentMove == 0 || playerUnit.Anigma.Moves.Count > 3 && currentMove == 2)
                {
                    ++currentMove;
                }
            }
        }
        else if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Up))
        {
            if (currentMove == 2 || currentMove == 3)
            {
                currentMove = currentMove - 2;
            }
            else
            {
                if (playerUnit.Anigma.Moves.Count > 2 && currentMove == 0 || playerUnit.Anigma.Moves.Count > 3 && currentMove == 2)
                {
                    currentMove = currentMove + 2;
                }
            }
        }
        else if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Down))
        {
            if (currentMove == 0 && playerUnit.Anigma.Moves.Count > 2 || currentMove == 1 && playerUnit.Anigma.Moves.Count > 3)
            {
                currentMove = currentMove + 2;
            }
            else if (currentMove == 1 && playerUnit.Anigma.Moves.Count == 3)
            {
                currentMove++;
            }
            else
            {
                if (currentMove != 0 && currentMove != 1)
                    currentMove = currentMove - 2;
            }
        }

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Anigma.Moves[currentMove]);

        if(GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Enter))
        {
            var move = playerUnit.Anigma.Moves[currentMove];
            if (move.UP <= 0) return;

            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            dialogBox.EnableBigDialogBox(true);
            StartCoroutine(RunTurns(BattleAction.Move));
        }
    }

    #endregion Battle States Handlers
}
