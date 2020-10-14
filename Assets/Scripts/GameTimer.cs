using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameTimer : MonoBehaviour {
	public int TimeLeft = 20;
	public Text txtTimeLeft;
	Animator animTimeLeft;
	bool b10Sec = false;

	public Image digit1;
	public Image digit2;
	public Sprite[] digits;

	void Start()
	{
		TimeLeft = 20;
		digit1.sprite =  digits[   TimeLeft/10  ];
		digit2.sprite =  digits[  TimeLeft%10  ];
	}

	public void StartTimer () 
	{
		InvokeRepeating("TimerTick",0f,1f);
		//TimeLeft = 15;
	}
	
 
	void TimerTick()
	{
		 
		if(!GlobalVariables.bPauseGame)
		{
			TimeLeft--;
			if(TimeLeft <=0)
			{
				TimeLeft = 0;
				transform.SendMessage("OutOfTime");
				StopTimer();

			}
			else if(!b10Sec && TimeLeft == 10)
			{
				b10Sec = true;
				transform.SendMessage("Time10SecLeft");
			}
	 
		}

		digit1.sprite =  digits[   TimeLeft/10  ];
		digit2.sprite =  digits[  TimeLeft%10  ];
	}

	public void StopTimer()
	{

       // SDKManager.Instance.ShowAd(ShowAdType.VideoAD, 1, "吃完菜肴倒计时结束之后弹出");
        if (SoundManager.Instance!=null) SoundManager.Instance.Stop_Sound(SoundManager.Instance.TimerSound);	
		CancelInvoke("TimerTick");
		digit1.sprite =  digits[   TimeLeft/10  ];
		digit2.sprite =  digits[  TimeLeft%10  ];

		//txtTimeLeft.text = Mathf.FloorToInt( TimeLeft/60) +":"+ ( TimeLeft%60).ToString().PadLeft(2,'0');
		//animTimeLeft.SetBool("bTimerBlink",false);
		//SoundManager.Instance.Stop_TimeCountdown();

	}
}
