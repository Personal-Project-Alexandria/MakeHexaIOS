using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveDailyReward
{
    public string StartDay;

    public int IndexDay;
}


public class CheckDailyReward  {

    private const int BASE_COIN = 350;

    private const int BONUS_DAILY = 150;

    private static CheckDailyReward _instance;

    private SaveDailyReward _saveDailyReward;

    private List<int> _listCoinReward = new List<int>(7);

    public SaveDailyReward DailyRewardModel
    {
        get
        {
            return this._saveDailyReward;
        }
        set
        {
            this._saveDailyReward = value;
        }
    }

    public CheckDailyReward()
    {
        for (int i = 0; i < 7; i++)
        {
            this._listCoinReward.Add(200 + 50 * i);
        }
    }

    public static CheckDailyReward GetInstance()
    {
        if (CheckDailyReward._instance == null)
        {
            CheckDailyReward._instance = new CheckDailyReward();
        }
        return CheckDailyReward._instance;
    }

    public void UpdateFirstPlay()
    {
        this._saveDailyReward = new SaveDailyReward();
        this._saveDailyReward.StartDay = DateTime.UtcNow.ToString();
        this._saveDailyReward.IndexDay = 0;
    }

    public bool Check()
    {
        DateTime utcNow = DateTime.UtcNow;
        DateTime d = DateTime.Parse(this._saveDailyReward.StartDay);
        TimeSpan timeSpan = utcNow - d;
        bool result;
        if (timeSpan.Days == 0)
        {
            result = false;
        }
        else if (timeSpan.Days == 1)
        {
            this._saveDailyReward.IndexDay++;
            this._saveDailyReward.IndexDay = ((this._saveDailyReward.IndexDay < this._listCoinReward.Count) ? this._saveDailyReward.IndexDay : (this._listCoinReward.Count - 1));
            this._saveDailyReward.StartDay = d.AddDays(1.0).ToString();
            result = true;
        }
        else
        {
            this._saveDailyReward.IndexDay = 0;
            this._saveDailyReward.StartDay = utcNow.ToString();
            result = true;
        }
        return result;
    }

    public int GetCoin()
    {
        return this._listCoinReward[this._saveDailyReward.IndexDay];
    }
}
