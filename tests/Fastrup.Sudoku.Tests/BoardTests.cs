using Fastrup.Sudoku.Domain.Model;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Fastrup.Sudoku.Tests;

public sealed class BoardTests(ITestOutputHelper output)
{
    [Fact]
    public void Test1()
    {
        Board board = Board.Generate();
        board.IsSolved.Should().BeTrue();
        output.WriteLine(board.ToString());
    }

    [Fact]
    public void Test2()
    {
        Board board = Board.Generate();
        board.RemoveNumbers(1);
        board.IsSolved.Should().BeFalse();
        output.WriteLine(board.ToString());
    }
}
