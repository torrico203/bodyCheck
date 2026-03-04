//BOMIN
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;


public class SmallNotice : PoolingObject
{
    public string content;

    [SerializeField]
    private TextMeshProUGUI text;

    [SerializeField]
    private SmallNoticeType type = SmallNoticeType.Scroll;
   

    void Awake()
    {
        
    }

    protected override void OnInit()
    {
        
        text.text = content;
        
        UI.SmallNoticeCount++;

        switch(type){
            case SmallNoticeType.Scroll:
                this.transform.localScale = new Vector3(0f,1f,1f);
                this.transform.DOScaleX(1f, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    this.transform.DOLocalMoveY(this.transform.localPosition.y + 50f, 1.5f).SetEase(Ease.OutExpo).OnComplete(() =>
                    {
                        
                        this.End();
                        
                    });
                });
                break;
        }
        
    }


    public void End(){
        switch(type){
            case SmallNoticeType.Scroll:
                this.transform.DOScaleX(0f,0.2f).SetEase(Ease.OutCubic).OnComplete(()=>{
                    UI.SmallNoticeCount--;
                    this.ReturnObject();
                    //this.gameObject.SetActive(false);
                });
                break;
        }
        
    }

    
}
public enum SmallNoticeType{
    None = 0,
    Scroll
}