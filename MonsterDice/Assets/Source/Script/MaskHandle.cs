using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class MaskHandle : MonoBehaviour, IPointerClickHandler
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
		Debug.Log("mask");
	}
}
