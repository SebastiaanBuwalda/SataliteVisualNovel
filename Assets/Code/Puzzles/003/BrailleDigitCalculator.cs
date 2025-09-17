using System.Collections.Generic;

public static class BrailleDigitCalculator
{
    public static readonly Dictionary<int, bool[]> DigitToPattern = new Dictionary<int, bool[]>
    {
        {1, new bool[]{ true, false, false, false, false, false }},
        {2, new bool[]{ true, true,  false, false, false, false }},
        {3, new bool[]{ true, false, false, true,  false, false }},
        {4, new bool[]{ true, false, false, true,  true,  false }},
        {5, new bool[]{ true, false, false, false, true,  false }},
        {6, new bool[]{ true, true,  false, true,  false, false }},
        {7, new bool[]{ true, true,  false, true,  true,  false }},
        {8, new bool[]{ true, true,  false, false, true,  false }},
        {9, new bool[]{ false, true,  false, true,  false, false }},
        {0, new bool[]{ false, true,  false, true,  true,  false }},
    };

    public static bool PatternsEqual(bool[] a, bool[] b)
    {
        if (a == null || b == null || a.Length != 6 || b.Length != 6) return false;
        for (int i = 0; i < 6; i++) if (a[i] != b[i]) return false;
        return true;
    }

    public static bool TryGetDigit(bool[] pattern, out int digit)
    {
        foreach (var kv in DigitToPattern)
        {
            if (PatternsEqual(kv.Value, pattern))
            {
                digit = kv.Key;
                return true;
            }
        }
        digit = -1;
        return false;
    }
}
