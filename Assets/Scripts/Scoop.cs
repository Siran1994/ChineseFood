using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Scoop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	Quaternion StartRotation;
	Vector3 StartPosition ;
 
	public bool snapToTarget = false;

	public bool bIskoriscen = false;
	[HideInInspector()]
	public  bool bDrag = false;

	 
	public int ItemNo = 0; 
	float x;
	float y;
	Vector3 diffPos = new Vector3(0,0,0);
	float testDistance1 =   0.5f;
	float testDistance =  0.9f;

	PointerEventData pointerEventData;
	public Animator animScoop; 
 
	public Transform TestPoint;
	public Transform TargetPointDish;
	public Transform[] TargetPointsMold;

 
	public Transform TestTopMovementLimit;
	public Transform TopMovementLimit;

	public bool bEnableDrag = true;

	bool bTakeFill = false;
	public Image imgFillInScoop; //OVO JE SLICICA MESA KOJE SE NALAZI U KUTLACI
	public Image imgFillOnDough; //OVA SE KORISTI PRILIKOM SPUSTANJA NA RAZVUCENO TESTO
 
	bool bTmpDrag;
	bool bMold = false;
	int targetPointIndex = -1;

	public int choclateDropsInMold = 4;
 
	public void Start()//InitStart()
	{
		

		StartRotation =  transform.localRotation;
		TestPoint.localScale = Vector3.zero;
		imgFillInScoop.enabled = false;
 
		StartPosition  = GameObject.Find("ScoopP1").transform.position;
		bIskoriscen = false;
	}

	void Update()
	{ 
		
		if( !bIskoriscen &&  bDrag  &&  bEnableDrag  )
		{
			x = Input.mousePosition.x;
			y = Input.mousePosition.y;

			Vector3 posM = Camera.main.ScreenToWorldPoint(new Vector3(x ,y,10f) ) + diffPos;

			if(TestTopMovementLimit!=null && TopMovementLimit!=null)
			{
				float diffTL = TestTopMovementLimit.position.y-transform.position.y;
				if(posM.y+ diffTL>TopMovementLimit.position.y) posM = new Vector3(posM.x, TopMovementLimit.position.y - diffTL ,posM.z);
			}
			transform.position =  Vector3.Lerp (transform.position  , posM  , 10 * Time.deltaTime)   ;
		}
	}


	void TestTarget()
	{ 
		if(  bIskoriscen) return;
	 
		 
		if(!bTakeFill)
		{
			float distance = 0;
			//UZIMANJE  IZ CINIJE
			distance = Vector2.Distance(TestPoint.position, TargetPointDish.position);
			if(distance< testDistance && imgFillInScoop.transform.localScale.x<1  ) 
			{
				if(!imgFillInScoop.enabled ) 	StartCoroutine("CTakeFill");
				 
				imgFillInScoop.transform.localScale +=.05f*Vector3.one;
				if(imgFillInScoop.transform.localScale.x >=1)
				{
					bTakeFill = true;
					imgFillInScoop.transform.localScale = Vector3.one;
					//SOUND
				}
			}
		}
		else
		{
			  
			int closestPoint= -1;
			float distance = 1000;
			float distance2 = 0;


			for(int i= 0;i<TargetPointsMold.Length; i++)
			{
				if(TargetPointsMold[i] == null) continue;
				distance2 = Vector2.Distance(TestPoint.position,TargetPointsMold[i].position);
				if(distance2< testDistance1 &&  distance2 < distance ) //&& !TargetPointsMold[i].GetChild(0).GetComponent<Image>().enabled)
				{
					closestPoint = i;
					distance = distance2;
				}
			}
			targetPointIndex = closestPoint;

			if(targetPointIndex >-1)
			{
				StartCoroutine("CFillDimSum",TargetPointsMold[targetPointIndex]);
			}
 
		 
		}
	}


	IEnumerator CTakeFill()
	{
		//Tutorial.Instance.StopTutorial();
		 
		imgFillInScoop.enabled = true;
		Color c = imgFillInScoop.color;
		imgFillInScoop.color = new Color(c.r,c.g,c.b,0);
		imgFillInScoop.transform.localScale   =.45f*Vector3.one;
		float pom = 0;
		while(pom<1)
		{
			pom += Time.deltaTime*4;
			imgFillInScoop.color = new Color(c.r,c.g,c.b, pom);
			yield return new WaitForEndOfFrame();
		}
		imgFillInScoop.color = new Color(c.r,c.g,c.b,1);
		yield return new WaitForEndOfFrame();

 
	 	//Tutorial.Instance.ShowTutorial(1);
	}


	IEnumerator CFillDimSum( )
	{
		Tutorial.Instance.StopTutorial();
		bMold = true; 
		bTmpDrag = bDrag;

		 
		bDrag = false;
		CancelInvoke("TestTarget");
		bIskoriscen = true;
		yield return new WaitForEndOfFrame();
		Vector3 rot = new Vector3( 0,0,50);
		float pom = 0;
 
		imgFillOnDough.gameObject.SetActive(true);
		Transform imgF = imgFillOnDough.transform;
		Transform parentOld = imgF.parent;
		imgF.SetParent(imgFillInScoop.transform.parent);
		imgF.position = imgFillInScoop.transform.position;
		imgF.localScale = imgFillInScoop.transform.localScale;
		imgF.rotation = imgFillInScoop.transform.rotation;
		yield return new WaitForEndOfFrame();
		imgFillInScoop.transform.localScale = Vector3.zero;
		animScoop.Play("Rotate");

		//Vector3 snapPos = TargetPointsMold[targetPointIndex].position + (transform.position - TestPoint.position);
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.InsertFruit);
		while(pom<1)
		{
			pom+=.8f*Time.deltaTime ;
			//transform.position = Vector3.Lerp(transform.position, snapPos, pom);
			imgF.position = Vector3.Lerp( imgFillInScoop.transform.position, parentOld.position, pom);
			imgF.rotation = Quaternion.Lerp( imgF.rotation, Quaternion.identity, pom);
			yield return new WaitForEndOfFrame();
		}
		//transform.position = snapPos;
		imgF.SetParent(parentOld);
		imgF.SetAsFirstSibling();
		imgF.localPosition = Vector3.zero; 
	}

	public void AnimFillDimSumStart()
	{
 
		//StartCoroutine("CPutFillDimSum");
	}

//	IEnumerator CPutFillDimSum()
//	{
//		yield return new WaitForEndOfFrame();
//	}

	bool bShowTut = true;
	public void AnimFillDimSumEnd()
	{
		Debug.Log("AnimFillDimSumEnd");
		//if(SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound(SoundManager.Instance.WaterSound);

		imgFillInScoop.enabled = false;
		Color c = imgFillInScoop.color;
		imgFillInScoop.color = new Color(c.r,c.g,c.b,0);
		imgFillInScoop.transform.localScale   =.45f*Vector3.one;

		bMold = false; 
		bDrag = false;//bTmpDrag; -- NEMA NASTAVLJANJA DRZANJA KASIKE
		bTakeFill = false;
		bIskoriscen = true;

		imgFillOnDough.GetComponentInParent<ItemAction>().enabled = true;
		if(bShowTut)
		{
			bShowTut = false;
			Tutorial.Instance.ShowTutorial(4);
		}
		StartCoroutine("MoveBack" );
		//
		//TargetPointsMold[targetPointIndex] = null;
		//targetPointIndex = -1;
	}
 

	public void OnBeginDrag (PointerEventData eventData)
	{
		if(  !bEnableDrag || bIskoriscen ) return;

		if(bMovingBack) return;
		StopAllCoroutines(); 
		pointerEventData = eventData;

		if(  !bIskoriscen   && !bDrag  )
		{

			bDrag = true;
			//StartPosition = transform.position;
			diffPos =transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)   ;
			diffPos = new Vector3(diffPos.x,diffPos.y,0);
 
		}
	}




	public void OnDrag (PointerEventData eventData)
	{
		if(bDrag) TestTarget();
	}

	public void OnEndDrag (PointerEventData eventData)
	{
		if(bDrag) 
		{
			bDrag = false;
			bTmpDrag = false;
			StopAllCoroutines();
			StartCoroutine("MoveBack" );
		}
		if(bMold)  
		{
			bDrag = false;
			bTmpDrag = false;
		 
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
			Quaternion rotTmp = transform.localRotation;
			while(pom<1 )
			{ 
				pom+=Time.fixedDeltaTime*2;
				transform.position = Vector3.Lerp(positionS, StartPosition,pom);
				transform.localRotation = Quaternion.Lerp(rotTmp, StartRotation, pom);
				yield return new WaitForFixedUpdate( );
			}

			transform.position = StartPosition;

			bMovingBack = false;
			if(bIskoriscen) 
			{

				//transform.FindChild("Finished").GetComponent<Image>().enabled = true;
				//SoundManager.Instance.StopAndPlay_Sound(SoundManager.Instance.CleaningFinished);
			}
		}

	}

	public void StartMoveBack()
	{
		StopAllCoroutines();
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
				bTmpDrag = false; 
				//StopAllCoroutines();
				StartCoroutine("MoveBack" );
			}
		}
		appFoucs = hasFocus;

	}



}
