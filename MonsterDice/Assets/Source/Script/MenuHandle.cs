using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using Model;

public class MenuHandle : MonoBehaviour
{
	private bool isSelected;

	// Use this for initialization
	void Start ()
	{
		isSelected = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
			
	}
	
	void OnMouseDown ()
	{
		Debug.Log ("click");
		Tuple<int, int> index = Tool.getBlockIndex (this.transform.position);
		Debug.Log (index.Item1 + ", " + index.Item2);
		Tool.getManagerHandle ().showMonsterMenu (this.transform.position);
	}
}
