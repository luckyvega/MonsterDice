using System;
using System.Collections.Generic;

using Model;

public enum GameStage
{
	standby,
	summon,
	move,
	attack
}

public enum BlockType
{
	invalid,
	normal,
	monster_0,
	monster_1,
	obstacle,
}

public class SummonProcess
{
	private HashSet<int> selectedBlockes;
	private int selectedMonster;

	public SummonProcess()
	{
		selectedBlockes = new HashSet<int>();
	}

	public void startProcess()
	{
		selectedBlockes.Clear();
		Engine.gs = GameStage.summon;
	}

	public bool inProcess()
	{
		return Engine.gs == GameStage.summon;
	}

	public void endProcess()
	{
		Engine.gs = GameStage.standby;
	}

	public bool selectMonster(int id)
	{
		// Add some check here
		selectedMonster = id;
		return true;
	}

	public int getMonster()
	{
		// For test, set a static value here
		// selectedMonster = 5;
		return selectedMonster;
	}

	public int getSelectedNum()
	{
		return selectedBlockes.Count;
	}

	public List<Tuple<int, int>> getSelectedBlockes()
	{
		List<Tuple<int, int>> ret = new List<Tuple<int, int>>();
		foreach (int key in selectedBlockes)
		{
			Tuple<int, int> b = Tool.intToTuple(key);
			ret.Add(new Tuple<int, int>(b.Item1, b.Item2));
		}
		return ret;
	}

	private bool checkBlockAdjacent(Tuple<int, int> b1, Tuple<int, int> b2)
	{
		int xDiff = Math.Abs(b1.Item1 - b2.Item1);
		int yDiff = Math.Abs(b1.Item2 - b2.Item2);
		return xDiff + yDiff == 1;
	}

	public bool selectBlock(Tuple<int, int> b)
	{
		if (selectedBlockes.Count >= Global.summonPlaneSize)
			return false;
		if (!Tool.checkBlockIndex(b))
			return false;
		int key = Tool.tupleToInt(b);
		if (selectedBlockes.Contains(key))
			return false;
		if (selectedBlockes.Count == 0)
		{
			// should check if this block adjacent to existing blockes
			selectedBlockes.Add(key);
			return true;
		}
		foreach (int _key in selectedBlockes)
		{
			Tuple<int, int> eb = Tool.intToTuple(_key);
			if (checkBlockAdjacent(eb, b))
			{
				selectedBlockes.Add(key);
				return true;
			}
		}
		return false;
	}

	public void unSelectBlock(Tuple<int, int> b)
	{
		int key = Tool.tupleToInt(b);
		if (selectedBlockes.Contains(key))
			selectedBlockes.Remove(key);
	}
}

public class MoveProcess
{
	private List<Tuple<int, int>> moveRegion;
	private Dictionary<int, int> distanceMap;

	public MoveProcess()
	{
		moveRegion = new List<Tuple<int, int>>();
		distanceMap = new Dictionary<int, int>();
	}

	public void startProcess(Tuple<int, int> start, int distance)
	{
		moveRegion.Clear();
		distanceMap.Clear();
		foreach (Tuple<Tuple<int, int>, int> tuple in Engine.bf.getReachableBlock(start, distance))
		{
			moveRegion.Add(tuple.Item1);
			distanceMap[Tool.tupleToInt(tuple.Item1)] = tuple.Item2;
		}
		Engine.gs = GameStage.move;
	}

	public bool inProcess()
	{
		return Engine.gs == GameStage.move;
	}

	public void endProcess()
	{
		Engine.gs = GameStage.standby;
	}

	public List<Tuple<int, int>> getMoveRegion()
	{
		return moveRegion;
	}

	public int getDistance(Tuple<int, int> index)
	{
		int key = Tool.tupleToInt(index);
		if (distanceMap.ContainsKey(key))
			return distanceMap[key];
		else
			return -1;
	}
}

public class AttackProcess
{
	private Tuple<int, int> attacker;
	private Tuple<int, int> attackee;

	public AttackProcess()
	{
		attacker = null;
		attackee = null;
	}

	public void startProces(Tuple<int, int> position)
	{
		Engine.gs = GameStage.attack;
		attacker = position;
	}

	public bool canAttack(Tuple<int, int> target)
	{
		if (!inProcess())
			return false;
		if(Engine.bf.checkTarget(attacker, target, 1))
		{
			attackee = target;
			return true;
		}
		return false;
	}

	public Tuple<int, int> getAttackerPosition()
	{
		return attacker;
	}

	public Tuple<int, int> getAttackeePosition()
	{
		return attackee;
	}

	public string getAttackAnimateTrigger()
	{
		string[] triggers = { "moveToRight", "moveToDown", "moveToUp", "moveToLeft" };
		int xdiff = attacker.Item1 - attackee.Item1;
		int ydiff = attacker.Item2 - attackee.Item2;
		int triggerIndex = (xdiff * 3 + ydiff + 3) / 2;
		// Weird code
		// Since monster object for player 0 is mirrored, need to switch left and right
		if(Engine.bf.getBlockType(attacker) == BlockType.monster_0)
		{
			if (triggerIndex == 0)
				triggerIndex = 3;
			else if (triggerIndex == 3)
				triggerIndex = 0;
		}
		return triggers[triggerIndex];
	}

	public bool inProcess()
	{
		return Engine.gs == GameStage.attack;
	}

	public void endProcess()
	{
		Engine.gs = GameStage.standby;
		attacker = null;
		attackee = null;
	}
}

public class BattleField
{
	private BlockType[,] map;

	public BattleField()
	{
		map = new BlockType[Global.mapSize, Global.mapSize];
		for (int i = 0; i < Global.mapSize; i++)
		{
			for (int j = 0; j < Global.mapSize; j++)
			{
				map[i, j] = BlockType.invalid;
			}
		}
	}

	public string getMapString()
	{
		string output = "";
		for (int i = 0; i < Global.mapSize; i++)
		{
			for (int j = 0; j < Global.mapSize; j++)
			{
				if (map[j, i] == BlockType.normal)
				{
					output += "1, ";
				}
				else
				{
					output += "0, ";
				}
			}
			output += "\n";
		}
		return output;
	}

	public bool setBlockType(Tuple<int, int> b, BlockType bt)
	{
		if (!Tool.checkBlockIndex(b))
			return false;
		map[b.Item1, b.Item2] = bt;
		return true;
	}

	public bool checkTarget(Tuple<int, int> b1, Tuple<int, int> b2, int distance)
	{
		BlockType type1 = getBlockType(b1);
		BlockType type2 = getBlockType(b2);
		if ((type1 == BlockType.monster_0 && type2 == BlockType.monster_1) || (type1 == BlockType.monster_1 && type2 == BlockType.monster_0))
		{
			if (distance == -1)
				return true;
			int realDistance = Math.Abs(b1.Item1 - b2.Item1) + Math.Abs(b1.Item2 - b2.Item2);
			if (realDistance <= distance)
				return true;
			else
				return false;
		}
		return false;
	}

	public BlockType getBlockType(Tuple<int, int> b)
	{
		if (!Tool.checkBlockIndex(b))
			return BlockType.invalid;
		else
			return map[b.Item1, b.Item2];
	}

	public List<Tuple<Tuple<int, int>, int>> getReachableBlock(Tuple<int, int> b, int distance)
	{
		// TODO
		// To support passing through teammate monsters, treat block occupied by teammate monster
		// as normal block, but remove them from the result before returning
		// Done
		List<Tuple<Tuple<int, int>, int>> ret = new List<Tuple<Tuple<int, int>, int>>();
		if (!Tool.checkBlockIndex(b))
			return ret;
		BlockType bt = getBlockType(b);
		if (bt != BlockType.monster_0 && bt != BlockType.monster_1)
			return ret;
		Queue<Tuple<Tuple<int, int>, int>> blockQueue = new Queue<Tuple<Tuple<int, int>, int>>();
		HashSet<int> visited = new HashSet<int>();
		blockQueue.Enqueue(new Tuple<Tuple<int, int>, int>(b, 0));
		int[] diffs = { 1, -1 };
		while (blockQueue.Count > 0)
		{
			Tuple<Tuple<int, int>, int> head = blockQueue.Dequeue();
			int key = Tool.tupleToInt(head.Item1);
			int depth = head.Item2;
			if (visited.Contains(key))
				continue;
			if (getBlockType(head.Item1) == BlockType.normal)
				ret.Add(head);
			visited.Add(key);
			if (depth >= distance)
				continue;
			foreach (int i in diffs)
			{
				Tuple<int, int> candidate1 = new Tuple<int, int>(head.Item1.Item1 + i, head.Item1.Item2);
				if (getBlockType(candidate1) == BlockType.normal || getBlockType(candidate1) == bt)
					blockQueue.Enqueue(new Tuple<Tuple<int, int>, int>(candidate1, depth + 1));
				Tuple<int, int> candidate2 = new Tuple<int, int>(head.Item1.Item1, head.Item1.Item2 + i);
				if (getBlockType(candidate2) == BlockType.normal || getBlockType(candidate2) == bt)
					blockQueue.Enqueue(new Tuple<Tuple<int, int>, int>(candidate2, depth + 1));
			}
		}
		return ret;
	}
}

public class Engine
{
	public static GameStage gs = GameStage.standby;
	public static SummonProcess sp = new SummonProcess();
	public static MoveProcess mp = new MoveProcess();
	public static AttackProcess ap = new AttackProcess();
	public static BattleField bf = new BattleField();
}


