using Godot;
using System;
using Accord.Math;
using System.Linq;
using Accord.Collections;
using System.Collections.Generic;

public partial class Simulation : Node2D
{
    private Timer timer;
    private Label label;
    private Label winLabel;
    private Label countdown;
    private int PADDING = 10;
    private MarginContainer gameOver;
    private PackedScene packedScene = GD.Load<PackedScene>("res://Entity/Entity.tscn");

    public override void _Ready()
    {
        timer = GetNode<Timer>("Timer");
        label = GetNode<Label>("overlay/Label");
        countdown = GetNode<Label>("overlay/Countdown");
        gameOver = GetNode<MarginContainer>("overlay/MarginContainer");
        winLabel = GetNode<Label>("overlay/MarginContainer/VBoxContainer/Title");

        Vector2 maxSize = GetViewportRect().Size;
        SetupSize(maxSize);
        SpawnEntities(maxSize);
    }

    private void SpawnEntities(Vector2 maxSize)
    {
        Global.enemies[TYPE.ROCK].Clear();
        Global.enemies[TYPE.PAPER].Clear();
        Global.enemies[TYPE.SCISSORS].Clear();

        RandomNumberGenerator random = new RandomNumberGenerator();
        random.Randomize();


        HashSet<Vector2> positions = new HashSet<Vector2>();
        Vector2 pos;
        for (int i = 0; i < Global.entriesNum; i++)
        {
            Entity e = packedScene.Instantiate<Entity>();
            AddChild(e);
            e.SetRandomType(random);
            do
            {
                pos = new Vector2(
                    random.RandfRange(PADDING, maxSize.X - PADDING),
                    random.RandfRange(PADDING, maxSize.Y - PADDING)
                );
            }
            while (positions.Any(p => p.DistanceTo(pos) < Entity.size));

            positions.Add(pos);
            e.Position = pos;
        }
    }

    private void SetupSize(Vector2 maxSize)
    {
        CollisionShape2D up = GetNode<CollisionShape2D>("StaticBody2D/up");
        CollisionShape2D down = GetNode<CollisionShape2D>("StaticBody2D/down");
        CollisionShape2D left = GetNode<CollisionShape2D>("StaticBody2D/left");
        CollisionShape2D right = GetNode<CollisionShape2D>("StaticBody2D/right");
        up.Shape.Set("extents", new Vector2(maxSize.X, 80));
        up.Position = new Vector2(maxSize.X / 2, up.Position.Y);
        left.Position = new Vector2(left.Position.X, maxSize.Y / 2);
        right.Position = new Vector2(maxSize.X + 80, maxSize.Y / 2);
        down.Position = new Vector2(right.Position.X / 2, maxSize.Y + 80);
    }

    public override void _PhysicsProcess(double delta)
    {
        Global.positions[TYPE.ROCK] = Global.enemies[TYPE.ROCK].Select(e => new double[] { e.Position.X, e.Position.Y }).ToArray();
        Global.positions[TYPE.PAPER] = Global.enemies[TYPE.PAPER].Select(e => new double[] { e.Position.X, e.Position.Y }).ToArray();
        Global.positions[TYPE.SCISSORS] = Global.enemies[TYPE.SCISSORS].Select(e => new double[] { e.Position.X, e.Position.Y }).ToArray();

        foreach (var t in new TYPE[] { TYPE.ROCK, TYPE.PAPER, TYPE.SCISSORS })
        {
            Global.entityPositions[t].Clear();
            foreach (var e in Global.enemies[t])
            {
                Global.entityPositions[t].Add(e.Position, e);
            }
        }
        foreach (var t in new TYPE[] { TYPE.ROCK, TYPE.PAPER, TYPE.SCISSORS })
        {
            try
            {
                Global.tree[t] = KDTree.FromData(Global.positions[t]);
            }
            catch (ArgumentException)
            {

            }
        }

    }

    public override void _Process(double _delta)
    {
        if (timer.TimeLeft != 0)
        {
            countdown.Text = $"{timer.TimeLeft:f1}";
        }

        label.Text = (
            $"FPS: {Engine.GetFramesPerSecond()}\n\n" +
            $"Rocks: {Global.enemies[TYPE.ROCK].Count}\n" +
            $"Papers: {Global.enemies[TYPE.PAPER].Count}\n" +
            $"Scissors: {Global.enemies[TYPE.SCISSORS].Count}"
        );
        if (Global.enemies[TYPE.ROCK].Count == Global.entriesNum)
        {
            winLabel.Text = "ROCK WINS!";
            gameOver.Show();
            label.Hide();
        }
        else if (Global.enemies[TYPE.PAPER].Count == Global.entriesNum)
        {
            winLabel.Text = "PAPER WINS!";
            gameOver.Show();
            label.Hide();
        }
        else if (Global.enemies[TYPE.SCISSORS].Count == Global.entriesNum)
        {
            winLabel.Text = "SCISSORS WINS!";
            gameOver.Show();
            label.Hide();
        }
    }

    public void _OnButtonRetartPressed()
    {
        GetTree().ReloadCurrentScene();
    }

    public void _OnButtonExitPressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/Main.tscn");
    }

    public void _OnTimerTimeout()
    {
        foreach (Entity enemy in Global.enemies.Values.SelectMany(e => e))
        {
            enemy.SetPhysicsProcess(true);
        }
        countdown.Hide();
    }
}
