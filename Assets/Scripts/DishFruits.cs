using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DishFruits : MonoBehaviour {

 
	Vector3 StartPos;
	public Transform EndPosition;
	public Animator animDish;
	 
	public Collider2D colliderDish;
	public Collider2D [] collidersDishContent;
	public Transform EndParent;
	 
	public void Awake ()
	{
		StartPos = transform.position;
	}


	// Use this for initialization
	void Start () {
		
	}
	
	public IEnumerator InsertFruits()
	{
		yield return new WaitForSeconds(1);
		float timeMove = 0;
		Vector3 arcMax = new Vector3(0,5,0); 
		StartPos = transform.position;
		while(timeMove <1f )
		{
			timeMove+= Time.deltaTime*.8f;
			yield return new WaitForEndOfFrame();
			transform.position = Vector3.Lerp( StartPos, EndPosition.position, timeMove)  + timeMove* (1-timeMove) *arcMax; 
		}
		transform.GetComponent<ItemAction>().bEnabled = true;
		Tutorial.Instance.ShowTutorial(3);


	}


	public IEnumerator InsertFruits2()
	{
		 animDish.Play("DishFruit");

		EnableColliders ();
		 
		 
		if(SoundManager.Instance!=null) SoundManager.Instance.StopAndPlay_Sound(SoundManager.Instance.InsertFruit);
//		CanvasGroup cg = collidersDishContent[0].transform.parent.GetComponent<CanvasGroup>();
//		cg.transform.parent = EndParent;
		yield return new WaitForSeconds(1f);

		collidersDishContent[0].transform.parent.SetParent(EndParent);
		yield return new WaitForSeconds(3);
//		while(cg.alpha >0 )
//		{
//			yield return new WaitForEndOfFrame();
//			cg.alpha -= Time.deltaTime;
//		}
//		yield return new WaitForEndOfFrame();
		 //colliderDish.enabled = false;
		//GameObject.Destroy(cg.gameObject);
		DisableRigidbodys();
		Vector3 arcMax = new Vector3(0,5,0); 
		float timeMove = 0;
		Camera.main.SendMessage("NextPhase",  "InsertFruitsEnd", SendMessageOptions.DontRequireReceiver);
		while(timeMove <1f )
		{
			timeMove+= Time.deltaTime*.8f;
			yield return new WaitForEndOfFrame();
			transform.position = Vector3.Lerp( EndPosition.position, StartPos,  timeMove)  + timeMove* (1-timeMove) *arcMax; 
		}

		yield return new WaitForSeconds(.1f);
		GameObject.Destroy(this.gameObject);

	}

	 

 

	void EnableColliders ()
	{
		colliderDish.enabled = true;
		for(int i = 0; i < collidersDishContent.Length; i++)
		{
			collidersDishContent[i].enabled = true;
			collidersDishContent[i].GetComponent<Rigidbody2D>().isKinematic = false;

		}
	}

	void DisableRigidbodys()
	{
		colliderDish.enabled = false;
		for(int i = 0; i < collidersDishContent.Length; i++)
		{
			collidersDishContent[i].enabled = true;
			//collidersDishContent[i].GetComponent<Rigidbody2D>().Sleep();
			collidersDishContent[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			collidersDishContent[i].GetComponent<Rigidbody2D>().angularVelocity = 0;
			collidersDishContent[i].GetComponent<Rigidbody2D>().isKinematic = true;
		}
	}
	 

	public void ChangeSprites(Sprite sprFruit)
	{
		for(int i = 0; i < collidersDishContent.Length; i++)
		{
			collidersDishContent[i].GetComponent<Image>().sprite  = sprFruit;
		}
	}
}
