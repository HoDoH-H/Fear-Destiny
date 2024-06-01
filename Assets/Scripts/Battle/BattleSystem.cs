using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHUD playerHUD;
    [SerializeField] BattleUnit opponentUnit;
    [SerializeField] BattleHUD opponentHUD;
    [SerializeField] BattleDialogBox dialogBox;

    public event Action<bool> OnBattleOver;

    BattleState state;
    [SerializeField] int currentAction;
    [SerializeField] int currentMove;

    AnigmaParty playerParty;
    Anigma wildAnigma;

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

        dialogBox.SetMoveNames(playerUnit.Anigma.Moves);

        yield return dialogBox.TypeDialog($"A wild {opponentUnit.Anigma.Base.Name} appeared.");

        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("Choose an action"));
        dialogBox.EnableActionSelector(true);
        dialogBox.EnableDialogText(true);
        dialogBox.EnableMoveSelector(false);
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

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
            OnBattleOver(false);
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
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(currentAction < 1)
            {
                ++currentAction;
            }
            else
            {
                --currentAction;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
            {
                --currentAction;
            }
            else
            {
                ++currentAction;
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
                print("Run");
            }
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
            if (currentMove < 1 && playerUnit.Anigma.Moves.Count > 1 || currentMove < 3 && currentMove > 1 && playerUnit.Anigma.Moves.Count > 3)
            {
                ++currentMove;
            }
            else
            {
                if (currentMove == 1 || currentMove == 3)
                    --currentMove;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0 && currentMove < 2 || currentMove > 2)
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
}
