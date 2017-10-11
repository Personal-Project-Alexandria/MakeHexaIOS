using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;

public delegate void CallbackReplay();

public class GameOverDialog : StartDialog {

    public CallbackReplay callbackReplay;
	public Text txtScore;
	public Text txtHighScore;
    public Text txtCoins;
	public override void OnShow(Transform transf, object data)
	{
        AdManager.Instance.ShowInterstitial();
        base.OnShow(transf, data);
		StartCoroutine(Setup());
		int rand = Random.Range(0, 5);
		if (rand == 0)
		{
			NotifyDialog rate = GameManager.Instance.OnShowNotiFyDialog("Notify", NotifyType.RATE);
		}
	}

	IEnumerator Setup()
	{
		OnCloseAllDialogs();
        int score = (int)data;

        UserProfile.Instance.SetHighScore(score);
        int coins = score / 10;
        yield return new WaitForEndOfFrame();
		txtScore.text = ((int)data).ToString();
		txtHighScore.text = UserProfile.Instance.GetHighScore().ToString();
        txtCoins.text = coins.ToString();
        UserProfile.Instance.AddDiamond(coins);

        CommitHighscore();
	}

	public void CommitHighscore()
	{
		if (Application.internetReachability != NetworkReachability.NotReachable)
		{
			if (FB.IsLoggedIn)
			{
				LeaderBoard.Instance.UploadHighscore(0);
			}
		}
	}
	public void setCallbackReplay(CallbackReplay callback)
    {
        this.callbackReplay = callback;
    }
	public void OnReplay()
    {
        if(this.callbackReplay != null)
        {
            //oogleAnalyticsV4.getInstance().LogEvent("Menu", "Replay Click", string.Empty, 0L);
            callbackReplay.Invoke();
            GameManager.Instance.OnHideDialog(this);
        }
    }
}
