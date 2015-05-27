using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using Model;

public class GameManagerHandle : MonoBehaviour
{
	private Dictionary<int, GameObject> bottomBlockMap;
	private Dictionary<int, GameObject> topBlockMap;
	private Dictionary<string, GameObject> menuMap;
	private Dictionary<int, GameObject> monsterMap;
	// Cool feature, T? shorts for Nullable<T>, otherwise value type (like int, bool) cannot be assigned null
	private Vector3? focusedMonsterPos; // the position of the current focused monster
	public GameObject bottomBlockPrefab;
	public GameObject topBlockPrefab;
	public GameObject buttonPrefab;
	public GameObject monsterPrefab;

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
		if (name == "move")
		{
			Tuple<int, int> index = Tool.getBlockIndex(focusedMonsterPos.Value);
			Engine.mp.startProcess(index, 2);
			List<Tuple<int, int>> blockList = Engine.mp.getMoveRegion();
			foreach (Tuple<int, int> block in blockList)
			{
				GameObject topBlock = getTopBlock(block);
				Color c = topBlock.GetComponent<SpriteRenderer>().color;
				c.a = 0.5f;
				topBlock.GetComponent<SpriteRenderer>().color = c;
				topBlock.GetComponent<TopBlockHandle>().isEnabled = true;
			}
		}
		hideAllMenu();
	}

	public void showMonsterMenu(Vector3 monsterPos)
	{
		focusedMonsterPos = monsterPos;
		string[] menuTexts = { "move", "attack", "skill" };
		float ydiff = -0.1f;
		foreach (string menuText in menuTexts)
		{
			GameObject buttonObj = menuMap[menuText];
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
		GameObject monster = Instantiate(monsterPrefab, Tool.getPosition(index), Quaternion.identity) as GameObject;
		monster.GetComponent<SpriteRenderer>().sortingOrder = 3;
		Sprite s = Resources.Load<Sprite>(Engine.sp.getMonster());
		monster.GetComponent<SpriteRenderer>().sprite = s;
		monsterMap.Add(Tool.tupleToInt(index), monster);
		Engine.bf.setBlockType(index, BlockType.monster);
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
		Engine.bf.setBlockType(index, BlockType.normal);
		Tuple<int, int> desIndex = Tool.getBlockIndex(destination);
		monsterMap.Add(Tool.tupleToInt(desIndex), monster);
		Engine.bf.setBlockType(desIndex, BlockType.monster);
		focusedMonsterPos = null;
		foreach (Tuple<int, int> block in Engine.mp.getMoveRegion())
		{
			GameObject topBlock = getTopBlock(block);
			Color c = topBlock.GetComponent<SpriteRenderer>().color;
			c.a = 1;
			topBlock.GetComponent<SpriteRenderer>().color = c;
			topBlock.GetComponent<TopBlockHandle>().isEnabled = false;
		}
		Engine.mp.endProcess();
	}
	#endregion

	public void startSummon()
	{
		Engine.sp.startProcess();
	}

	// Use this for initialization
	void Start()
	{
		bottomBlockMap = new Dictionary<int, GameObject>();
		topBlockMap = new Dictionary<int, GameObject>();
		menuMap = new Dictionary<string, GameObject>();
		monsterMap = new Dictionary<int, GameObject>();
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
		string[] menuTexts = { "move", "attack", "skill" };
		foreach (string menuText in menuTexts)
		{
			GameObject buttonObj = Instantiate(buttonPrefab) as GameObject;
			Text txt = buttonObj.GetComponentsInChildren<Text>()[0] as Text;
			txt.text = menuText;
			buttonObj.transform.SetParent(canvas.transform, false);
			buttonObj.transform.position = Global.outOfGamePos;

			Button button = buttonObj.GetComponent<Button>();
			// If not saved, the last value in buttonTexts will be used for all the buttons
			string savedValue = menuText;
			button.onClick.AddListener(() => monsterMenuHandle(savedValue));

			buttonObj.SetActive(false);
			menuMap[menuText] = buttonObj;
		}
	}

	// Update is called once per frame
	void Update()
	{

	}
}
