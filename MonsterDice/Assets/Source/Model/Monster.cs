using System;
using System.Collections.Generic;

namespace Model
{
	public class Monster
	{
		public int monsterDataId;
		private List<Skill> skillList;
		private List<Effect> effectList;
		private int ownerId;
		public float realHp;
		public float realAttack;
		public float realDefense;

		public Monster(MonsterData md, int playerId)
		{
			monsterDataId = md.id;
			skillList = new List<Skill>();
			effectList = new List<Effect>();
			ownerId = playerId;
			realAttack = md.attack;
			realHp = md.hp;
			realDefense = 0;
		}
	}
}
