using Godot;


public class masInputAxisEvent
{
    private InputEvent NativeEvent = null;
    private float      Scaler      = 0.0f;
    private float      DeadZone    = 0.0f;

    public masInputAxisEvent(JoyAxis Axis, float InScaler, float InDeadZone = 0.2f)
    {
        InputEventJoypadMotion GamepadEvent = new InputEventJoypadMotion();
        GamepadEvent.Axis = Axis;

        Scaler      = InScaler;
        DeadZone    = InDeadZone;
        NativeEvent = GamepadEvent;
    }

    public masInputAxisEvent(JoyButton Button, float InScaler)
    {
        InputEventJoypadButton GamepadButton = new InputEventJoypadButton();
        GamepadButton.ButtonIndex = Button;
        GamepadButton.Pressed     = true;

        Scaler      = InScaler;
        NativeEvent = GamepadButton;
    }

    public masInputAxisEvent(Key InKey, float InScaler)
    {
        InputEventKey KeyEvent = new InputEventKey();
        KeyEvent.Keycode = InKey;
        KeyEvent.Pressed = true;

        Scaler = InScaler;
        NativeEvent = KeyEvent;
    }


    public float GetValue(int ControllerID) 
    { 
        if(NativeEvent is InputEventJoypadMotion ThisGamepadAxis)
        {
            float Value = Input.GetJoyAxis(ControllerID, ThisGamepadAxis.Axis);
            if(Value > DeadZone || Value < -DeadZone)
                return Value * Scaler;
        }
        else if(NativeEvent is InputEventJoypadButton ThisGamepadButton)
        {
            if(Input.IsJoyButtonPressed(ControllerID, ThisGamepadButton.ButtonIndex))
                return Scaler;
        }
        else if(NativeEvent is InputEventKey ThisKeyboardKey)
        {
            if(Input.IsKeyPressed(ThisKeyboardKey.Keycode))
                return Scaler;
        }

        return 0.0f;
    }

    //public bool IsActive(int ControllerID)
    //{
    //    if(NativeEvent is InputEventJoypadMotion ThisGamepadAxis)
    //    {
    //        float Value = Input.GetJoyAxis(ControllerID, ThisGamepadAxis.Axis);
    //        if(Value > DeadZone || Value < -DeadZone)
    //        {
    //            AxisValue = Value;
    //            return true;
    //        }
    //    }
    //    else if(NativeEvent is InputEventJoypadButton ThisGamepadButton)
    //    {
    //        if(Input.IsJoyButtonPressed(ControllerID, ThisGamepadButton.ButtonIndex))
    //        {
    //            AxisValue = Scaler;
    //            return true;
    //        }
    //    }
    //    else if(NativeEvent is InputEventKey ThisKeyboardKey)
    //    {
    //        if(Input.IsKeyPressed(ThisKeyboardKey.Keycode))
    //        {
    //            AxisValue = Scaler;
    //            return true; 
    //        }
    //    }

    //    AxisValue = 0.0f;
    //    return false;
    //}
}
