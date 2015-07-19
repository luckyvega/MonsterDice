using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Xml;

using Model;
using Newtonsoft.Json;

public class GameManagerHandle : MonoBehaviour
{
	private Dictionary<int, GameObject> bottomBlockMap;
	private Dictionary<int, GameObject> topBlockMap;
	private Dictionary<string, GameObject> menuMap;
	// monsterMap contains GameObject reference of monster_container,
	// use GetComponentInChildren to access monster
	private Dictionary<int, GameObject> monsterMap;
	private List<Player> playerList;
	private List<Monster> monsterList;
	private List<MonsterData> monsterDataList;
	private int[] standbyMonsterIds;
	private int currentPlayerId;
	private GameObject mask;
	// Cool feature, T? shorts for Nullable<T>, otherwise value type (like int, bool) cannot be assigned null
	private Vector3? focusedMonsterPos; // the position of the current focused monster
	public GameObject bottomBlockPrefab;
	public GameObject topBlockPrefab;
	public GameObject buttonPrefab;
	public GameObject monsterPrefab;

	private PopupType popupType = PopupType.none;
	// Such ugly implemetation is just for test!!
	private DialogPopup dialogPopup = new DialogPopup();

	private int tmpLoop = 0;
	public void tmpTriggerAnimation()
	{
		GameObject container = GameObject.Find("monster_container");
		string[] directions = { "moveToLeft", "moveToUp", "moveToRight", "moveToDown" };
		container.GetComponent<Animator>().SetTrigger(directions[tmpLoop]);
		tmpLoop = (tmpLoop + 1) % 4;
	}


	#region block_related
	private void addBlock(Tuple<int, int> index, Dictionary<int, GameObject> map, GameObject prefab)
	{
		int key = Tool.tupleToInt(index);
		if (map.ContainsKey(key))
			return;
		GameObject block = Instantiate(prefab, Tool.getPosition(index), Quaternion.identity) as GameObject;
		map.Add(key, block);
	}

	public void addBottomBlock(Tuple<int, int> index)
	{
		addBlock(index, bottomBlockMap, bottomBlockPrefab);
		GameObject block = getBottomBlock(index);
		block.GetComponent<SpriteRenderer>().sortingOrder = 1;
	}

	public void addTopBlock(Tuple<int, int> index)
	{
		addBlock(index, topBlockMap, topBlockPrefab);
		GameObject block = getTopBlock(index);
		block.GetComponent<SpriteRenderer>().sortingOrder = 2;
		Engine.bf.setBlockType(index, BlockType.normal);
	}

	private GameObject getBlock(Tuple<int, int> index, Dictionary<int, GameObject> map)
	{
		int key = Tool.tupleToInt(index);
		if (!map.ContainsKey(key))
			return null;
		return map[key];
	}

	public GameObject getBottomBlock(Tuple<int, int> index)
	{
		return getBlock(index, bottomBlockMap);
	}

	public GameObject getTopBlock(Tuple<int, int> index)
	{
		return getBlock(index, topBlockMap);
	}
	#endregion

	#region monster_menu_related
	private void monsterMenuHandle(string name)
	{
		if (focusedMonsterPos == null)
		{
			Debug.LogError("No monster focused on!");
			return;
		}
		if (name == "attack")
		{
			if (playerList[currentPlayerId].resourceMap['A'] == 0)
			{
				dialogPopup.setContent("攻击资源不足");
				popupType = PopupType.dialog;
			}
			else
			{
				Tuple<int, int> index = Tool.getBlockIndex(focusedMonsterPos.Value);
				Engine.ap.startProces(index);
			}
			// GameObject monster = monsterMap[Tool.tupleToInt(index)];
			// monster.GetComponent<Animator>().SetTrigger("moveToLeft");
		}
		if (name == "move")
		{
			if (playerList[currentPlayerId].resourceMap['P'] == 0)
			{
				dialogPopup.setContent("移动资源不足");
				popupType = PopupType.dialog;
			}
			else
			{
				Tuple<int, int> index = Tool.getBlockIndex(focusedMonsterPos.Value);
				Engine.mp.startProcess(index, playerList[currentPlayerId].resourceMap['P']);
				List<Tuple<int, int>> blockList = Engine.mp.getMoveRegion();
				if (blockList.Count == 0)
				{
					dialogPopup.setContent("无法移动");
					popupType = PopupType.dialog;
					Engine.mp.endProcess();
				}
				else
				{
					foreach (Tuple<int, int> block in blockList)
					{
						GameObject topBlock = getTopBlock(block);
						Color c = topBlock.GetComponent<SpriteRenderer>().color;
						c.a = 0.5f;
						topBlock.GetComponent<SpriteRenderer>().color = c;
						topBlock.GetComponent<TopBlockHandle>().isEnabled = true;
					}
				}
			}
		}
		hideAllMenu();
	}

	public void showMonsterMenu(Vector3 monsterPos)
	{
		focusedMonsterPos = monsterPos;
		string[] menuNames = { "move", "attack", "skill" };
		float ydiff = -0.1f;
		foreach (string menuName in menuNames)
		{
			GameObject buttonObj = menuMap[menuName];
			buttonObj.SetActive(true);
			buttonObj.transform.position = focusedMonsterPos.Value;
			Vector3 diff = new Vector3(0.45f, ydiff, 0f);
			buttonObj.transform.position += diff;
			ydiff -= 0.2f;
		}
	}

	public void hideAllMenu()
	{
		foreach (GameObject menu in menuMap.Values)
		{
			menu.SetActive(false);
			menu.transform.position = Global.outOfGamePos;
		}
		// focusedMonsterPos = null;
	}
	#endregion

	#region monster_related
	public void addMonster(Tuple<int, int> index)
	{
		if (monsterMap.ContainsKey(Tool.tupleToInt(index)))
		{
			Debug.LogError("Block already contains monster");
			return;
		}
		GameObject monsterObj = Instantiate(monsterPrefab, Tool.getPosition(index), Quaternion.identity) as GameObject;
		monsterObj.GetComponentInChildren<SpriteRenderer>().sortingOrder = 3;
		MonsterData md = monsterDataList[Engine.sp.getMonster()];
		Sprite s = Resources.Load<Sprite>(md.getNormalizedResourceId() + "_small");
		// for player 0, x = -1, for player 1 x = 1
		monsterObj.transform.localScale = new Vector3(currentPlayerId * 2 - 1, 1, 1);
		monsterObj.GetComponentInChildren<SpriteRenderer>().sprite = s;
		Monster monster = new Monster(md, currentPlayerId);
		monsterList.Add(monster);
		playerList[currentPlayerId].monsterList.Add(monsterList.Count - 1);
		Tool.setMonsterId(monsterObj, monsterList.Count - 1);
		// monsterObj.GetComponentInChildren<MenuHandle>().monsterId = monsterList.Count - 1;
		Debug.Log(Tool.getMonsterId(monsterObj));
		// Debug.Log(monsterObj.GetComponentInChildren<MenuHandle>().monsterId);
		monsterMap.Add(Tool.tupleToInt(index), monsterObj);
		if (currentPlayerId == 0)
			Engine.bf.setBlockType(index, BlockType.monster_0);
		else
			Engine.bf.setBlockType(index, BlockType.monster_1);
		updateResourceInfo();
	}

	public void removeMonster(Tuple<int, int> index)
	{
		if (!monsterMap.ContainsKey(Tool.tupleToInt(index)))
		{
			Debug.LogError("Block not contains monster");
			return;
		}
		int key = Tool.tupleToInt(index);
		int monsterId = Tool.getMonsterId(monsterMap[key]);
		Monster monster = monsterList[monsterId];
		Engine.bf.setBlockType(index, BlockType.normal);
		playerList[monster.ownerId].monsterList.Remove(monsterId);
		// Can not remove, only set to null
		monsterList[monsterId] = null;
		Destroy(monsterMap[key]);
		monsterMap.Remove(key);
	}

	public GameObject getMonster(Tuple<int, int> index)
	{
		int key = Tool.tupleToInt(index);
		if (monsterMap.ContainsKey(key))
			return monsterMap[key];
		else
		{
			Debug.LogError("Monster not exists!");
			return null;
		}
	}

	public void moveMonster(Vector3 destination)
	{
		if (focusedMonsterPos == null)
		{
			// Yes, the source is focuedMonsterPos
			Debug.LogError("No monster focused on!");
			return;
		}
		Tuple<int, int> index = Tool.getBlockIndex(focusedMonsterPos.Value);
		GameObject monster = getMonster(index);
		monster.transform.position = destination;
		monsterMap.Remove(Tool.tupleToInt(index));
		BlockType bt = Engine.bf.getBlockType(index);
		Engine.bf.setBlockType(index, BlockType.normal);
		Tuple<int, int> desIndex = Tool.getBlockIndex(destination);
		monsterMap.Add(Tool.tupleToInt(desIndex), monster);
		Engine.bf.setBlockType(desIndex, bt);
		focusedMonsterPos = null;
		foreach (Tuple<int, int> block in Engine.mp.getMoveRegion())
		{
			GameObject topBlock = getTopBlock(block);
			Color c = topBlock.GetComponent<SpriteRenderer>().color;
			c.a = 1;
			topBlock.GetComponent<SpriteRenderer>().color = c;
			topBlock.GetComponent<TopBlockHandle>().isEnabled = false;
		}
		consumeResource('P', Engine.mp.getDistance(desIndex));
		updateResourceInfo();
		Engine.mp.endProcess();
	}

	private void calcDamage(Monster attacker, Monster attackee)
	{
		float damage = attacker.realAttack - attackee.realDefense;
		if (damage < 0)
			damage = 0;
		attackee.realHp -= damage;
	}

	public void attackMonster(bool isCheckPassed)
	{
		if (isCheckPassed)
		{
			GameObject attackerMonsterObj = monsterMap[Tool.tupleToInt(Engine.ap.getAttackerPosition())];
			attackerMonsterObj.GetComponent<Animator>().SetTrigger(Engine.ap.getAttackAnimateTrigger());
			GameObject attackeeMonsterObj = monsterMap[Tool.tupleToInt(Engine.ap.getAttackeePosition())];
			Monster attackerMonster = monsterList[Tool.getMonsterId(attackerMonsterObj)];
			Monster attackeeMonster = monsterList[Tool.getMonsterId(attackeeMonsterObj)];
			calcDamage(attackerMonster, attackeeMonster);
			// Counter attack
			if (attackeeMonster.realHp > 0)
			{
				calcDamage(attackeeMonster, attackerMonster);
				if (attackerMonster.realHp <= 0)
					removeMonster(Engine.ap.getAttackerPosition());
			}
			else
				removeMonster(Engine.ap.getAttackeePosition());
			Engine.ap.endProcess();
		}
		else
		{
			Engine.ap.endProcess();
			dialogPopup.setContent("无效攻击对象");
			popupType = PopupType.dialog;
		}
	}

	public bool monsterOwnedByCurrentPlayer(int monsterId)
	{
		if (monsterList[monsterId].ownerId == currentPlayerId)
			return true;
		else
			return false;
	}
	#endregion

	#region gui_popup_process
	void OnGUI()
	{
		switch (popupType)
		{
			case PopupType.none:
				break;
			case PopupType.dialog:
				showDialog();
				break;
			case PopupType.summon:
				showSummonChoice();
				break;
		}
	}

	private void showDialog()
	{
		mask.SetActive(true);
		Texture2D texture = new Texture2D(10, 10);
		Color[] cols = new Color[100];
		for (int i = 0; i < cols.Length; i++)
		{
			cols[i] = new Color(0, 0, 0, 0.75f);
		}
		texture.SetPixels(cols);
		texture.Apply();
		GUIStyle boxStyle = new GUIStyle();
		boxStyle.normal.background = texture;
		boxStyle.normal.textColor = Color.white;
		boxStyle.fontSize = 14;
		boxStyle.alignment = TextAnchor.MiddleCenter;
		Tuple<int, int> border = dialogPopup.getBorder();
		int buttonHeight = 24;
		int boxWidth = border.Item1 * 16;
		int boxHeight = border.Item2 * 24;
		int boxLeft = (960 - boxWidth) / 2;
		int boxTop = (540 - boxHeight) / 2 - buttonHeight;
		GUI.Box(new Rect(boxLeft, boxTop, boxWidth, boxHeight), dialogPopup.getContent(), boxStyle);
		if (GUI.Button(new Rect(boxLeft, boxTop + boxHeight + 2, boxWidth, buttonHeight), "关闭"))
		{
			popupType = PopupType.none;
			updateResourceInfo();
			mask.SetActive(false);
		}
	}

	private bool consumeResource(char res, int required)
	{
		if (playerList[currentPlayerId].resourceMap[res] >= required)
		{
			playerList[currentPlayerId].resourceMap[res] -= required;
			return true;
		}
		return false;
	}

	private void showSummonChoice()
	{
		mask.SetActive(true);
		Texture2D[] images = new Texture2D[3];
		for (int i = 0; i < 3; i++)
		{
			int id = standbyMonsterIds[i];
			Sprite s = Resources.Load<Sprite>(monsterDataList[id].getNormalizedResourceId() + "_large");
			images[i] = s.texture;
			GUI.Box(new Rect(325 + 105 * i, 182, 100, 24), string.Format("Level {0}", monsterDataList[id].level));
		}
		int selected = GUI.SelectionGrid(new Rect(325, 210, 310, 100), -1, images, 3);
		Debug.Log(selected);
		if (selected != -1)
		{
			if (consumeResource('S', monsterDataList[standbyMonsterIds[selected]].level))
			{
				Engine.sp.selectMonster(standbyMonsterIds[selected]);
				Engine.sp.startProcess();
				popupType = PopupType.none;
				mask.SetActive(false);
			}
			else
			{
				dialogPopup.setContent("召唤资源不足");
				popupType = PopupType.dialog;
			}
		}
	}
	#endregion

	# region roll_process
	public void showRollResult()
	{
		dialogPopup.setContent(rollResource());
		popupType = PopupType.dialog;
	}

	private string rollResource()
	{
		string ret = "";
		System.Random r = new System.Random(System.DateTime.Now.Millisecond);
		for (int i = 0; i < 3; i++)
		{
			MonsterData md = monsterDataList[standbyMonsterIds[i]];
			int dice = r.Next(6) + 1;
			string item = string.Format("{0}：{1} => {2}\n", md.name, dice,
				MonsterData.diceMap[md.diceDistribution.ToCharArray()[dice - 1]]);
			playerList[currentPlayerId].resourceMap[md.diceDistribution.ToCharArray()[dice - 1]] += 1;
			ret += item;
		}
		ret = "获得资源\n========\n" + ret;
		return ret;
	}

	private void updateResourceInfo()
	{
		GameObject statusBar = GameObject.Find("status_bar");
		string formatStr = "资源：前进（{0}） 攻击（{1}） 防御（{2}） 魔法（{3}） 陷阱（{4}） 召唤（{5}）";
		string resourceInfo = string.Format(formatStr,
			playerList[currentPlayerId].resourceMap['P'],
			playerList[currentPlayerId].resourceMap['A'],
			playerList[currentPlayerId].resourceMap['D'],
			playerList[currentPlayerId].resourceMap['M'],
			playerList[currentPlayerId].resourceMap['T'],
			playerList[currentPlayerId].resourceMap['S']);
		resourceInfo += "怪兽：";
		foreach (int monsterId in playerList[currentPlayerId].monsterList)
		{
			MonsterData md = monsterDataList[monsterList[monsterId].monsterDataId];
			resourceInfo += md.name + ", ";
		}
		statusBar.GetComponent<Text>().text = resourceInfo;
	}
	#endregion

	public void startSummon()
	{
		Debug.Log("summon");
		popupType = PopupType.summon;
		// Engine.sp.startProcess();
	}

	public void endTurn()
	{
		currentPlayerId = 1 - currentPlayerId;
		updateResourceInfo();
	}

	private void showMonsterInfo(string resourceId, string name, int level, string property, float attack, float hp)
	{
		GameObject displayMonster = GameObject.Find("display_mons");
		Sprite s = Resources.Load<Sprite>(resourceId + "_large");
		displayMonster.GetComponent<Image>().sprite = s;
		string info = string.Format("{0}\n等级 {1}\n属性 {2}\n攻击 {3}\n生命 {4}", name, level, property, attack, hp);
		GameObject displayInfo = GameObject.Find("display_info");
		displayInfo.GetComponent<Text>().text = info;
	}

	public void showStandbyMonsterInfo(int id)
	{
		MonsterData md = monsterDataList[standbyMonsterIds[id]];
		string resourceId = md.getNormalizedResourceId();
		showMonsterInfo(resourceId, md.name, md.level, md.property, md.attack, md.hp);
	}

	public void showActiveMonsterInfo(int monsterId)
	{
		Monster monster = monsterList[monsterId];
		MonsterData md = monsterDataList[monster.monsterDataId];
		string resourceId = md.getNormalizedResourceId();
		showMonsterInfo(resourceId, md.name, md.level, md.property, monster.realAttack, monster.realHp);
	}

	// Use this for initialization
	void Start()
	{
		bottomBlockMap = new Dictionary<int, GameObject>();
		topBlockMap = new Dictionary<int, GameObject>();
		menuMap = new Dictionary<string, GameObject>();
		monsterMap = new Dictionary<int, GameObject>();
		playerList = new List<Player>();
		monsterDataList = new List<MonsterData>();
		monsterList = new List<Monster>();
		standbyMonsterIds = new int[3];
		currentPlayerId = 0;
		focusedMonsterPos = null;

		for (int i = 0; i < Global.mapSize; i++)
		{
			for (int j = 0; j < Global.mapSize; j++)
			{
				Tuple<int, int> index = new Tuple<int, int>(i, j);
				addBottomBlock(index);
			}
		}

		GameObject canvas = GameObject.Find("Canvas");
		string[] menuNames = { "move", "attack", "skill" };
		Dictionary<string, string> menuTextMap = new Dictionary<string, string>();
		menuTextMap["move"] = "移动";
		menuTextMap["attack"] = "攻击";
		menuTextMap["skill"] = "技能";
		foreach (string menuName in menuNames)
		{
			GameObject buttonObj = Instantiate(buttonPrefab) as GameObject;
			Text txt = buttonObj.GetComponentsInChildren<Text>()[0] as Text;
			txt.text = menuTextMap[menuName];
			buttonObj.transform.SetParent(canvas.transform, false);
			buttonObj.transform.position = Global.outOfGamePos;

			Button button = buttonObj.GetComponent<Button>();
			// If not saved, the last value in buttonTexts will be used for all the buttons
			string savedValue = menuName;
			button.onClick.AddListener(() => monsterMenuHandle(savedValue));

			buttonObj.SetActive(false);
			menuMap[menuName] = buttonObj;
		}

		TextAsset xmlAsset = Resources.Load<TextAsset>("data");
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(xmlAsset.text);
		XmlNodeList nodeList = xmlDoc.GetElementsByTagName("monster");

		int monsterDataId = 0;
		foreach (XmlNode node in nodeList)
		{
			MonsterData md = new MonsterData(monsterDataId, node);
			monsterDataList.Add(md);
			monsterDataId++;
		}
		List<int> randomList = Tool.getRandomList(0, monsterDataList.Count - 1, 3);
		for (int i = 1; i <= randomList.Count; i++)
		{
			GameObject standbyMonster = GameObject.Find("standby_mons_" + i);
			GameObject diceDistribution = GameObject.Find("dice_dist_" + i);
			int id = randomList[i - 1];
			Sprite s = Resources.Load<Sprite>(monsterDataList[id].getNormalizedResourceId() + "_small");
			standbyMonsterIds[i - 1] = id;
			standbyMonster.GetComponent<Image>().sprite = s;
			diceDistribution.GetComponent<Text>().text = monsterDataList[id].getDiceDistributionString();
		}
		showStandbyMonsterInfo(0);

		playerList.Add(new Player(0));
		playerList.Add(new Player(1));

		// Only for test
		playerList[0].resourceMap['S'] = 10;
		playerList[1].resourceMap['S'] = 10;

		mask = GameObject.Find("mask");
		mask.SetActive(false);

		Comparison c1 = new Comparison();
		c1.op = "eq";
		Property p11 = new Property();
		p11.propertyName = "name";
		c1.leftValue = p11;
		Constant p12 = new Constant();
		p12.constantValue = "mizumaru";
		c1.rightValue = p12;
		Comparison c2 = new Comparison();
		c2.op = "le";
		Property p21 = new Property();
		p21.propertyName = "hp";
		c2.leftValue = p21;
		Constant p22 = new Constant();
		p22.constantValue = "3";
		c2.rightValue = p22;
		Logic l1 = new Logic();
		l1.op = "and";
		l1.logicElements = new Expression[] { c1, c2 };
		Filter f1 = new Filter();
		f1.id = 1;
		f1.expression = l1;

		Comparison c0 = new Comparison();
		c0.op = "ge";
		Aggregate p01 = new Aggregate();
		p01.filterId = 1;
		p01.propertyName = "name";
		p01.aggregateMethod = "Count";
		c0.leftValue = p01;
		Constant p02 = new Constant();
		p02.constantValue = "1";
		c0.rightValue = p02;

		Condition c = new Condition();
		c.target = "Monster";
		c.filters = new Filter[] { f1 };
		c.criteria = new Expression[] { c0 };

		string dump = JsonConvert.SerializeObject(c);
		Debug.Log(dump);

		string json = "{'target':'Monster','filters':[{'id':1,'expression':{'logicElements':[{'leftValue':{'propertyName':'name'},'rightValue':{'constantValue':'mizumaru'},'op':'eq'},{'leftValue':{'propertyName':'hp'},'rightValue':{'constantValue':'3'},'op':'le'}],'op':'and'}}],'criteria':[{'leftValue':{'filterId':1,'aggregateMethod':'Count','propertyName':'name'},'rightValue':{'constantValue':'1'},'op':'ge'}]}";
		Condition cc = JsonConvert.DeserializeObject<Condition>(json, new JsonExpressionConverter(), new JsonValueConverter());
		Debug.Log(cc.debugPrint());
	}

	// Update is called once per frame
	void Update()
	{

	}
}
