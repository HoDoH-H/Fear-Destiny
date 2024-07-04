using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    [SerializeField] Color highlightedColor;
    [SerializeField] Color baseInvColor;

    public Color HighlightedColor => highlightedColor;
    public Color BaseInvColor => baseInvColor;

    public static GlobalSettings Instance;

    private void Awake()
    {
        Instance = this;
    }
}
