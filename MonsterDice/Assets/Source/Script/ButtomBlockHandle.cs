using UnityEngine;
using System.Collections.Generic;

using Model;

public class ButtomBlockHandle : MonoBehaviour
{
	private bool isSelected;
	public GameObject topBlockPrefab;

	private Tuple<int, int> getBlockIndex ()
	{
		Vector3 pos = this.transform.position;
		int x = Mathf.RoundToInt ((pos.x - Global.mapLeft) / Global.blockSize);
		int y = Mathf.RoundToInt ((Global.mapTop - pos.y) / Global.blockSize);
		return new Tuple<int, int> (x, y);
	}

	// Use this for initialization
	void Start ()
	{
		isSelected = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnMouseDown ()
	{
		if (!Engine.sp.inProcess ())
			return;
		Tuple<int, int> index = Tool.getBlockIndex (transform.position);
		if (!isSelected) {
			if (Engine.sp.selectBlock (index)) {
				Color c = GetComponent<SpriteRenderer> ().color;
				c.a = 0.5f;
				GetComponent<SpriteRenderer> ().color = c;
			}
			if (Engine.sp.getSelectedNum () == Global.summonPlaneSize) {
				GameManagerHandle handle = Tool.getManagerHandle ();
				List<Tuple<int, int>> indexList = Engine.sp.getSelectedBlockes ();
				foreach (Tuple<int, int> _index in indexList)
					handle.addTopBlock (_index);
				handle.addMonster(index);
				Engine.sp.endProcess ();
			}
			isSelected = true;
		} else {
			Engine.sp.unSelectBlock (index);
			Color c = GetComponent<SpriteRenderer> ().color;
			c.a = 1;
			GetComponent<SpriteRenderer> ().color = c;
			isSelected = false;
		}
	}
}
