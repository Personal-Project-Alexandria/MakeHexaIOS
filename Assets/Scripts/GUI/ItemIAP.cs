using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemIAP : MonoBehaviour {

	public delegate void Callback();

	public Button buyButton;
	public Text itemName;
	public Text itemPrice;

	public void Init(Callback callback, string itemName, string price)
	{
		buyButton.onClick.AddListener(delegate { callback(); });
		this.itemName.text = itemName;
		this.itemPrice.text = price;
	}
}
