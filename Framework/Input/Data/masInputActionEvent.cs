using Godot;

public enum masInputEventState
{
    Press,
    Release
}

public class masInputActionEvent
{
    private InputEvent  NativeEvent;

    public masInputActionEvent(JoyButton Button, masInputEventState State)
    {
        InputEventJoypadButton Gamepad = new InputEventJoypadButton();
        Gamepad.ButtonIndex = Button;
        Gamepad.Pressed     = (State == masInputEventState.Press) ? true : false;

        NativeEvent = Gamepad;
    }

    public masInputActionEvent(Key KeyCode, masInputEventState State)
    {
        InputEventKey Keyboard = new InputEventKey();
        Keyboard.Keycode = KeyCode;
        Keyboard.Pressed = (State == masInputEventState.Press) ? true : false;
        
        NativeEvent = Keyboard;
    }

    public masInputActionEvent(MouseButton Button, masInputEventState State)
    {
        InputEventMouseButton Mouse = new InputEventMouseButton();
        Mouse.ButtonIndex = Button;
        Mouse.Pressed     = (State == masInputEventState.Press) ? true : false;

        NativeEvent = Mouse;
    }

    public bool CompareTo(InputEvent Event)
    {
        if(NativeEvent is InputEventJoypadButton ThisGamepadButton && Event is InputEventJoypadButton GamepadButton)
        {
            return (ThisGamepadButton.ButtonIndex == GamepadButton.ButtonIndex && ThisGamepadButton.Pressed == GamepadButton.Pressed);
        }
        else if(NativeEvent is InputEventKey ThisKeyboardKey && Event is InputEventKey KeyboardKey)
        {
            return (ThisKeyboardKey.Keycode == KeyboardKey.Keycode && ThisKeyboardKey.Pressed == KeyboardKey.Pressed);
        }
        else if(NativeEvent is InputEventMouseButton ThisMouseButton && Event is InputEventMouseButton MouseButton)
        {
            return (ThisMouseButton.ButtonIndex == MouseButton.ButtonIndex && ThisMouseButton.Pressed == MouseButton.Pressed);
        }

        return false;
    }
}
