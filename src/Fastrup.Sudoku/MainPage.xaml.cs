using Fastrup.Sudoku.Views;

namespace Fastrup.Sudoku;

public partial class MainPage : ContentPage
{
    private int count = 0;

    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnStartSudokuClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SudokuPage());
    }
}
