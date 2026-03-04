//BOMIN
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using System;

public class BoxRewardPopup : MonoBehaviour
{


    [SerializeField]
    private RectTransform subBody,mainBody;

    
    public List<Stuff> rewards = new List<Stuff>();

    [SerializeField]
    private Transform rewardParent;

    [SerializeField]
    private StuffButton singleReward = null;

    private List<StuffButton> rewardList = new List<StuffButton>();

    [SerializeField]
    private BoxUI box;
    public BoxUI Box{
        get => box;
    }

    private int idx = 0;

    private bool ended = false;

    public Action callback = null;

    void Awake(){
        
    }

    void OnEnable(){
        idx = 0;
        
        subBody.gameObject.SetActive(true);
        mainBody.gameObject.SetActive(false);
        ended = false;
        
    }

    public void Show(){
        if(idx >= rewards.Count){
            EndShow();
        }
        singleReward.gameObject.SetActive(false);
        box.Close();
        box.Open(false,true,() =>{
            singleReward.gameObject.SetActive(true);
            singleReward.Stuff = rewards[idx];
            singleReward.InitObject();
            singleReward.Show();
            idx++;
        });
    }

    public void EndShow(){
        ended = true;
        subBody.gameObject.SetActive(false);
        mainBody.gameObject.SetActive(true);

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

    public void OnClick(){
        if(!ended){
            Show();
        }
    }


}
