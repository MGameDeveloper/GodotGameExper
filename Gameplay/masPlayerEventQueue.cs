using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class masPlayerEventQueue : Node
{
    private Queue<String> Events;

    public void AddEvent(String Event)
    { 
        Events.Enqueue(Event);
    }

    public override void _Ready()
    {
        Events = new();
    }

    public override void _PhysicsProcess(double delta)
    {
        String Event;
        while(Events.TryDequeue(out Event))
        {
            masDebug.Log($"[ PLAYER_EVENT ]: {Event}", Colors.White);
        }
    }
}
