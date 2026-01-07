using Godot;
using System;

public partial class masPlayerCamera : Node
{
    // Editor Variables
    [Export] private Camera3D Camera;
    [Export] private Node3D   CameraPivot;
    [Export] private Node3D   CameraTarget;
    [Export] private float    YawSpeed          = 3.0f;
	[Export] private float    PitchSpeed        = 2.0f;
	[Export] private float    PitchLimit        = 60.0f;
	[Export] private float    DampingFactor     = 1.5f;
    [Export] private float    InputDeadZone     = 0.2f;
    [Export] private float    MoveRotationSpeed = 1.2f;
    [Export] private bool     InvertYaw         = false;
    [Export] private bool     InvertPitch       = false;
    
    //private masInputComponent InputComponent;
    private float   InputCameraMoveX = 0.0f;
    private float   InputCameraYaw   = 0.0f;
    private float   InputCameraPitch = 0.0f;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // EXTERNAL API
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public Vector3 GetForwardVector() { return -Camera.GlobalTransform.Basis.Z; }
    public Vector3 GetRightVector()   { return -Camera.GlobalTransform.Basis.X; }
    

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // INPUT FUNCTIONS
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //private void OnCameraYaw(float Value)   { InputCameraYaw   = ( Value != 0.0f ) ? Value : 0.0f; }
    //private void OnCameraPitch(float Value) { InputCameraPitch = ( Value != 0.0f ) ? Value : 0.0f; }
    //private void OnCameraMoveX(float Value) { InputCameraMoveX = ( Value != 0.0f ) ? Value : 0.0f; }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public override void _Ready()
    {
        base._Ready();

        //InputComponent = new masInputComponent("PlayerCamera_InputComponent", false);
        //InputComponent.AddAxis("OnCameraYaw",   new masInputAxisEvent(JoyAxis.RightX, -1.0f), OnCameraYaw);
        //InputComponent.AddAxis("OnCameraPitch", new masInputAxisEvent(JoyAxis.RightY, -1.0f), OnCameraPitch);
        //InputComponent.AddAxis("OnCameraMoveX", new masInputAxisEvent(JoyAxis.LeftX,  -1.0f), OnCameraMoveX);

        //masInputController PlayerController = masInput.GetInputController(masInputPlayerID.Player_0);
        //PlayerController.AddInputComponent(InputComponent);
    }
    
    public override void _Process(double delta)
    {
        float dt = (float)delta;    

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // LEFT ANALOG HANDLING
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        Vector3 TargetPosition     = CameraTarget.GlobalPosition;
        Vector3 CurrentPosition    = CameraPivot.GlobalPosition;
        float   DampingWeight      = 1f - Mathf.Exp(-DampingFactor * dt);
        CurrentPosition.Z          = Mathf.Lerp(CurrentPosition.Z, TargetPosition.Z, DampingWeight);
        CurrentPosition.X          = Mathf.Lerp(CurrentPosition.X, TargetPosition.X, DampingWeight);
     
        if(InputCameraMoveX != 0.0f)
        {
            Vector3 NewRot = CameraPivot.GlobalRotation;
            NewRot.Y      += InputCameraMoveX * MoveRotationSpeed * dt;
            CameraPivot.GlobalRotation = NewRot;
        }
        
        CameraPivot.GlobalPosition = CurrentPosition; 
        Camera.LookAt(CameraTarget.GlobalPosition);
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // RIGHT ANALOG HANDLING
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        if(InputCameraYaw == 0.0f && InputCameraPitch == 0.0f)
            return;

		float TargetYaw     = YawSpeed   * InputCameraYaw   * dt;
		float TargetPitch   = PitchSpeed * InputCameraPitch * dt;
        float LimitPitchRad = Mathf.DegToRad(PitchLimit);

	    if(InvertYaw)
		    TargetYaw = -TargetYaw;
		if(InvertPitch)
		    TargetPitch = -TargetPitch;
        
        Vector3 TargetRotation = CameraPivot.Rotation;
		TargetRotation.Y += TargetYaw;
		TargetRotation.X += TargetPitch;
		TargetRotation.X  = Mathf.Clamp(TargetRotation.X, -LimitPitchRad, LimitPitchRad);

        CameraPivot.Rotation = TargetRotation; 
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
