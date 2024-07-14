using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, ISavable
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;

    public Vector2 input;
    public bool CanMove = true;
    private Character character;

    
    public Vector2 direction;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    int GetKey(GlobalSettings.KeyList key)
    {
        return GlobalSettings.Instance.BoolToInt(GlobalSettings.Instance.IsKeyPressed(key));
    }

    public void HandleUpdate()
    {
        if (!character.IsMoving && CanMove)
        {
            //input = move.ReadValue<Vector2>();
            input = new Vector2(GetKey(GlobalSettings.KeyList.Right) - GetKey(GlobalSettings.KeyList.Left), GetKey(GlobalSettings.KeyList.Up) - GetKey(GlobalSettings.KeyList.Down));

            if (input.x != 0) { input.y = 0; input.x = input.normalized.x; } 

            if (input != Vector2.zero)
            {
                direction = input.normalized;
                StartCoroutine(character.Move(input, OnMoveOver));
            }
        }

        character.HandleUpdate();

        if (GlobalSettings.Instance.IsKeyDown(GlobalSettings.KeyList.Enter))
            StartCoroutine(Interact());
    }

    private IEnumerator Interact()
    {
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos = transform.position + facingDir;

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer);
        if (collider != null)
        {
            yield return collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    IPlayerTriggerable currentlyInTrigger;
    private void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, 0.45f, GameLayers.i.TriggerableLayers);

        IPlayerTriggerable triggerable = null;
        foreach(var collider in colliders)
        {
            triggerable = collider.GetComponent<IPlayerTriggerable>();
            if (triggerable != null)
            {
                if (triggerable == currentlyInTrigger && !triggerable.TriggerRepeatedly)
                    break;

                triggerable.OnPlayerTriggered(this);
                currentlyInTrigger = triggerable;
                break;
            }
        }

        if (colliders.Count() == 0 || triggerable != currentlyInTrigger)
            currentlyInTrigger = null;
    }

    public object CaptureState()
    {
        var saveData = new PlayerSaveData() 
        {
            position = new float[] { transform.position.x, transform.position.y },
            direction = new float[] { direction.x, direction.y },
            anigmas = GetComponent<AnigmaParty>().Anigmas.Select(a => a.GetSaveData()).ToList(),
        };

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (PlayerSaveData)state;

        // Restore Player's position
        transform.position = new Vector3(saveData.position[0], saveData.position[1]);

        // Restore Player's direction
        direction = new Vector3(saveData.direction[0], saveData.direction[1]);
        Debug.Log(transform.position + new Vector3(direction.x, direction.y));
        character.LookTowards(transform.position + new Vector3(direction.x, direction.y));

        // Restore Player's Party
        GetComponent<AnigmaParty>().Anigmas =  saveData.anigmas.Select(a => new Anigma(a)).ToList();
    }

    public string Name { get => name; }
    public Sprite Sprite { get => sprite; }
    public Character Character => character;
}

[Serializable]
public class PlayerSaveData
{
    public float[] position;
    public float[] direction;
    public List<AnigmaSaveData> anigmas;
}