using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FortuneCookieDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler 
{
	 
	Vector3 StartPosition ;
	Vector3 StartScale;
	Vector3 DragScale;
	Vector3 EndScale;
	 
	 
	public static bool bEnabled = false;
	[HideInInspector()]
	public bool bDrag = false;
	public bool bStretchDough = false;

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
	public float scaleFactor = 1;

    public Transform TestTopMovementLimit;
	public Transform TestBotMovementLimit;
    public Transform TopMovementLimit;
	public Transform BotMovementLimit;

	bool bShapeCookie = false;
	int shapeNo = -1;

	public static bool bShowTut = true;
	IEnumerator Start()
	{
		yield return new WaitForSeconds(0.1f);

		DragItemParent = GameObject.Find("ActiveItemHolder").transform;
		ParentOld = transform.parent;
		StartPosition  = transform.position;

		DragScale = new Vector3(1.3f,1.3f,1)  ;
		EndScale = new Vector3(1.3f,1.3f,1);
		 
		StartScale = transform.localScale;
		TestPoint = transform;
	}


	public void SetColor(Color col)
	{
		foreach( Transform child in transform)
		{
			if(child.CompareTag("ScrollMenuColor"))child.GetComponent<Image>().color = col;
		}

	}

  
	void Update()
	{ 
		if( bEnabled &&  bDrag )
		{
			 
			x = Input.mousePosition.x;
			y = Input.mousePosition.y;

			Vector3 posM = Camera.main.ScreenToWorldPoint(new Vector3(x ,y,10f) ) + diffPos;
 
			if(TopMovementLimit ==null ) TopMovementLimit = GameObject.Find("TopMovementLimit").transform;
			 

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
		Tutorial.Instance.StopTutorial();
		bEnabled = false;
		bDrag = false;
		//bShapeCookie = true;

		CancelInvoke("TestTarget");

		yield return new WaitForEndOfFrame();

		 
		float pom = 0;
		Vector3 sPos = transform.position;
		while(pom<1)
		{
			pom+=Time.fixedDeltaTime* 3;
			transform.position = Vector3.Lerp(sPos, target.position,pom);
			transform.localScale = Vector3.Lerp(DragScale, EndScale,pom);
			yield return new WaitForFixedUpdate();
		}

		yield return new WaitForSeconds(0.1f);

		// Camera.main.SendMessage("NextPhase","FC", SendMessageOptions.DontRequireReceiver);
		DragItem.OneItemEnabledNo = 1;
		 //transform.parent = target;
		//this.enabled = false;
		//bIskoriscen = true;

		if(bShowTut) Tutorial.Instance.ShowTutorial(1);
	}

	public void AddedMessage()
	{
		if(bShowTut)
		{
			Tutorial.Instance.StopTutorial();
			Tutorial.Instance.ShowTutorial(2);
			bShowTut = false;
		}
		shapeNo=1;
		StartCoroutine("CChangeImage");
	}

	IEnumerator CChangeImage()
	{
		float pom = 0;
		transform.GetChild(1).gameObject.SetActive(true);
		Image img1 =  transform.GetChild(0).GetComponent<Image>();
		Image img2 =  transform.GetChild(1).GetComponent<Image>();
		img2.color = Color.clear;
		Color c1 =img1.color;
		while(pom<1)
		{
			img1.color = new Color(c1.r,c1.g,c1.b,1- pom);
			img2.color = new Color(c1.r,c1.g,c1.b, pom);
			pom+=2*Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		img2.color = c1;
		transform.GetChild(0).gameObject.SetActive(false);
		yield return new WaitForSeconds(.2f);
		bShapeCookie = true;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if(  !bShapeCookie) return;
	 	//ZAMENA OBLIKA
		if(shapeNo==1)
		{
			StartCoroutine("CChangeShape");
		}
	}
 
	IEnumerator CChangeShape( )
	{
		shapeNo=2;
		float pom = 0;
		transform.GetChild(2).gameObject.SetActive(true);
		Image img1 =  transform.GetChild(1).GetComponent<Image>();
		Image img2 =  transform.GetChild(2).GetComponent<Image>();
		Color c1 =img2.color;
		img2.color = Color.clear;

		while(pom<1)
		{
			img1.color = new Color(c1.r,c1.g,c1.b,1- pom);
			img2.color = new Color(c1.r,c1.g,c1.b, pom);
			pom+=2*Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		img2.color = c1;
		transform.GetChild(1).gameObject.SetActive(false);


		transform.GetChild(1).gameObject.SetActive(false);
		transform.GetChild(2).gameObject.SetActive(true);
	//	Camera.main.SendMessage("NextPhase","FC", SendMessageOptions.DontRequireReceiver);
		bIskoriscen = true;
		Camera.main.SendMessage("FinishMakingFortuneCookie");
		yield return new WaitForSeconds(1);
		bEnabled = true;
		StartCoroutine("MoveBack");
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

			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.PlugInSound);
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
			StartCoroutine("MoveBack");
		}
	}



	bool bMovingBack = false;
	IEnumerator MoveBack( )
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
				pom+=Time.fixedDeltaTime *2.5f;
				transform.position = Vector3.Lerp(positionS, StartPosition,pom);
				transform.localScale =  Vector3.Lerp(scaleTmp, StartScale,pom);  
				 
				yield return new WaitForEndOfFrame( );
			}
			bMovingBack = false;
		}
		transform.SetParent (ParentOld);
		transform.localScale = StartScale;
		yield return new WaitForEndOfFrame( );
	}

	 
	bool appFoucs = true;
	void OnApplicationFocus( bool hasFocus )
	{
		if(  !appFoucs && hasFocus )
		{
			if(  bDrag )
			{
				bDrag = false;
				bStretchDough = false;
				StartCoroutine("MoveBack");
			}
		}
		appFoucs = hasFocus;
	}

}

