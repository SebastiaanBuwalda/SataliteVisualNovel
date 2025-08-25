using UnityEngine;

public class NumericalValue: MonoBehaviour
{
    [SerializeField] private int value;

    public int Value
    {
        get { return value; }
        set { this.value = value; }
    }
}
