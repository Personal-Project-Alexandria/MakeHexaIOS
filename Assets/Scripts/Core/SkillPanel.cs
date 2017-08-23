using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillPanel : MonoBehaviour, IPointerClickHandler {

    public delegate void OnUseSkillClick(PointerEventData eventData, SkillPanel skill, int coins);
    public OnUseSkillClick callbackUseSkill;
    private int coins = 0;
    public void setCallback(OnUseSkillClick callback, int coins)
    {
        this.coins = coins;
        this.callbackUseSkill = callback;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(callbackUseSkill != null)
        {
            callbackUseSkill.Invoke(eventData, this, coins);
        }
    }
}
