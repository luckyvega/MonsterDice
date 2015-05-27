using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class TopBlockHandle : MonoBehaviour, IPointerClickHandler
{
	public bool isEnabled;
	// Use this for initialization
	void Start()
	{
		isEnabled = false;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (Engine.mp.inProcess() && isEnabled)
		{
			Debug.Log("top click");
			Tool.getManagerHandle().moveMonster(transform.position);
		}
	}
}
