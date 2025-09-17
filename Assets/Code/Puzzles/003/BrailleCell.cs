using UnityEngine;
using System;

public class BrailleCell : MonoBehaviour
{
    [SerializeField] private BrailleDot[] dots = new BrailleDot[6];

    public event Action<int, bool> OnAnyDotToggled;

    private void Awake()
    {
        for (int i = 0; i < dots.Length; i++)
        {
            int idx = i;
            dots[i].SetState(false);
            dots[i].OnDotToggled += (dotIndex, newState) =>
            {
                OnAnyDotToggled?.Invoke(dotIndex, newState);
            };
        }
    }

    public bool[] GetPattern()
    {
        bool[] pattern = new bool[6];
        for (int i = 0; i < 6; i++) pattern[i] = dots[i].GetState();
        return pattern;
    }

    public void SetPattern(bool[] pattern)
    {
        if (pattern == null || pattern.Length < 6) return;
        for (int i = 0; i < 6; i++) dots[i].SetState(pattern[i]);
    }
}
