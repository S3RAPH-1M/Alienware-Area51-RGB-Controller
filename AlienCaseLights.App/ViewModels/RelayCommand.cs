using System.Windows.Input;

namespace AlienCaseLights.App.ViewModels;

public sealed class RelayCommand<T> : ICommand
{
    private readonly Action<T> execute;

    public RelayCommand(Action<T> execute)
    {
        this.execute = execute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add { }
        remove { }
    }

    public bool CanExecute(object? parameter)
    {
        return parameter is T;
    }

    public void Execute(object? parameter)
    {
        if (parameter is T typed)
        {
            execute(typed);
        }
    }
}
