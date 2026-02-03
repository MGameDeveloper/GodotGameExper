using Godot;

public partial class masPlayerController : Node
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Input Events
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    [Signal] public delegate void InputEvent_OnMoveEventHandler(Vector3 Velocity);
    [Signal] public delegate void InputEvent_OnSprintEventHandler(bool IsSprinting);
    [Signal] public delegate void InputEvent_OnJumpEventHandler();
    [Signal] public delegate void InputEvent_OnCrouchEventHandler();
    [Signal] public delegate void InputEvent_OnStepBackEventHandler();
    [Signal] public delegate void InputEvent_OnRollEventHandler();


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    private Vector3 Velocity             = Vector3.Zero;
    private float   StartRunThreshold   = 1f;
    private float   StartRunAccumulator = 0f; 
    private float   ElapsedTime         = 0f;


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    // MOVEMENT/MOTION INPUTS
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void MoveForward(float Value)
    {
        Velocity.Z = Value;
        EmitSignal(SignalName.InputEvent_OnMove, Velocity);
    }

    private void MoveRight(float Value)
    {
        Velocity.X = Value; 
        EmitSignal(SignalName.InputEvent_OnMove, Velocity);
    }

    private void OnBeginRun(float Value)
    {
        StartRunAccumulator += Value * ElapsedTime;
        if(StartRunAccumulator >= StartRunThreshold)
            EmitSignal(SignalName.InputEvent_OnSprint, true);

        if(Velocity.Length() == 0.0f)
            EmitSignal(SignalName.InputEvent_OnStepBack);
    }

    private void OnEndRun()
    {
        if(StartRunAccumulator >= StartRunThreshold)
        {
            StartRunAccumulator = 0f;
            EmitSignal(SignalName.InputEvent_OnSprint, false);
        }
        else
            EmitSignal(SignalName.InputEvent_OnRoll);
    }

	private void OnJump()
    {
        EmitSignal(SignalName.InputEvent_OnJump);
    }

	private void OnCrouch()                     { EmitSignal(SignalName.InputEvent_OnCrouch); }
    private void OnStepBack()                   { }
    private void OnDodge()                      { }
    private void OnToggleLock()                 { }  

    
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    // CHANING ITEMS / EQUIPMENTS
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
	private void OnChangeItem()               { }
	private void OnChangeLeftHandEquipment()  { }
	private void OnChangeRightHandEquipment() { }
	private void OnChangeAbilityItem()        { }   


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    // USING CURRENT ITEMS / EQUIPMENTS
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void OnRightHandAction()                   { }
	private void OnLeftHandAction()                    { }
    private void OnRightHandSpecialAction(float Value) { }
    private void OnLeftHandSpecialAction(float Value)  { }
	private void OnUseItem()                           { }
    

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    // ACCESSING MENU, IN GAME SUBMENU, HUD, AND MAPS
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void OnToggleMenu()                        { }
	private void OnToggleMap()                         { }
    private void OnToggleHUD()                         { }
    private void OnToggleSubHUD(float Value)           { }

    
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    // FOR USING CAMERA
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void OnCameraRotateY(float Value)   { } 
    private void OnCameraRotateX(float Value)   { } 
    private void OnCameraMoveRight(float Value) { }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
    public override void _Ready()
    {
        //
        masInputComponent PlayerInputComponent = new masInputComponent("PlayerInputComponent",    false);

        // Register Axes
		PlayerInputComponent.AddAxis("MoveForward",            new masInputAxisEvent(JoyAxis.LeftY, -1.0f),     MoveForward);
		PlayerInputComponent.AddAxis("MoveRight",              new masInputAxisEvent(JoyAxis.LeftX, -1.0f),     MoveRight);
		PlayerInputComponent.AddAxis("BeginRun",               new masInputAxisEvent(JoyButton.B,    1.0f),     OnBeginRun);
        PlayerInputComponent.AddAxis("LeftHandSpecialAction",  new masInputAxisEvent(JoyAxis.TriggerLeft, 1f),  OnLeftHandSpecialAction);
        PlayerInputComponent.AddAxis("RightHandSpecialAction", new masInputAxisEvent(JoyAxis.TriggerRight, 1f), OnRightHandSpecialAction);
        PlayerInputComponent.AddAxis("OnSubMenu",              new masInputAxisEvent(JoyButton.Y, 1f),          OnToggleSubHUD);

        // Register Actions
		PlayerInputComponent.AddAction("EndRun",                   new masInputActionEvent(JoyButton.B,             masInputEventState.Release), OnEndRun);
		PlayerInputComponent.AddAction("RightHandAction",          new masInputActionEvent(JoyButton.RightShoulder, masInputEventState.Press),   OnRightHandAction);
		PlayerInputComponent.AddAction("LeftHandleAction",         new masInputActionEvent(JoyButton.LeftShoulder,  masInputEventState.Press),   OnLeftHandAction);
		PlayerInputComponent.AddAction("Crouch",                   new masInputActionEvent(JoyButton.LeftStick,     masInputEventState.Press),   OnCrouch);
		PlayerInputComponent.AddAction("Jump",                     new masInputActionEvent(JoyButton.A,             masInputEventState.Press),   OnJump);
		PlayerInputComponent.AddAction("UseItem",                  new masInputActionEvent(JoyButton.X,             masInputEventState.Press),   OnUseItem);
		PlayerInputComponent.AddAction("ChangeItem",               new masInputActionEvent(JoyButton.DpadDown,      masInputEventState.Press),   OnChangeItem);
		PlayerInputComponent.AddAction("ChangeLeftHandEquipment",  new masInputActionEvent(JoyButton.DpadLeft,      masInputEventState.Press),   OnChangeLeftHandEquipment);
		PlayerInputComponent.AddAction("ChangeRightHandEquipment", new masInputActionEvent(JoyButton.DpadRight,     masInputEventState.Press),   OnChangeRightHandEquipment);
        PlayerInputComponent.AddAction("ToggleMap",                new masInputActionEvent(JoyButton.Touchpad,      masInputEventState.Press),   OnToggleMap);
		PlayerInputComponent.AddAction("ToggleMenu",               new masInputActionEvent(JoyButton.Start,         masInputEventState.Press),   OnToggleMenu);
        PlayerInputComponent.AddAction("ToggleLock",               new masInputActionEvent(JoyButton.RightStick,    masInputEventState.Press),   OnToggleLock);
		PlayerInputComponent.AddAction("ChangeAbilityItem",        new masInputActionEvent(JoyButton.DpadUp,        masInputEventState.Press),   OnChangeAbilityItem);
        PlayerInputComponent.AddAction("ToggleHUD",                new masInputActionEvent(JoyButton.Y,             masInputEventState.Press),   OnToggleHUD);
        
        
        // Camera Generic Actions
        masInputComponent PlayerCameraInputComponent = new masInputComponent("PlayerCameraInputComponent",    false);
        PlayerCameraInputComponent.AddAxis("OnCameraYaw",   new masInputAxisEvent(JoyAxis.RightX, -1.0f), OnCameraRotateY);
        PlayerCameraInputComponent.AddAxis("OnCameraPitch", new masInputAxisEvent(JoyAxis.RightY, -1.0f), OnCameraRotateX);
        PlayerCameraInputComponent.AddAxis("OnCameraMoveX", new masInputAxisEvent(JoyAxis.LeftX,  -1.0f), OnCameraMoveRight);


        //
        masInputController InputController = masInput.GetInputController(masInputPlayerID.Player_0);
        InputController.AddInputComponent(PlayerInputComponent);
        InputController.AddInputComponent(PlayerCameraInputComponent);
    }

    public override void _Process(double delta)
    {
        ElapsedTime = (float)delta;
    }
}