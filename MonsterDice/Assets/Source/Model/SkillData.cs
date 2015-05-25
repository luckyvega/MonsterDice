using System;

namespace Model
{
	public class SkillData
	{
		private int id;
		private string name;
		private int[] expense;

		private SkillData()
		{
		}

		public static SkillData generateFromConfig()
		{
			SkillData s = new SkillData();

			return s;
		}
	}
}
