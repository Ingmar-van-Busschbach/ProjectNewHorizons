using UnityEngine;

public class RatHats : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    public void AssignHat(Room room)
    {
        spriteRenderer.sprite = room.ratHat;
    }
}
