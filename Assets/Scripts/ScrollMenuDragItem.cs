using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollMenuDragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
	public int buttonNo = 0;
	public ScrollMenu scrollMenu;
	public static bool bEnableDrag = true;
	public bool bLocked = false;

	//Quaternion StartRotation;
	Vector3 StartPositionOffset;
	Vector3 StartScale;

	public float DragScaleFactor = 1;

	public bool snapToTarget = false;
	public bool bIskoriscen = false;
	[HideInInspector()]
	public  bool bDrag = false;


	float x;
	float y;
	Vector3 diffPos = new Vector3(0,0,0);
	public float testDistance =  1f;


	ParticleSystem psFinishAction;


	public Animator animator; 
	bool bAnimationActive = false;
	public string animationType = "";

	Transform ParentOld;
	Transform DragItemParent;

	public Transform TestPoint;
	public Transform[] TargetPoint;
	int targetPointIndex = -1;

	public bool bTestOnlyOnEndDrag = false;

    public Transform TestTopMovementLimit;
	public Transform TestBotMovementLimit;
    public Transform TopMovementLimit;
	public Transform BotMovementLimit;

	public ScrollRect scrollRect;


	IEnumerator Start()
	{
		yield return new WaitForSeconds(0.1f);
		Init();

	}
	public void Init()
	{
		DragItemParent = GameObject.Find("ActiveItemHolder").transform;
		StartPositionOffset  = transform.parent.position - transform.position;
		ParentOld = transform.parent;
		bIskoriscen = false;
		if(TestPoint == null)	TestPoint = transform.Find("TestPoint");
		//bLocked = !scrollMenu. menus[0].UnlockedItems[buttonNo-1];
	}

	void Update()
	{ 
		if(   bDrag )
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
			transform.position =  Vector3.Lerp (transform.position  , posM  , 10 * Time.deltaTime)   ;
		}	 
	}


	void TestTarget()
	{ 
		if(bAnimationActive || bIskoriscen) return;

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
		}
		else if( bTestOnlyOnEndDrag) 
		{
			StartCoroutine("MoveBack" );
		}

	}




	IEnumerator SnapToTarget( Transform target)
	{
		//OneItemEnabledNo = 0;
		bDrag = false;
		CancelInvoke("TestTarget");

		yield return new WaitForEndOfFrame();

		//OVDE SE DODAJU SPECIFICNA PONASANJA ITEMA KADA SE PRIBLIZI ODREDISTU

		  
		if(animationType =="flavorBottle" && animator != null)
		{
			bEnableDrag = false;
			Debug.Log("flavorBottle");
			bIskoriscen = true;
			float pom = 0;
			Vector3 sPos = transform.position;
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime* 3;
				transform.position = Vector3.Lerp(sPos, target.position,pom);
				yield return new WaitForFixedUpdate();
			}
			animator.enabled = true;
			animator.Play("flavorBottle",-1,0);

			Camera.main.SendMessage("NextPhase","F"+buttonNo.ToString());
			yield return new WaitForSeconds(.7f);
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.WaterSound);
			yield return new WaitForSeconds(2.8f);
			StartCoroutine("MoveBack" );

		}



		else if(animationType =="fillPlate" && animator != null)
		{
			bEnableDrag = false;
			bIskoriscen = true;
			float pom = 0;
			Vector3 sPos = transform.position;
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime* 3;
				transform.position = Vector3.Lerp(sPos, target.position,pom);
				yield return new WaitForFixedUpdate();
			}
			animator.enabled = true;
			animator.Play("SelectFill",-1,0);
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.PickMeatSound);

			yield return new WaitForSeconds(1f);

			Transform testo = target.parent.GetChild(1);

			GameObject.Instantiate(  transform.GetChild(0).GetChild(0).gameObject, testo.GetChild(5));//.FindChild("FillHolder"));
			transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
			StartCoroutine("MoveBack" );
			yield return new WaitForSeconds(1f);

			testo.GetComponent<ItemAction>().enabled = true;
			testo.GetComponent<DragItem>().enabled = false; 
			 
			//testo.GetComponent<Animator>().enabled = false;

			Camera.main.SendMessage("NextPhase","F"+buttonNo.ToString());
			yield return new WaitForSeconds(1f);
			transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
			bIskoriscen = false;
		}

		else if(animationType =="selectFlavorJar" && animator != null)
		{
			bEnableDrag = false;
			Debug.Log("selectFlavorJar");
			bIskoriscen = true;
			float pom = 0;
			Vector3 sPos = transform.position;
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime* 3;
				transform.position = Vector3.Lerp(sPos, target.position,pom);
				yield return new WaitForFixedUpdate();
			}
			animator.enabled = true;
			animator.Play("selectFlavorJar",-1,0);

			Camera.main.SendMessage("NextPhase","JF"+buttonNo.ToString());
			yield return new WaitForSeconds(1.5f);
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.WaterSound);
			yield return new WaitForSeconds(3f);
			StartCoroutine("MoveBack" );

		}

		else 	if(animationType =="dimSumFlavor" && animator != null)
		{
			bEnableDrag = false;
			Debug.Log("dimSumFlavor");
			bIskoriscen = true;
			float pom = 0;
			Vector3 sPos = transform.position;
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime* 3;
				transform.position = Vector3.Lerp(sPos, target.position,pom);
				yield return new WaitForFixedUpdate();
			}
			animator.enabled = true;
			animator.Play("flavorBottle2",-1,0);
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.WaterSound);
			Camera.main.SendMessage("NextPhase","ds"+buttonNo.ToString());
			yield return new WaitForSeconds(3.5f);
			StartCoroutine("MoveBack" );
			animator.enabled = false;
			yield return new WaitForSeconds(1.0f);
			bIskoriscen = false;
			bEnableDrag = true;
		}

		else if(animationType =="dimSumSauce" )
		{
			bEnableDrag = false;
			Debug.Log("dimSumSauce");
			bIskoriscen = true;
			float pom = 0;
			Vector3 sPos = transform.position;
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime* 3;
				transform.position = Vector3.Lerp(sPos, target.position,pom);
				yield return new WaitForFixedUpdate();
			}
			transform.SetParent(target);
			Camera.main.SendMessage("NextPhase","ds"+buttonNo.ToString());

		}

	}


	public void AnimationFinished()
	{
		if( !bMovingBack)
		{
			StopAllCoroutines();
			StartMoveBack();
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if(!bEnableDrag) return;
		bLocked = !scrollMenu.menus[scrollMenu.activeMenu].UnlockedItems[buttonNo-1];
		if(!scrollMenu. menus[0].UnlockedItems[buttonNo-1]) {  Debug.Log("LOCK");scrollMenu.MenuButtonClick(buttonNo); }
	}

	public void OnBeginDrag (PointerEventData eventData)
	{
		if(!bEnableDrag) return;
		bLocked = !scrollMenu.menus[scrollMenu.activeMenu].UnlockedItems[buttonNo-1];
		mouseStartDragPosY = Input.mousePosition.y;
	 
		if(scrollRect!=null)
		{
			bInitBeginDrag = false;
			scrollRect.SendMessage("OnBeginDrag", eventData);
			return;
		}
		else InitBeginDrag(eventData);
	
	}

	float mouseStartDragPosY;
	bool bInitBeginDrag = false;

	void InitBeginDrag(PointerEventData eventData)
	{
		
		if(  bIskoriscen || bLocked || bMovingBack || bDrag) return;

		bDrag = true;

		StopAllCoroutines(); 

		bAnimationActive = false;
		bInitBeginDrag = true;

		if(DragItemParent == null) DragItemParent = GameObject.Find("ActiveItemHolder").transform;
		if(ParentOld == null) ParentOld = transform.parent;
		//StartPosition = transform.localPosition;//pozicija u odnosu na parent
		StartPositionOffset  = ParentOld.position - transform.position;
		StartScale =  transform.localScale;

		diffPos =transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)   ;
		diffPos = new Vector3(diffPos.x,diffPos.y,0);



		transform.SetParent(DragItemParent);
		if(DragScaleFactor!=1) 
		{
			StartCoroutine("CScaleOnBeginDrag");
		}
		if(!bTestOnlyOnEndDrag) { CancelInvoke(); InvokeRepeating("TestTarget",0f, .1f); }  
	}

 
	public void OnDrag (PointerEventData eventData)
	{
		if(!bEnableDrag) return;
		if(scrollRect!=null && !bInitBeginDrag) 
		{
			
			if( ((mouseStartDragPosY - Input.mousePosition.y)/(float)Screen.height) < 0.05f)
				 scrollRect.SendMessage("OnDrag", eventData);
			else
			{
				//if(bLocked) scrollMenu.MenuButtonClick(buttonNo);
				//else	
					InitBeginDrag( eventData);
			}
		}
	}

	public void OnEndDrag (PointerEventData eventData)
	{
		if(    bDrag 	)  
		{ 
			bDrag = false;
			if(bTestOnlyOnEndDrag &&  !bIskoriscen) TestTarget();
			else
			{
				CancelInvoke("TestTarget");
				StartCoroutine("MoveBack" );
			}
		}
		else if(scrollRect!=null && !bInitBeginDrag) 
		{
			if( ((mouseStartDragPosY - Input.mousePosition.y)/(float)Screen.height) < 0.05f)
				scrollRect.SendMessage("OnEndDrag", eventData);
		}
		bInitBeginDrag = false;
	}


	IEnumerator CScaleOnBeginDrag()
	{
		yield return new WaitForEndOfFrame();
		float pom = 0;
		Vector3 EndScale = StartScale*DragScaleFactor;
		while(pom<1)
		{
			pom +=5*Time.deltaTime;
			transform.localScale = Vector3.Lerp(StartScale,EndScale,pom);
			yield return new WaitForEndOfFrame();
		}
	}

	bool bMovingBack = false;
	IEnumerator MoveBack(  )
	{
		if(!bMovingBack)
		{
			bMovingBack = true;

			float pom = 0;
			Vector3 positionS = transform.position;
		 
			Vector3 EndScale = transform.localScale;
		 
			while(pom<1 )
			{ 
				pom+=Time.deltaTime*2;
				transform.position = Vector3.Lerp(positionS, ParentOld.position,pom );//+ StartPositionOffset,pom);
				if(DragScaleFactor!=1) transform.localScale = Vector3.Lerp(EndScale,StartScale,pom );
				yield return new WaitForEndOfFrame( ); 
			}

			transform.SetParent(ParentOld);
			transform.position = ParentOld.position;// + StartPositionOffset;

			 
			bMovingBack = false;
			 
		}
	}

	public void StartMoveBack()
	{
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
