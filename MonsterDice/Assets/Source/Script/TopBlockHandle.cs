using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class TopBlockHandle : MonoBehaviour, IPointerClickHandler
{

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (Engine.gs == GameStage.move)
			Tool.getManagerHandle().moveMonster(transform.position);
	}
}
