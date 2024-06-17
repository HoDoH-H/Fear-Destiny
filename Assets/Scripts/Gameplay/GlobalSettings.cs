using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    [SerializeField] Color highlightedColor;

    public Color HighlightedColor => highlightedColor;

    public static GlobalSettings Instance;

    private void Awake()
    {
        Instance = this;
    }
}
