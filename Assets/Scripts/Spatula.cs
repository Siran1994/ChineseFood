using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Spatula : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

	public Image[] imgWrappers;
	public Image imgSpatulaDough;


	float[] scale = {1, 1.7f, 2.6f}; //skaliranje slike testa na spatuli
	bool bIskoriscen = false;
	bool bDrag = false;
	Vector3 StartPosition;

	int imgIndex = 0;
 
	float offsetAngle = 0;

	Vector3 worldPos;
	Vector3 localPos;
	float angle;
	float prevAngle;

	int krugBr = 1;

	public void Init()
	{
		StartPosition = transform.position;
		angle = transform.rotation.eulerAngles.z;
		prevAngle = transform.rotation.eulerAngles.z;

		imgWrappers[krugBr].gameObject.SetActive(true);
		imgWrappers[krugBr].fillAmount = 0;

		StartCoroutine("CShowDoughImage");
	}

	public void OnBeginDrag (PointerEventData eventData)
	{
		if(  bIskoriscen ) return;
		StopAllCoroutines(); 
		 

		if(  !bIskoriscen   && !bDrag  )
		{ 
			bDrag = true;
			 
			worldPos  =  Camera.main.ScreenToWorldPoint(new Vector3 (eventData.position.x, eventData.position.y,0) );
			localPos =worldPos - transform.position;

			offsetAngle =  Mathf.Rad2Deg * Mathf.Atan( localPos.y/localPos.x)  - 180 * ((localPos.x<0) ?1:0 ) - transform.rotation.eulerAngles.z;
			imgSpatulaDough.transform.localScale = new Vector3(scale[krugBr],1,1);
		}
	}
 
	public void OnDrag (PointerEventData eventData)
	{
		if(krugBr > 2) return;
		worldPos  =  Camera.main.ScreenToWorldPoint(new Vector3 (eventData.position.x, eventData.position.y,0) );
		localPos =worldPos - transform.position;
		 
		angle =  Mathf.Rad2Deg * Mathf.Atan( localPos.y/localPos.x)  - 180 * ((localPos.x<0) ?1:0 ) - offsetAngle;
		if(angle>360) angle -= 360;
		if(angle< 0) angle += 360;
		//Debug.Log(angle);
		 
		if(angle <= prevAngle && (prevAngle - angle) < 90)
		{
			transform.rotation  = Quaternion.Euler(0,0,angle);
		}
 
		else if( angle>350 && prevAngle <20) 
		{
			
			transform.rotation  = Quaternion.Euler(0,0,angle);
			krugBr++;
			if(krugBr ==2)
			{
				imgWrappers[0].gameObject.SetActive(false);
				imgWrappers[1].fillAmount = 1;
				imgWrappers[2].gameObject.SetActive(true);
				imgWrappers[2].fillAmount = 0;
				//Debug.Log("Novi Krug " + krugBr);
				imgSpatulaDough.transform.localScale = new Vector3(scale[krugBr],1,1);
			}
			else if(krugBr > 2)
			{
				imgWrappers[1].gameObject.SetActive(false);
				imgWrappers[2].fillAmount = 1;

				Camera.main.SendMessage ("NextPhase", "Spatula");
				return;
			}
		} 
	 
		prevAngle = transform.rotation.eulerAngles.z;
		imgWrappers[krugBr].fillAmount = 1 - prevAngle/360;
		 
	}

	public void OnEndDrag (PointerEventData eventData)
	{
		if(    bDrag 	)  
		{ 
			bDrag = false;
			offsetAngle = 0;
		}
	}
 

	IEnumerator CShowDoughImage()
	{
		float pom = 0;
		imgSpatulaDough.gameObject.SetActive(true);
		Color c = new Color(1,1,1,0);
		imgSpatulaDough.color = c;
		while(pom<1)
		{
			yield return new WaitForEndOfFrame();
			pom +=Time.deltaTime*3;
			imgSpatulaDough.color = Color.Lerp(c, Color.white,pom);
		}
		imgSpatulaDough.color = Color.white;
	}
}
