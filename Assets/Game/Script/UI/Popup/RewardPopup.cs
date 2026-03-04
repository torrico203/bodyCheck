//BOMIN
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using System;
using System.Collections;

public class RewardPopup : MonoBehaviour
{
    
    public List<Stuff> rewards = new List<Stuff>();

    [SerializeField]
    private Transform rewardParent;

    private List<StuffButton> rewardList = new List<StuffButton>();

    [SerializeField]
    private TextMeshProUGUI titleText;

    public string Title{
        set{
            titleText.text = value;
        }
    }


    public Action callback = null;

    void Awake(){
        
    }


    public void Show(){
        
        

        for(int i=0;i<rewardList.Count;i++){
            rewardList[i].ReturnObject();
        }
        rewardList.Clear();
        for(int i=0;i<rewards.Count;i++){
            StuffButton button = Pool.GetObject<StuffButton>("StuffButton");
            button.transform.SetParent(rewardParent);
            button.transform.localScale = Vector3.one;
            button.Stuff = rewards[i];
            button.InitObject();
            button.Show();
            rewardList.Add(button);
        }

        StartCoroutine(ShowLate());
    }

    IEnumerator ShowLate(){
        yield return new WaitForSeconds(0.3f);
        Effect.Play("BuyGoldEffect",titleText.transform.position,UI.I.transform);
    }

    public void Close(){
        callback?.Invoke();
        gameObject.SetActive(false);
    }


}
