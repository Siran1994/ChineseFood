using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Decoration : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	Vector3 dragOffset;
	public static bool bEnableDrag = true;
	public DecorationTransform decorationTransform;
//	public void ButtonMove_PointerStartDrag(  )
//	{
//		dragOffset =  Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position; 
//		dragOffset = new Vector3(dragOffset.x,dragOffset.y,0);
//	}
	
	Vector3 destintaion;  
	float sens = 10;
	Rigidbody2D rigdbody;
	Collider2D collider;
	bool bMove = false;

	public void OnBeginDrag (PointerEventData eventData)
	{
		if(  !bEnableDrag ||  decorationTransform == null ) return;
		dragOffset =  Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position; 
		dragOffset = new Vector3(dragOffset.x,dragOffset.y,0);
 
		rigdbody.WakeUp();
		collider.isTrigger = false;
		rigdbody.isKinematic = false;

	}
	
	void Start () {
		rigdbody = transform.GetComponent<Rigidbody2D>();
		collider = transform.GetComponent<Collider2D>();
		destintaion = transform.position;
	}

	void Update () {
		if(  !bEnableDrag   ) return;
		if(bMove)
		{
			Vector2 velocity =  (destintaion - transform.position) * sens;
			//Debug.Log("mag: "+velocity.magnitude);
			float magnitude = velocity.magnitude;
			if(magnitude >10) velocity = 10f/magnitude*velocity;
			rigdbody.velocity = velocity;
			decorationTransform.transform.position = transform.position;

		}
	}
 
	
	public void OnDrag (PointerEventData eventData)
	{
		if(  !bEnableDrag ) return;
		PointerEventData pointerData = eventData as PointerEventData;
		if(pointerData.IsPointerMoving() && decorationTransform !=null &&   !decorationTransform.bDecorationTransformButtonDown)
		{
			bMove = true;
			Vector3 s =  Camera.main.ScreenToWorldPoint(Input.mousePosition ) - dragOffset;

//			if( transform.parent.name == "ToysHolder")
//			{
//				if(s.x >1.6f) s= new Vector3(1.6f,s.y,0);
//				else if(s.x <-1.6f) s= new Vector3(-1.6f,s.y,0);
//			
//				if(s.y >2.5f) s= new Vector3(s.x,2.5f,0);
//				else if(s.y <-1.1f) s= new Vector3(s.x,-1.1f,0);
//			}
			
//			decorationTransform.transform.position = new Vector3 (s.x,s.y,-5);
//			transform.position = new Vector3 (s.x,s.y,0);


			destintaion = new Vector3 (s.x,s.y,0);
		}
		 
	}
	public void OnEndDrag (PointerEventData eventData)
	{
		DeactivateDecoration();
	}


	public void DeactivateDecoration()
	{
		rigdbody.Sleep();
		bMove = false;
		collider.isTrigger = true;
		rigdbody.isKinematic = true;
		destintaion = transform.position;
	}

}
