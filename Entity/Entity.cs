using Godot;
using System;
using System.Collections.Generic;

public enum TYPE
{
	ROCK,
	PAPER,
	SCISSORS,
}

public partial class Entity : CharacterBody2D
{
	int ATTACK_SPEED = Global.entriesSpeed;
	int ESCAPE_SPEED = Global.entriesSpeed - 20;
	const float ROTATION_AMOUNT = 7.0f;
	public static float size = 20f;
	private TYPE _type;
	private Entity target;

	public TYPE type
	{
		get { return _type; }
		set
		{
			Global.enemies[type].Remove(this);
			_type = value;
			Global.enemies[type].Add(this);
			sprite.Texture = Global.getTexture(this);
		}
	}
	private Sprite2D sprite;

	public void SetRandomType(RandomNumberGenerator random)
	{
		type = new[] { TYPE.ROCK, TYPE.PAPER, TYPE.SCISSORS }[random.Randi() % 3];
	}

	private TYPE winVersus
	{
		get
		{
			return Global.winVersus[type];
		}
	}

	private TYPE loseVersus
	{
		get
		{
			return Global.loseVersus[type];
		}
	}

	public override void _Ready()
	{
		SetPhysicsProcess(false);
		sprite = GetNode<Sprite2D>("Sprite2D");
	}

	private bool findNearest(bool attacking)
	{
		double[] p = new double[2] { Position.X, Position.Y };
		try
		{
			double[] nearestPosition = Global.tree[attacking ? winVersus : loseVersus].Nearest(p).Position;
			target = Global.entityPositions[attacking ? winVersus : loseVersus][new Vector2((float)nearestPosition[0], (float)nearestPosition[1])];

			Velocity = attacking ?
				Position.DirectionTo(target.Position) * ATTACK_SPEED :
				target.Position.DirectionTo(Position) * ESCAPE_SPEED;

		}
		catch (ArgumentException)
		{
			return false;
		}
		catch (KeyNotFoundException)
		{
			return false;
		}
		return true;
	}

	private void CollisionHandler()
	{
		{
			KinematicCollision2D c = GetLastSlideCollision();
			if (c != null)
			{
				GodotObject collider = c.GetCollider();
				if (collider is Entity)
				{
					Entity e = collider as Entity;
					if (e.type == winVersus)
					{
						e.type = type;
					}
				}
			}
		}
	}

	private bool HandleMovement(double delta, bool attacking)
	{

		if (!findNearest(attacking))
		{
			return false;
		}

		Rotation = Mathf.LerpAngle(Rotation, Mathf.Atan2(Velocity.Y, Velocity.X), (float)delta * ROTATION_AMOUNT);
		MoveAndSlide();

		if (attacking)
		{
			CollisionHandler();
		}
		return true;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (HandleMovement(delta, true)) return;
		if (HandleMovement(delta, false)) return;
	}

}
