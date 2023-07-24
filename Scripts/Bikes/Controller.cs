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
	[Export] private int _downForce = 10;
	
	[ExportGroup("Components")]
	[Export] private Array<RigidBody2D> _wheels;
	[Export] private RayCast2D _floorCast;

	private float _speed;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_downForce *= 10000;
		_maxSpeed *= 10;
		_reverseMaxSpeed *= 10;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("Throttle"))
		{
			_speed = Mathf.Lerp(_speed, _maxSpeed, _acceleration * (float)delta);
		}
		else
		{
			// Applies counter torque to slowdown the vehicle when not pressing on the gas if it is moving forward
			var torqueTarget = Mathf.Round(_wheels.First().AngularVelocity) > 0 ? _reverseMaxSpeed : 0;
			_speed = Mathf.Lerp(_speed, torqueTarget, _breakingForce * (float)delta);
		}
	}


	public override void _PhysicsProcess(double delta)
	{
		foreach (var wheel in _wheels)
		{
			// Applies stick for wall/ceiling riding
			if (wheel.GetContactCount() > 0  && _floorCast.IsColliding() && Input.IsActionPressed("Throttle"))
			{
				wheel.ApplyCentralForce(-_floorCast.GetCollisionNormal() * _downForce);
			}

			wheel.AngularVelocity = _speed;
		}
		

	}

}
