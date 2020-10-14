using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stickers : MonoBehaviour {


	public GameObject [] StickerPrefabs;
	public Transform StickersHolder;
	public DecorationTransform decorationTransform;

 
	 
	void Start () {
		
	}
	
	 
	void Update () {
		
	}

	public void CreateSticker(int stickerIndex)
	{
		if(decorationTransform.ActiveDecoration!=null) decorationTransform.ActiveDecoration.GetComponent<Decoration>( ).DeactivateDecoration();

		GameObject s = GameObject.Instantiate( StickerPrefabs[stickerIndex]);
		s.transform.SetParent(StickersHolder);
		s.transform.localScale = Vector3.one;
		s.transform.localPosition = 10*Random.insideUnitCircle;

		decorationTransform.ResetDecorationTransform();
		Decoration dec = s.GetComponent<Decoration>();
		dec.decorationTransform = decorationTransform;
		decorationTransform.ActiveDecoration = s;
		decorationTransform.ShowDecorationTransformTool();


	}


	public void DeleteAllStickers()
	{
 
		for(int i = StickersHolder.childCount-1; i>=0; i--)
		{
			GameObject.Destroy(StickersHolder.GetChild(i).gameObject);
		}

		decorationTransform.ActiveDecoration = null;
		decorationTransform.HideDecorationTransformTool();
	}
}
