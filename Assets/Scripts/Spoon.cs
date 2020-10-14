using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Spoon: MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
 
	Vector3 StartPosition ;
	Vector3 StartScale;

	public float EndScaleFactor;

 
	public bool bIskoriscen = false;
	[HideInInspector()]
	public  bool bDrag = false;

	 
	float x;
	float y;
	Vector3 diffPos = new Vector3(0,0,0);
	float testDistance =  .25f; 
 
	PointerEventData pointerEventData;
	int childIndex;
	 
	Transform ParentOld;
	Transform DragItemParent;
	Transform DragItemParent2;

	public Transform TestPoint;
	public Transform[] TargetPoint;
	int targetPointIndex = -1;

 

    public Transform TestTopMovementLimit;
	public Transform TestBotMovementLimit;
    public Transform TopMovementLimit;
	public Transform BotMovementLimit;

	public Transform DishTopLeft;
	public Transform DishBottomRight;

	Vector3 TL;
	Vector3 BR;
	Vector3 PrevPos;

	public bool bInDish;
	public Transform activeItem;
	public bool bEmpty = true;
	public float snapSpeed = .5f;
	IEnumerator Start()
	{
		yield return new WaitForSeconds(0.1f);

		DragItemParent = GameObject.Find("ActiveItemHolder").transform;
		DragItemParent2 = GameObject.Find("ActiveItemHolderDish").transform;
		StartPosition  = transform.position;

		ParentOld = transform.parent;
		childIndex = transform.GetSiblingIndex();
		bIskoriscen = false;
		if(TestPoint == null)	TestPoint = transform.Find("TestPoint");

		TL =  DishTopLeft.position;
		BR = DishBottomRight.position;
		PrevPos = transform.position;
	}

	void Update()
	{ 

		if(  bDrag )
		{
			x = Input.mousePosition.x;
			y = Input.mousePosition.y;

			Vector3 posM = Camera.main.ScreenToWorldPoint(new Vector3(x ,y,10f) ) + diffPos;
			if(TestTopMovementLimit!=null && TopMovementLimit!=null)
			{
				float diffTL = TestTopMovementLimit.position.y-transform.position.y;
				if(posM.y+ diffTL>TopMovementLimit.position.y) posM = new Vector3(posM.x, TopMovementLimit.position.y - diffTL ,posM.z);
			}
            if(TestBotMovementLimit!=null && BotMovementLimit!=null)
            {
                float diffTL = TestBotMovementLimit.position.y-transform.position.y;
                if(posM.y+ diffTL<BotMovementLimit.position.y) posM = new Vector3(posM.x, BotMovementLimit.position.y - diffTL ,posM.z);
            }
			 
			if(bInDish) 
			{
				if( posM.y<BR.y) posM = new Vector3( posM.x,  BR.y ,posM.z);
				if( posM.x<TL.x ) posM = new Vector3( TL.x,  posM.y ,posM.z);
				else if( posM.x>BR.x ) posM = new Vector3( BR.x,  posM.y ,posM.z);
			}


			if(  (posM.y>=BR.y && posM.y<TL.y) && (posM.x>=TL.x && posM.x<=BR.x) )
			{
				if(!bInDish && PrevPos.x>TL.x && PrevPos.x<BR.x && PrevPos.y>TL.y)
				{
					transform.SetParent(DragItemParent2);
					bInDish = true;
				}
			}
			else if( bInDish )
			{
				transform.SetParent(DragItemParent);
				bInDish = false;
			}

			transform.position =  Vector3.Lerp (transform.position  , posM  , 10 * Time.deltaTime)   ;
			PrevPos = transform.position;




		}	 
	}


	void TestTarget()
	{ 
 
		if( (!bInDish && bEmpty) || bIskoriscen) return;

		int closestPoint= -1;
		float distance = 1000;
		float distance2 = 0;

		for(int i= 0;i<TargetPoint.Length; i++)
		{
			if(TargetPoint[i] == null) continue;
			distance2 = Vector2.Distance(TestPoint.position,TargetPoint[i].position);
			if(distance2< testDistance &&  distance2 < distance )//&& TargetPoint[i].childCount==0)
			{
				closestPoint = i;
				distance = distance2;
			}
		}
		targetPointIndex = closestPoint;

		if(targetPointIndex >-1)
		{
 
			StartCoroutine("SnapToTarget",TargetPoint[targetPointIndex]);
			targetPointIndex = -1;
		}
	

	}




	IEnumerator SnapToTarget( Transform target)
	{
		yield return new WaitForEndOfFrame();

		if(bEmpty  )
		{
			bIskoriscen = true;
 
			float pom = 0;
			activeItem = target;
			target.SetParent(transform);
			GameObject.Destroy(target.GetComponent<Rigidbody2D>());
			GameObject.Destroy(target.GetComponent<CircleCollider2D>());
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime* 3;
				target.position = Vector3.Lerp(target.position, TestPoint.position  ,pom);
				yield return new WaitForFixedUpdate();
			}
			Camera.main.SendMessage("NextPhase", target.name, SendMessageOptions.DontRequireReceiver);
			yield return new WaitForSeconds(.5f);
			bEmpty = false;
			bIskoriscen = false;
			testDistance = 1;
		}
		else
		{
			bIskoriscen = true;
			float pom = 0;
			//bDrag = false;
			activeItem.SetParent(transform.parent);
			while(pom<1)
			{
				
				pom+=Time.deltaTime*snapSpeed;
				activeItem.position = Vector3.Lerp(activeItem.position, target.position ,pom);
				yield return new WaitForEndOfFrame();
			}
			activeItem.SetParent(target);
//			if(pointerEventData.dragging) bDrag = true;
//			else
//			{
//				CancelInvoke("TestTarget");
//				StartCoroutine("MoveBack" );
//			}
			activeItem = null;
			Camera.main.SendMessage("NextPhase", "EndDrag", SendMessageOptions.DontRequireReceiver);
			yield return new WaitForSeconds(.5f);
			bEmpty = true;
			bIskoriscen = false;
			testDistance = .25f;
		}


	}


	public void OnBeginDrag (PointerEventData eventData)
	{
		if(  bIskoriscen || bMovingBack) return;
		StopAllCoroutines(); 
		pointerEventData = eventData;


		if(  !bIskoriscen   && !bDrag  )
		{

		
			bDrag = true;
			StartPosition = transform.position;
			diffPos =transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)   ;
			diffPos = new Vector3(diffPos.x,diffPos.y,0);
			if(DragItemParent == null) DragItemParent = GameObject.Find("ActiveItemHolder").transform;
			if(ParentOld == null) 
			{
				ParentOld = transform.parent;
				childIndex = transform.GetSiblingIndex();
			}
			DragItemParent.position = Vector3.zero;
			DragItemParent.position = transform.parent .position;


			transform.SetParent(DragItemParent);

			 CancelInvoke(); 
			InvokeRepeating("TestTarget",0f, .1f); 

		}
	}




	public void OnDrag (PointerEventData eventData)
	{

	}

	public void OnEndDrag (PointerEventData eventData)
	{
		if(    bDrag 	)  
		{ 
			pointerEventData = eventData;
			bDrag = false;
			CancelInvoke("TestTarget");
			StartCoroutine("MoveBack" );

		}
	}



	bool bMovingBack = false;
	IEnumerator MoveBack(  )
	{
		if(!bMovingBack)
		{
			bMovingBack = true;
			yield return new WaitForEndOfFrame( );

			float pom = 0;
			Vector3 positionS = transform.position;

			while(pom<1 )
			{ 
				pom+=Time.fixedDeltaTime*2;
				transform.position = Vector3.Lerp(positionS, StartPosition,pom);
				yield return new WaitForFixedUpdate( );
			}

			transform.SetParent(ParentOld);
			transform.SetSiblingIndex(childIndex);
			transform.position = StartPosition;

		
			bMovingBack = false;

		}

	}

	public void StartMoveBack()
	{
		StopAllCoroutines();
		CancelInvoke("TestTarget");
		StartCoroutine("MoveBack" );
	}


	bool appFoucs = true;
	void OnApplicationFocus( bool hasFocus )
	{
		if(  !appFoucs && hasFocus )
		{
			if(  !bIskoriscen &&  bDrag )
			{
				bDrag = false;

				CancelInvoke("TestTarget");
				StartCoroutine("MoveBack" );
			}
		}
		appFoucs = hasFocus;

	}



}
