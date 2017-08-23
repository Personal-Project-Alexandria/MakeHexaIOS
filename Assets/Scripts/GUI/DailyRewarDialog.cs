using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewarDialog : BaseDialog {
    public Text txtContent;

    public void ParseData(int coin, int day)
    {
        string text = "";
        switch (day)
        {
            case 0:
                text = "1st";
                break;
            case 1:
                text = "2nd";
                break;
            case 2:
                text = "3rd";
                break;
            default:
                text = day + 1 + "th";
                break;
        }
        this.txtContent.text = string.Concat(new object[]
        {
            "You get daily reward in\n ",
            text,
            " day!!!\n",
            coin,
            " Coin"
        });
    }
}
