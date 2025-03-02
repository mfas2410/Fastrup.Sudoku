using Fastrup.Sudoku.Domain.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Fastrup.Sudoku.ViewModels;

public class SudokuViewModel : INotifyPropertyChanged
{
    private Board _board;

    public SudokuViewModel()
    {
        NewGameCommand = new Command(NewGame);
        CheckSolutionCommand = new Command(CheckSolution);
        NewGame();
    }

    public Board Board
    {
        get => _board;
        set
        {
            _board = value;
            OnPropertyChanged();
        }
    }

    public ICommand NewGameCommand { get; }
    public ICommand CheckSolutionCommand { get; }

    public event PropertyChangedEventHandler PropertyChanged;

    private void NewGame()
    {
        Board = Board.Generate();
        OnPropertyChanged(nameof(Board));
    }

    private void CheckSolution()
    {
        if (Board.IsSolved)
        {
            Application.Current!.MainPage!.DisplayAlert("Congratulations", "You solved the puzzle!", "OK");
        }
        else
        {
            Application.Current!.MainPage!.DisplayAlert("Try Again", "The solution is not correct.", "OK");
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
