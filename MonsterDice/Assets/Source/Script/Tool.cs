using UnityEngine;
using System;

using Model;

public class Tool
{
	public static Tuple<int, int> getBlockIndex(Vector3 pos)
	{
		int x = Mathf.RoundToInt((pos.x - Global.mapLeft) / Global.blockSize);
		int y = Mathf.RoundToInt((Global.mapTop - pos.y) / Global.blockSize);
		return new Tuple<int, int>(x, y);
	}

	public static Vector3 getPosition(Tuple<int, int> index)
	{
		float x = Global.mapLeft + Global.blockSize * index.Item1;
		float y = Global.mapTop - Global.blockSize * index.Item2;
		return new Vector3(x, y, 0f);
	}

	public static int tupleToInt(Tuple<int, int> index)
	{
		return index.Item1 * Global.keyBase + index.Item2;
	}

	public static Tuple<int, int> intToTuple(int key)
	{
		return new Tuple<int, int>(key / Global.keyBase, key % Global.keyBase);
	}

	public static bool checkBlockIndex(Tuple<int, int> index)
	{
		if (index.Item1 < 0 || index.Item1 >= Global.mapSize)
			return false;
		if (index.Item2 < 0 || index.Item2 >= Global.mapSize)
			return false;
		return true;
	}

	public static GameManagerHandle getManagerHandle()
	{
		GameObject manager = GameObject.Find(Global.gameManagerName);
		GameManagerHandle handle = manager.GetComponent<GameManagerHandle>();
		return handle;
	}
}


