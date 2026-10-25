using System.Windows;
using CourseTasksList.Services;
using CourseTasksList.ViewModels;

namespace CourseTasksList;

/// <summary>Главное окно приложения.</summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var viewModel = new MainViewModel(new JsonTaskStorage());
        viewModel.SaveAndExitRequested += OnSaveAndExitRequested;
        DataContext = viewModel;
    }

    private void OnSaveAndExitRequested(object? sender, EventArgs e) => Close();
}
