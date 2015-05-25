using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

using Model;

public class MenuHandle : MonoBehaviour, IPointerClickHandler
{
	private bool isSelected;

	// Use this for initialization
	void Start()
	{
		isSelected = true;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void OnPointerClick(PointerEventData eventData)
	{
		Tuple<int, int> index = Tool.getBlockIndex(this.transform.position);
		Tool.getManagerHandle().showMonsterMenu(this.transform.position);
	}
}
