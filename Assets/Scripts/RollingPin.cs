using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RollingPin : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler 
{
	Quaternion StartRotation;
	Quaternion DragRotation2;
	Vector3 StartPosition ;
	Vector3 StartScale;
	Vector3 EndScale;

	public float EndScaleFactor;
	public int DragRotation;
 
	public  bool bEnabled = false;
	[HideInInspector()]
	public bool bDrag = false;
	public bool bStretchDough = false;

	float x;
	float y;
	Vector3 diffPos = new Vector3(0,0,0);
	float limitDistance = 0;
 
	float normDistance = 0;

 
	public Transform LimitTopLeft;
	public Transform LimitBotomRight;
 
	//Vector3 tmpPosM;



	IEnumerator Start()
	{
		yield return new WaitForSeconds(0.1f);
 
		StartPosition  = transform.position;
		StartScale = transform.localScale;
		StartRotation = transform.rotation;
		EndScale = StartScale*EndScaleFactor;
		limitDistance = LimitTopLeft.position.y - LimitBotomRight.position.y;
	}

	void Update()
	{ 
		if(  bDrag )
		{
			normDistance =     (  transform.position.y - LimitBotomRight.position.y  )  /limitDistance ;
			x = Input.mousePosition.x;
			y = Input.mousePosition.y;

			Vector3 posM = Camera.main.ScreenToWorldPoint(new Vector3(x ,y,10f) ) + diffPos;


			if(  posM.x<LimitTopLeft.position.x  ) posM = new Vector3(  LimitTopLeft.position.x ,posM.y, transform.position.z)  ;	
			else if(  posM.x>LimitBotomRight.position.x   ) posM = new Vector3(  LimitBotomRight.position.x , posM.y, transform.position.z) ;

			if(  posM.y>LimitTopLeft.position.y  ) posM = new Vector3(  posM.x, LimitTopLeft.position.y , transform.position.z)  ;	
			else if(  posM.y<LimitBotomRight.position.y  ) posM = new Vector3(  posM.x, LimitBotomRight.position.y,  transform.position.z) ;

			 

			transform.position =  Vector3.Lerp (transform.position  , posM  , 10 * Time.deltaTime) ;
			transform.rotation = Quaternion.Euler(0,0, ( StartPosition.x - posM.x) *5 * normDistance);
			transform.localScale = Vector3.Lerp( StartScale,EndScale,normDistance );

		}
	}

	 
	public void OnBeginDrag (PointerEventData eventData)
	{
		if( !bEnabled || bMovingBack ) return;
		StopAllCoroutines(); 
 
		if(  !bDrag  )
		{
			transform.localScale =  StartScale;
			bDrag = true;
 
			diffPos =transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)   ;
			diffPos = new Vector3(diffPos.x,diffPos.y,0);
 
			 Tutorial.Instance.StopTutorial();
	 
		}
	}




	public void OnDrag (PointerEventData eventData)
	{
		if( !bMovingBack ) 
		{
			if(bDrag && bEnabled)
			{
				if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.RollingPinSound);
				bStretchDough = true;
				if (OnStretchDough != null)
					OnStretchDough();
			}
		}
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
			if(SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound(SoundManager.Instance.RollingPinSound);
			bMovingBack = true;
 
			float pom = 0;
			Vector3 positionS = transform.position;
			Vector3 scaleTmp = transform.localScale ;
			while(pom<1f )
			{ 
				pom+=Time.fixedDeltaTime *1.5f;
				transform.position = Vector3.Lerp(positionS, StartPosition,pom);
				transform.localScale =  Vector3.Lerp(scaleTmp, StartScale,pom);  
			

				//transform.rotation = Quaternion.Lerp (DragRotation2,  StartRotation, pom);
				yield return new WaitForEndOfFrame( );
			}
 
			bMovingBack = false;
			 
		}
		yield return new WaitForFixedUpdate( );
	}

 
	public void EndStretching()
	{
		bStretchDough = false;
		bDrag = false;
		StartCoroutine("MoveBack" );
		bEnabled = false;
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
				StartCoroutine("MoveBack" );
			}
		}
		appFoucs = hasFocus;

	}

	//--------------------------------------

	public delegate void StretchDoughAction();
	public static event StretchDoughAction OnStretchDough;

	void OnDestroy()
	{
		if(SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound(SoundManager.Instance.RollingPinSound);
		OnStretchDough = null;
	}
}
