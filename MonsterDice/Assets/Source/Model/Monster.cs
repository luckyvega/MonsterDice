using System;
using System.Collections.Generic;

namespace Model
{
	public class Monster
	{
		private int monsterDataId;
		private List<Skill> skillList;
		private List<Effect> effectList;
		private int playerId;
		public float realHp;
		public float realAttack;
		public float realDefense;

		public Monster()
		{
		}
	}
}
