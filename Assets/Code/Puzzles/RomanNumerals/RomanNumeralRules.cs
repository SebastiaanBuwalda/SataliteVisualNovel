/// <summary>X in the middle slot acts as multiplication; otherwise add all three tiles.</summary>
public static class RomanNumeralRules
{
    public static bool TryEvaluate(int? first, int? middle, int? last, out int result)
    {
        result = 0;
        if (!first.HasValue || !middle.HasValue || !last.HasValue)
        {
            return false;
        }

        result = middle.Value == 10
            ? first.Value * last.Value
            : first.Value + middle.Value + last.Value;
        return true;
    }
}
