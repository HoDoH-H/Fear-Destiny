using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    [SerializeField] Color highlightedColor;
    [SerializeField] Color baseInvColor;
    [SerializeField] List<KeyCode> enterKeys;
    [SerializeField] List<KeyCode> backKeys;
    [SerializeField] List<KeyCode> upKeys;
    [SerializeField] List<KeyCode> downKeys;
    [SerializeField] List<KeyCode> leftKeys;
    [SerializeField] List<KeyCode> rightKeys;

    public Color HighlightedColor => highlightedColor;
    public Color BaseInvColor => baseInvColor;
    public List<KeyCode> EnterKeys => enterKeys;
    public List<KeyCode> BackKeys => backKeys;
    public List<KeyCode> UpKeys => upKeys;
    public List<KeyCode> DownKeys => downKeys;
    public List<KeyCode> LeftKeys => leftKeys;
    public List<KeyCode> RightKeys => rightKeys;

    public static GlobalSettings Instance;

    private void Awake()
    {
        Instance = this;
    }
}
