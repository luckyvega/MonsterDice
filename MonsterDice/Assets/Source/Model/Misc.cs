using System;

namespace Model
{
	public class Alive
	{
		private PhaseTracker pt;

		public Alive()
		{
			pt = new PhaseTracker();
		}

		private void create(Phase p, int phaseId, int lifetime)
		{
			pt.registerPhase(p, phaseId, lifetime);
		}

		public void createInTurn(int turnId, int duration)
		{
			create(Phase.Turn, turnId, duration);
		}

		public void createInBattle(int battleId, int turnId, int battleDuration, int turnDuration)
		{
			create(Phase.Turn, turnId, turnDuration);
			create(Phase.Battle, battleId, battleDuration);
		}

		public bool checkAlive(Phase p, int phaseId)
		{
			return pt.isAliveAtPhase(p, phaseId);
		}
	}

	public interface IsTriggered
	{
		int getTriggeredCountInGame();

		int getTriggeredCountInTurn();
	}

	public interface IsAlive
	{
		int getBornAt();

		int getLifetime();
	}

	// Not sure if this design is proper.
	// Current idea is that Monster and Skill should implement this interface
	// To do some GUI handling or data clearing jobs.
	// Or maybe just data clearing jobs, GUI handling can be put in an *engine* class
	public interface IsDynamic
	{
		void onTurnStart();

		void onTurnEnd();
	}
}
