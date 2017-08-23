using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class UserProfile : MonoSingleton<UserProfile> {
	public Sprite noSound;
	public Sprite hasSound;
	public Sprite noAds;
	public Sprite hasAds;

	private string KEY_HIGH_SCORE = "KEY_HIGH_SCORE";
	private string KEY_DIAMOND = "KEY_DIAMOND";
	private string KEY_ADS = "KEY_ADS";

	private int highScore;
	private int diamond;
	private bool ads; // 0 = no ads, 1 = has ads

	private void Awake()
	{
		this.LoadProfile();
	}
    private void Start()
    {
        this.LoadDailyRewardModel();
    }
    // High score function
    public bool IsHighScore(int newScore)
	{
		if (newScore > this.highScore)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	public void SetHighScore(int newScore)
	{
		if (IsHighScore(newScore))
		{
			this.highScore = newScore;
			PlayerPrefs.SetInt(KEY_HIGH_SCORE, this.highScore);
		}
	}
	public int GetHighScore()
	{
		return this.highScore;
	}

	// Diamond function
	public void AddDiamond(int addedDiamond)
	{
		this.diamond += addedDiamond;
		PlayerPrefs.SetInt(KEY_DIAMOND, this.diamond);
	}
	public bool ReduceDiamond(int reducedDiamond)
	{
		int temp = this.diamond - reducedDiamond;
		if (temp >= 0)
		{
			this.diamond -= reducedDiamond;
			PlayerPrefs.SetInt(KEY_DIAMOND, this.diamond);
			return true;
		}
		else
		{
			return false;
		}
	}

	public int GetDiamond()
	{
		return this.diamond;
	}
	[ContextMenu("Clear Diamond - test only")]
	public void ClearDiamond()
	{
		ReduceDiamond(GetDiamond());
	}

	// Ads function
	public void RemoveAds()
	{
		if (HasAds())
		{
			this.ads = false;
			AdManager.Instance.RemoveBanner();
			PlayerPrefs.SetInt(KEY_ADS, HasAds() ? 1 : 0);
		}
	}
	public bool HasAds()
	{
		return this.ads;
	}
	public void SetupNoAds(Button adsButton)
	{
		if (HasAds())
		{
			adsButton.GetComponent<Image>().sprite = hasAds;
			adsButton.interactable = true;
		}
		else
		{
			adsButton.GetComponent<Image>().sprite = noAds;
			adsButton.interactable = false;
		}
	}

	// Save - load function
	public void LoadProfile()
	{
		// Init for first play
		this.diamond = 0;
		this.ads = true;
		this.highScore = 0;

		// Init for second, third, ... play
		if (PlayerPrefs.HasKey(KEY_DIAMOND))
		{
			this.highScore = PlayerPrefs.GetInt(KEY_HIGH_SCORE);
		}
		if (PlayerPrefs.HasKey(KEY_DIAMOND))
		{
			this.diamond = PlayerPrefs.GetInt(KEY_DIAMOND);
		}
		if (PlayerPrefs.HasKey(KEY_ADS))
		{
			this.ads = PlayerPrefs.GetInt(KEY_ADS) == 1 ? true : false;
		}
        CheckDailyReward.GetInstance().DailyRewardModel = this.LoadDailyReward();
	}
	public void SaveProfile()
	{ 
		PlayerPrefs.SetInt(KEY_DIAMOND, this.diamond);
		PlayerPrefs.SetInt(KEY_ADS, HasAds() ? 1 : 0);
		PlayerPrefs.SetInt(KEY_HIGH_SCORE, this.highScore);
	}
    #region Daily Award

    private void LoadDailyRewardModel()
    {
        SaveDailyReward saveDailyReward = LoadDailyReward();
        //this._bLoadDailySucess = true;
        if (saveDailyReward == null)
        {
            CheckDailyReward.GetInstance().UpdateFirstPlay();
            int coin = CheckDailyReward.GetInstance().GetCoin();
            int indexDay = CheckDailyReward.GetInstance().DailyRewardModel.IndexDay;
            ////////oogleAnalyticsV4.getInstance().LogEvent("Daily Reward", "Daily Reward " + indexDay.ToString(), string.Empty, 0L);

            DailyRewarDialog dialog = GameManager.Instance.OnShowDialog<DailyRewarDialog>("DailyAward");
            dialog.ParseData(coin, indexDay);
            this.AddDiamond(coin);
            //this._panelDailyReward.Show(indexDay, coin);
            //int num = PlayerPrefs.GetInt(Constant.UserCoin);
            //num += coin;
            //PlayerPrefs.SetInt(Constant.UserCoin, num);

            SaveDailyRewardModel(CheckDailyReward.GetInstance().DailyRewardModel);
        }
        else
        {
            CheckDailyReward.GetInstance().DailyRewardModel = saveDailyReward;
            this.ProcessDailyReward();
        }
    }
    private void ProcessDailyReward()
    {
        if (CheckDailyReward.GetInstance().Check())
        {
            int coin = CheckDailyReward.GetInstance().GetCoin();
            int indexDay = CheckDailyReward.GetInstance().DailyRewardModel.IndexDay;
            DailyRewarDialog dialog = GameManager.Instance.OnShowDialog<DailyRewarDialog>("DailyAward");
            dialog.ParseData(coin, indexDay);
            this.AddDiamond(coin);
            SaveDailyRewardModel(CheckDailyReward.GetInstance().DailyRewardModel);
        }
    }
    public static bool ConvertIntToBool(int nValue)
    {
        return nValue == 0;
    }

    public int ConvertBoolToInt(bool bValue)
    {
        return (!bValue) ? 1 : 0;
    }

    public void SaveDailyRewardModel(SaveDailyReward saveDailyReward)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = File.Create(Application.persistentDataPath + "/dailyreward.savegame");
        binaryFormatter.Serialize(fileStream, saveDailyReward);
        fileStream.Close();
    }

    public SaveDailyReward LoadDailyReward()
    {
        SaveDailyReward result = null;
        if (File.Exists(Application.persistentDataPath + "/dailyreward.savegame"))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = File.Open(Application.persistentDataPath + "/dailyreward.savegame", FileMode.Open);
            result = (SaveDailyReward)binaryFormatter.Deserialize(fileStream);
            fileStream.Close();
        }
        return result;
    }
    #endregion
}
