using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Mixer :   MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler 
{
	public Animator animMixer;

	Quaternion StartRotation;
	Quaternion DragRotation2;
	Vector3 StartPosition ;
	Vector3 StartScale;

	public float EndScaleFactor;
	public int DragRotation;
 


	public static bool bEnabled = false;
	[HideInInspector()]
	public  bool bDrag = false;

	 
	float x;
	float y;
	Vector3 diffPos = new Vector3(0,0,0);
	public float testDistance =  1.5f;//1; // .25f;
	Vector3 diffTP = new Vector3(0,0,0);
 
 
 
	Transform ParentOld;
	public Transform BowlParent;
	public Transform TestPoint;
	public Transform ParentActiveItem;

	//koristi se za ispitivanje da li da se prebaci u posudu ili ne
	public Transform LimitTopLeft;
	public Transform LimitBotomRight;
	public   bool bInBowl = false;
	public static  bool bMixBowl = false;
	Vector3 tmpPosM;

	public static bool bHandMixer = false;

    public Transform TestTopMovementLimit;
	public Transform TestBotMovementLimit;
    public Transform TopMovementLimit;
	public Transform BotMovementLimit;

	public static bool bMixerUnlocked = false;

	IEnumerator Start()
	{
		yield return new WaitForSeconds(0.1f);
		Init();
		 
	}

	public void Init()
	{
		if(BowlParent ==null) BowlParent = GameObject.Find("BowlHolder/BowlAnimationHolder/MixerHolder").transform;

		StartPosition  = transform.position;
		StartScale = transform.localScale;
		StartRotation = transform.rotation;

		DragRotation2 = Quaternion.Euler(0,0,DragRotation);

		ParentOld = transform.parent;

		//bIskoriscen = false;
		if(TestPoint == null)	TestPoint = transform.Find("TestPoint");
	}

	void Update()
	{ 
		if(  bDrag )
		{

			x = Input.mousePosition.x;
			y = Input.mousePosition.y;

			Vector3 posM = Camera.main.ScreenToWorldPoint(new Vector3(x ,y,10f) ) + diffPos;
			diffTP = transform.position - TestPoint.position;

			 tmpPosM = posM- diffTP;

			if( bInBowl  && tmpPosM.x<LimitTopLeft.position.x-0.01f ) posM = new Vector3(  LimitTopLeft.position.x ,tmpPosM.y, transform.position.z) + diffTP;	
			else if( bInBowl  && tmpPosM.x>LimitBotomRight.position.x + 0.01f  ) posM = new Vector3(  LimitBotomRight.position.x , tmpPosM.y, transform.position.z) + diffTP;
			if( bInBowl  && tmpPosM.y<LimitBotomRight.position.y + 0.01f  ) posM = new Vector3(   posM.x,  LimitBotomRight.position.y + diffTP.y, transform.position.z) ;

			 
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
			transform.rotation = Quaternion.Lerp (transform.rotation,  DragRotation2, 10 * Time.deltaTime);
//            if(animMixer==null)  bMixBowl = false;
        }
    }


	void TestTarget()
	{ 
		if(  !bEnabled) return;

		//test da li je u ciniji
 
		if( TestPoint.position.x>LimitTopLeft.position.x &&  TestPoint.position.x<LimitBotomRight.position.x
			&&  TestPoint.position.y<LimitTopLeft.position.y &&  TestPoint.position.y>LimitBotomRight.position.y)
		{
			transform.SetParent(BowlParent);
			bInBowl = true;
			if(animMixer!=null)  bMixBowl = true;
			Tutorial.Instance.StopTutorial();
		}
		else
		{
			transform.SetParent(ParentActiveItem);
			bInBowl = false;
			bMixBowl = false;
		}
	}


	public void OnBeginDrag (PointerEventData eventData)
	{
		if( !bEnabled || bMovingBack ) return;
		StopAllCoroutines(); 

		if(  !bDrag  )
		{
			 
			transform.localScale = EndScaleFactor*StartScale;
			bDrag = true;
			//StartPosition = transform.position;
			transform.SetParent(ParentActiveItem);

			diffPos =transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)   ;
			diffPos = new Vector3(diffPos.x,diffPos.y,0);

			InvokeRepeating("TestTarget",0f, .1f);
 
 //			Tutorial.Instance.StopTutorial();
			if(animMixer!=null) 
			{
				animMixer.enabled= true;
				bHandMixer = true;
			}
			else bHandMixer = false;
				
			Tutorial.Instance.StopCoroutine("ShowMixerTut");
			Tutorial.Instance.StopTutorial();
		}
	}
	 

	public void OnDrag (PointerEventData eventData)
	{
		if(  bInBowl &&  !bMovingBack )  
			bMixBowl = true;
	}

	public void OnEndDrag (PointerEventData eventData)
	{
		if( bDrag)  
		{
			
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

			bInBowl = false;
			bMixBowl = false;

			if(animMixer!=null) animMixer.enabled= false;
			float pom = 0;
			Vector3 positionS = transform.position;
			while(pom<1f )
			{ 
				pom+=Time.fixedDeltaTime *1.5f;
				transform.position = Vector3.Lerp(positionS, StartPosition,pom);

				yield return new WaitForEndOfFrame( );
				 
				if( pom>.6f && EndScaleFactor !=1 ) transform.localScale =  (2f - pom)*Vector3.one;
				 
				transform.rotation = Quaternion.Lerp (DragRotation2,  StartRotation, pom);
			}

			transform.SetParent(ParentOld);
			transform.position = StartPosition;

			pom = 0;
			 

			bMovingBack = false;
			//if(!bIskoriscen) 
			//{

				//transform.FindChild("Finished").GetComponent<Image>().enabled = true;
				//SoundManager.Instance.StopAndPlay_Sound(SoundManager.Instance.CleaningFinished);
		//	}
		}
		yield return new WaitForFixedUpdate( );
	}

	public void EndMixing()
	{
		bInBowl = false;
		bMixBowl = false;

		bDrag = false;
		CancelInvoke("TestTarget");
		StartCoroutine("MoveBack" );
		bEnabled = false;
	}


	bool appFoucs = true;
	void OnApplicationFocus( bool hasFocus )
	{
		if(  !appFoucs && hasFocus )
		{
			if(   bDrag )
			{
				bDrag = false;

				CancelInvoke("TestTarget");
				StartCoroutine("MoveBack" );
			}
		}
		appFoucs = hasFocus;

	}
	 
}
