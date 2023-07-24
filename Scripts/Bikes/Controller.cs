using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;

public partial class Controller : RigidBody2D
{
	[ExportGroup("Stats")]
	[Export] private float _maxSpeed = 25;
	[Export] private float _acceleration = 10;
	[Export] private float _breakingForce = 10;
	[Export] private float _reverseMaxSpeed = -50;

	[ExportGroup("Components")]
	[Export] private Array<RigidBody2D> _wheels;
	[Export] private RayCast2D _floorCast;

	private float _speed;
	private bool _applyDownforce;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_maxSpeed *= 10;
		_reverseMaxSpeed *= 10;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// TODO remove for actual release
		if (Input.IsActionJustPressed("ui_cancel"))
		{
			Position = new Vector2(Position.X, Position.Y - 10);
			Rotation = 0;
		}
		
		if (Input.IsActionPressed("Throttle"))
		{
			_applyDownforce = _wheels.Any(wheel => wheel.GetContactCount() > 0) && _floorCast.IsColliding();
			_speed = Mathf.Lerp(_speed, _maxSpeed, _acceleration * (float)delta);
		}
		else
		{
			// Applies counter torque to slowdown the vehicle when not pressing on the gas if it is moving forward
			var torqueTarget = Mathf.Round(_wheels.First().AngularVelocity) > 0 ? _reverseMaxSpeed : 0;
			_speed = Mathf.Lerp(_speed, torqueTarget, _breakingForce * (float)delta);
			_applyDownforce = false;
		}
	}


	public override void _PhysicsProcess(double delta)
	{
		foreach (var wheel in _wheels)
		{
			wheel.AngularVelocity = _speed;
		}
	}
	
	
	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
	{
		// Custom gravity for wall / ceiling riding by shifting the direction towards it's normal
		var dt = state.Step;
		var gravity = _floorCast.IsColliding() && _applyDownforce
			? GlobalPosition.DirectionTo(_floorCast.GetCollisionPoint()).Normalized() * state.TotalGravity.Length()
			: state.TotalGravity;
		var velocity = state.LinearVelocity + gravity * dt;

		state.LinearVelocity = velocity;
	}

}
