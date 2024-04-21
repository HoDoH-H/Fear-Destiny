using UnityEngine;

public class TilemapManager : MonoBehaviour
{
    public Sprite newSprite;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GetComponent<SpriteRenderer>().sprite = newSprite;
        }
    }
}
