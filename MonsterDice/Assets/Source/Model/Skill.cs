using System;

namespace Model
{
	public class Skill: IsTriggered
	{
		private int triggeredInGame;
		private int triggeredInTurn;
		private int skillDataId;

		public Skill ()
		{
			triggeredInGame = 0;
			triggeredInTurn = 0;
		}

		public int getTriggeredCountInGame ()
		{
			return triggeredInGame;
		}

		public int getTriggeredCountInTurn ()
		{
			return triggeredInTurn;
		}

		public void trigger ()
		{
			triggeredInGame += 1;
			triggeredInTurn += 1;
		}
	}
}

