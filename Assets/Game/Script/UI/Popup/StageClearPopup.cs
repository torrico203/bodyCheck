//BOMIN
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;

public class StageClearPopup : MonoBehaviour
{



    [SerializeField]
    private RectTransform body;

    [SerializeField]
    private CanvasGroup touch;

    
    public List<Stuff> rewards = new List<Stuff>();

    [SerializeField]
    private Transform rewardParent;

    private List<StuffButton> rewardList = new List<StuffButton>();

    private bool on = false;

    void OnEnable(){
        if(!UI.Loaded) return;

        
        for(int i=0;i<rewardList.Count;i++){
            rewardList[i].ReturnObject();
        }
        rewardList.Clear();
        
        //Effect.Play("Power_Burst_V9",body.position,UI.I.transform);

        // for(int i=0;i<rewards.Count;i++){
        //     Debug.Log("reward1 : "+i);
        // }
        on = false;
        
        body.anchoredPosition = new Vector2(0, 0);
        body.localScale = new Vector3(0, 1, 1);
        touch.alpha = 0;
        body.DOAnchorPosY(-600f, 1.6f).SetEase(Ease.OutBack).onComplete = ()=>{
            Effect.Play("Power_Burst_V9",body.position,UI.I.transform);

            body.DOScale(1, 0.5f).SetEase(Ease.OutBack);

            touch.DOFade(1, 0.5f).SetDelay(0.5f).OnComplete(()=>{
                on = true;
            });;

            SetReward();
    
        };

        //스테이지 데이터 클리어 처리
        Data.StageClear(Data.DungeonName);
        
    }

    void SetReward(){

        for(int i=0;i<rewards.Count;i++){
            //Debug.Log("reward2 : "+i);
            StuffButton button = Pool.GetObject<StuffButton>("StuffButton");
            button.transform.SetParent(rewardParent);
            button.transform.localScale = Vector3.one;
            button.Stuff = rewards[i];
            button.InitObject();
            button.Show();
            rewardList.Add(button);
            
        }
    }

    public void OnClick(){
        if(!on) return;

        foreach(Transform child in rewardParent){
            StuffButton button = child.GetComponent<StuffButton>();
            if(button != null){
                button.ReturnObject();
            }
        }

        gameObject.SetActive(false);
        UI.Game.GoMain();
    }


}
