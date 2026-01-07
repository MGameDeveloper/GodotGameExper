
using Godot;

public partial class masPlayerController : Node
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    private masPlayerEventQueue PlayerEventQueue; // 
    private Node3D              CameraRoot;       // current used camera by the player


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    // MOVEMENT/MOTION INPUTS
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void OnMoveForward(float Value)     { if(Value != 0f) PlayerEventQueue.AddEvent("MOVE_FORWARD"); }
    private void OnMoveRight(float Value)       { if(Value != 0f) PlayerEventQueue.AddEvent("MOVE_RIGHT");   }
    private void OnBeginRun(float Value)        { if(Value != 0f) PlayerEventQueue.AddEvent("DODGE_OR_STEPBACK_OR_RUN"); }
    private void OnEndRun()                     { }
	private void OnCrouch()                     { PlayerEventQueue.AddEvent("CROUCH"); }
	private void OnJump()                       { PlayerEventQueue.AddEvent("JUMP");   }
    private void OnStepBack()                   { }
    private void OnDodge()                      { }
    private void OnToggleLock()                 { PlayerEventQueue.AddEvent("TOGGLE_LOCK"); }   

    
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    // CHANING ITEMS / EQUIPMENTS
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
	private void OnChangeItem()               { PlayerEventQueue.AddEvent("CHANGE_CONSUMABLE_ITEM");      }
	private void OnChangeLeftHandEquipment()  { PlayerEventQueue.AddEvent("CHANGE_LEFT_EQUIPMENT");  }
	private void OnChangeRightHandEquipment() { PlayerEventQueue.AddEvent("CHANGE_RIGHT_EQUIPMENT"); }
	private void OnChangeAbilityItem()        { PlayerEventQueue.AddEvent("CHANGE_ABILITY_ITEM");         }   


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    // USING CURRENT ITEMS / EQUIPMENTS
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void OnRightHandAction()                   { PlayerEventQueue.AddEvent("RIGHT_EQUIPMENT_ACTION");         }
	private void OnLeftHandAction()                    { PlayerEventQueue.AddEvent("LEFT_EQUIPMENT_ACTION");          }
    private void OnRightHandSpecialAction(float Value) { if(Value != 0f) PlayerEventQueue.AddEvent("RIGHT_EQUIPMENT_SPECIAL_ACTION"); }
    private void OnLeftHandSpecialAction(float Value)  { if(Value != 0f) PlayerEventQueue.AddEvent("LEFT_EQUIPMENT_SPECIAL_ACTION");  }
	private void OnUseItem()                           { PlayerEventQueue.AddEvent("USE_CONSUMABLEE_ITEM");           }
    

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    // ACCESSING MENU, IN GAME SUBMENU, HUD, AND MAPS
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void OnToggleMenu()                        { PlayerEventQueue.AddEvent("TOGGLE_MAIN_MENU"); }
	private void OnToggleMap()                         { PlayerEventQueue.AddEvent("TOGGLE_MAP");       }
    private void OnToggleHUD()                         { PlayerEventQueue.AddEvent("SHOW_HUD");         }
    private void OnToggleSubHUD(float Value)           { if(Value != 0f) PlayerEventQueue.AddEvent("SHOW_SUB_HUD");     }

    
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    // FOR USING CAMERA
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void OnCameraRotateY(float Value)   { if(Value != 0f) PlayerEventQueue.AddEvent("CAMERA_ROTATE_Y");  } 
    private void OnCameraRotateX(float Value)   { if(Value != 0f) PlayerEventQueue.AddEvent("CAMERA_ROTATE_X");  } 
    private void OnCameraMoveRight(float Value) { if(Value != 0f) PlayerEventQueue.AddEvent("CAMERA_MOVE_RIGHT");}


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
    public override void _Ready()
    {
        PlayerEventQueue = GetNode<masPlayerEventQueue>("masPlayerEventQueue");
        
        //
        masInputComponent PlayerInputComponent = new masInputComponent("PlayerInputComponent",    false);

        // Register Axes
		PlayerInputComponent.AddAxis("MoveForward",            new masInputAxisEvent(JoyAxis.LeftY, -1.0f),     OnMoveForward);
		PlayerInputComponent.AddAxis("MoveRight",              new masInputAxisEvent(JoyAxis.LeftX, -1.0f),     OnMoveRight);
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
}