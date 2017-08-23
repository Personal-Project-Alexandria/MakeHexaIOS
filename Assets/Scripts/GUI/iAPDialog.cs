using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class iAPDialog : BaseDialog {

	public delegate void Transaction();

	public GameObject itemPrefab;
	public Transform itemParent;

	public override void OnShow(Transform transf, object data)
	{
		base.OnShow(transf, data);
		Setup();
	}

	public void Setup()
	{
		for (int i = 0; i < 11; i++)
		{
			GameObject item = (GameObject)Instantiate(itemPrefab, itemParent);
			item.transform.localScale = Vector3.one;
			Init(item.GetComponent<ItemIAP>(), i);
		}
	}

	public void Init(ItemIAP item, int i)
	{
		switch (i)
		{
		case 0: item.Init((ItemIAP.Callback)IAPManager.Instance.Buy1000Diamonds, "1000 diamonds", "0.99$"); break;
		case 1: item.Init((ItemIAP.Callback)IAPManager.Instance.Buy3500Diamonds, "3500 diamonds", "2.99$"); break;
		case 2: item.Init((ItemIAP.Callback)IAPManager.Instance.Buy7000Diamonds, "7000 diamonds", "4.99$"); break;
		case 3: item.Init((ItemIAP.Callback)IAPManager.Instance.Buy10000Diamonds, "10000 diamonds", "6.99$"); break;
		case 4: item.Init((ItemIAP.Callback)IAPManager.Instance.Buy15000Diamonds, "15000 diamonds", "9.99$"); break;
		case 5: item.Init((ItemIAP.Callback)IAPManager.Instance.Buy20000Diamonds, "20000 diamonds", "13.99$"); break;
		case 6: item.Init((ItemIAP.Callback)IAPManager.Instance.Buy30000Diamonds, "30000 diamonds", "19.99$"); break;
		case 7: item.Init((ItemIAP.Callback)IAPManager.Instance.Buy40000Diamonds, "40000 diamonds", "29.99$"); break;
		case 8: item.Init((ItemIAP.Callback)IAPManager.Instance.Buy50000Diamonds, "50000 diamonds", "39.99$"); break;
		case 9: item.Init((ItemIAP.Callback)IAPManager.Instance.Buy70000Diamonds, "70000 diamonds", "49.99$"); break;
		case 10: item.Init((ItemIAP.Callback)IAPManager.Instance.Buy150000Diamonds, "150000 diamonds", "89.99$"); break;
		default: break;
		}
	}

	public void RestorePurchase()
	{
        SoundManager.Instance.PlaySfx(SFX.Button);
        IAPManager.Instance.RestorePurchases();
	}
}
