using DG.Tweening;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { Start, ActionSelection, MoveSelection, RunningTurn, Busy, PartyScreen, AboutToUse, MoveToForget, BattleOver }
public enum BattleAction { Move, SwitchAnigma, UseItem, Run}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit opponentUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;
    [SerializeField] GameObject ringSprite;
    [SerializeField] MoveSelectionUI moveSelectionUI;

    public event Action<bool> OnBattleOver;

    BattleState state;
    BattleState? previousState;


     int currentAction;
     int currentMove;
     int currentMember;
    bool aboutToUseChoice = true;

    AnigmaParty playerParty;
    AnigmaParty trainerParty;
    Anigma wildAnigma;

    bool isTrainerBattle = false;
    PlayerController player;
    TrainerController trainer;

    int escapeAttemps;
    MoveBase moveToLearn;

    // Region Start - Start battle

    public void StartBattle(AnigmaParty playerParty, Anigma wildAnigma)
    {
        isTrainerBattle=false;
        this.playerParty = playerParty;
        this.wildAnigma = wildAnigma;
        player = playerParty.GetComponent<PlayerController>();
        StartCoroutine(SetupBattle());
    }

    public void StartTrainerBattle(AnigmaParty playerParty, AnigmaParty trainerParty)
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
            playerUnit.Setup(playerParty.GetHealthyAnigma());
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
            playerImage.sprite = player.Sprite;
            trainerImage.sprite = trainer.Sprite;

            yield return dialogBox.TypeDialog($"{trainer.Name} wants to battle!");

            // Send out first anigma of the trainer
            trainerImage.gameObject.SetActive(false);
            opponentUnit.gameObject.SetActive(true);
            var opponentAnigma = trainerParty.GetHealthyAnigma();
            opponentUnit.Setup(opponentAnigma);
            yield return dialogBox.TypeDialog($"{trainer.Name} send out {opponentAnigma.Base.Name}!");

            // Send out first anigma of the player
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            var playerAnigma = playerParty.GetHealthyAnigma();
            playerUnit.Setup(playerAnigma);
            yield return dialogBox.TypeDialog($"{playerAnigma.Base.Name} go!");
            dialogBox.SetMoveNames(playerUnit.Anigma.Moves);
        }

        partyScreen.Init();
        ActionSelection();
    }

    // Region End - Start battle

    // Region Start - UI Managers

    void BattleOver(bool isWon)
    {
        state = BattleState.BattleOver;
        playerParty.Anigmas.ForEach(p => p.OnBattleOver());
        OnBattleOver(isWon);
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
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Anigmas);
        partyScreen.gameObject.SetActive(true);
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator ChooseMoveToForget(Anigma anigma, MoveBase newMove)
    {
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog($"Do you want that {anigma.Base.Name} forget a move to learn this new one?");
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(anigma.Moves.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;

        state = BattleState.MoveToForget;
    }

    IEnumerator AboutToUse(Anigma newAnigma)
    {
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog($"{trainer.Name} is about to send another anigma. Do you want to change yours?");

        state = BattleState.AboutToUse;
        dialogBox.EnableChoiceBox(true);
    }

    // Region End - UI Managers

    // Region Start - Battle


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

            // First turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.Anigma.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) yield break;

            if (secondAnigma.HP > 0)
            {
                // Second turn
                yield return RunMove(secondUnit, firstUnit, secondUnit.Anigma.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }
        }
        else
        {
            if (playerAction == BattleAction.SwitchAnigma)
            {
                var selectedAnigma = playerParty.Anigmas[currentMember];
                state = BattleState.Busy;
                yield return SwitchAnigma(selectedAnigma);
            }
            else if (playerAction == BattleAction.UseItem)
            {
                if (isTrainerBattle)
                {
                    yield return dialogBox.TypeDialog($"You can't catch other trainer's anigmas!");
                    ActionSelection();
                    yield break;
                }
                else
                {
                    dialogBox.EnableActionSelector(false);
                    yield return TriggerRing();
                }
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
            yield return RunAfterTurn(opponentUnit);
            if (state == BattleState.BattleOver) yield break;
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
            yield return sourceUnit.Hud.UpdateHP();
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

        sourceUnit.PlayAttackAnimation();

        if (CheckIfMoveHits(move, sourceUnit.Anigma, targetUnit.Anigma))
        {

            yield return new WaitForSeconds(0.5f);
            targetUnit.PlayHitAnimation();

            if (move.Base.Category == AttackCategory.Status)
            {
                yield return RunMoveEffect(move.Base.Effects, sourceUnit.Anigma, targetUnit.Anigma, move.Base.Target);
            }
            else
            {
                var damageDetails = targetUnit.Anigma.TakeDamage(move, sourceUnit.Anigma);
                yield return targetUnit.Hud.UpdateHP();
                yield return ShowDamageDetail(damageDetails, targetUnit.Anigma);
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

            if (targetUnit.Anigma.HP <= 0)
            {
                yield return HandleAnigmaFainted(targetUnit);
            }
        }
        else
        {
            yield return new WaitForSeconds(0.35f);
            yield return dialogBox.TypeDialog($"{sourceUnit.Anigma.Base.Name}'s attack missed...");
        }
    }

    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (state == BattleState.BattleOver) yield break;
        yield return new WaitUntil(() => state == BattleState.RunningTurn);

        // Statuses like burn or poison will hurt the anigma after the turn
        sourceUnit.Anigma.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Anigma);
        yield return sourceUnit.Hud.UpdateHP();

        if (sourceUnit.Anigma.HP <= 0)
        {
            yield return HandleAnigmaFainted(sourceUnit);
            yield return new WaitUntil(() => state == BattleState.RunningTurn);
        }
    }

    bool CheckIfMoveHits(Move move, Anigma source, Anigma target)
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

    IEnumerator ShowStatusChanges(Anigma anigma)
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

            int expGain = Mathf.FloorToInt((expYield * opponentLevel * trainerBonus) / 7);
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
                    if (playerUnit.Anigma.Moves.Count < AnigmaBase.MaxNumOfMoves)
                    {
                        playerUnit.Anigma.LearnMove(newMove);
                        yield return dialogBox.TypeDialog($"{playerUnit.Anigma.Base.Name} learned {newMove.Base.Name}!");
                        dialogBox.SetMoveNames(playerUnit.Anigma.Moves);
                    }
                    else
                    {
                        // Need to forget a move to learn a new one
                        yield return dialogBox.TypeDialog($"{playerUnit.Anigma.Base.Name} is attempting to acquire {newMove.Base.Name}...");
                        yield return dialogBox.TypeDialog($"But an Anigma cannot learn more than {AnigmaBase.MaxNumOfMoves} moves!");
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
            var nextAnigma = playerParty.GetHealthyAnigma();
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
                var nextAnigma = trainerParty.GetHealthyAnigma();
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

    IEnumerator RunMoveEffect(MoveEffects effect, Anigma source, Anigma target, MoveTarget moveTarget)
    {
        // Stat Boosting
        if (effect.Boosts != null)
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

        // Status Condition
        if (effect.Status != ConditionID.None)
        {
            target.SetStatus(effect.Status);
        }

        // Volatile Status Condition
        if (effect.VolatileStatus != ConditionID.None)
        {
            target.SetVolatileStatus(effect.VolatileStatus);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    IEnumerator ShowDamageDetail(DamageDetails damageDetails, Anigma anigma)
    {
        if (damageDetails.Critical > 1f)
        {
            yield return dialogBox.TypeDialog("A critical hit!");
        }

        if (damageDetails.TypeEffectiveness > 2f)
        {
            yield return dialogBox.TypeDialog("It's tremendously effective!");
        }
        else if (damageDetails.TypeEffectiveness > 1f)
        {
            yield return dialogBox.TypeDialog("It's super effective!");
        }
        else if (damageDetails.TypeEffectiveness < 1f)
        {
            yield return dialogBox.TypeDialog("It's not very effective!");
        }
        else if (damageDetails.TypeEffectiveness < 0.5f)
        {
            yield return dialogBox.TypeDialog("It's really uneffective!");
        }
        else if (damageDetails.TypeEffectiveness == 0)
        {
            yield return dialogBox.TypeDialog($"It doesn't affect {anigma.Base.Name}!");
        }
    }

    IEnumerator SwitchAnigma(Anigma newAnigma)
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

        if (previousState == null)
        {
            state = BattleState.RunningTurn;
        }
        else if (previousState == BattleState.AboutToUse)
        {
            previousState = null;
            StartCoroutine(SendNextTrainerAnigma());
        }
    }

    IEnumerator SendNextTrainerAnigma()
    {
        state = BattleState.Busy;
        var nextAnigma = trainerParty.GetHealthyAnigma();
        opponentUnit.Setup(nextAnigma);
        yield return dialogBox.TypeDialog($"{trainer.Name} send out {nextAnigma.Base.Name}!");

        state = BattleState.RunningTurn;
    }

    IEnumerator TriggerRing()
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog($"You can't steal trainers anigmas!");
            state = BattleState.RunningTurn;
            yield break;
        }

        yield return dialogBox.TypeDialog($"{player.Name} triggered a Ring!");

        var ringObj = Instantiate(ringSprite, playerUnit.transform.position - new Vector3(2, 0), Quaternion.identity);
        var ring = ringObj.GetComponent<SpriteRenderer>();

        // Animations
        yield return ring.transform.DOJump(opponentUnit.transform.position + new Vector3(0, 2.5f), 2, 1, 1f).WaitForCompletion();
        yield return opponentUnit.PlayCaptureAnimation();
        yield return ring.transform.DOMoveY(opponentUnit.transform.position.y - 0.5f, 1f).WaitForCompletion();

        int shakeCount = TryToCatchAnigma(opponentUnit.Anigma);

        for (int i = 0; i < Mathf.Min(shakeCount, 3); i++)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(ring.transform.DOPunchPosition(new Vector3(0.10f, 0.10f), 0.8f));
            //sequence.Join(ring.transform.DOPunchPosition(new Vector3(0, 0.10f), 0.8f));
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

            playerParty.AddAnigma(opponentUnit.Anigma);
            yield return dialogBox.TypeDialog($"{opponentUnit.Anigma.Base.Name} has been added to your party.");

            Destroy(ring);
            BattleOver(true);
        }
        else
        {
            // Anigma broke out
            yield return new WaitForSeconds(1);
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

    int TryToCatchAnigma(Anigma anigma)
    {
        float a = (3 * anigma.MaxHp - 2 * anigma.HP) * anigma.Base.CatchRate * ConditionDB.GetStatusBonus(anigma.Status) / (3 * anigma.MaxHp);

        if (a >= 255)
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

    // Region End - Battle

    // Region Start - Battle States Handlers

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
        else if (state == BattleState.MoveToForget)
        {
            Action<int> onMoveSelected = (moveIndex) =>
            {
                moveSelectionUI.gameObject.SetActive(false);
                if (moveIndex == AnigmaBase.MaxNumOfMoves)
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
        if (Input.GetKeyDown(KeyCode.RightArrow))
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
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
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
        else if (Input.GetKeyDown(KeyCode.UpArrow))
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
        else if (Input.GetKeyDown(KeyCode.DownArrow))
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

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (currentAction == 0)
            {
                //Fight
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                //Open party screen
                previousState = state;
                OpenPartyScreen();
            }
            else if (currentAction == 2)
            {
                // TODO - Open bag ; NOW - Trigger ring
                StartCoroutine(RunTurns(BattleAction.UseItem));
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
        if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (playerUnit.Anigma.HP <= 0)
            {
                partyScreen.SetMessageText("You have to choose another anigma to continue.");
                return;
            }

            //Get back to action selection
            if (previousState == BattleState.AboutToUse)
            {
                previousState = null;
                StartCoroutine(SendNextTrainerAnigma());
            }
            else
                ActionSelection();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMember == 0 && playerParty.Anigmas.Count > 1 || currentMember == 2 && playerParty.Anigmas.Count > 3 || currentMember == 4 && playerParty.Anigmas.Count > 5)
            {
                ++currentMember;
            }
            else if (currentMember == 2 && playerParty.Anigmas.Count == 3 || currentMember == 4 && playerParty.Anigmas.Count == 5)
            {
                --currentMember;
            }
            else
            {
                if (currentMember == 1 || currentMember == 3 || currentMember == 5)
                    --currentMember;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMember == 1 || currentMember == 3 || currentMember == 5)
            {
                --currentMember;
            }
            else
            {
                if (playerParty.Anigmas.Count > 1 && currentMember == 0 || playerParty.Anigmas.Count > 3 && currentMember == 2 || playerParty.Anigmas.Count > 5 && currentMember == 4)
                {
                    ++currentMember;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMember > 1)
            {
                currentMember = currentMember - 2;
            }
            else
            {
                if (playerParty.Anigmas.Count > 4 && currentMember == 0 || playerParty.Anigmas.Count > 5 && currentMember == 1)
                {
                    currentMember = currentMember + 4;
                }
                else if (playerParty.Anigmas.Count > 2 && currentMember == 0 || playerParty.Anigmas.Count > 3 && currentMember == 1)
                {
                    currentMember = currentMember + 2;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMember + 2 < playerParty.Anigmas.Count)
            {
                currentMember = currentMember + 2;
            }
            else if (currentMember + 1 < playerParty.Anigmas.Count && currentMember % 2 == 1)
            {
                currentMember++;
            }
            else
            {
                if (currentMember % 2 == 0)
                    currentMember = 0;
                else
                    currentMember = 1;
            }
        }

        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            var selectedMember = playerParty.Anigmas[currentMember];
            if(selectedMember.HP <= 0)
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

            if (previousState == BattleState.ActionSelection)
            {
                previousState = null;
                StartCoroutine(RunTurns(BattleAction.SwitchAnigma));
            }
            else
            {
                state = BattleState.Busy;
                StartCoroutine(SwitchAnigma(selectedMember));
            }
        }
    }

    void HandleAboutToUse()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) ||  Input.GetKeyDown(KeyCode.DownArrow))
            aboutToUseChoice = !aboutToUseChoice;

        dialogBox.UpdateChoiceBox(aboutToUseChoice);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            dialogBox.EnableChoiceBox(false);
            if (aboutToUseChoice)
            {
                // yes
                previousState = BattleState.AboutToUse;
                OpenPartyScreen();
            }
            else
            {
                // no
                StartCoroutine(SendNextTrainerAnigma());
            }
        }
        else if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape))
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

        if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape))
        {
            //Get back to action selection
            ActionSelection();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
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
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
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
        else if (Input.GetKeyDown(KeyCode.UpArrow))
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
        else if (Input.GetKeyDown(KeyCode.DownArrow))
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

        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            var move = playerUnit.Anigma.Moves[currentMove];
            if (move.UP <= 0) return;

            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(RunTurns(BattleAction.Move));
        }
    }

    // Region End - Battle States Handlers
}
