using System;
using System.Collections.Generic;
using System.Xml;

namespace Model
{
	public class MonsterData
	{
		public static Dictionary<char, string> diceMap = new Dictionary<char, string>()
		{
			{'P', "前进"},
			{'A', "攻击"},
			{'D', "防御"},
			{'M', "魔法"},
			{'T', "陷阱"},
			{'S', "召唤"}
		};

		public int id;
		public string name;
		public int level;
		public string property;
		public int attack;
		public int hp;
		public string diceDistribution;
		public string resourceId;
		public int[] skillList;

		public MonsterData(int monsterDataId, XmlNode node)
		{
			id = monsterDataId;
			name = node["name"].InnerText;
			level = int.Parse(node["level"].InnerText);
			property = node["property"].InnerText;
			attack = int.Parse(node["attack"].InnerText);
			hp = int.Parse(node["hp"].InnerText);
			diceDistribution = node["dice"].InnerText;
			resourceId = node["res"].InnerText;
			// Waiting to fill skill data
			skillList = new int[1];
		}

		public string getDiceDistributionString()
		{
			string ret = "";
			int index = 1;
			foreach (char symbol in diceDistribution.ToCharArray())
			{
				ret += string.Format("{0}-{1} ", index, diceMap[symbol]);
				index++;
			}
			return ret;
		}

		public string getNormalizedResourceId()
		{
			if (resourceId.Contains("."))
				return resourceId;
			else
				return resourceId + ".00";
		}
	}
}
