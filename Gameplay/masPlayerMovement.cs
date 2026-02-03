using System;
using Godot;


public partial class masPlayerMovement : Node
{
    [Export] public masPlayerController PlayerController;
    [Export] public  CharacterBody3D Character;
    [Export] public  masPlayerCamera Camera;
    [Export] private float           Gravity          = 9.8f;
    [Export] private float           MeshRotateSpeed  = 12.0f;
    [Export] private bool            UseVelocityForMeshRotation = true;

    [ExportGroup("Jump")]
    [Export] private float JumpHeight    = 10f;
    [Export] private float TimeToPeak    = 0.35f;
    [Export] private float TimeToDescent = 0.35f;
    private float   UpGravity  ;
    private float   DownGravity;
    private Vector3 JumpVelocity = Vector3.Zero;

    [ExportGroup("Crouch")]
    [Export] private float Crouch_WalkSpeed   = 8f;
    [Export] private float Crouch_SprintSpeed = 15f;

    [ExportGroup("Movement")]
    [Export] private float WalkSpeed   = 15f;
    [Export] private float SprintSpeed = 50f;

    private float  CurrentSpeed    = 0.0f;
	private float  DeltaTime       = 0.0f;
	private float  RunStartTimer   = 0.0f;
    public float   JumpSpeed       = 15f;
    private float  RunThreshold    = 1.0f;


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [Signal] public delegate void MovementEvent_OnWalkEventHandler(Vector3 InVelocity);
    [Signal] public delegate void MovementEvent_OnSprintEventHandler(Vector3 InVelocity);
    [Signal] public delegate void MovementEvent_OnCrouchEventHandler(bool IsCrouching);
    [Signal] public delegate void MovementEvent_OnJumpStartEventHandler();
    [Signal] public delegate void MovementEvent_OnJumpFinishEventHandler();

    private Vector3 InputVelocity    = Vector3.Zero;
    public  bool    IsOnJump         = false;
    private bool    IsOnSprint       = false;
    public  bool    IsOnLand         = true;
    private bool    IsOnCrouch       = false;
    private bool    IsOnRoll         = false;
    private Action<float> UpdateMovement = null;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void OnVelocityChange(Vector3 InVelocity)
    {
        InputVelocity.X = InVelocity.X;
        InputVelocity.Z = InVelocity.Z;
    }

    private void OnSprint(bool Value)
    {
        IsOnSprint = Value;
    }

    private float GetMovementSpeed() 
    { 
        if(IsOnSprint)
            return (IsOnCrouch) ? Crouch_SprintSpeed : SprintSpeed;
        
        return (IsOnCrouch) ? Crouch_WalkSpeed : WalkSpeed; 
    }

    private void OnJump()
    {
        if(!IsOnJump)
        {
            EmitSignal(SignalName.MovementEvent_OnJumpStart);

            IsOnJump     = true;
            IsOnLand     = false;
            IsOnCrouch   = false;

            UpGravity    = (2f * JumpHeight) / Mathf.Pow(TimeToPeak, 2f);
            DownGravity  = (2f * JumpHeight) / Mathf.Pow(TimeToDescent, 2f);

            InputVelocity = CalculateVelocityRelativeToCamera(InputVelocity);
            RotateCharacterByVelocity(InputVelocity);

            Vector3 Forward = (Character.GlobalTransform.Basis.Z).Normalized();
            JumpVelocity.X  = Forward.X * GetMovementSpeed();
            JumpVelocity.Z  = Forward.Z * GetMovementSpeed();
            JumpVelocity.Y  = (2.0f * JumpHeight) / TimeToPeak;

            //Vector3 V = -Forward + JumpVelocity; 
            //JumpVelocity = V;
            
            masDebug.Log("MOVEMENT_VELOCITY: "  +  InputVelocity.ToString(), Colors.Gray,  100);
            masDebug.Log("JUMP_VELOCITY:     "  +  JumpVelocity.ToString(),     Colors.White, 100);   

            UpdateMovement = Jump;
        }
    }

    public void OnJumpEnd() 
    { 
        masDebug.Log("FINISH_JUMPPING", Colors.Red);
        IsOnJump       = false;
        UpdateMovement = Move;
    }

    private void OnCrouch()
    {
        IsOnCrouch = !IsOnCrouch;
        if(IsOnCrouch)
            EmitSignal(SignalName.MovementEvent_OnCrouch, true);
        else
            EmitSignal(SignalName.MovementEvent_OnCrouch, false);
    }

    private void OnRoll()
    {
        if(!IsOnRoll)
            IsOnRoll = true;
    }

    private void OnStepBack()
    {
        
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public override void _Ready()
    {
        PlayerController.InputEvent_OnMove     += OnVelocityChange;
        PlayerController.InputEvent_OnSprint   += OnSprint;
        PlayerController.InputEvent_OnCrouch   += OnCrouch;
        PlayerController.InputEvent_OnJump     += OnJump;
        PlayerController.InputEvent_OnRoll     += OnRoll;
        PlayerController.InputEvent_OnStepBack += OnStepBack;

        UpdateMovement = Move;
    }

    private Vector3 CalculateVelocityRelativeToCamera(Vector3 RawVelocity)
    {
        Vector3 OutVelocity = Vector3.Zero;

        Vector3 CameraForward = Camera.GetForwardVector();
        Vector3 CameraRight   = Camera.GetRightVector();
        Vector3 ResultVector  = (CameraRight * RawVelocity.X + CameraForward * RawVelocity.Z).Normalized() * GetMovementSpeed();	
        
        OutVelocity.X = ResultVector.X;
        OutVelocity.Z = ResultVector.Z;
        OutVelocity.Y = RawVelocity.Y;
        
        return OutVelocity;
    }

    private void RotateCharacterByVelocity(Vector3 Velocity)
    {
        if(UseVelocityForMeshRotation)
        {
            // TOSTUDY: [ using moving direction calculated above to rotate the mesh correctly ]
            float   TargetYaw   = Mathf.Atan2(Velocity.X, Velocity.Z);
		    float   CurrentYaw  = Character.Rotation.Y;
		    float   LerpWeight  = 1f - Mathf.Exp(-MeshRotateSpeed * DeltaTime);
		    float   NewYaw      = Mathf.LerpAngle(CurrentYaw, TargetYaw, LerpWeight);
		    Vector3 MeshRot     = Character.Rotation;
		    MeshRot.Y           = NewYaw;
		    Character.Rotation  = MeshRot;
        }
    }

    private void Move(float dt)
    {
        if(InputVelocity.X != 0.0f || InputVelocity.Z != 0.0f)
        {
            InputVelocity = CalculateVelocityRelativeToCamera(InputVelocity);
            RotateCharacterByVelocity(InputVelocity);
        }

        InputVelocity.Y -= Gravity * DeltaTime;

        Character.Velocity  = InputVelocity;
		Character.MoveAndSlide();
    }

    private void Jump(float dt)
    {
        float CurrentGravity = (JumpVelocity.Y > 0) ? UpGravity : DownGravity;
        JumpVelocity.Y      -= CurrentGravity * dt;

        Vector3 TargetVelocity = JumpVelocity;
        if(JumpVelocity.X == 0.0f && JumpVelocity.Z == 0.0f)
        {
            float MoveSpeed = 0.4f;
            TargetVelocity.X = InputVelocity.X * MoveSpeed;
            TargetVelocity.Z = InputVelocity.Z * MoveSpeed;

            TargetVelocity = CalculateVelocityRelativeToCamera(TargetVelocity);
            RotateCharacterByVelocity(TargetVelocity);
        }

        if(!IsOnLand)
        {
            Character.Velocity = TargetVelocity;
            Character.MoveAndSlide();
        }

        if(!IsOnLand && Character.IsOnFloor())
        {
            IsOnLand = true;
            EmitSignal(SignalName.MovementEvent_OnJumpFinish);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        DeltaTime = (float)delta;

        UpdateMovement(DeltaTime);
    }
}

public struct masMovementData
{
    public Vector3 Velocity;
    public bool    bJump;
    public bool    bCrouch;
}