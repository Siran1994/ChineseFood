using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

	//Quaternion StartRotation;
	Vector3 StartPosition ;
	Vector3 StartScale;

	public float EndScaleFactor;
 
	public bool snapToTarget = false;
	public bool bIskoriscen = false;
	[HideInInspector()]
	public  bool bDrag = false;
	
	public static int OneItemEnabledNo = -1; //-1 dozvoljeni svi, 0 zabranjeni svi, 1,2,3.. dozvoljen samo odgovarajuci item
	public int ItemNo = 0; 
	float x;
	float y;
	Vector3 diffPos = new Vector3(0,0,0);
	public float testDistance =  1.5f;//1; // .25f;


	ParticleSystem psFinishAction;

	PointerEventData pointerEventData;
 
	public Animator animator; 
	bool bAnimationActive = false;
	public string animationType = "";
 
	Transform ParentOld;
	Transform DragItemParent;

	public Transform TestPoint;
	public Transform[] TargetPoint;
	int targetPointIndex = -1;

	public bool bTestOnlyOnEndDrag = true;

	public Image Shadow;
	bool bShowShadow = false;

    public Transform TestTopMovementLimit;
    public Transform TestBotMovementLimit;
    public Transform TopMovementLimit;
    public Transform BotMovementLimit;
	public bool bOnlyEmptyTargetPoints; //da li je potreban uslov da tacka koja se testira ne sadrzi druge objekte
	IEnumerator Start()
	{
		yield return new WaitForSeconds(0.1f);
		
		DragItemParent = GameObject.Find("ActiveItemHolder").transform;
		
		StartPosition  = transform.position;
		
	
		ParentOld = transform.parent;
		
		bIskoriscen = false;
		if(TestPoint == null)	TestPoint = transform.Find("TestPoint");
	}
	
	void Update()
	{ 

		if(bDrag)
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
			if(Vector2.Distance(transform.position,posM) >1)
				transform.position =  Vector3.MoveTowards (transform.position  , posM  , 10 * Time.deltaTime)   ;
			else
				transform.position =  Vector3.Lerp (transform.position  , posM  , 8 * Time.deltaTime)   ;
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
			if(distance2< testDistance &&  distance2 < distance) 
			{
				if(  (bOnlyEmptyTargetPoints && TargetPoint[i].childCount==0) || !bOnlyEmptyTargetPoints)
				{
					closestPoint = i;
					distance = distance2;
				}
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
		if(animationType !="StrainerDish" && animationType !="eatDimSum"   ) 
		{
			OneItemEnabledNo = 0;
			bDrag = false;
			CancelInvoke("TestTarget");
		}
		yield return new WaitForEndOfFrame();
		if(animationType =="PitcherFillWater" && animator != null) 
		{
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
			animator.Play("PitcherFillWater",-1,0);
			Camera.main.SendMessage("FillWater", SendMessageOptions.DontRequireReceiver);
			yield return new WaitForSeconds(.5f);
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.WaterSound);
			 yield return new WaitForSeconds(4.5f);

  
			Camera.main.SendMessage("NextPhase", animationType, SendMessageOptions.DontRequireReceiver);

			sPos = transform.position;
			Vector3 ePos = sPos+ new Vector3(-8.5f,0,0);
			Vector3 arcMax = new Vector3(0,2,0); 

			pom = 0;
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime;
				transform.position = Vector3.Lerp(sPos, ePos,pom)  + pom* (1-pom) *arcMax;

				yield return new WaitForFixedUpdate();
			}

			yield return new WaitForSeconds( .3f);
			GameObject.Destroy(this.gameObject);// gameObject.SetActive (false);

		}
	 
 

		else if(animationType =="Strainer"  ) 
		{
			bDrag = false;
			bIskoriscen = true;
 
			float pom = 0;
			Vector3 sPos = transform.position;
			Vector3 offsetS = transform.position - TestPoint.position;
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime* 3;
				transform.position = Vector3.Lerp(sPos, target.position + offsetS,pom);
				yield return new WaitForFixedUpdate();
			}
			Camera.main.SendMessage("NextPhase", animationType, SendMessageOptions.DontRequireReceiver);
			InvokeRepeating("TestTarget",0f, .1f);
		}
 
		else if(animationType =="Oil" || animationType =="SoySauce" && animator != null) 
		{
			Tutorial.Instance.StopTutorial();
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
			animator.Play(animationType,-1,0);

			yield return new WaitForSeconds(1f);
			Camera.main.SendMessage("NextPhase", animationType, SendMessageOptions.DontRequireReceiver);
			if(SoundManager.Instance!=null)
			{
				if(animationType =="Oil") SoundManager.Instance.Play_Sound(SoundManager.Instance.OilSound);
				else if(animationType =="SoySauce") SoundManager.Instance.Play_Sound(SoundManager.Instance.SauceSound);
			}
			yield return new WaitForSeconds(3.5f);
		 
 
			sPos = transform.position;
			Vector3 ePos = sPos+ new Vector3(-5.5f,0,0);
			Vector3 arcMax = new Vector3(0,2,0); 

			pom = 0;
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime;
				transform.position = Vector3.Lerp(sPos, ePos,pom)  + pom* (1-pom) *arcMax;

				yield return new WaitForFixedUpdate();
			}

			yield return new WaitForSeconds( .3f);
			GameObject.Destroy(this.gameObject);// gameObject.SetActive (false);

		}
 
		else if( (animationType =="Salt" ||  animationType =="Papper" ) && animator != null) 
		{
			Tutorial.Instance.StopTutorial();
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
 
			animator.Play("Flour",-1,0);
			yield return new WaitForSeconds(.5f);
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.SugarSound);
			yield return new WaitForSeconds( 3.0f);

			Camera.main.SendMessage("NextPhase", animationType, SendMessageOptions.DontRequireReceiver);

			sPos = transform.position;
			Vector3 ePos = sPos+ new Vector3(-5.5f,0,0);
			Vector3 arcMax = new Vector3(0,2,0); 

			pom = 0;
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime;
				transform.position = Vector3.Lerp(sPos, ePos,pom)  + pom* (1-pom) *arcMax;

				yield return new WaitForFixedUpdate();
			}

			yield return new WaitForSeconds( .3f);
			Destroy(gameObject);

		}

		else if(animationType.StartsWith("D") && animationType.Length ==2 && animator != null)
		{
			OneItemEnabledNo = 0;
			bIskoriscen = true;
			float pom = 0;
			Vector3 sPos = transform.position;
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime* 2;
				transform.position = Vector3.Lerp(sPos, target.position,pom);
				yield return new WaitForFixedUpdate();
			}

			animator.enabled = true;
			animator.speed = 0;
			//animator.Play(animationType,-1,0);
			transform.parent = TargetPoint[0];
			yield return new WaitForSeconds(.1f);
			Camera.main.SendMessage("NextPhase", animationType, SendMessageOptions.DontRequireReceiver);
		}

  
		else if(animationType =="PitcherFillMold2" && animator != null) 
		{
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
			animator.Play("PitcherFillWater2",-1,0);

			yield return new WaitForSeconds(0.1f);
			Camera.main.SendMessage("NextPhase", "PitcherFillMold", SendMessageOptions.DontRequireReceiver);
			yield return new WaitForSeconds( 2.5f);
			Camera.main.SendMessage("NextPhase", "PitcherFillMoldEnd", SendMessageOptions.DontRequireReceiver);

 

			sPos = transform.position;
			Vector3 ePos = sPos+ new Vector3(-5.5f,0,0);
			Vector3 arcMax = new Vector3(0,2,0); 

			pom = 0;
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime;
				transform.position = Vector3.Lerp(sPos, ePos,pom)  + pom* (1-pom) *arcMax;;
				yield return new WaitForFixedUpdate();
			}

			yield return new WaitForSeconds( .3f);
			gameObject.SetActive (false);
		}

		else if( (animationType =="Flour" ||  animationType =="Sugar" ||  animationType =="Butter" ||   animationType =="Sirup") && animator != null) 
		{
			Tutorial.Instance.StopTutorial();
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

			Camera.main.SendMessage("NextPhase", animationType, SendMessageOptions.DontRequireReceiver);
			if(animationType =="Butter"  )
			{
				animator.Play("Butter",-1,0);

				yield return new WaitForSeconds(1.5f);
				if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound(SoundManager.Instance.KnifeCutSound);
				yield return new WaitForSeconds(2f);
				if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound(SoundManager.Instance.KnifeCutSound);
				yield return new WaitForSeconds(2f);
				if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound(SoundManager.Instance.KnifeCutSound);
				yield return new WaitForSeconds( 2f);
			}
		 
			else if(animationType =="Sirup")
			{
				animator.Play("Sirup",-1,0);
				yield return new WaitForSeconds( 3f);
			}
			else
			{
				animator.Play("Flour",-1,0);
				yield return new WaitForSeconds( 3.5f);
			}

			Camera.main.SendMessage("NextPhase", animationType+"End", SendMessageOptions.DontRequireReceiver);


			sPos = transform.position;
			Vector3 ePos = sPos+ new Vector3(-5.5f,0,0);
			Vector3 arcMax = new Vector3(0,2,0); 

			pom = 0;
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime;
				transform.position = Vector3.Lerp(sPos, ePos,pom)  + pom* (1-pom) *arcMax;;

				yield return new WaitForFixedUpdate();
			}

			yield return new WaitForSeconds( .3f);
			gameObject.SetActive (false);

		}
	 
		else if( animationType =="DoughCutter" && animator != null) 
		{
			bDrag = false;
			float pom = 0;
			animator.Play("Cut",-1,0);
			Vector3 sPos = transform.position;
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime *2f;
				transform.position = Vector3.Lerp(sPos, target.position,pom);
				yield return new WaitForFixedUpdate();
			}

		}

		if(animationType =="JarFlavor")   
		{
			bIskoriscen = true;
			float pom = 0;
			Vector3 sPos = transform.position;
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime* 5;
				transform.position = Vector3.Lerp(sPos, target.position,pom);
				yield return new WaitForFixedUpdate();
			}
			transform.SetParent( target );
			animator.Play("fill",-1,0);
			Camera.main.SendMessage("SelectedFill",int.Parse(transform.name.Substring(3,2)), SendMessageOptions.DontRequireReceiver);
			yield return new WaitForSeconds( 2.5f);

			StartCoroutine("MoveBack");
			//OneItemEnabledNo =-1;
		}

		else if(animationType =="BakeFC")
		{
			bIskoriscen = true;
			float pom = 0;
			Vector3 sPos = transform.position;
			Vector3 sScale = transform.localScale;
			Vector3 eScale = new Vector3(.7f,.5f,.7f);
			//if(EndScaleFactor>0 && transform.name == "Mold2") eScale *=EndScaleFactor;
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime* 2;
				transform.position = Vector3.Lerp(sPos, target.position,pom);
				transform.localScale = Vector3.Lerp(sScale, eScale,pom);
				yield return new WaitForFixedUpdate();
			}


			transform.parent = target;
			Tutorial.Instance.StopTutorial();
			Camera.main.SendMessage( "OvenOn", SendMessageOptions.DontRequireReceiver);

			if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound( SoundManager.Instance.FreezerDoorClose);
			while(GlobalVariables.bPauseGame) yield return new WaitForEndOfFrame();
			if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound( SoundManager.Instance.MachineOnSound);
			yield return new WaitForSeconds(.5f);
			while(GlobalVariables.bPauseGame) yield return new WaitForEndOfFrame();
			yield return new WaitForSeconds(.5f);
			while(GlobalVariables.bPauseGame) yield return new WaitForEndOfFrame();
			yield return new WaitForSeconds(.5f);
			while(GlobalVariables.bPauseGame) yield return new WaitForEndOfFrame();
			yield return new WaitForSeconds(.5f);
			while(GlobalVariables.bPauseGame) yield return new WaitForEndOfFrame();
			yield return new WaitForSeconds(.5f);
			while(GlobalVariables.bPauseGame) yield return new WaitForEndOfFrame();
			yield return new WaitForSeconds(.5f);

			while(GlobalVariables.bPauseGame) yield return new WaitForEndOfFrame();
			Camera.main.SendMessage( "ChangeImages", SendMessageOptions.DontRequireReceiver);

			while(GlobalVariables.bPauseGame) yield return new WaitForEndOfFrame();
			yield return new WaitForSeconds(.5f);
			while(GlobalVariables.bPauseGame) yield return new WaitForEndOfFrame();
			yield return new WaitForSeconds(.5f);
			while(GlobalVariables.bPauseGame) yield return new WaitForEndOfFrame();
			yield return new WaitForSeconds(.5f);
			while(GlobalVariables.bPauseGame) yield return new WaitForEndOfFrame();
			yield return new WaitForSeconds(.5f);
			while(GlobalVariables.bPauseGame) yield return new WaitForEndOfFrame();
			yield return new WaitForSeconds(.5f);
			while(GlobalVariables.bPauseGame) yield return new WaitForEndOfFrame();
			yield return new WaitForSeconds(.5f);

			if(SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound( SoundManager.Instance.MachineOnSound);
			if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound( SoundManager.Instance.FreezerDoorClose);
			Camera.main.SendMessage( "StoveOff", SendMessageOptions.DontRequireReceiver);
			while(GlobalVariables.bPauseGame) yield return new WaitForEndOfFrame();



			transform.parent = ParentOld;
			sPos = transform.position;
			Vector3 ePos = Vector3.zero;
			sPos = transform.position;

			sScale = transform.localScale;
			eScale = new Vector3(1.5f,1.5f,1.5f);
			//if(EndScaleFactor>0 && transform.name == "Mold2") eScale *=EndScaleFactor;
			while(GlobalVariables.bPauseGame) yield return new WaitForEndOfFrame();
			pom = 0;
			while(pom<1)
			{
				while(GlobalVariables.bPauseGame) yield return new WaitForEndOfFrame();
				pom+=Time.fixedDeltaTime* .8f;
				transform.position = Vector3.Lerp(sPos, ePos,pom);
				transform.localScale = Vector3.Lerp(sScale, eScale,pom);
				yield return new WaitForFixedUpdate();
			}
			this.enabled = false;
		}

		if(animationType =="FortuneMessage")   
		{
			OneItemEnabledNo = 0;
			float pom = 0;
			transform.SetParent( transform.parent.Find("FortuneCookie/fc1")  );
			Vector3 sPos = transform.localPosition;
			Vector3 sScale = Vector3.one;
			Vector3 eScale = sScale*.7f;
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime* 3;
				transform.localPosition = Vector3.Lerp(sPos, Vector3.zero,pom);
				transform.localScale = Vector3.Lerp(sScale, eScale ,pom);
				yield return new WaitForFixedUpdate();
			}
			 
			yield return new WaitForSeconds( 0.5f);
			transform.parent.parent.SendMessage("AddedMessage", SendMessageOptions.DontRequireReceiver);
			yield return new WaitForSeconds( 0.1f);

			transform.SetParent(ParentOld);
			transform.position = StartPosition;
			transform.localScale = Vector3.one;
		}

		else if(animationType =="FortuneCookieChocolate"  ) 
		{
			OneItemEnabledNo = 0;
			bDrag = false;
			bIskoriscen = true;

			float pom = 0;
			Vector3 sPos = transform.position;
			Vector3 offsetS = transform.position - TestPoint.position;
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime* 3;
				transform.position = Vector3.Lerp(sPos, target.position + offsetS,pom);
				yield return new WaitForFixedUpdate();
			}

			animator.enabled = true;
			animator.Play("FortuneCookieChocolate",-1,0);
  

			yield return new WaitForSeconds(3.5f);
			Camera.main.SendMessage("NextPhase", "FCChocolateDone", SendMessageOptions.DontRequireReceiver);
			StartCoroutine("MoveBack" );
			OneItemEnabledNo = 1;
		}

		else if(animationType =="BSTop"  ) 
		{
			OneItemEnabledNo = 0;
			bDrag = false;
			bIskoriscen = true;

			float pom = 0;
			Vector3 sPos = transform.position;
			Vector3 offsetS = transform.position - TestPoint.position;
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime* 1.5f;
				transform.position = Vector3.Lerp(sPos, target.position + offsetS,pom);
				transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one,pom);
				yield return new WaitForFixedUpdate();
			}
			 
			Camera.main.SendMessage("NextPhase", "BSTop", SendMessageOptions.DontRequireReceiver);
		 
		}
		else if(animationType =="eatDimSum")
		{
			OneItemEnabledNo = 2;
			ItemNo = 2;
			bDrag = false;
			bIskoriscen = true;
			ParentOld = transform.parent;
			StartPosition = ParentOld.position;
			float pom = 0;
			Vector3 sPos = transform.position;
			//Vector3 offsetS = transform.position - TestPoint.position;
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime* 1.5f;
				transform.position = Vector3.Lerp(sPos, target.position  ,pom);
				yield return new WaitForFixedUpdate();
			}

			Camera.main.SendMessage("NextPhase", "InSauce", SendMessageOptions.DontRequireReceiver);

			animator.Play("sauce");
			yield return new WaitForSeconds(2);
			bIskoriscen = false;

			bDrag = pointerEventData.dragging;
			animationType = "eatDimSum2";
		}
		else if(animationType =="eatDimSum2")
		{
			bIskoriscen = false;
			bDrag = false;
			Camera.main.SendMessage("NextPhase", "DSEat", SendMessageOptions.DontRequireReceiver);
		}


		else if(animationType =="Ladle"  ) 
		{
			OneItemEnabledNo = 1;
			 
			bDrag = false;
			bIskoriscen = true;
			ParentOld = transform.parent;
			StartPosition = ParentOld.position;
 
			float pom = 0;
			Vector3 sPos = transform.position;
			Vector3 offsetS = transform.position - TestPoint.position;
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime* 3;
				transform.position = Vector3.Lerp(sPos, target.position + offsetS,pom);
				yield return new WaitForFixedUpdate();
			}
 
			Camera.main.SendMessage("NextPhase", animationType, SendMessageOptions.DontRequireReceiver);
			animator.Play("Ladle");
			yield return new WaitForSeconds(1.5f);
			//PONOVNO AKTIVIRANJE POMERANJA
			InvokeRepeating("TestTarget",0f, .1f);
			bIskoriscen = false;
			bDrag = pointerEventData.dragging;
			animationType = "Ladle2";
		}
		else if(animationType =="Ladle2"  ) 
		{
			bDrag = false;
			bIskoriscen = true;

			float pom = 0;
			Vector3 sPos = transform.position;
			Vector3 offsetS = transform.position - TestPoint.position;
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime* 3;
				transform.position = Vector3.Lerp(sPos, target.position + offsetS,pom);
				yield return new WaitForFixedUpdate();
			}


			animator.Play("Ladle2");
			Camera.main.SendMessage("NextPhase", animationType, SendMessageOptions.DontRequireReceiver);
		}

	}

 
	//--------------------------------------------------------------------------------------------------------------------------------------------
	public void CutDough()
	{
		 
		Camera.main.GetComponent<FortuneCookieStretchAndCutDough>().CutDough(  TargetPoint[targetPointIndex]);
		StartMoveBack();
 

		 
	}


	public void PitcherFillWater()
	{
	}
 

	public void AnimationFinished()
	{
	  if( !bMovingBack)
		{
			StopAllCoroutines();
			StartMoveBack();
		}
	}
 

	public void OnBeginDrag (PointerEventData eventData)
	{
		if(  bIskoriscen || bMovingBack) return;
		StopAllCoroutines(); 
		pointerEventData = eventData;
		bAnimationActive = false;
		if(OneItemEnabledNo >-1 && ItemNo != OneItemEnabledNo)
		{
			bDrag = false;
			return;
		}

		if(  !bIskoriscen   && !bDrag  )
		{
		 
			//transform.localScale = 1.4f*Vector3.one;
			//AnimationChild.transform.parent.rotation = Quaternion.Euler(0,0,0);
			 
			bDrag = true;
			StartPosition = transform.position;
			diffPos =transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)   ;
			diffPos = new Vector3(diffPos.x,diffPos.y,0);
			if(DragItemParent == null) DragItemParent = GameObject.Find("ActiveItemHolder").transform;
			if(ParentOld == null) ParentOld = transform.parent;
			//DragItemParent.position = Vector3.zero;
			//DragItemParent.position = transform.parent .position;


			transform.SetParent(DragItemParent);

			if(!bTestOnlyOnEndDrag) { CancelInvoke(); InvokeRepeating("TestTarget",0f, .1f); }
			
			//if(animationType =="PitcherFillWater" && animator != null)
			//{
				//Tutorial.Instance.StopTutorial();
				//StartCoroutine("HideShadow");
			//}
			if(animationType =="FortuneCookieChocolate"  ||  animationType =="BSTop" )
			{
				if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound( SoundManager.Instance.PlugInSound);
			}
		 
		}
	}
	



	public void OnDrag (PointerEventData eventData)
	{
		//pointerEventData = eventData;
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
	}
	
	
	
	bool bMovingBack = false;
	IEnumerator MoveBack(  )
	{
		if(!bMovingBack)
		{
			bMovingBack = true;
			
			if(animationType =="PitcherFillWater" && Shadow!=null)  bShowShadow = true;
			yield return new WaitForEndOfFrame( );
		 
			float pom = 0;
			Vector3 positionS = transform.position;
		
			while(pom<1 )
			{ 
				pom+=Time.fixedDeltaTime*2;
				transform.position = Vector3.Lerp(positionS, StartPosition,pom);
				 
				//	if(transform.localScale.x >  1) transform.localScale =  (1.4f -  pom)*Vector3.one;
				//	else transform.localScale =  Vector3.one;
				 
				yield return new WaitForFixedUpdate( );

				if(  bShowShadow &&  pom>0.8f )
				{
					yield return new WaitForEndOfFrame();
					Shadow.color = new Color(1,1,1,(pom-.8f)*5);
				}
			}
			
			transform.SetParent(ParentOld);
			transform.position = StartPosition;

			//activeToolNo = 0;
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
		CancelInvoke("TestTarget");
		StartCoroutine("MoveBack" );
	}

	IEnumerator HideShadow()
	{
		if(Shadow!=null)
		{
			float f = 1;
			while( f>0)
			{
				f -=Time.deltaTime*10;
				yield return new WaitForEndOfFrame();
				Shadow.color = new Color(1,1,1,f);
			}
		}
		yield return new WaitForEndOfFrame();
	}

	IEnumerator ShowShadow()
	{	
		if(Shadow!=null)
		{
			float f = 0;
			while( f<1)
			{
				f +=Time.deltaTime*5;
				yield return new WaitForEndOfFrame();
				Shadow.color = new Color(1,1,1,f);
			}
		}
		yield return new WaitForEndOfFrame();
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
