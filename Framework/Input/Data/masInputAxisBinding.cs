using Godot;
using System;
using System.Collections.Generic;

public class masInputAxisBinding
{
    private StringName         Name;
    private masInputAxisEvent  Event;
    private Action<float>      Function;

    public masInputAxisBinding(StringName AxisName, masInputAxisEvent AxisEvent, Action<float> AxisFunction)
    {
        Name     = AxisName;
        Event    = AxisEvent;
        Function = AxisFunction;
    }

    public masInputAxisEvent GetEvent() { return Event; }
    public StringName        GetName()  { return Name ; }

    public void Invoke(float Value)
    {
        Function(Value);
    }
}