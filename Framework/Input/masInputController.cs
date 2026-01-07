using System.Collections.Generic;
using System.Diagnostics;
using Godot;


public partial class masInputController
{
    private int                     Controller          = -1;
    private List<masInputComponent> InputComponentList = null;

    public masInputController(int ControllerID)
    {
        bool IsValidControllerID = (ControllerID < 0 || ControllerID > 3);
        Debug.Assert(!IsValidControllerID, "InputControllerID passed to masInputController is not valid");

        Controller         = ControllerID;
        InputComponentList = new List<masInputComponent>();
    }

    public int GetControllerID()        { return (int)Controller;          }
    public int GetInputComponentCount() { return InputComponentList.Count; }

    public masInputComponent GetInputComponent(int InputComponentIdx)
    {
        if(InputComponentIdx < 0 || InputComponentIdx >= InputComponentList.Count)
            return null;
            
        int Index = (InputComponentList.Count - 1) - InputComponentIdx;
        return InputComponentList[Index];
    }

    public void AddInputComponent(masInputComponent InputComponent)
    {
        InputComponentList.Add(InputComponent);
    }
}