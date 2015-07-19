using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

using Model;

public class MenuHandle : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
	public int monsterId;

	// Use this for initialization
	void Start()
	{
		// Setting monsterId here will overwrite the value set by AddMonster
		// monsterId = -1;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		Tool.getManagerHandle().showActiveMonsterInfo(monsterId);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (Engine.ap.inProcess())
		{
			Tuple<int, int> target = Tool.getBlockIndex(this.transform.position);
			Tool.getManagerHandle().attackMonster(Engine.ap.canAttack(target));
			return;
		}
		if (Engine.gs != GameStage.standby) return;
		if (!Tool.getManagerHandle().monsterOwnedByCurrentPlayer(monsterId)) return;
		// Debug.Log(monsterId);
		Tool.getManagerHandle().showMonsterMenu(this.transform.position);
	}
}
