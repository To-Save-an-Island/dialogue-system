using Godot;
using System;

public partial class Player : CharacterBody2D
{
    public int Speed = 100;
    private bool movementEnabled = true;

    public void SetMovementEnabled(bool status)
    {
        GD.Print($"Setting movement enabled: {status}"); // DEBUG
        movementEnabled = status;
    }

    public override void _Ready()
    {
        GD.Print("Player is ready."); // DEBUG
        movementEnabled = true; 
    }

    public override void _Process(double delta)
    {
        if (!movementEnabled)
        {
            GD.Print("Movement is disabled."); // DEBUG
            return;
        }
        // Only allow movement if enabled
        if (Input.IsActionPressed("ui_right"))
        {
            Position += new Vector2(Speed * (float)delta, 0);
        }
        if (Input.IsActionPressed("ui_left"))
        {
            Position -= new Vector2(Speed * (float)delta, 0);
        }
        if (Input.IsActionPressed("ui_down"))
        {
            Position += new Vector2(0, Speed * (float)delta);
        }
        if (Input.IsActionPressed("ui_up"))
        {
            Position -= new Vector2(0, Speed * (float)delta);
        }
    }
}
