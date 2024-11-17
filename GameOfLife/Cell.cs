using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GameOfLife;

public class Cell : INotifyPropertyChanged
{
    private bool _state;

    public bool State
    {
        get => _state;
        set
        {
            _state = value;
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("State"));
            }
        }
    }

    public static Cell Alive()
    {
        return new Cell { State = true };
    }

    public static Cell Dead()
    {
        return new Cell { State = false };
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
