# CourseTasksList — задачник

Учебный проект на C# / WPF (.NET 10): настольный дневник задач.
Приложение собирается пошагово — по одному элементу интерфейса.

## Запуск

Открыть `CourseTasksList.sln` в Visual Studio 2026 и нажать F5, либо из консоли:

```powershell
dotnet run --project CourseTasksList
```

## Текущее состояние

Готов каркас проекта и главное окно: по центру сверху заголовок «Задачник» (28 pt),
ниже — пустая область под будущие элементы.

## Структура

```
CourseTasksList.sln
CourseTasksList/
    App.xaml            ресурсы приложения
    App.xaml.cs
    MainWindow.xaml     разметка главного окна
    MainWindow.xaml.cs
```
