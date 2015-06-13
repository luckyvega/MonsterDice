using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
	public class Player
	{
		public int id;
		public int hp;
		public List<int> monsterList;
		public Dictionary<char, int> resourceMap;

		public Player(int playerId)
		{
			id = playerId;
			hp = Global.playerHp;
			monsterList = new List<int>();
			resourceMap = new Dictionary<char, int>();
			foreach(char res in "PADMTS".ToCharArray())
				resourceMap[res] = 0;
		}
	}
}
