using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MorlenisManager : MonoBehaviour
{
    [SerializeField] GameObject morlenisUI;
    [SerializeField] Image anigmaImage;
    [SerializeField] AudioClip morlenisMusic;

    public event Action OnMorlenisStart;
    public event Action OnMorlenisCompleted;

    public static MorlenisManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator Morlenis(Battler anigma, Morlen metamorphosis)
    {
        OnMorlenisStart?.Invoke();
        morlenisUI.SetActive(true);

        AudioManager.Instance.PlayMusic(morlenisMusic);

        anigmaImage.sprite = anigma.Base.FrontSprite;
        anigmaImage.SetNativeSize();

        yield return DialogManager.Instance.ShowDialogText($"{anigma.Base.Name} is trying to morlen...");

        anigma.Morlen(metamorphosis);

        var oldAnigma = anigma.Base;
        anigmaImage.sprite = anigma.Base.FrontSprite;
        anigmaImage.SetNativeSize();

        yield return DialogManager.Instance.ShowDialogText($"{oldAnigma.Name} completed its morlenis into a {anigma.Base.Name}");

        morlenisUI.SetActive(false);
        OnMorlenisCompleted?.Invoke();
    }
}
