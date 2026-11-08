using UnityEngine;

public class TileValue : MonoBehaviour
{
    [SerializeField] private int value;

    public int Value
    {
        get => value;
        set => this.value = value;
    }
}
