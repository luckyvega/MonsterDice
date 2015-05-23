using System;

namespace Model
{
	public class MonsterData
	{
		private int id;
		private string name;
		private int[] skillList;
		private int hp;
		private int attack;

		private MonsterData ()
		{
		}

		public static Monster generateFromConfig ()
		{
			Monster m = new Monster ();

			return m;
		}
	}
}
