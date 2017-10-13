using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayDialog : BaseDialog {
    public Text txtScore;
    public Text txtHightScore;
	public Text diamond;
    public SkillDialog skill;
    
	private void Update()
	{
		diamond.text = UserProfile.Instance.GetDiamond().ToString();
	}

	public override void OnShow(Transform transf, object data)
    {
        base.OnShow(transf, data);
        AdManager.Instance.ShowInterstitial();
        StartCoroutine(OnStartGame());
        Hexagon.Instance.callbackScore.Add(UpdateScore);
        this.txtHightScore.text = "High Score: " + UserProfile.Instance.GetHighScore().ToString();
    }
    public override void OnHide()
    {
        Hexagon.Instance.callbackScore.Remove(UpdateScore);
        base.OnHide();

    }
    public IEnumerator OnStartGame()
    {
        yield return new WaitForEndOfFrame();
        Hexagon.Instance.OnNewGame();
    }
    public void UpdateScore(int score)
    {
        this.txtScore.text = "Score: " + score.ToString();
        if(score > UserProfile.Instance.GetHighScore())
        {
            this.txtHightScore.text = "Hight Score: " + score.ToString();
            UserProfile.Instance.SetHighScore(score);
        }
    }

    public void OnUseSkillHammer(Transform trans)
    {
        SoundManager.Instance.PlaySfx(SFX.Button);
        this.skill.gameObject.SetActive(true);
        this.skill.ShowSkill(trans);
        this.skill.ShowSkillHammer();
        //oogleAnalyticsV4.getInstance().LogEvent("SKILL", "Use Skill Hammer Click", string.Empty, 0L);

    }
    public void OnUseSkillDelete(Transform trans)
    {
        SoundManager.Instance.PlaySfx(SFX.Button);
        this.skill.gameObject.SetActive(true);
        this.skill.ShowSkill(trans);
        this.skill.ShowSkillDelete();
        //oogleAnalyticsV4.getInstance().LogEvent("SKILL", "Use Skill Delete Click", string.Empty, 0L);

    }

    public void OnPause()
    {
        SoundManager.Instance.PlaySfx(SFX.Button);
        PauseDialog dialog = GameManager.Instance.OnShowDialog<PauseDialog>("Pause");
        dialog.setCallbackReplay(Hexagon.Instance.OnReplay);
    }
}
