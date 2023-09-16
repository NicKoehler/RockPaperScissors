using Godot;

public partial class Main : Node2D
{
	private int entriesNum
	{
		get { return Global.entriesNum; }
		set
		{
			Global.entriesNum = value;
			labelNum.Text = $"NUMBER OF ENTITIES: {Global.entriesNum}";
		}
	}

	private int entriesSpeed
	{
		get { return Global.entriesSpeed; }
		set
		{
			Global.entriesSpeed = value;
			labelSpeed.Text = $"SPEED OF ENTITIES: {Global.entriesSpeed}";
		}
	}
	private Label labelNum;
	private Label labelSpeed;
	private Button buttonStart;
	private Button buttonExit;
	private OptionButton optionButton;

	public override void _Ready()
	{
		buttonExit = GetNode<Button>("background/MarginContainer/VBoxContainer/ButtonExit");
		buttonStart = GetNode<Button>("background/MarginContainer/VBoxContainer/ButtonStart");
		labelNum = GetNode<Label>("background/MarginContainer/VBoxContainer/VBoxContainer/Label");
		labelSpeed = GetNode<Label>("background/MarginContainer/VBoxContainer/VBoxContainer2/Label");
		optionButton = GetNode<OptionButton>("background/MarginContainer/VBoxContainer/HBoxContainer/OptionButton");
		CheckButton fullscreenButton = GetNode<CheckButton>("background/MarginContainer/VBoxContainer/HBoxContainer/CheckButton");
		entriesNum = Global.entriesNum;
		entriesSpeed = Global.entriesSpeed;
		GetNode<HSlider>("background/MarginContainer/VBoxContainer/VBoxContainer/HSlider").Value = entriesNum;
		GetNode<HSlider>("background/MarginContainer/VBoxContainer/VBoxContainer2/HSlider").Value = entriesSpeed;

		foreach (var item in Global.resolutions)
		{
			optionButton.AddItem(item.Item1);
		}

		// set resolution and fullscreen
		if (!Global.fullscreen)
		{
			DisplayServer.WindowSetSize(Global.resolutions[Global.resIndex].Item2);
		}
		optionButton.Select(Global.resIndex);
		fullscreenButton.ButtonPressed = DisplayServer.WindowGetMode() == Godot.DisplayServer.WindowMode.Fullscreen;
		optionButton.Disabled = Global.fullscreen;
	}

	public void _OnButtonStartPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/Simulation.tscn");
	}

	public void _OnButtonExitPressed()
	{
		GetTree().Quit();
	}

	public void _OnNumSliderChanged(float value)
	{
		entriesNum = (int)value;
	}

	public void _OnSpeedSliderChanged(float value)
	{
		entriesSpeed = (int)value;
	}

	public void _OnOptionButtonItemSelected(int index)
	{
		Global.resIndex = index;
		DisplayServer.WindowSetSize(Global.resolutions[index].Item2);
	}

	public void _OnCheckButtonToggled(bool full)
	{
		Global.fullscreen = full;
		DisplayServer.WindowSetMode(full ? Godot.DisplayServer.WindowMode.Fullscreen : Godot.DisplayServer.WindowMode.Windowed);
		optionButton.Disabled = full;
	}

}
