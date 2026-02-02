using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

public partial class masPlayerMovement : Node
{
    [Export] public masPlayerController PlayerController;
    [Export] public  CharacterBody3D Character;
    [Export] public  masPlayerCamera Camera;
    [Export] private float           Gravity    = 9.8f;
    [Export] private float           TurnSpeed  = 12.0f;
    
    [ExportGroup("Jump")]
    [Export] private float JumpHeight    = 10f;
    [Export] private float TimeToPeak    = 0.35f;
    [Export] private float TimeToDescent = 0.25f;
    private float JumpVelocity;
    private float UpGravity;
    private float DownGravity;

    private float  CurrentSpeed    = 0.0f;
	private float  DeltaTime       = 0.0f;
	private float  RunStartTimer   = 0.0f;
    public float   MovementSpeed   = 15f;
    public float   JumpSpeed       = 10f;
    private float  RunThreshold    = 1.0f;


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [Signal] public delegate void OnJumpStartEventHandler();
    [Signal] public delegate void OnJumpFinishEventHandler();

    private Vector3 Velocity   = Vector3.Zero;
    public  bool    IsOnJump   = false;
    public  bool    IsOnLand   = true;
    private bool    IsOnCrouch = false;
    private bool    IsOnRoll   = false;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void OnVelocityChange(Vector3 InVelocity)
    {
        Velocity.X = InVelocity.X;
        Velocity.Z = InVelocity.Z;
    }

    private void OnSprint(bool Value)
    {
        if(Value)
            MovementSpeed = 50f;
        else
            MovementSpeed = 12f;
    }

    private void OnJump()
    {
        if(Character.IsOnFloor() && !IsOnJump)
        {
            EmitSignal(SignalName.OnJumpStart);

            IsOnJump     = true;
            IsOnLand     = false;
            JumpVelocity = (2.0f * JumpHeight) / TimeToPeak;
            UpGravity    = (2.0f * JumpHeight) / Mathf.Pow(TimeToPeak, 2.0f);
            DownGravity  = (2.0f * JumpHeight) / Mathf.Pow(TimeToDescent, 2.0f);

            Velocity.Y = JumpVelocity;
        }
    }

    public void OnJumpEnd() 
    { 
        IsOnJump = false; 
    }

    private void Crouch()
    {
        IsOnCrouch = !IsOnCrouch;
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
        PlayerController.OnMoveInput     += OnVelocityChange;
        PlayerController.OnSprintInput   += OnSprint;
        PlayerController.OnJumpInput     += OnJump;
        PlayerController.OnRollInput     += OnRoll;
        PlayerController.OnStepBackInput += OnStepBack;
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;

        Vector3 TargetDirection = Velocity;
        Vector3 TargetVelocity  = Vector3.Zero;

        if(Velocity.X != 0.0f || Velocity.Z != 0.0f)
 		{
            //TOSTUDY: [ using camera forward and right vectors to correctly calculate moving direction] 
            Vector3 CameraForward = Camera.GetForwardVector();
            Vector3 CameraRight   = Camera.GetRightVector();
            TargetVelocity        = (CameraRight * TargetDirection.X + CameraForward * TargetDirection.Z).Normalized() * MovementSpeed;	
    
            // TOSTUDY: [ using moving direction calculated above to rotate the mesh correctly ]
		    float   TargetYaw   = Mathf.Atan2(TargetVelocity.X, TargetVelocity.Z);
		    float   CurrentYaw  = Character.Rotation.Y;
		    float   LerpWeight  = 1f - Mathf.Exp(-TurnSpeed * dt);
		    float   NewYaw      = Mathf.LerpAngle(CurrentYaw, TargetYaw, LerpWeight);
		    Vector3 MeshRot     = Character.Rotation;
		    MeshRot.Y           = NewYaw;
            
		    //
		    Character.Rotation = MeshRot;
        }

        float CurrentGravity = (Velocity.Y > 0) ? UpGravity : DownGravity;
        if(!IsOnJump)
            CurrentGravity = Gravity;
        Velocity.Y -= CurrentGravity * dt;

        TargetVelocity.Y   = Velocity.Y;
		Character.Velocity = TargetVelocity;
		Character.MoveAndSlide(); 

        if(!IsOnLand && Character.IsOnFloor())
        {
            IsOnLand = true;
            EmitSignal(SignalName.OnJumpFinish);
        }
    }
}