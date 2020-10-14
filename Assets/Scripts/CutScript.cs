using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutScript : MonoBehaviour {


	Transform[] KnifeStartPosition;
	Transform[] KnifeEndPosition;
	int cutNo = 0;
	int maxCuts = 1;
	int[] maxFruits = new int[]{  1 }; //broj potrebnog voca, ili neke druge namirnice koja se secka


	int fruitNo = 1;

	public Transform knife;
	public Transform [] Fruits ;
	Transform  Fruit ;
	public int selectedFruit = 0;

	Animator animFruit;

	Vector3  startPos ;
	Vector3  endPos;
	Vector3  knifeCurrentPos;
	Vector3  knifeStartPos;
	Vector3 fruitStartPos;

	float deltaPosY = 0;
	float relativePosY = 0;

	public Transform fruitCutPosition;
	public Transform fruitHidePosition;

	bool bMoveKnife = false;
	Transform startParent;

 

	public void InitKnife () {
		selectedFruit = 0;

		Fruit = Fruits[selectedFruit];

		startParent =  knife.parent;
		fruitStartPos = Fruit.position;
		knifeStartPos = knife.position;
		Knife.bEnableDrag = false;
		animFruit = Fruit.GetComponent<Animator>();
		maxCuts = (Fruit.childCount-1)/3;
//		Debug.Log(maxCuts);

		KnifeStartPosition = new Transform[maxCuts];
		KnifeEndPosition = new Transform[maxCuts];

		for(int i=1; i<=maxCuts;i++)
		{
			KnifeStartPosition[i-1] = Fruit.Find("KnifeStartPos"+ i.ToString());
			KnifeEndPosition[i-1] = Fruit.Find("KnifeEndPos"+ i.ToString());
		}

	 
		cutNo = 1;

		//StartCoroutine("ShowFruit");
		//yield return new WaitForSeconds(1.1f);

		StartCoroutine("SetKnifeToStartPosition") ;
		//Tutorial.Instance.ShowTutorial(0);
	 
	}
  
	void Update () {
		if(bMoveKnife)  knife.position =  Vector3.Lerp (knife.position  , knifeCurrentPos  , 8 * Time.deltaTime);//6
	}

 
	public void MoveKnife(float mousePosY)
	{
		if(!bMoveKnife)   return;

	 
		Tutorial.Instance.StopTutorial();
		relativePosY = mousePosY - endPos.y;
		float pom = 1-relativePosY/deltaPosY;

		knifeCurrentPos = Vector3.Lerp(startPos, endPos, pom); 

		if(   knife.position.y <=   (endPos.y+.05f))            //(pom > 1)
		{
			animFruit.Play("cut"+cutNo.ToString());
			StartCoroutine("SetKnifeToStartPosition") ;

			cutNo++;
			if(SoundManager.Instance!=null) SoundManager.Instance.Play_Sound(SoundManager.Instance.KnifeCutSound2);
		}
	}

	 
	//-------------------------------------

	IEnumerator SetKnifeToStartPosition()
	{
		yield return new WaitForEndOfFrame();
		if(cutNo<=maxCuts) //NIJE ISECEN POSLEDNJI KOMADIC
		{

			Knife.bEnableDrag = false;
			bMoveKnife = false; 
			if(cutNo == 1) startPos = knife.position;
			else startPos = KnifeEndPosition[cutNo-2].position;
			endPos = KnifeStartPosition[cutNo-1].position;

			//vraca se na pocetnu poziciju da bi se izbeglo preklapanje noza 
			Vector3 midPos = (cutNo == 1)? KnifeStartPosition[cutNo-1].position :KnifeStartPosition[cutNo-2].position;

			float timeMove = 0;
			while(timeMove <.7f )
			{
				timeMove+= Time.deltaTime;
				yield return new WaitForEndOfFrame();
				knife.position = Vector3.Lerp(startPos, midPos, timeMove*1.4f); 
			}
			startPos = knife.position;
			timeMove = 0;
			knife.SetParent(KnifeStartPosition[cutNo-1]);
			while(timeMove <1f )
			{
				timeMove+= Time.deltaTime*4f;
				yield return new WaitForEndOfFrame();
				knife.position = Vector3.Lerp(startPos, endPos, timeMove); 
			}

			startPos = KnifeStartPosition[cutNo-1].position;
			endPos = KnifeEndPosition[cutNo-1].position;
			deltaPosY = startPos.y - endPos.y;

			knifeCurrentPos =   startPos;
			//knife.SetParent(KnifeStartPosition[cutNo-1]);
			bMoveKnife = true;
			Knife.bEnableDrag = true;
		}
		else if(fruitNo < maxFruits[selectedFruit]) //ISECEN POSLEDNJI KOMAD ALI IMA JOS VOCA
		{
			fruitNo++;
			bMoveKnife = false;
			Knife.bEnableDrag = false;
			float timeMove = 0;

			Vector3 kp = knife.position;
			while(timeMove <1f )
			{
				timeMove+= Time.deltaTime;
				yield return new WaitForEndOfFrame();
				knife.position = Vector3.Lerp(kp, KnifeStartPosition[0].position, timeMove); 


			}
			knife.SetParent(startParent);
			startPos = KnifeStartPosition[0].position;
			endPos = KnifeEndPosition[0].position;
			knifeCurrentPos = startPos;
			cutNo = 1;



			yield return new WaitForSeconds(.1f);//1
			StartCoroutine("HideFruit");

			yield return new WaitForSeconds(1);
			animFruit.Play("Default");
			StartCoroutine("ShowFruit");



			yield return new WaitForSeconds(1.1f);
			knife.SetParent(KnifeStartPosition[cutNo-1]);
			bMoveKnife = true;
			Knife.bEnableDrag = true;



		}
		else  //ISECKANO SVO VOCE I ZAVRSENO SECJANJE POSLEDNJEG KOMADICA
		{
			bMoveKnife = false;
			Knife.bEnableDrag = false;
			float timeMove = 0;

			Vector3 kp = knife.position;
			while(timeMove <1f )
			{
				timeMove+= Time.deltaTime;
				yield return new WaitForEndOfFrame();
				knife.position = Vector3.Lerp(kp, KnifeStartPosition[0].position, timeMove); 


			}
			startPos = KnifeStartPosition[0].position;
			knifeCurrentPos = startPos;
			knife.SetParent(startParent);



			//StartCoroutine("HideFruit");
			yield return new WaitForSeconds(.1f);

			Debug.Log("Kraj seckanja");
			transform.SendMessage("NextPhase","CutEnd");
		}
	}

 

	IEnumerator ShowFruit()
	{
		Fruit.gameObject.SetActive(true);

		float timeMove = 0;
		Vector3 arcMax = new Vector3(0,5,0); 

		while(timeMove <1f )
		{
			timeMove+= Time.deltaTime;
			yield return new WaitForEndOfFrame();
			Fruit.position = Vector3.Lerp(fruitStartPos, fruitCutPosition.position, timeMove)  + timeMove* (1-timeMove) *arcMax; 
		}


	}


	IEnumerator HideFruit()
	{
		Fruit.gameObject.SetActive(true);

		float timeMove = 0;
		Vector3 arcMax = new Vector3(0,5,0); 

		while(timeMove <1f )
		{
			timeMove+= Time.deltaTime;
			yield return new WaitForEndOfFrame();
			Fruit.position = Vector3.Lerp( fruitCutPosition.position, fruitHidePosition.position, timeMove)  + timeMove* (1-timeMove) *arcMax; 
		}


	}

	 
}



