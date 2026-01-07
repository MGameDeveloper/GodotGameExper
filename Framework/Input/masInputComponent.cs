using System;
using System.Collections.Generic;
using Godot;


public partial class masInputComponent
{
    private List<masInputActionBinding> ActionBindingList;
    private List<masInputAxisBinding>   AxisBindingList;
    private StringName                  Name;
    private bool                        ConsumeEvent;
    
    public masInputComponent(StringName ComponentName, bool ShouldConsumeEvent)
    {
        ActionBindingList = new List<masInputActionBinding>();
        AxisBindingList   = new List<masInputAxisBinding>();
        Name              = ComponentName;
        ConsumeEvent      = ShouldConsumeEvent;
    }

    public int                   GetActionCount()               { return ActionBindingList.Count; }
    public int                   GetAxisCount()                 { return AxisBindingList.Count;   }
    public masInputActionBinding GetInputActionBinding(int idx) { return ActionBindingList[idx];  }
    public masInputAxisBinding   GetInputAxisBinding(int idx)   { return AxisBindingList[idx];    }
    public bool                  ShouldConsumeEvent()           { return ConsumeEvent;            }

    public void AddAction(StringName ActionName, masInputActionEvent ActionEvent, Action ActionFunction)
    {
        ActionBindingList.Add(new masInputActionBinding(ActionName, ActionEvent, ActionFunction));
    }

    public void AddAxis(StringName AxisName, masInputAxisEvent AxisEvent, Action<float> AxisFunction)
    {
        AxisBindingList.Add(new masInputAxisBinding(AxisName, AxisEvent, AxisFunction));
    }
}