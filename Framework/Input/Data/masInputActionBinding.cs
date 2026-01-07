using Godot;
using System;
using System.Reflection.Metadata.Ecma335;


public struct masInputActionBinding
{
    private StringName          Name;
    private masInputActionEvent Event;
    private Action              Function;

    public masInputActionBinding(StringName ActionName, masInputActionEvent ActionEvent, Action ActionFunction)
    {
        Name     = ActionName;
        Event    = ActionEvent;
        Function = ActionFunction;
    }

    public masInputActionEvent GetEvent() { return Event; }
    public StringName          GetName()  { return Name;  }
    
    public void Invoke()
    {
        Function();
    }
}