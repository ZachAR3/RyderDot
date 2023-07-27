using Godot;
using System;

public partial class Wheel : RigidBody2D
{
	[Export] private Marker2D _wheelMarker;


	// public override void _Process(double delta)
	// {
	// 	GlobalPosition = _wheelMarker.GlobalPosition;
	// }


	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
	{
		// Custom gravity for wall / ceiling riding by shifting the direction towards it's normal
		var dt = state.Step;
		if (state.GetContactCount() > 0)
		{
			GD.Print("fuck you");
			var collision = MoveAndCollide(GlobalPosition, true);
			if (collision != null)
			{
				GD.Print(collision.GetDepth());
				if (collision.GetDepth() > 0.06)
				{
					return;
				}
			}
		}
		// var gravity = state.TotalGravity;
		// var velocity = state.LinearVelocity + gravity * dt;
		//
		// state.LinearVelocity = velocity;
		GlobalPosition = _wheelMarker.GlobalPosition;
	}
}	
