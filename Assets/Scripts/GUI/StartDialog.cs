using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;

public class StartDialog : BaseDialog {

	public Button noAdsButton;
	public Image sound;

	public override void OnShow(Transform transf, object data)
	{
		base.OnShow(transf, data);
		SoundManager.Instance.ChangeIcon(sound);
		UserProfile.Instance.SetupNoAds(noAdsButton);
		AdManager.Instance.ShowBanner();
	}

	public void OnClickPlay()
    {
        SoundManager.Instance.PlaySfx(SFX.Button);
        //oogleAnalyticsV4.getInstance().LogEvent("Menu", "Play Click", string.Empty, 0L);
        PlayDialog dialog = GameManager.Instance.OnShowDialog<PlayDialog>("Play");
		this.OnCloseDialog();
    }

	public void OnClickNoAds()
	{
        SoundManager.Instance.PlaySfx(SFX.Button);
        //oogleAnalyticsV4.getInstance().LogEvent("Menu", "No Ads Click", string.Empty, 0L);
        NotifyDialog noti = GameManager.Instance.OnShowNotiFyDialog("Notify", NotifyType.NOADS, noAdsButton);
	}

	public void OnClickShare()
	{
        //oogleAnalyticsV4.getInstance().LogEvent("Menu", "Share Click", string.Empty, 0L);
        SoundManager.Instance.PlaySfx(SFX.Button);
        FBManager.Instance.ShareLink();
	}

	public void OnClickStore()
	{
        SoundManager.Instance.PlaySfx(SFX.Button);
        //oogleAnalyticsV4.getInstance().LogEvent("Menu", "Store Click", string.Empty, 0L);
        iAPDialog store = GameManager.Instance.OnShowDialog<iAPDialog>("iAP");
	}

	public void OnClickLeaderBoard()
	{
        SoundManager.Instance.PlaySfx(SFX.Button);
        //oogleAnalyticsV4.getInstance().LogEvent("Menu", "Leaderboard Click", string.Empty, 0L);
        LeaderDialog leader = GameManager.Instance.OnShowDialog<LeaderDialog>("Leader");
	}

	public void OnClickSound()
	{
		SoundManager.Instance.ToggleMusic(!SoundManager.Instance.IsBackgroundPlaying());
        //oogleAnalyticsV4.getInstance().LogEvent("Menu", "Sound Click", string.Empty, 0L);
        SoundManager.Instance.ChangeIcon(sound);
	}
}
