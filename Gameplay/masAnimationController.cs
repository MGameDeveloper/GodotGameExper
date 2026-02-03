using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Godot;

public partial class masAnimationController : Node
{
    [Export] private AnimationTree       AnimTree;
    [Export] private masPlayerMovement   MovementComp;
    [Export] private masPlayerController PlayerController;
    

    ///////////////////////////////////////////
    private AnimationNodeStateMachinePlayback LocomotionFSM;
    private AnimationNodeStateMachinePlayback MovementFSM;
    private AnimationNodeStateMachinePlayback JumpFSM;
    private AnimationNodeStateMachinePlayback CrouchFSM;
    private AnimationNodeStateMachinePlayback MotionFSM;


    ///////////////////////////////////////////
    private bool  IsOnSprint = false;


    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    private void OnVelocityChange(Vector3 InVelocity) 
    { 
        float VelocityLength = InVelocity.Length();
        if(VelocityLength != 0f)
        {
            if(IsOnSprint)
                MotionFSM.Travel("Sprint");
            else
                MotionFSM.Travel("Walk");
        }
        else
            MotionFSM.Travel("Idle");
    }

    private void OnSprint(bool IsSprinting)           
    { 
        IsOnSprint = IsSprinting; 
    }

    private void OnJumpStart()
    {
        LocomotionFSM.Travel("JumpFSM");
    }

    private void OnJumpFinish()
    {
        JumpFSM.Travel("Jump_Land");
        
        Callable FinishJump = Callable.From((StringName StateName) =>
        {
            if(StateName == "Jump_Land")
            {
                MovementComp.OnJumpEnd();
                MotionFSM = MovementFSM;
                masDebug.Log(StateName, Colors.Cyan, 20f);
            }
        });

        if(!JumpFSM.IsConnected(AnimationNodeStateMachinePlayback.SignalName.StateFinished, FinishJump))
            JumpFSM.Connect(AnimationNodeStateMachinePlayback.SignalName.StateFinished, FinishJump, (uint)ConnectFlags.Persist);
    }

    private void OnCrouch(bool IsOnCrouch)
    {
        if(IsOnCrouch)
        {
            LocomotionFSM.Travel("CrouchFSM");
            MotionFSM = CrouchFSM;
        }
        else
        {
            LocomotionFSM.Travel("MovementFSM");
            MotionFSM = MovementFSM;
        }
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    public override void _Ready()
    {
        LocomotionFSM = (AnimationNodeStateMachinePlayback)AnimTree.Get("parameters/LocomotionFSM/playback");
        MovementFSM   = (AnimationNodeStateMachinePlayback)AnimTree.Get("parameters/LocomotionFSM/MovementFSM/playback");
        JumpFSM       = (AnimationNodeStateMachinePlayback)AnimTree.Get("parameters/LocomotionFSM/JumpFSM/playback");
        CrouchFSM     = (AnimationNodeStateMachinePlayback)AnimTree.Get("parameters/LocomotionFSM/CrouchFSM/playback");
        MotionFSM     = MovementFSM;

        MovementComp.MovementEvent_OnJumpStart  += OnJumpStart;
        MovementComp.MovementEvent_OnJumpFinish += OnJumpFinish;
        MovementComp.MovementEvent_OnCrouch     += OnCrouch;

        PlayerController.InputEvent_OnMove   += OnVelocityChange;
        PlayerController.InputEvent_OnSprint += OnSprint;
        //PlayerController.OnStepBackInput ;
        //PlayerController.OnRollInput     ;
    }

    public override void _Process(double delta)
    {
        
    }
}