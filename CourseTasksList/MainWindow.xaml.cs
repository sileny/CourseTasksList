using System.Windows;
using CourseTasksList.Models;
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
        viewModel.NotificationRequested += OnNotificationRequested;
        DataContext = viewModel;
    }

    private void OnSaveAndExitRequested(object? sender, EventArgs e) => Close();

    private void OnNotificationRequested(object? sender, InvalidInputEventArgs e)
        => MessageBox.Show(this, e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
}
