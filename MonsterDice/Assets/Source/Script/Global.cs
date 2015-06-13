using UnityEngine;
using System;

public class Global
{
	public static int playerHp = 10;
	public static float mapLeft = -1.5f;
	public static float mapTop = 1.5f;
	public static float blockSize = 0.3f;
	public static int mapSize = 11;
	public static int keyBase = 100; // 10^a and > mapSize
	public static int summonPlaneSize = 4;
	public static string gameManagerName = "game_manager";
	public static Vector3 outOfGamePos = new Vector3(-10, -10, 0);
}

