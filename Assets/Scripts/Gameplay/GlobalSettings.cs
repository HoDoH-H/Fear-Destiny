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

    List<List<KeyCode>> keyTypes;

    public Color HighlightedColor => highlightedColor;
    public Color BaseInvColor => baseInvColor;

    public static GlobalSettings Instance;

    private void Awake()
    {
        keyTypes = new List<List<KeyCode>>() { enterKeys, backKeys, upKeys, downKeys, leftKeys, rightKeys};

        Instance = this;
    }

    public bool IsKeyDown(KeyList keyType)
    {
        foreach (var key in keyTypes[((int)keyType)])
        {
            if (Input.GetKeyDown(key))
            {
                return true;
            }
        }

        return false;
    }

    public bool IsKeyPressed(KeyList keyType)
    {
        foreach (var key in keyTypes[((int)keyType)])
        {
            if (Input.GetKey(key))
            {
                return true;
            }
        }

        return false;
    }

    public int BoolToInt(bool value)
    {
        if (value)
            return 1;
        return 0;
    }

    public enum KeyList { Enter, Back, Up, Down, Left, Right }
}
