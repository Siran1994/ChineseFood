using UnityEngine;
using UnityEngine.UI;
using System.Collections;
//using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ItemAction : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	 
	public  bool bEnabled = true;

	public bool bClickForAction = false;
	public bool bDragForAction = false;
	public string  gamePhaseState = "";
	public itemActionType actionType;
 
	float dragProgres =0;
	float targertProgres = 0;

	Vector3 StartPos;
	public Transform EndPos;
 

	float timePom;
	int phase = 0;
	Animator animator;

	void Awake () {
		if(actionType == itemActionType.fill)
		{
			StartPos = transform.localPosition-new Vector3(0,20f,0);
			EndPos = transform.parent;
		}
	}

	void Update () {
		 

	}

	public enum itemActionType
	{
		none,
		plug,
		fill,
		PlateNoodles,
		sweetDumplings,
		StoveOn,
		chocolateBowl,
		fortuneCookieMessage,
		machineOn,
		shapeDimSum,
		shapeSpringRolls
	}

	public void OnBeginDrag (PointerEventData eventData)
	{
		if(actionType == itemActionType.shapeSpringRolls && phase== 4)
		{
			bEnabled = false;
			phase++;
			Camera.main.SendMessage( "ShapeSprinRolls", phase, SendMessageOptions.DontRequireReceiver);	 
		}
	}
	
	
	public void OnDrag (PointerEventData eventData)
	{
		 
	}


	
	public void OnEndDrag (PointerEventData eventData)
	{
 
	}

	IEnumerator WaitEnable(float timeE)
	{
		yield return new WaitForSeconds(timeE);
		bEnabled = true;

	}


	public void OnPointerDown (PointerEventData eventData)
	{
		 
		if(bClickForAction && bEnabled ) 
		{
			 
			//if(actionType == itemActionType.plug)
			if(actionType == itemActionType.plug) 
			{
				if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound(SoundManager.Instance.PlugInSound);
				Camera.main.SendMessage("NextPhase","Plug");
				bEnabled = false;
				this.enabled = false;

				//Tutorial.Instance.StopTutorial(1);

			}
			else 	if(gamePhaseState == "NoodleMachineOn")  
			{
				if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound(SoundManager.Instance.OnOffSound);
				Camera.main.SendMessage("NextPhase",gamePhaseState);
				bEnabled = false;
				this.enabled = false;
			}
			else if(actionType == itemActionType.PlateNoodles)
			{
				Camera.main.SendMessage("NextPhase","PlateNoodles");
				bEnabled = false;
				this.enabled = false;
			}
			else if(actionType == itemActionType.StoveOn)
			{
				if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound(SoundManager.Instance.OnOffSound);
				Camera.main.SendMessage("NextPhase","StoveOn");
				bEnabled = false;
				this.enabled = false;
			}
			else if(gamePhaseState == "DishFruits") 
			{
				Tutorial.Instance.StopTutorial();
				transform.GetComponent<DishFruits>().StartCoroutine("InsertFruits2");
				bEnabled = false;
				this.enabled = false;
			}
			else if(actionType == itemActionType.fill)
			{
				if(animator == null) 
				{
					animator  = transform.GetComponent<Animator>();
					animator.speed = 1;
				}
				StartCoroutine("CMakeSweetDumplings");
				 
			}
			else if(actionType == itemActionType.chocolateBowl)
			{
 
				animator  = transform.GetChild(0).GetComponent<Animator>();
				animator.enabled = true;
				Camera.main.SendMessage("NextPhase","ChocolateBowl");
			}
			else if(actionType == itemActionType.fortuneCookieMessage)
			{
				Debug.Log("FinishReadingMessage");
				Camera.main.SendMessage("FinishReadingMessage");
				bEnabled = false;
				this.enabled = false;
			}
			else if(actionType == itemActionType.machineOn)
			{
				if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound(SoundManager.Instance.OnOffSound);
				Camera.main.SendMessage("NextPhase","MachineOn");
				bEnabled = false;
				 
			}
			else if(actionType == itemActionType.shapeDimSum)
			{
				bEnabled = false;
				phase++;
				Camera.main.SendMessage( "ShapeDimSum", phase, SendMessageOptions.DontRequireReceiver);	 
			}
			else if(actionType == itemActionType.shapeSpringRolls)
			{
				bEnabled = false;
				phase++;
				if(phase<4) Camera.main.SendMessage( "ShapeSprinRolls", phase, SendMessageOptions.DontRequireReceiver);	 
			}


		}
	}

	int makeSweetDumplingsPhase = 0;
 
	IEnumerator CMakeSweetDumplings()
	{
		bEnabled = false;
		//float pom = 0;
		//CHANGE IMAGE
 
		makeSweetDumplingsPhase++;
		transform.GetComponent<Animator>().Play("P"+makeSweetDumplingsPhase.ToString());


		yield return new WaitForSeconds(.5f);
		if(makeSweetDumplingsPhase< 4)
		{
			bEnabled = true;
		}
		else
		{
			yield return new WaitForSeconds(.5f);
			animator.enabled = false;
			transform.SetParent(EndPos);
			float pom = 0;
			Vector3 sPos = transform.localPosition;
			Vector3 sScale = transform.localScale;
			Vector3 eScale = transform.localScale*.6f;
			while(pom<1)
			{
				pom+=Time.fixedDeltaTime* 2;
				transform.localPosition = Vector3.Lerp(sPos, StartPos, pom);
				transform.localScale = Vector3.Lerp(sScale, eScale, pom);
				yield return new WaitForFixedUpdate();
			}
			Camera.main.SendMessage("NextPhase", "End", SendMessageOptions.DontRequireReceiver);
			this.enabled = false;

			transform.GetComponent<RaycastTarget>().enabled = false;
		}

	}

 
}
