using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class DecorationTransform : MonoBehaviour {

	 
	public Image BoxImage;
	 
	float rotation;
	 
	public Vector2 startSizeDelta;
	Vector2 offsetSizeDelta = new Vector2(20,20);

	float WorldToCanvasPercent;

	public Transform ButtonScale;
	public Transform ButtonDelete;
	public Transform ButtonCheck;

	CanvasGroup canvasGroup;
	public GameObject ActiveDecoration = null;
	Vector3 posOffset = new Vector3(0,0,-5);
	public bool bMoveDecoratins = false;
	Vector3 dragOffset;
	//int layerMask;
//---------VRATI--------------	public DecorationsMenuScript decorationsMenu;
	public   bool bDecorationTransformButtonDown = false;


	void Awake () {
		 
		canvasGroup = transform.GetComponent<CanvasGroup>();
		canvasGroup.alpha  =0;
		canvasGroup.interactable = false;

		startSizeDelta = BoxImage.rectTransform.sizeDelta;

	}

	void Start () 
	{
		bDecorationTransformButtonDown = false;
		//layerMask = (1 << LayerMask.NameToLayer("DecorationTransform"))  | (1 << LayerMask.NameToLayer("Decoration"))  ;
		float cs = BoxImage.rectTransform.sizeDelta.x/2; //canvas space
		float ws = ButtonScale.transform.position.x - BoxImage.transform.position.x;
		WorldToCanvasPercent =1.41f* cs/ws;

		bMoveDecoratins = true;
	}





	void Update () 
	{
		if(    Input.GetMouseButtonDown(0) && bMoveDecoratins )
		{
 
			if(MenuManager.activeMenu != "") 
			{			 
				HideDecorationTransformTool();
				return;
			}

			Collider2D[] hitColliders2 = Physics2D.OverlapCircleAll( Camera.main.ScreenToWorldPoint(Input.mousePosition) , .1f  , 1 << LayerMask.NameToLayer("DecorationTransform") );// layerMask);
			
			if(hitColliders2.Length  > 0) 
			{
				bDecorationTransformButtonDown = true;
			}
			else
			{
				bDecorationTransformButtonDown = false;

				Collider2D[] hitColliders = Physics2D.OverlapCircleAll( Camera.main.ScreenToWorldPoint(Input.mousePosition) , .1f    , 1 << LayerMask.NameToLayer("Decoration"));
				 
				if(hitColliders.Length  > 0) 
				{
 
					bool bCurrentDec = false;
					for(int i = 0 ; i< hitColliders.Length;i++)
					{
						if(hitColliders[i].gameObject == ActiveDecoration)
							bCurrentDec = true;

					}
					if(!bCurrentDec)
					{
						if(ActiveDecoration!=null)
						{
							//ActiveDecoration.GetComponent<Decoration>( ).enabled = false;
							ActiveDecoration.transform.position = new Vector3(ActiveDecoration.transform.position.x,ActiveDecoration.transform.position.y,0);
							ActiveDecoration.GetComponent<Decoration>( ).decorationTransform = null;
						}
						
						ActiveDecoration = hitColliders[0].gameObject;
						ActiveDecoration.transform.position = new Vector3(ActiveDecoration.transform.position.x,ActiveDecoration.transform.position.y,-1f);
						transform.position = new Vector3(ActiveDecoration.transform.position.x,ActiveDecoration.transform.position.y,-10);    //ActiveDecoration.transform.position+posOffset;
						BoxImage.transform.rotation  = ActiveDecoration.transform.rotation;
						ButtonDelete.rotation = Quaternion.identity;
						BoxImage.rectTransform.sizeDelta  = ActiveDecoration.GetComponent<RectTransform>().sizeDelta;
						
						
					//	transform.GetComponent<RectTransform>().SetAsLastSibling();  //VRATI
			 
					 
						ActiveDecoration.GetComponent<RectTransform>().SetAsLastSibling();
						 
						BoxImage.rectTransform.sizeDelta = ActiveDecoration.GetComponent<RectTransform>().sizeDelta + offsetSizeDelta;
						BoxImage.transform.rotation = ActiveDecoration.transform.rotation;
						
					}
					
					//ActiveDecoration.GetComponent<Decoration>( ).enabled = true;
					ActiveDecoration.GetComponent<Decoration>( ).decorationTransform = this; 
					ShowDecorationTransformTool();
				}
				else
				{
				//	 Debug.Log("HIDE");
					HideDecorationTransformTool();
				}
			}
 


		}
 
	}


	public void ButtonCheckClicked()
	{
		HideDecorationTransformTool();
	}
 
	public void ButtonDeleteClicked()
	{
		 //Debug.Log ("X");

		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
		HideDecorationTransformTool();
	
		//ActiveDecoration.GetComponent<Decoration>( ).enabled = false;
	//-----------vrati  ------------------  	decorationsMenu.RemoveFromDecorationsList(ActiveDecoration.GetComponent<CircleCollider2D>());
		ActiveDecoration.GetComponent<Decoration>( ).decorationTransform = null; 

		GameObject.Destroy(ActiveDecoration);
		ActiveDecoration = null;
		//ResetDecorationTransform();
	}

 

	public void ButtonScale_PointerDown( BaseEventData data)
	{
		//ActiveDecoration.transform.GetComponent<Collider2D>().enabled = false;
		if(SoundManager.Instance!=null) SoundManager.Instance.Play_ButtonClick();
	}



	//ISPRAVKA U ODNOSU NA STARU
	//SKALIRA DEKORACIJU, I KOLAJEDER ZAJEDNO SA NJOM
	public void ButtonScale_PointerDrag( BaseEventData data)
	{
		 
		PointerEventData pointerData = data as PointerEventData;
		if( canvasGroup.interactable && pointerData.IsPointerMoving())
		{

			Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 screenPos =  transform.position; 

			BoxImage.transform.rotation  = Quaternion.Euler (  0,0, Mathf.Atan2((screenPos.y - mousePos.y), (screenPos.x - mousePos.x))*Mathf.Rad2Deg - 135 );  

			float deltaScale = WorldToCanvasPercent * (mousePos-screenPos).magnitude/startSizeDelta.x;
			float deltaSize  = WorldToCanvasPercent * (mousePos-screenPos).magnitude;

			BoxImage.rectTransform.sizeDelta  = deltaSize*Vector2.one;
		 
			if(deltaScale > 1.2f) deltaScale = 1.2f;
			if(deltaScale < .5f) deltaScale = .5f;

			if(BoxImage.rectTransform.sizeDelta.x >startSizeDelta.x*1.2f) BoxImage.rectTransform.sizeDelta  =  startSizeDelta*1.2f ;
			if(BoxImage.rectTransform.sizeDelta.x <startSizeDelta.x*.5f) BoxImage.rectTransform.sizeDelta  =  startSizeDelta*.5f;
			 
			ButtonDelete.rotation = Quaternion.identity;
 
			ActiveDecoration.transform.localScale = deltaScale*Vector3.one;
			ActiveDecoration.transform.rotation = BoxImage.transform.rotation;

		 
		}
	}





	public void ResetDecorationTransform()
	{
		BoxImage.transform.rotation = Quaternion.identity;
		//BoxImage.rectTransform.sizeDelta = startSizeDelta;
		BoxImage.rectTransform.sizeDelta  =  startSizeDelta  ;
	}
	public void ResetDecorationTransform2()
	{
		BoxImage.rectTransform.sizeDelta  =  startSizeDelta ;
	}


	public void ShowDecorationTransformTool()
	{
		transform.position = ActiveDecoration.transform.position + posOffset;
		//transform.GetComponent<RectTransform>().SetAsLastSibling();
		StopAllCoroutines();
		StartCoroutine("_ShowDecorationTransformTool");
	}

	IEnumerator _ShowDecorationTransformTool()
	{
		yield return new WaitForEndOfFrame();

		while(canvasGroup.alpha < 1)
		{
			canvasGroup.alpha += Time.deltaTime*4;
			yield return new WaitForEndOfFrame();
		}

		canvasGroup.alpha  =1;
		canvasGroup.interactable = true;
	}
 
	public void HideDecorationTransformTool()
	{
		StopAllCoroutines();
		StartCoroutine("_HideDecorationTransformTool");
	}

	IEnumerator _HideDecorationTransformTool()
	{
		canvasGroup.interactable = false;
		yield return new WaitForEndOfFrame();
		
		while(canvasGroup.alpha >0)
		{
			canvasGroup.alpha -= Time.deltaTime*3;
			yield return new WaitForEndOfFrame();
		}
		canvasGroup.alpha  =0;
		 
	}


}
