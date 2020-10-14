using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SweetDumplingsSugar : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
 
	Transform TestPoint;
    Transform TestTopMovementLimit;
	Transform TestBotMovementLimit;
    public Transform TopMovementLimit;
	public Transform BotMovementLimit;

	public Transform TrayLimitTopLeft;
	public Transform TrayLimitBottomRight;
	public Transform ActiveItemHolder;

	Quaternion StartRotation;
	Vector3 StartPosition ;
	Transform StartParent;

	public bool bIskoriscen = false;
	[HideInInspector()]
	public  bool bDrag = false;
 
	float x;
	float y;
	Vector3 diffPos = new Vector3(0,0,0);
	public static bool bShowTut = true;

	PointerEventData pointerEventData;

	public bool bEnableDrag = true;
	bool bInSugar = false;
	 

	float sugarAlpha = 0; 
 
	public Image sugar;
	float sign = 1;

	public static bool bFirstTut = true;
	void Start () {
		
		TestPoint = transform;
        TestTopMovementLimit = TestPoint;
		TestBotMovementLimit = TestPoint;
		StartRotation = transform.rotation;
		StartPosition  = transform.position;
		StartParent    = transform.parent;
	}
	
	 
	void Update()
	{ 
		if(   bDrag  &&  bEnableDrag  )
		{
			x = Input.mousePosition.x;
			y = Input.mousePosition.y;

			Vector3 posM = Camera.main.ScreenToWorldPoint(new Vector3(x ,y,10f) ) + diffPos;
			if(bInSugar)
			{
				if(posM.y >TrayLimitTopLeft.position.y) posM = new Vector3(posM.x, TrayLimitTopLeft.position.y ,posM.z);
				if(posM.y <TrayLimitBottomRight.position.y) posM = new Vector3(posM.x, TrayLimitBottomRight.position.y ,posM.z);

				if(posM.x <TrayLimitTopLeft.position.x) posM = new Vector3( TrayLimitTopLeft.position.x , posM.y, posM.z);
				if(posM.x >TrayLimitBottomRight.position.x) posM = new Vector3( TrayLimitBottomRight.position.x , posM.y, posM.z);
 				
			}
            else if(TestTopMovementLimit!=null && TopMovementLimit!=null && TestTopMovementLimit!=null && TopMovementLimit!=null)
			{
				if(posM.y >TopMovementLimit.position.y) posM = new Vector3(posM.x, TopMovementLimit.position.y ,posM.z);
                if(posM.y <BotMovementLimit.position.y) posM = new Vector3(posM.x, BotMovementLimit.position.y ,posM.z);
            }
			transform.position =  Vector3.Lerp (transform.position  , posM  , 10 * Time.deltaTime)   ;
		}
	}

	//testita se da li je u posluzavniku sa secerom
	void TestTarget()
	{
		if(TestPoint.position.y<TrayLimitTopLeft.position.y  &&  TestPoint.position.y> TrayLimitBottomRight.position.y && TestPoint.position.x>TrayLimitTopLeft.position.x  &&  TestPoint.position.x < TrayLimitBottomRight.position.x )
		{
			bInSugar = true;
			CancelInvoke("TestTarget");
		 
		}
	}

	public void OnBeginDrag (PointerEventData eventData)
	{
		if(  !bEnableDrag || bIskoriscen ) return;
		if(ActiveItemHolder.childCount >0 && ActiveItemHolder.GetChild(0) != transform) return;


		if(bMovingBack) return;
		StopAllCoroutines(); 
		pointerEventData = eventData;
		sign = Random.Range(0,2)*2-1;
 
		if(  !bIskoriscen   && !bDrag  )
		{

			bDrag = true;
			 
			transform.SetParent(ActiveItemHolder);
			diffPos =transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)   ;
			diffPos = new Vector3(diffPos.x,diffPos.y,0);

			Tutorial.Instance.StopTutorial();

			InvokeRepeating("TestTarget",.1f,.1f);
		}
	}




	public void OnDrag (PointerEventData eventData)
	{
		if(bInSugar)
		{
			//eventData.delta
			if(sugarAlpha <1)
			{
				if(sugarAlpha == 0 ) Tutorial.Instance.StopTutorial();
				if( bFirstTut && sugarAlpha == 0)
				{
					Tutorial.Instance.ShowTutorial(1);
				}
				else if(bFirstTut && sugarAlpha>.3f)
				{
					Tutorial.Instance.StopTutorial();
					bFirstTut = false;

				}

				sugarAlpha += Time.deltaTime*.5f;
				sugar.enabled = true;
				Color c = sugar.color;
				sugar.color = new Color(c.r,c.g,c.b,sugarAlpha);
				transform.Rotate(new Vector3(0,0, sign*Time.deltaTime*500)); 
				if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.RollInSugarSound);
			}
			else 
			{
				bDrag = false;
				bIskoriscen = true;
				bInSugar = false;
				Camera.main.SendMessage("FinishAddingSugar");
				StartMoveBack();
				if(SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound(SoundManager.Instance.RollInSugarSound);
			}
		}
	}

	public void OnEndDrag (PointerEventData eventData)
	{
		if(bDrag) 
		{
			bDrag = false;
			if(!bInSugar)
			{
				if(SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound(SoundManager.Instance.RollInSugarSound);
				StopAllCoroutines();
				StartCoroutine("MoveBack");
			}
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
			transform.SetParent(StartParent);
			bMovingBack = false;
			 
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
				StopAllCoroutines();
				StartCoroutine("MoveBack" );
			}
		}
		appFoucs = hasFocus;

	}

}
