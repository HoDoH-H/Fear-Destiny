using System.Collections;
using UnityEngine;

public class TrainerController : MonoBehaviour, Interactable, ISavable
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;
    [SerializeField] Dialog dialog;
    [SerializeField] Dialog dialogAfterBattle;
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;
    [SerializeField] bool seriousFight;

    [SerializeField] AudioClip provocClip;

    // State
    bool battleLost = false;

    Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        SetFovRotation(character.Animator.DefaultDirection);

    }

    private void Update()
    {
        character.HandleUpdate();
    }

    public IEnumerator ShowExclamation()
    {
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);
    }

    public IEnumerator Interact(Transform initiator)
    {
        character.LookTowards(initiator.position);
        if (!battleLost)
            StartCoroutine(TriggerTrainerBattle(initiator.GetComponent<PlayerController>()));
        else
            yield return DialogManager.Instance.ShowDialog(dialogAfterBattle);
    }

    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        AudioManager.Instance.PlayMusic(provocClip);

        // Exclaim
        yield return ShowExclamation();

        // Walk towards the player
        var diff = player.transform.position - transform.position;
        var moveVec = diff - diff.normalized;
        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));

        yield return character.Move(moveVec);

        // Show dialog
        yield return DialogManager.Instance.ShowDialog(dialog);

        GameController.Instance.StartTrainerBattle(this);
    }

    public void BattleLost()
    {
        battleLost = true;
        fov.gameObject.SetActive(false);

        if (seriousFight)
            this.gameObject.SetActive(false);
    }

    public void SetFovRotation(FacingDirection dir)
    {
        float angle = 0f;
        if (dir == FacingDirection.Right)
        {
            angle = 90f;
        }
        else if (dir == FacingDirection.Up)
        {
            angle = 180f;
        }
        else if (dir == FacingDirection.Left)
        {
            angle = 270f;
        }

        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    public object CaptureState()
    {
        return battleLost;
    }

    public void RestoreState(object state)
    {
        battleLost = (bool) state;

        fov.gameObject.SetActive(!battleLost);

        if (seriousFight)
            this.gameObject.SetActive(!battleLost);
    }

    public string Name { get => name; }
    public Sprite Sprite { get => sprite; }
}
