//BOMIN
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Collections;
using DG.Tweening;
using TMPro;
public class SlotMachinePopup : MonoBehaviour
{
    [SerializeField]
    private Popup popup;
    public Action callback = null;

    [SerializeField]
    private SlotLineUI[] lines;

    [SerializeField]
    private Button startButton;

    [SerializeField]
    private GameObject result;
    [SerializeField]
    private Transform resultBody;

    [SerializeField]
    private TextMeshProUGUI resultNameText,resultExplainText;
    [SerializeField]
    private Image resultIcon;
    [SerializeField]
    private StarLevel resultLevel;


    [SerializeField]
    private SlotMachineInfo[] infos;

    private bool isStart = false;

    //public int lineCount = 0;

    void Update()
    {
        if(isStart){
            bool isStop = true;
            for(int i = 0; i < lines.Length; i++){
                if(lines[i].isRolling){
                    isStop = false;
                    break;
                }
            }
            if(isStop){
                isStart = false;
                int level = 0;
                for(int i = 0; i < lines.Length; i++){
                    int _level =0;
                    int idx = lines[i].idx;
                    for(int j = 0; j < lines.Length; j++){
                        if(lines[j].idx == idx){
                            _level++;
                        }
                    }
                    if(_level > level){
                        level = _level;
                    }
                }
                
                List<int> list = new List<int>();
                for(int i = 0; i < lines.Length; i++){
                    int _level = 0;
                    int idx = lines[i].idx;
                    for(int j = 0; j < lines.Length; j++){
                        if(lines[j].idx == idx){
                            _level++;
                        }
                    }
                    if(_level == level){
                        list.Add(idx);
                    }
                }
                int random = UnityEngine.Random.Range(0, list.Count);
                int resultidx = list[random];

                resultNameText.text = Util.LocalStr("Stat",infos[resultidx].type.ToString());

                float value = infos[resultidx].values[level-1];
                if(!infos[resultidx].addition){
                    value = UI.Game.Player.MStat[(int)infos[resultidx].type]*value;
                }

                resultExplainText.text = Util.LocalStr("Stat",infos[resultidx].type.ToString());
                resultExplainText.text += " " +Util.LocalStr("UI","increase",new[]{new{value = (Util.StatToString(value,infos[resultidx].type))}});
                //resultExplainText.text = infos[resultidx].type.ToString();

                resultIcon.sprite = infos[resultidx].icon;
                resultLevel.MaxLevel = 3;
                resultLevel.Level = level;

                Stat stat = UI.Game.Player.VStat;
                
                stat[(int)infos[resultidx].type] += value;
                
                UI.Game.Player.VStat = stat;


                StartCoroutine(Result(level));
            }
        }
    }

    IEnumerator Result(int level){
        yield return new WaitForSecondsRealtime(1f);
        result.gameObject.SetActive(true);
        resultBody.localScale = new Vector3(0, 1, 1);
        resultBody.DOScale(1, 0.2f).SetUpdate(true).SetEase(Ease.OutBack);
        Effect.Play("Confetti_V1",this.transform.position,UI.I.transform);
        switch(level){
            case 1:
                //Effect.Play("Power_Burst_V9",resultBody.position,UI.I.transform);
                //Effect.Play("Power_Burst_V8",resultBody.position,UI.I.transform);
                break;
            case 2:
                Effect.Play("Power_Burst_V8",resultBody.position,UI.I.transform);
                break;
            case 3:
                Effect.Play("Power_Burst_V9",resultBody.position,UI.I.transform);
                break;
        }
    }
    public void OnEnable(){
        if(!UI.Loaded) {
            gameObject.SetActive(false);
            return;
        }
        if(!Info.Loaded) {
            gameObject.SetActive(false);
            return;
        }
        Time.timeScale = 0;
    }


    public void OnClickStart(){
        for(int i = 0; i < lines.Length; i++){
            lines[i].Roll();
        }
        startButton.gameObject.SetActive(false);
        isStart = true;
    }

    public void Open(){

        //Effect.Play("Power_Up_v29",titleTransform.position,UI.I.transform);

        for(int i = 0; i < lines.Length; i++){
            lines[i].Setup(infos);
        }

        
        startButton.gameObject.SetActive(true);
    }

    public void Load(){
        //icon.sprite = Info.Equipment[data.infoKey].icon;
    }

    public void Close(){
        
        isStart = false;
        result.gameObject.SetActive(false);
        // for(int i = 0; i < buttons.Length; i++){
            
        //     buttons[i].Aura.SetActive(true);
        // }

        //UI.Game.JoystickOnOff(true);
        popup.Close(()=>{
            gameObject.SetActive(false);
            if(callback != null) callback();
            Time.timeScale = 1;
        });
    }

}
