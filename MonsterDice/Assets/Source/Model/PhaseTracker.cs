using System;
using System.Collections.Generic;

namespace Model
{
	public enum Phase
	{
		Game,
		Turn,
		Battle
	}

	public class PhaseTracker
	{
		private Dictionary<Phase, Tuple<int, int>> phaseMap;
		public static int infiniteDuration = -1;

		public PhaseTracker ()
		{
			phaseMap = new Dictionary<Phase, Tuple<int, int>> ();
		}

		public void registerPhase (Phase p, int createdAt, int duration)
		{
			if (!phaseMap.ContainsKey (p)) {
				phaseMap.Add (p, new Tuple<int, int> (createdAt, duration));
			}
		}

		public bool isAliveAtPhase (Phase p, int phaseId)
		{
			if (!phaseMap.ContainsKey (p)) {
				return true;
			} else {
				int createdAt = phaseMap [p].Item1;
				int duration = phaseMap [p].Item2;
				if (duration == PhaseTracker.infiniteDuration) {
					return true;
				} else {
					return duration > (phaseId - createdAt);
				}
			}
		}
	}
}

