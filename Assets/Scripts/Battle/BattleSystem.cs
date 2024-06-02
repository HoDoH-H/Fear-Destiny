using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy, PartyScreen }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHUD playerHUD;
    [SerializeField] BattleUnit opponentUnit;
    [SerializeField] BattleHUD opponentHUD;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;

    public event Action<bool> OnBattleOver;

    BattleState state;
    [SerializeField] int currentAction;
    [SerializeField] int currentMove;
    [SerializeField] int currentMember;

    AnigmaParty playerParty;
    Anigma wildAnigma;

    // Region Start - Start battle

    public void StartBattle(AnigmaParty playerParty, Anigma wildAnigma)
    {
        this.playerParty = playerParty;
        this.wildAnigma = wildAnigma;
        StartCoroutine(SetupBattle());
    }





    public IEnumerator SetupBattle()
    {
        playerUnit.Setup(playerParty.GetHealthyPokemon());
        opponentUnit.Setup(wildAnigma);
        playerHUD.SetData(playerUnit.Anigma);
        opponentHUD.SetData(opponentUnit.Anigma);

        partyScreen.Init();

        dialogBox.SetMoveNames(playerUnit.Anigma.Moves);

        yield return dialogBox.TypeDialog($"A wild {opponentUnit.Anigma.Base.Name} appeared.");

        PlayerAction();
    }

    // Region End - Start battle

    // Region Start - UI Managers

    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        dialogBox.SetDialog("Choose an action");
        dialogBox.EnableActionSelector(true);
        dialogBox.EnableDialogText(true);
        dialogBox.EnableMoveSelector(false);
        partyScreen.gameObject.SetActive(false);
    }




    void PlayerParty()
    {
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Anigmas);
        partyScreen.gameObject.SetActive(true);
    }




    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    // Region End - UI Managers

    // Region Start - Battle

    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;

        var move = playerUnit.Anigma.Moves[currentMove];

        move.UP--;
        yield return dialogBox.TypeDialog($"{playerUnit.Anigma.Base.Name} used {move.Base.Name}.");

        playerUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(0.5f);

        opponentUnit.PlayHitAnimation();
        var damageDetails = opponentUnit.Anigma.TakeDamage(move, playerUnit.Anigma);

        yield return opponentHUD.UpdateHP();

        yield return ShowDamageDetail(damageDetails, opponentUnit.Anigma);

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{opponentUnit.Anigma.Base.Name} fainted.");
            opponentUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(1.5f);
            OnBattleOver(true);
        }
        else
        {
            StartCoroutine(PerformOpponentMove());
        }
    }






    IEnumerator PerformOpponentMove()
    {
        state = BattleState.EnemyMove;

        var move = opponentUnit.Anigma.GetRandomMove();

        move.UP--;
        yield return dialogBox.TypeDialog($"{opponentUnit.Anigma.Base.Name} used {move.Base.Name}.");

        opponentUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(0.5f);

        playerUnit.PlayHitAnimation();
        var damageDetails = playerUnit.Anigma.TakeDamage(move, opponentUnit.Anigma);

        yield return playerHUD.UpdateHP();

        yield return ShowDamageDetail(damageDetails, playerUnit.Anigma);

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Anigma.Base.Name} fainted.");
            playerUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(1.5f);
            var nextAnigma = playerParty.GetHealthyPokemon();
            if (nextAnigma != null)
            {
                PlayerParty();
            }
            else
            {
                OnBattleOver(false);
            }
        }
        else
        {
            PlayerAction();
        }
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

    // Region End - Battle

    // Region Start - Battle States Handlers

    public void HandleUpdate()
    {
        if (state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
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
                PlayerMove();
            }
            else if (currentAction == 1)
            {
                //Open party screen
                PlayerParty();
            }
            else if (currentAction == 2)
            {
                print("Bag");
            }
            else if (currentAction == 3)
            {
                print("Run");
            }
        }
    }





    void HandlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape))
        {
            //Get back to action selection
            PlayerAction();
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
            state = BattleState.Busy;
            StartCoroutine(SwitchAnigma(selectedMember));
        }
    }

    IEnumerator SwitchAnigma(Anigma newAnigma)
    {
        var wasFainted = true;
        if (playerUnit.Anigma.HP > 0)
        {
            wasFainted = false;
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Anigma.Base.Name}!");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1.5f);
        }

        playerUnit.Setup(newAnigma);
        playerHUD.SetData(newAnigma);

        dialogBox.SetMoveNames(newAnigma.Moves);

        yield return dialogBox.TypeDialog($"{newAnigma.Base.Name} go!");

        if (!wasFainted)
        {
            yield return PerformOpponentMove();
        }
        else
        {
            PlayerAction();
        }
    }






    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape))
        {
            //Get back to action selection
            PlayerAction();
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
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerMove());
        }
    }

    // Region End - Battle States Handlers
}
