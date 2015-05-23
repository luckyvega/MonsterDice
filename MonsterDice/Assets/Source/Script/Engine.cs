using System;
using System.Collections.Generic;

using Model;

public enum GameStage
{
	standby,
	summon,
}

public enum BlockType
{
	invalid,
	normal,
	monster,
	obstacle,
}

public class SummonProcess
{
	private HashSet<int> selectedBlockes;
	private int selectedMonster;

	public SummonProcess ()
	{
		selectedBlockes = new HashSet<int> ();
	}

	public void startProcess ()
	{
		selectedBlockes.Clear ();
		Engine.gs = GameStage.summon;
	}

	public bool inProcess ()
	{
		return Engine.gs == GameStage.summon;
	}

	public void endProcess ()
	{
		Engine.gs = GameStage.standby;
	}

	public bool selectMonster (int id)
	{
		// Add some check here
		selectedMonster = id;
		return true;
	}

	public string getMonster ()
	{
		return "094";
	}

	public int getSelectedNum ()
	{
		return selectedBlockes.Count;
	}

	public List<Tuple<int, int>> getSelectedBlockes ()
	{
		List<Tuple<int, int>> ret = new List<Tuple<int, int>> ();
		foreach (int key in selectedBlockes) {
			Tuple<int, int> b = Tool.intToTuple (key);
			ret.Add (new Tuple<int, int> (b.Item1, b.Item2));
		}
		return ret;
	}

	private bool checkBlockAdjacent (Tuple<int, int> b1, Tuple<int, int> b2)
	{
		int xDiff = Math.Abs (b1.Item1 - b2.Item1);
		int yDiff = Math.Abs (b1.Item2 - b2.Item2);
		return xDiff + yDiff == 1;
	}

	public bool selectBlock (Tuple<int, int> b)
	{
		if (selectedBlockes.Count >= Global.summonPlaneSize)
			return false;
		if (!Tool.checkBlockIndex (b))
			return false;
		int key = Tool.tupleToInt (b);
		if (selectedBlockes.Contains (key))
			return false;
		if (selectedBlockes.Count == 0) {
			// should check if this block adjacent to existing blockes
			selectedBlockes.Add (key);
			return true;
		}
		foreach (int _key in selectedBlockes) {
			Tuple<int, int> eb = Tool.intToTuple (_key);
			if (checkBlockAdjacent (eb, b)) {
				selectedBlockes.Add (key);
				return true;
			}
		}
		return false;
	}

	public void unSelectBlock (Tuple<int, int> b)
	{
		int key = Tool.tupleToInt (b);
		if (selectedBlockes.Contains (key))
			selectedBlockes.Remove (key);
	}
}

public class BattleField
{
	private BlockType[,] map;

	public BattleField ()
	{
		map = new BlockType[Global.mapSize, Global.mapSize];
		for (int i = 0; i < Global.mapSize; i++) {
			for (int j = 0; j < Global.mapSize; j++) {
				map [i, j] = BlockType.invalid;
			}
		}
	}

	public string getMapString ()
	{
		string output = "";
		for (int i = 0; i < Global.mapSize; i++) {
			for (int j = 0; j < Global.mapSize; j++) {
				if (map [j, i] == BlockType.normal) {
					output += "1, ";
				} else {
					output += "0, ";
				}
			}
			output += "\n";
		}
		return output;
	}

	public bool setBlockType (Tuple<int, int> b, BlockType bt)
	{
		if (!Tool.checkBlockIndex (b))
			return false;
		map [b.Item1, b.Item2] = bt;
		return true;
	}

	public BlockType getBlockType (Tuple<int, int> b)
	{
		if (!Tool.checkBlockIndex (b))
			return BlockType.invalid;
		else
			return map [b.Item1, b.Item2];
	}

	public List<Tuple<int, int>> getReachableBlock (Tuple<int, int> b, int distance)
	{
		List<Tuple<int, int>> ret = new List<Tuple<int, int>> ();
		if (!Tool.checkBlockIndex (b))
			return ret;
		Queue<Tuple<Tuple<int, int>, int>> blockQueue = new Queue<Tuple<Tuple<int, int>,int>> ();
		HashSet<int> visited = new HashSet<int> ();
		blockQueue.Enqueue (new Tuple<Tuple<int,int>,int> (b, 0));
		int[] diffs = {1, -1};
		while (blockQueue.Count > 0) {
			Tuple<Tuple<int, int>, int> head = blockQueue.Dequeue ();
			int key = Tool.tupleToInt (head.Item1);
			int depth = head.Item2;
			if (visited.Contains (key))
				continue;
			ret.Add (head.Item1);
			visited.Add (key);
			if (depth >= distance)
				continue;
			foreach (int i in diffs) {
				Tuple<int, int> candidate1 = new Tuple<int, int> (head.Item1.Item1 + i, head.Item1.Item2);
				if (getBlockType (candidate1) == BlockType.normal)
					blockQueue.Enqueue (new Tuple<Tuple<int, int>, int> (candidate1, depth + 1));
				Tuple<int, int> candidate2 = new Tuple<int, int> (head.Item1.Item1, head.Item1.Item2 + i);
				if (getBlockType (candidate2) == BlockType.normal)
					blockQueue.Enqueue (new Tuple<Tuple<int, int>, int> (candidate2, depth + 1));
			}
		}
		return ret;
	}
}

public class Engine
{
	public static GameStage gs = GameStage.standby;
	public static SummonProcess sp = new SummonProcess ();
	public static BattleField bf = new BattleField ();
}


