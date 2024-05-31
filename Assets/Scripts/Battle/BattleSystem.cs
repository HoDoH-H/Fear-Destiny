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

    BattleState state;
    [SerializeField]int currentAction;

    private void Start()
    {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup();
        playerHUD.SetData(playerUnit.Anigma);
        opponentUnit.Setup();
        opponentHUD.SetData(opponentUnit.Anigma);

        dialogBox.SetMoveNames(playerUnit.Anigma.Moves);

        yield return dialogBox.TypeDialog($"A wild {opponentUnit.Anigma.Base.Name} appeared.");
        yield return new WaitForSeconds(1f);

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

    private void Update()
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

    void ResetCurrentAction()
    {
        currentAction = 0;
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
                ResetCurrentAction();
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
            ResetCurrentAction();
            PlayerAction();
        }

            if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentAction < 1 && playerUnit.Anigma.Moves.Count > 1 || currentAction < 3 && currentAction > 1 && playerUnit.Anigma.Moves.Count > 4)
            {
                ++currentAction;
            }
            else
            {
                if (currentAction != 0 && currentAction != 2)
                    --currentAction;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentAction > 0 && currentAction < 2 || currentAction > 2)
            {
                --currentAction;
            }
            else
            {
                if (playerUnit.Anigma.Moves.Count > 1 && currentAction == 0 || playerUnit.Anigma.Moves.Count > 3 && currentAction == 2)
                {
                    ++currentAction;
                }
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
                if (playerUnit.Anigma.Moves.Count > 2 && currentAction == 0 || playerUnit.Anigma.Moves.Count > 3 && currentAction == 2)
                {
                    currentAction = currentAction + 2;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction == 0 && playerUnit.Anigma.Moves.Count > 2 || currentAction == 1 && playerUnit.Anigma.Moves.Count > 3)
            {
                currentAction = currentAction + 2;
            }
            else
            {
                if (currentAction != 0 && currentAction != 1)
                    currentAction = currentAction - 2;
            }
        }

        if (currentAction >= playerUnit.Anigma.Moves.Count)
        {
            dialogBox.UpdateMoveSelection(currentAction, null);
        }
        else
        {
            dialogBox.UpdateMoveSelection(currentAction, playerUnit.Anigma.Moves[currentAction]);
        }
    }
}
