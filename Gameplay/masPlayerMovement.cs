using System;
using System.Collections.Generic;
using Godot;

public partial class masPlayerMovement : Node
{
    [Export] public  CharacterBody3D   Character;
    [Export] public  masPlayerCamera   Camera;

    [Export] private float WalkSpeed         = 20f;
	[Export] private float RunSpeed          = 50f;
    [Export] private float Gravity           = 30f;
	[Export] private float RunStartThreshold = 0.5f;
    [Export] private float TurnSpeed         = 12.0f;

    private Vector2         TargetDirection = Vector2.Zero;
    private float           CurrentSpeed  = 0.0f;
	private float           DeltaTime     = 0.0f;
	private float           RunStartTimer = 0.0f;
    private float           MovementSpeed = 20f;
    private float           RunThreshold  = 1.0f;


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
    private void ProcessMovement(float dt)
    {
        if(TargetDirection.X == 0.0f && TargetDirection.Y == 0.0f)
		    return;
    
        // Temporary to be removed later only for debugging
        MovementSpeed = WalkSpeed;

 		//TOSTUDY: [ using camera forward and right vectors to correctly calculate moving direction] 
        Vector3 CameraForward  = Camera.GetForwardVector();
        Vector3 CameraRight    = Camera.GetRightVector();
        Vector3 TargetVelocity = (CameraRight * TargetDirection.X + CameraForward * TargetDirection.Y).Normalized();
        TargetVelocity.Y       = -Gravity ;	
        TargetVelocity        *= MovementSpeed;

        // TOSTUDY: [ using moving direction calculated above to rotate the mesh correctly ]
		float   TargetYaw   = Mathf.Atan2(TargetVelocity.X, TargetVelocity.Z);
		float   CurrentYaw  = Character.Rotation.Y;
		float   LerpWeight  = 1f - Mathf.Exp(-TurnSpeed * dt);
		float   NewYaw      = Mathf.LerpAngle(CurrentYaw, TargetYaw, LerpWeight);
		Vector3 MeshRot     = Character.Rotation;
		MeshRot.Y           = NewYaw;
    
		//
		Character.Rotation = MeshRot;
		Character.Velocity = TargetVelocity;
		Character.MoveAndSlide();
    }



    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;

        ProcessMovement(dt);
    }
}