using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KneadDoughScript : MonoBehaviour  , IBeginDragHandler, IDragHandler, IEndDragHandler
{
	
	public  bool bEnableDrag = true;
  
	float mouseStartDragPosY = 0;
	float  kneadDoughDir = 0; 
	float  kneadDough = 0; 
	bool bKneadDough = false;
	float speed =.5f;
	int kneedCount = 0;
	Vector3 doughScale = new Vector3(1.4f,.7f,1);
 
	 
	 
	public Color color;
	Color colorStart;
	float transition;
 
	public Image imgFlavorDough;
	public Image imgDough;
	public Image imgDough2;
//	public SpriteRenderer Sprite1;
//	public SpriteRenderer Sprite2;

	public ProgressBar progressBar;

	void Awake()
	{
		colorStart = color;
		imgDough.color = color;
//		imgDough.material.SetColor("_Color",  color );
//		Sprite1.GetComponent<Renderer>().material.SetColor("_Color",  Color.blue);
//		Sprite2.GetComponent<Renderer>().material.SetColor("_Color",  Color.yellow);
	}
 
	float normalisedTime=0;
	bool bStopTut = true;
	public void OnBeginDrag (PointerEventData eventData)
	{
		if(bEnableDrag ) 
		{
			mouseStartDragPosY = Input.mousePosition.y;
			bKneadDough = true;
		}
	}


	public void OnDrag (PointerEventData eventData)
	{
		if(bEnableDrag && bKneadDough) 
		{
			kneadDough = Mathf.Clamp( (5*(mouseStartDragPosY - Input.mousePosition.y)/(float)Screen.height),-.5f,.5f);
			if( kneadDoughDir < 0.1f)
			{	
				if(kneadDoughDir <1 && kneadDough >.4f) 
				{
					kneedCount++; 
					if(bStopTut) 
					{
						Tutorial.Instance.StopTutorial();
						bStopTut = false;
					}
					kneadDoughDir = 1;
					if(kneedCount == 6  ) StartCoroutine("CChangeDoughImage");
					else if(kneedCount==10)
					{
						bEnableDrag = false;
						Camera.main.SendMessage("NextPhase", "DoughReady");

					}

				
				}
			}
			else if( kneadDoughDir > -0.1f)
			{
				if(kneadDoughDir >-1  && kneadDough <-.4f) 
				{
					kneedCount++;
					kneadDoughDir = -1;
					if(kneedCount == 6  ) StartCoroutine("CChangeDoughImage");
					else if(kneedCount==10)
					{
						bEnableDrag = false;
						Camera.main.SendMessage("NextPhase", "DoughReady");
						if(SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound(SoundManager.Instance.RollingPinSound);
					}
				}
			}
			transform.localScale = Vector3.LerpUnclamped(Vector3.one, doughScale,kneadDough);
			if(kneadDough < 4 && !bChanged)  DoughChangeColor(Time.deltaTime); 
			progressBar.SetProgress(kneedCount/10f, true);
			if(SoundManager.Instance!=null)
			{
				
				if(kneedCount>=10)  SoundManager.Instance.Stop_Sound(SoundManager.Instance.RollingPinSound);
				else 	if(SoundManager.Instance!=null)  SoundManager.Instance.Play_Sound(SoundManager.Instance.RollingPinSound);
			}
		} 
	}

	public void OnEndDrag (PointerEventData eventData)
	{
		if(SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound(SoundManager.Instance.RollingPinSound);
		kneadDoughDir = 0;
		kneadDough = 0;
		bKneadDough= false;
	}

 
	bool bChanged = false;
	public void DoughChangeColor ( float pom)
	{
		transition+= pom ;
		if(transition<1)
		{
			imgFlavorDough.color = new Color(color.r,color.g,color.b,1-transition);
			imgDough.color = Color.Lerp(colorStart, color, transition);
		}
		else if(pom >=1)
		{
			bChanged = true;
			imgFlavorDough.gameObject.SetActive(false);
			imgDough.color = color;
			 imgDough2.color = color;

		
		}
	}

	 

	IEnumerator CChangeDoughImage()
	{
		Color c = imgDough.color;
 
		imgDough2.gameObject.SetActive(true);
		imgDough2.color = new Color(c.r,c.g,c.b,0);
		float pom = 0;
		while(pom<1)
		{
			pom+=Time.deltaTime*2;
			imgDough.color = new Color(c.r,c.g,c.b,1-pom);
			if(imgDough2.color.a <1) imgDough2.color = new Color(c.r,c.g,c.b,pom*2);
			yield return new WaitForEndOfFrame();
		}
		imgDough.gameObject.SetActive(false);

		imgDough2.color = c;


//		imgDough2.material = Instantiate<Material>(imgDough2.material);
//		imgDough2.material.SetColor("_Color",  Color.red);
	}

	 

}
