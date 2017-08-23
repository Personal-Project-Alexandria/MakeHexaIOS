using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderDialog : BaseDialog {

	public GameObject grid;
	public GameObject itemPrefab;

	public override void OnShow(Transform transf, object data)
	{
		base.OnShow(transf, data);
		Setup();
	}

	public void Setup()
	{
		StartCoroutine(LoadScore());
	}

	IEnumerator LoadScore()
	{
		LeaderBoard.Instance.LoadScoreByMode(0);
		while (LeaderBoard.Instance.leaderBoards[0].load == true)
		{
			yield return null;
		}
		
		List<dreamloLeaderBoard.Score> scoreList = LeaderBoard.Instance.GetLeaderBoard(0);
		for (int i = 0; i < 10; i++)
		{
			if (i < scoreList.Count)
			{
				GameObject scoreBar = Instantiate(itemPrefab, grid.transform);
				scoreBar.transform.localScale = Vector3.one;
				scoreBar.GetComponent<ModeItem>().Setup(scoreList[i].playerName, scoreList[i].score.ToString());

				if (i == 0)
				{
					scoreBar.GetComponent<Image>().color = Palette.Translate(PColor.GOLD);
				}
				else if (i == 1)
				{
					scoreBar.GetComponent<Image>().color = Palette.Translate(PColor.SILVER);
				}
				else if (i == 2)
				{
					scoreBar.GetComponent<Image>().color = Palette.Translate(PColor.BRONZE);
				}
			}
		}
	}

	public void OnClickBack()
	{
		this.OnCloseDialog();
	}
}
