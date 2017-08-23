using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SkillDialog : MonoBehaviour {

    public GameObject panelSkill;
    public GameObject panel;
    public SkillPanel skillHammer;
    public Text txtCoins;

    public delegate void CallbackUseSkill();
    public CallbackUseSkill callbackUseSkill;
    private int coinUseIcon;
    public void setCallbackUseSkill(CallbackUseSkill callback)
    {
        this.callbackUseSkill = callback;
    }

	private void OnUseSkillHammer()
    {
        
        Hexagon.Instance.ActiveHammer();
        skillHammer.gameObject.SetActive(true);
        skillHammer.setCallback(Hexagon.Instance.OnUseHammerClick, this.coinUseIcon);
        this.gameObject.SetActive(false);
    }
    private void OnUserSkillDellete()
    {
        Hexagon.Instance.ActiveSkillDelete();
        skillHammer.gameObject.SetActive(true);
        skillHammer.setCallback(Hexagon.Instance.OnUseDeleteClick, this.coinUseIcon);
        this.gameObject.SetActive(false);
    }
    public void OnClickUseByDiamond()
    {
        //oogleAnalyticsV4.getInstance().LogEvent("SKILL", "Use Skill Hammer By Diamond", string.Empty, 0L);

        if (this.callbackUseSkill != null )
        {
            if (UserProfile.Instance.GetDiamond() >= this.coinUseIcon)
                this.callbackUseSkill.Invoke();
            else
            {
                iAPDialog dialog = GameManager.Instance.OnShowDialog<iAPDialog>("iAP");
                this.HideSkill();
            }
        }
    }
    public void OnClickUseByAds()
    {
		if (this.callbackUseSkill != null)
		{
            //oogleAnalyticsV4.getInstance().LogEvent("SKILL", "Use Skill Hammer By Ads", string.Empty, 0L);

            AdManager.Instance.ShowRewardVideo(this.callbackUseSkill);
		}
	}
    public void ShowSkill(Transform trans)
    {
        Vector3 pos = panelSkill.transform.InverseTransformPoint(trans.position);
        panel.transform.localPosition = new Vector3(pos.x, -250, 0);
        panel.transform.DOLocalMoveY(0, 0.25f, false);
    }
    public void ShowSkillHammer()
    {
        this.setCallbackUseSkill(this.OnUseSkillHammer);
        this.coinUseIcon = 150;
        this.txtCoins.text = this.coinUseIcon.ToString() + "$";
    }
    public void ShowSkillDelete()
    {
        this.setCallbackUseSkill(this.OnUserSkillDellete);
        this.coinUseIcon = 270;
        this.txtCoins.text = this.coinUseIcon.ToString() + "$";
    }
    public void HideSkill()
    {
        panel.transform.DOLocalMoveY(-250, 0.25f, false).OnComplete(() => { this.gameObject.SetActive(false); });
    }
}
