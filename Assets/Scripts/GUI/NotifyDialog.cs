using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum NotifyType
{
	NOADS, TRANSACTION_SUCCESS, TRANSACTION_FAIL, TRANSACTION_CONFIRM, RESTORE_PURCHASE, RATE
}

public class NotifyDialog : BaseDialog {

	public Text title;
	public Text content;
	public Button accept;
	public Button decline;
	public Button close;
	public Text acceptText;
	public Text declineText;

	public void OnShow(Transform transf, object data, NotifyType type)
	{
		base.OnShow(transf, data);
		
		if (type == NotifyType.NOADS)
		{
			title.text = "REMOVE ADS";
			content.text = "2.99$\nAre you sure ?";
			accept.onClick.AddListener(delegate { BuyNoAds(); });
			decline.onClick.AddListener(delegate { OnCloseDialog(); });
			close.onClick.AddListener(delegate { OnCloseDialog(); });
		}

		if (type == NotifyType.TRANSACTION_SUCCESS)
		{
			title.text = "TRANSACTION SUCCESS";
			content.text = "Thank you\nfor your support";
			accept.onClick.AddListener(delegate { OnCloseDialog(); });
			close.onClick.AddListener(delegate { OnCloseDialog(); });
			decline.gameObject.SetActive(false);
		}

		if (type == NotifyType.TRANSACTION_FAIL)
		{
			title.text = "TRANSACTION FAIL";
			content.text = "Oops! Transaction\ncan't be made";
			accept.onClick.AddListener(delegate { OnCloseDialog(); });
			close.onClick.AddListener(delegate { OnCloseDialog(); });
			decline.gameObject.SetActive(false);
		}

		if (type == NotifyType.TRANSACTION_CONFIRM)
		{
			title.text = "TRANSACTION CONFIRM";
			content.text = "Are you sure ?";
			accept.onClick.AddListener(delegate { Confirm(); });
			decline.onClick.AddListener(delegate { OnCloseDialog(); });
			close.onClick.AddListener(delegate { OnCloseDialog(); });
		}

		if (type == NotifyType.RESTORE_PURCHASE)
		{
			if ((int)data == 0)
			{
				title.text = "RESTORE SUCCESS";
				content.text = "Your purchases\n have been restored";
			}
			else if ((int)data == 1)
			{
				title.text = "RESTORE FAIL";
				content.text = "Error in \nrestoring purchase";
			}
			else
			{
				title.text = "RESTORE FAIL";
				content.text = "Your platform\nis not supported";
			}

			accept.onClick.AddListener(delegate { OnCloseDialog(); });
			close.onClick.AddListener(delegate { OnCloseDialog(); });
			decline.gameObject.SetActive(false);
		}

		if (type == NotifyType.RATE)
		{
			title.text = "RATE US";
			content.text = "Do you like\nCOLOR HEXA!";
			acceptText.text = "Like it";
			declineText.text = "Not like";
			accept.onClick.AddListener(delegate { Rate(); });
			decline.onClick.AddListener(delegate { OnCloseDialog(); });
			close.onClick.AddListener(delegate { OnCloseDialog(); });
		}
	}

	public void Rate()
	{
        //oogleAnalyticsV4.getInstance().LogEvent("Menu", "Rate Dialog Click", string.Empty, 0L);

        SoundManager.Instance.PlaySfx(SFX.Button);
#if UNITY_IOS
		Application.OpenURL("https://itunes.apple.com/us/app/id1268487460");
#elif UNITY_ANDROID
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.bestgameworld.puzzlefree.colorhexamakeblock");
#else 
		Application.OpenURL("https://itunes.apple.com/us/app/id1268487460");
#endif
		this.OnCloseDialog();
	}

	public void BuyNoAds()
	{
		Button noAdsButton = (Button)data;
		IAPManager.Instance.BuyNoAds();

		if (UserProfile.Instance.HasAds())
		{
			noAdsButton.GetComponent<Image>().sprite = UserProfile.Instance.hasAds;
			noAdsButton.interactable = true;
		}
		else
		{
			noAdsButton.GetComponent<Image>().sprite = UserProfile.Instance.noAds;
			noAdsButton.interactable = false;
		}
		OnCloseDialog();
	}

	public void Confirm()
	{
		iAPDialog.Transaction transaction = (iAPDialog.Transaction)data;
		transaction();
	}
}
