//BOMIN
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using System;

public class SynthesisPopup : MonoBehaviour
{

    
    public List<Stuff> rewards = new List<Stuff>();

    [SerializeField]
    private Transform rewardParent;

    private List<StuffButton> rewardList = new List<StuffButton>();



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
    }

    public void Close(){
        callback?.Invoke();
        gameObject.SetActive(false);
    }


}
