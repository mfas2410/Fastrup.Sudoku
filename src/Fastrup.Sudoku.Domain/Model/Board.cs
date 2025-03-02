namespace Fastrup.Sudoku.Domain.Model;

public sealed class Board
{
    private static readonly Random Random = new();
    private readonly Tile[] _tiles;

    private Board(Tile[] tiles) =>
        _tiles = tiles;

    public bool IsSolved => _tiles.All(x => x.IsCorrect);

    /// <summary>
    ///     •	Easy: Remove 20-30 numbers.
    ///     •	Medium: Remove 30-40 numbers.
    ///     •	Hard: Remove 40-50 numbers.
    ///     •	Expert: Remove 50-55 numbers.
    /// </summary>
    public void RemoveNumbers(int count)
    {
        int removed = 0;
        while (removed < count)
        {
            int index = Random.Next(0, 81);
            Tile tile = _tiles[index];
            if (tile.UserValue == 0) continue;

            tile.SetZeroValue();
            if (IsUniquelySolvable())
            {
                removed++;
            }
            else
            {
                tile.ResetValue();
            }
        }
    }

    private bool IsUniquelySolvable()
    {
        int solutions = 0;
        Solve(0, ref solutions);
        return solutions == 1;
    }

    private void Solve(int index, ref int solutions)
    {
        if (index == 81)
        {
            solutions++;
            return;
        }

        if (_tiles[index].UserValue != 0)
        {
            Solve(index + 1, ref solutions);
            return;
        }

        HashSet<int> existingNumbers = GetNumbersAlreadyTaken(index);

        for (int num = 1; num <= 9; num++)
        {
            if (existingNumbers.Contains(num)) continue;

            _tiles[index].UserValue = num;
            Solve(index + 1, ref solutions);
            _tiles[index].SetZeroValue();

            if (solutions > 1) return;
        }
    }

    private HashSet<int> GetNumbersAlreadyTaken(int index)
    {
        HashSet<int> existingNumbers = [];
        int row = index / 9;
        int col = index % 9;

        // Collect numbers in the row
        for (int i = 0; i < 9; i++)
        {
            Tile tile = _tiles[(row * 9) + i];
            if (tile.UserValue != 0) existingNumbers.Add(tile.UserValue);
        }

        // Collect numbers in the column
        for (int i = 0; i < 9; i++)
        {
            Tile tile = _tiles[(i * 9) + col];
            if (tile.UserValue != 0) existingNumbers.Add(tile.UserValue);
        }

        // Collect numbers in the subgrid
        int subGridRow = row / 3 * 3;
        int subGridCol = col / 3 * 3;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Tile tile = _tiles[((subGridRow + i) * 9) + subGridCol + j];
                if (tile.UserValue != 0) existingNumbers.Add(tile.UserValue);
            }
        }

        return existingNumbers;
    }

    public static Board Generate()
    {
        int[] numbers = new int[81];
        HashSet<int>[] rows = new HashSet<int>[9];
        HashSet<int>[] cols = new HashSet<int>[9];
        HashSet<int>[] subgrids = new HashSet<int>[9];

        for (int i = 0; i < 9; i++)
        {
            rows[i] = [];
            cols[i] = [];
            subgrids[i] = [];
        }

        FillDiagonalRegions(numbers, rows, cols, subgrids);
        FillRemainingCells(numbers, 0, 0, rows, cols, subgrids);
        return new Board(numbers.Select(x => new Tile(x)).ToArray());
    }

    private static void FillDiagonalRegions(int[] numbers, HashSet<int>[] rows, HashSet<int>[] cols, HashSet<int>[] subgrids)
    {
        for (int i = 0; i < 9; i += 3)
        {
            FillRegion(numbers, i, i, rows, cols, subgrids);
        }
    }

    private static void FillRegion(int[] numbers, int row, int col, HashSet<int>[] rows, HashSet<int>[] cols, HashSet<int>[] subgrids)
    {
        int[] randomLine = Enumerable.Range(1, 9).OrderBy(_ => Random.Next()).ToArray();
        int index = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int num = randomLine[index++];
                numbers[((row + i) * 9) + col + j] = num;
                rows[row + i].Add(num);
                cols[col + j].Add(num);
                subgrids[(row / 3 * 3) + (col / 3)].Add(num);
            }
        }
    }

    private static bool FillRemainingCells(int[] numbers, int i, int j, HashSet<int>[] rows, HashSet<int>[] cols, HashSet<int>[] subgrids)
    {
        if (i == 9) return true;

        if (j == 9) return FillRemainingCells(numbers, i + 1, 0, rows, cols, subgrids);

        if (numbers[(i * 9) + j] != 0) return FillRemainingCells(numbers, i, j + 1, rows, cols, subgrids);

        for (int num = 1; num <= 9; num++)
        {
            if (!IsSafe(num, i, j, rows, cols, subgrids)) continue;

            numbers[(i * 9) + j] = num;
            rows[i].Add(num);
            cols[j].Add(num);
            subgrids[(i / 3 * 3) + (j / 3)].Add(num);

            if (FillRemainingCells(numbers, i, j + 1, rows, cols, subgrids)) return true;

            numbers[(i * 9) + j] = 0;
            rows[i].Remove(num);
            cols[j].Remove(num);
            subgrids[(i / 3 * 3) + (j / 3)].Remove(num);
        }

        return false;
    }

    private static bool IsSafe(int num, int row, int col, HashSet<int>[] rows, HashSet<int>[] cols, HashSet<int>[] subgrids) =>
        !rows[row].Contains(num) && !cols[col].Contains(num) && !subgrids[(row / 3 * 3) + (col / 3)].Contains(num);

    public override string ToString()
    {
        List<string> lines = [];
        for (int i = 0; i < 9; i++)
        {
            lines.Add(string.Join(' ', _tiles.Skip(i * 9).Take(9).Select(x => x.UserValue == 0 ? "_" : x.UserValue.ToString())));
        }
        return string.Join(Environment.NewLine, lines);
    }
}
