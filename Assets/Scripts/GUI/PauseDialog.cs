using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseDialog : BaseDialog {
    public CallbackReplay callbackReplay;
	public Image sound;
	public Sprite soundOn;
	public Sprite soundOff;
    public Button noAdsButton;
    private void Start()
	{
		if (SoundManager.Instance.IsBackgroundPlaying())
		{
			sound.sprite = soundOn;
		}
		else
		{
			sound.sprite = soundOff;
		}
	}

	public void OnResumne()
    {
        //oogleAnalyticsV4.getInstance().LogEvent("Menu", "Resume Pause Click", string.Empty, 0L);
        SoundManager.Instance.PlaySfx(SFX.Button);
        GameManager.Instance.OnHideDialog(this);
    }
    public void setCallbackReplay(CallbackReplay callback)
    {
        this.callbackReplay = callback;
    }
    public void OnReplay()
    {
        //oogleAnalyticsV4.getInstance().LogEvent("Menu", "RePlay Pause Click", string.Empty, 0L);
        SoundManager.Instance.PlaySfx(SFX.Button);
        if (this.callbackReplay != null)
        {
            callbackReplay.Invoke();
            this.OnCloseDialog();
        }
    }

	public void OnClickHome()
	{
        //oogleAnalyticsV4.getInstance().LogEvent("Menu", "Home Pause Click", string.Empty, 0L);
        SoundManager.Instance.PlaySfx(SFX.Button);
        this.OnCloseAllDialogs();
		StartDialog start = GameManager.Instance.OnShowDialog<StartDialog>("Start");	
	}

	public void OnClickSound()
	{
        //oogleAnalyticsV4.getInstance().LogEvent("Menu", "Sound Pause Click", string.Empty, 0L);
        SoundManager.Instance.ToggleMusic(!SoundManager.Instance.IsBackgroundPlaying());
		if (SoundManager.Instance.IsBackgroundPlaying())
		{
			sound.sprite = soundOn;
		}
		else
		{
			sound.sprite = soundOff;
		}
	}

    public void OnClickNoAds()
    {
        //oogleAnalyticsV4.getInstance().LogEvent("Menu", "No Ads Pause Click", string.Empty, 0L);
        SoundManager.Instance.PlaySfx(SFX.Button);
        NotifyDialog noti = GameManager.Instance.OnShowNotiFyDialog("Notify", NotifyType.NOADS, noAdsButton);
    }

    public void OnClickShare()
    {
        //oogleAnalyticsV4.getInstance().LogEvent("Menu", "Share Pause Click", string.Empty, 0L);
        SoundManager.Instance.PlaySfx(SFX.Button);
        FBManager.Instance.ShareLink();
    }

    public void OnClickStore()
    {
        //oogleAnalyticsV4.getInstance().LogEvent("Menu", "Store Pause Click", string.Empty, 0L);
        SoundManager.Instance.PlaySfx(SFX.Button);
        iAPDialog store = GameManager.Instance.OnShowDialog<iAPDialog>("iAP");
    }

    public void OnRate()
    {
        //oogleAnalyticsV4.getInstance().LogEvent("Menu", "Rate Pause Click", string.Empty, 0L);
        SoundManager.Instance.PlaySfx(SFX.Button);
#if UNITY_IOS
		Application.OpenURL("https://itunes.apple.com/us/app/id1268487460");
#elif UNITY_ANDROID
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.bestgameworld.puzzlefree.colorhexamakeblock");
#else 
		Application.OpenURL("https://itunes.apple.com/us/app/id1268487460");
#endif
    }
}
