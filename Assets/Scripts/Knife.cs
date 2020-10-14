using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;




public class Knife :  MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	CutScript cs;
	Vector3 diffPos;
	bool bDrag = false; 
 
	public static bool bEnableDrag = false;

	public void Start()
	{
		cs = Camera.main.GetComponent<CutScript>();	
	}

	public void OnBeginDrag (PointerEventData eventData)
	{
		if(!bEnableDrag)
		{
			bDrag = false;
			return;
		}
		 bDrag = true;
		//StartPosition = transform.position;
		diffPos =transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)   ;

 
	}

 
	public void OnDrag (PointerEventData eventData)
	{
		if(!bEnableDrag)
		{
			bDrag = false;
			return;
		}
		if(bDrag)
		{
			float mPosW =  Camera.main.ScreenToWorldPoint(Input.mousePosition ).y *1.4f+ diffPos.y;//1.2
			cs.MoveKnife(mPosW);
		}

	}

	public void OnEndDrag (PointerEventData eventData)
	{
		bDrag = false;
	}
}
