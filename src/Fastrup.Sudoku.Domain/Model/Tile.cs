namespace Fastrup.Sudoku.Domain.Model;

public sealed class Tile(int initialValue)
{
    private int _userValue = initialValue;

    public int InitialValue { get; } = initialValue;

    public int UserValue
    {
        get => _userValue;
        set
        {
            if (IsValueReset && !IsCorrect) _userValue = value;
        }
    }

    public bool IsValueReset { get; private set; }

    public bool IsCorrect => UserValue == InitialValue;

    public bool IsError => IsValueReset && UserValue != 0 && !IsCorrect;

    public void SetZeroValue()
    {
        _userValue = 0;
        IsValueReset = true;
    }

    public void ResetValue()
    {
        _userValue = InitialValue;
        IsValueReset = false;
    }

    public override string ToString() => $"{UserValue} ({InitialValue})";
}
