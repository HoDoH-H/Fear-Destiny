using UnityEngine;

public class TilemapManager : MonoBehaviour
{
    public Sprite newSprite;
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = newSprite;
    }
}
