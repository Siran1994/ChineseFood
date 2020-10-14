using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RawSpringRoll : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler 
{
	 
	Vector3 StartPosition ;
	Vector3 StartScale;
	Vector3 DragScale;
	Vector3 EndScale;
	 
	 
	public static bool bEnabled = false;
	[HideInInspector()]
	public bool bDrag = false;
	 

	float x;
	float y;
	Vector3 diffPos = new Vector3(0,0,0);
	 
	bool bIskoriscen = false;

	Transform TestPoint;
	public Transform[] TargetPoint;
	int targetPointIndex = -1;
	public float testDistance = 0.5f;

	Transform ParentOld;
	Transform DragItemParent;
	float scaleFactor = 1;

	public Transform TestTopMovementLimit;
	public Transform TopMovementLimit;

	IEnumerator Start()
	{
		yield return new WaitForSeconds(0.1f);

		DragItemParent = GameObject.Find("ActiveItemHolder").transform;
		ParentOld = transform.parent;
		StartPosition  = transform.position;

		DragScale = new Vector3(1f,1f,1)  ;
		EndScale = new Vector3(1f,1,1);//Vector3(0.8f,0.5f,1);
		 
		StartScale = ParentOld.parent.localScale;
		TestPoint = transform;
	}

	public void SetFryerStartPostionAndScale()
	{
		
		StartScale = transform.localScale;
		DragScale = StartScale * scaleFactor;
		EndScale = new Vector3(1f,1f,1) * scaleFactor;
		StartPosition  = transform.position;
	}

	void Update()
	{ 
		if( bEnabled &&  bDrag )
		{
			 
			x = Input.mousePosition.x;
			y = Input.mousePosition.y;

			Vector3 posM = Camera.main.ScreenToWorldPoint(new Vector3(x ,y,10f) ) + diffPos;
 
			if(TopMovementLimit ==null ) TopMovementLimit = GameObject.Find("TopMovementLimit").transform;
			if(TestTopMovementLimit ==null ) TestTopMovementLimit = transform;

			if(TestTopMovementLimit!=null && TopMovementLimit!=null)
			{
				float diffTL = TestTopMovementLimit.position.y-transform.position.y;
				if(posM.y+ diffTL>TopMovementLimit.position.y) posM = new Vector3(posM.x, TopMovementLimit.position.y - diffTL ,posM.z);
			}

			transform.position =  Vector3.Lerp (transform.position  , posM  , 10 * Time.deltaTime) ;
		}
	}

	void TestTarget()
	{ 
		//Debug.Log(bEnabled + "   "  + bIskoriscen);
		if(!bEnabled || bIskoriscen) return;
		 
		int closestPoint= -1;
		float distance = 1000;
		float distance2 = 0;

		for(int i= 0;i<TargetPoint.Length; i++)
		{
			
			if(TargetPoint[i] == null) continue;
			distance2 = Vector2.Distance(TestPoint.position,TargetPoint[i].position);
			if(distance2< testDistance &&  distance2 < distance  && TargetPoint[i].childCount==0)
			{
				closestPoint = i;
				distance = distance2;
			}
		}
		targetPointIndex = closestPoint;

		if(targetPointIndex >-1)
		{
			StartCoroutine("SnapToTarget",TargetPoint[targetPointIndex]);
		}
 
	}


	IEnumerator SnapToTarget( Transform target)
	{
		bIskoriscen = true;
		bDrag = false;
		CancelInvoke("TestTarget");

		yield return new WaitForEndOfFrame();

		 
		float pom = 0;
		Vector3 sPos = transform.position;
		while(pom<1)
		{
			pom+=Time.fixedDeltaTime* 5;
			transform.position = Vector3.Lerp(sPos, target.position,pom);
			transform.localScale = Vector3.Lerp(DragScale, EndScale,pom);
			yield return new WaitForFixedUpdate();
		}

		yield return new WaitForSeconds(0.1f);
		 Camera.main.SendMessage("NextPhase","SpringRoll", SendMessageOptions.DontRequireReceiver);

		transform.parent = target;
		this.enabled = false;
	}






	public void OnBeginDrag (PointerEventData eventData)
	{
		if( !bEnabled || bMovingBack || bIskoriscen ) return;
		StopAllCoroutines(); 

		if(  !bDrag  )
		{
			InvokeRepeating("TestTarget",0f, .1f);
	 
			bDrag = true;

			diffPos =transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)   ;
			diffPos = new Vector3(diffPos.x,diffPos.y,0);

			transform.SetParent(DragItemParent);
			transform.localScale = DragScale;
		 	 Tutorial.Instance.StopTutorial();

		}
	}




	public void OnDrag (PointerEventData eventData)
	{
		 
	}

	public void OnEndDrag (PointerEventData eventData)
	{
		if( bDrag)  
		{
			
			bDrag = false;
			StartCoroutine("MoveBack" );
		}
	}



	bool bMovingBack = false;
	IEnumerator MoveBack(  )
	{

		if(!bMovingBack)
		{
			CancelInvoke("TestTarget");
			bMovingBack = true;

			float pom = 0;
			Vector3 positionS = transform.position;
			Vector3 scaleTmp = transform.localScale ;
			while(pom<1f )
			{ 
				pom+=Time.fixedDeltaTime *1.5f;
				transform.position = Vector3.Lerp(positionS, StartPosition,pom);
				transform.localScale =  Vector3.Lerp(scaleTmp, StartScale,pom);  
				 
				yield return new WaitForEndOfFrame( );
			}

			bMovingBack = false;

		}
		transform.SetParent (ParentOld);
		transform.localScale = Vector3.one;
		yield return new WaitForFixedUpdate( );
	}


	 
	bool appFoucs = true;
	void OnApplicationFocus( bool hasFocus )
	{
		if(  !appFoucs && hasFocus )
		{
			if(  bDrag )
			{
				bDrag = false;
				 
				StartCoroutine("MoveBack" );
			}
		}
		appFoucs = hasFocus;

	}

}

