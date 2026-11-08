/// <summary>Digit patterns in scene order: top-left, top-right, middle-left,
/// middle-right, bottom-left, bottom-right (Braille dots 1, 4, 2, 5, 3, 6).</summary>
public static class BrailleDigitPatterns
{
    public const int DotCount = 6;

    private static readonly string[] Patterns =
    {
        "011100", // 0: dots 2, 4, 5
        "100000", // 1: dot 1
        "101000", // 2: dots 1, 2
        "110000", // 3: dots 1, 4
        "110100", // 4: dots 1, 4, 5
        "100100", // 5: dots 1, 5
        "111000", // 6: dots 1, 2, 4
        "111100", // 7: dots 1, 2, 4, 5
        "101100", // 8: dots 1, 2, 5
        "011000"  // 9: dots 2, 4
    };

    public static int GetNumber(string pattern)
    {
        for (int number = 0; number < Patterns.Length; number++)
        {
            if (Patterns[number] == pattern)
            {
                return number;
            }
        }

        return -1;
    }

    public static string GetPattern(int number)
    {
        return number >= 0 && number < Patterns.Length ? Patterns[number] : null;
    }
}
