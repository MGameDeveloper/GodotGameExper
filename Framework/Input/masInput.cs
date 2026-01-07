using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;


public enum masInputPlayerID
{
    Player_0,
    Player_1,
    Player_2,
    Player_3,

    Player_Count
}

public partial class masInput : Node
{
    ///////////////////////////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////////////////////////
    private struct masInputAxis
    {
        public masInputAxisBinding AxisBinding;
        public float               Scaler;
    }
    

    ///////////////////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////////////////
    static private List<masInputController>    InputControllerList = null;
    static private List<masInputActionBinding> ActionList          = null;
    static private List<masInputAxis>          AxisList            = null;
    static private Queue<InputEvent>           EventQueue          = null;
    

    ///////////////////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////////////////
    private void FillActionList()
    {
        while(EventQueue.Count > 0)
        {
            InputEvent Event = EventQueue.Dequeue();       
            for(int ControllerIdx = 0; ControllerIdx < InputControllerList.Count; ++ControllerIdx)
            {
                masInputController Controller = InputControllerList[ControllerIdx];
                for(int InputComponentIdx = 0; InputComponentIdx < Controller.GetInputComponentCount(); ++InputComponentIdx)
                {
                    masInputComponent InputComponent = Controller.GetInputComponent(InputComponentIdx);
                    for(int ActionIdx = 0; ActionIdx < InputComponent.GetActionCount(); ++ActionIdx)
                    {
                        masInputActionBinding ActionBinding = InputComponent.GetInputActionBinding(ActionIdx);
                        if(ActionBinding.GetEvent().CompareTo(Event))
                            ActionList.Add(ActionBinding);
                    }

                    if(InputComponent.ShouldConsumeEvent())
                        break;
                }
            }
        }
    }

    private void FillAxisList()
    {
        for(int ControllerIdx = 0; ControllerIdx < InputControllerList.Count; ++ControllerIdx)
        {
            masInputController Controller = InputControllerList[ControllerIdx];
            for(int InputComponentIdx = 0; InputComponentIdx < Controller.GetInputComponentCount(); ++InputComponentIdx)
            {
                masInputComponent InputComponent = Controller.GetInputComponent(InputComponentIdx);
                for(int AxisIdx = 0; AxisIdx < InputComponent.GetAxisCount(); ++AxisIdx)
                {
                    masInputAxisBinding AxisBinding = InputComponent.GetInputAxisBinding(AxisIdx);
                    //if(AxisBinding.GetEvent().IsActive(ControllerIdx))
                    //{
                        masInputAxis InputAxis;
                        InputAxis.AxisBinding  = AxisBinding;
                        InputAxis.Scaler       = AxisBinding.GetEvent().GetValue(ControllerIdx);
                        AxisList.Add(InputAxis);
                    //}
                }
            }
        }
    }
    

    ///////////////////////////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////////////////////////
    static public masInputController GetInputController(masInputPlayerID InputPlayerID)
    {
        if(InputPlayerID < masInputPlayerID.Player_0 || InputPlayerID > masInputPlayerID.Player_3)
        {
            Debug.Assert(false, "CreateInputController: No more than 4 controllers are allowed to be created for 4 players maximum");
            return null;
        }
        
        return InputControllerList[(int)InputPlayerID];
    }


    ///////////////////////////////////////////////////////////////////////////////////////////
    // GODOT NODE API
    ///////////////////////////////////////////////////////////////////////////////////////////
    public override void _Ready()
    {
        InputControllerList = new List<masInputController>();
        ActionList          = new List<masInputActionBinding>();
        AxisList            = new List<masInputAxis>();
        EventQueue          = new Queue<InputEvent>();

        for(int ControllerIdx = 0; ControllerIdx < (int)masInputPlayerID.Player_Count; ++ControllerIdx)
            InputControllerList.Add(new masInputController(ControllerIdx));
    }
    
    public override void _Input(InputEvent @event)
    {
        EventQueue.Enqueue(@event);
    }

    public override void _Process(double delta)
    {
        FillActionList();
        FillAxisList();

        foreach (var Action in ActionList)
            Action.Invoke();
        
        foreach(var Axis in AxisList)
            Axis.AxisBinding.Invoke(Axis.Scaler);

        ActionList.Clear();
        AxisList.Clear();
    }
}