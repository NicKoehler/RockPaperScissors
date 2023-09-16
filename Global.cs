using Godot;
using Accord.Collections;
using System.Collections.Generic;

public partial class Global : Node
{
	public static bool fullscreen = false;
	internal static int resIndex = 2;
	internal static int entriesNum = 100;
	internal static int entriesSpeed = 80;
	public static Dictionary<TYPE, KDTree> tree = new Dictionary<TYPE, KDTree>();
	public static Dictionary<TYPE, double[][]> positions = new Dictionary<TYPE, double[][]>();
	public static Dictionary<TYPE, Dictionary<Vector2, Entity>> entityPositions = new Dictionary<TYPE, Dictionary<Vector2, Entity>>();
	public static Dictionary<TYPE, TYPE> winVersus = new Dictionary<TYPE, TYPE>();
	public static Dictionary<TYPE, TYPE> loseVersus = new Dictionary<TYPE, TYPE>();
	private static Dictionary<TYPE, Texture2D> textures = new Dictionary<TYPE, Texture2D>();
	public static Dictionary<TYPE, HashSet<Entity>> enemies = new Dictionary<TYPE, HashSet<Entity>>();
	public static List<(string, Vector2I)> resolutions = new List<(string, Vector2I)>();
	static Global()
	{
		winVersus.Add(TYPE.ROCK, TYPE.SCISSORS);
		winVersus.Add(TYPE.PAPER, TYPE.ROCK);
		winVersus.Add(TYPE.SCISSORS, TYPE.PAPER);

		loseVersus.Add(TYPE.ROCK, TYPE.PAPER);
		loseVersus.Add(TYPE.PAPER, TYPE.SCISSORS);
		loseVersus.Add(TYPE.SCISSORS, TYPE.ROCK);

		enemies.Add(TYPE.ROCK, new HashSet<Entity>());
		enemies.Add(TYPE.PAPER, new HashSet<Entity>());
		enemies.Add(TYPE.SCISSORS, new HashSet<Entity>());

		entityPositions.Add(TYPE.ROCK, new Dictionary<Vector2, Entity>());
		entityPositions.Add(TYPE.PAPER, new Dictionary<Vector2, Entity>());
		entityPositions.Add(TYPE.SCISSORS, new Dictionary<Vector2, Entity>());

		textures.Add(TYPE.ROCK, GD.Load<Texture2D>("res://assets/rock.png"));
		textures.Add(TYPE.PAPER, GD.Load<Texture2D>("res://assets/paper.png"));
		textures.Add(TYPE.SCISSORS, GD.Load<Texture2D>("res://assets/scissors.png"));

		resolutions.Add(("2560x1440", new Vector2I(2560, 1440)));
		resolutions.Add(("1920x1080", new Vector2I(1920, 1080)));
		resolutions.Add(("1366x768", new Vector2I(1366, 768)));
	}

	public static void AddEnemy(Entity e)
	{
		enemies[e.type].Add(e);
	}

	public static Texture2D getTexture(Entity e)
	{
		return textures[e.type];
	}

	public static void Clear()
	{
		enemies[TYPE.ROCK].Clear();
		enemies[TYPE.PAPER].Clear();
		enemies[TYPE.SCISSORS].Clear();
	}
}
