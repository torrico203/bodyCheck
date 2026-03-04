using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StageGauge : MonoBehaviour
{

    [SerializeField]
    private GameObject eventIcon;

    [SerializeField]
    private UIGauge gauge;

    [SerializeField]
    private Transform iconParent;

    private string infoKey;
    public string InfoKey { 
        get => infoKey; 
        set{
            infoKey = value;
            SetUp();
        }
    }

    private float startTime;
    public float StartTime { get => startTime; set => startTime = value; }
    
    private float max;

    private List<StagePointIcon> iconList = new List<StagePointIcon>();

    private void SetUp(){
        startTime = Time.time;
        foreach(Transform child in iconParent){
            Destroy(child.gameObject);
        }

        if(UI.Game.Manager.GameType == Game.Type.Field){
            StageInfo info = Info.Stage[infoKey];
        
            max = info.stagePoint[info.stagePoint.Length-1].time;
            gauge.Set(0, Mathf.RoundToInt(max), 0);
            foreach(StagePoint e in info.stagePoint){
                if(e.alert){
                    GameObject icon = Instantiate(eventIcon, iconParent);
                    RectTransform rt = icon.GetComponent<RectTransform>();
                    rt.anchoredPosition = new Vector2(e.time/max*500f, 0f);
                    icon.GetComponent<StagePointIcon>().type = e.type;
                } 
            }
        }
        else if(UI.Game.Manager.GameType == Game.Type.Dungeon){
            //Debug.Log("Gauge "+infoKey);
            DungeonInfo info = Info.Dungeon[infoKey];

            max = info.rooms.Length;
            gauge.Set(0, Mathf.RoundToInt(max-1), 0);
            
            iconList.Clear();

            for(int i=0;i<max;i++){
                //Debug.Log("Gauge : "+i);
                GameObject iconObj = Instantiate(eventIcon, iconParent);
                RectTransform rt = iconObj.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(i/(max-1)*500f, 0f);
                StagePointIcon icon = iconObj.GetComponent<StagePointIcon>();
                if(i==max-1) icon.type = StagePoint.Type.Boss;
                icon.color = Color.white;
                iconList.Add(icon);
            }

        }

        
    }

    void Start(){
        
    }

    void Update(){
        if(UI.Game.Manager.GameType == Game.Type.Field){
            gauge.Set(0, (int)max, (int)(Time.time - startTime));
        }
        else if(UI.Game.Manager.GameType == Game.Type.Dungeon){
            gauge.Set(0, (int)max-1, UI.Game.Manager.Dungeon.RoomIdx);
            for(int i=0;i<max;i++){
                if(i<UI.Game.Manager.Dungeon.RoomIdx)
                    iconList[i].color = Color.green;
                else if(i==UI.Game.Manager.Dungeon.RoomIdx)
                    iconList[i].color = Color.yellow;
                
            }
        }
    }

    

}