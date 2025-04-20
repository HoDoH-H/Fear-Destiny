using System.Collections;
using System.Linq;
using UnityEngine;

public class LocationPortal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] Destination destinationPortal;
    [SerializeField] AudioClip transitionSound;

    PlayerController player;

    public void OnPlayerTriggered(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
        this.player = player;
        StartCoroutine(Teleport());
    }

    Fader fader;
    private void Start()
    {
        fader = FindObjectOfType<Fader>();
    }

    IEnumerator Teleport()
    {
        AudioManager.Instance.PlaySFX(transitionSound);
        GameController.Instance.PauseGame(true);

        yield return fader.FadeIn(0.5f);

        var destPortal = FindObjectsOfType<LocationPortal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);

        yield return fader.FadeOut(0.5f);

        GameController.Instance.PauseGame(false);
    }

    public Transform SpawnPoint => spawnPoint;

    public bool TriggerRepeatedly => false;
}
