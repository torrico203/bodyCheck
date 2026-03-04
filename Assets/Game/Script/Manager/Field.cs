using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Lofelt.NiceVibrations;


public class Field : Game
{
    
    private StageInfo stageInfo;

    
    [SerializeField]
    protected StagePoint stagePoint = null;
    protected int stagePointIdx = 0;

    
    protected float monsterSpawnTime = 0f;
    //protected int maxMonster = 20;
    

    protected List<Monster> aggressive = new List<Monster>();
    public List<Monster> Aggressive { get => aggressive; }

    void Awake()
    {
        gameType = Game.Type.Field;
        base.Awake();
    }

    void Start()
    {
        base.Start();

        string stageName = "";//Data.StageName;
        stageInfo = Info.Stage[stageName];
        Debug.Log("Stage : "+stageName);


        //맵 팝업
        MapnamePopup mapnamePopup = UI.OpenPopup<MapnamePopup>("MapName");
        mapnamePopup.mapname = Util.LocalStr("Name",stageName);
        mapnamePopup.callback = ()=>{
            if(this.gameObject != null)
                UI.OpenPopup<SlotMachinePopup>("SlotMachine");
        };

        //필드생성
        int x=0;
        for(int i=0;i<5;i++){
            for(int j=0;j<=x;j++){
                GameObject cell = Instantiate(stageInfo.field,fieldParent);   
                
                cell.transform.position = new Vector3(-x*25f+50f*j, 25f-i*12.5f, 0f);
            }
            if(i<2)x++;
            else x--;
        }

        //라이트 설정
        globalLight.intensity = stageInfo.globalLightIntensity;
        globalLight.color = stageInfo.globalLightColor;
        if(stageInfo.globalLightIntensity < 0.5f){
            player.Light.enabled = true;
        }
        else{
            player.Light.enabled = false;
        }

        //몬스터 풀 셋업
        monsterPool.InitPool(stageInfo.monster);

        //바렐은 60초 동안 생성되지 않음
        barrelTime = Time.time + 60f;
    }


    void Update()
    {
        base.Update();
        
        float now = Time.time;
        
        fieldParent.position = new Vector3(Mathf.FloorToInt((player.transform.position.x+25f)/50f)*50f,Mathf.FloorToInt((player.transform.position.y+12.5f)/25f)*25f,0f);


        if(aggressive.Count > 0){
            for(int i=0;i<aggressive.Count;i++){
                if(aggressive[i].InDeath){
                    aggressive.RemoveAt(i);
                    i--;
                }
            }
        }

        if(barrelCount < 5){
            if(now - barrelTime > 1f){
                MonsterSpawn("Barrel");
                barrelCount++;
                barrelTime = Time.time;
            }
        }


        // 스테이지 포인트가 넘어가는 경우에
        if(stagePointIdx<stageInfo.stagePoint.Length){
            if(stageInfo.stagePoint[stagePointIdx].time<(now-gameStartTime)){
                if(stageInfo.stagePoint[stagePointIdx].type == StagePoint.Type.Monster){
                    stagePoint = stageInfo.stagePoint[stagePointIdx];
                }
                else if(stageInfo.stagePoint[stagePointIdx].type == StagePoint.Type.Boss){
                    StagePoint bossPoint = stageInfo.stagePoint[stagePointIdx];

                    foreach(MonsterRate monRate in bossPoint.monsters){
                        Monster mon = MonsterSpawn(monRate.name, stageInfo.level, bossPoint.rate);
                        boss.Add(mon);
                        MainCamera.Focusing(mon);
                        Sound.PlaySFX("BossAppear");

                    }

                }
                else if(stageInfo.stagePoint[stagePointIdx].type == StagePoint.Type.aggressive){
                    StagePoint aggressivePoint = stageInfo.stagePoint[stagePointIdx];

                    foreach(MonsterRate monRate in aggressivePoint.monsters){

                        for(int i=0;i<aggressivePoint.count/aggressivePoint.monsters.Length;i++){
                            Monster mon = MonsterSpawn(monRate.name, stageInfo.level, aggressivePoint.rate);
                            aggressive.Add(mon);
                        }
                        
                        
                    }
                    if(aggressivePoint.alert)
                        UI.OpenPopupSimple("AggressivePoint");
                }
                stagePointIdx++;
            }
        }
        else{
            if(boss.Count <= 0){
                //엔딩 조건
                UI.Game.SetStageClear();
                return;
            }
        }

        if(stagePoint != null){
            //몬스터 스폰
            if(now - monsterSpawnTime > 0.5f){
                if(monsterPool.Monsters.Count-barrelCount-aggressive.Count < stagePoint.count){
                    
                    float total = 0f;
                    for(int i=0;i<stagePoint.monsters.Length;i++)
                        total += stagePoint.monsters[i].rate;
                    
                    float r = Random.Range(0,total);
                    
                    for(int i=0;i<stagePoint.monsters.Length;i++){
                        if(r<=stagePoint.monsters[i].rate){
                            this.MonsterSpawn(stagePoint.monsters[i].name, stageInfo.level, stagePoint.rate);
                            break;
                        }
                        r -= stagePoint.monsters[i].rate;
                    }

                    //MonsterRate monRate = stagePoint.monsters[Random.Range(0,stagePoint.monsters.Length)];

                    
                }
            }
        }
    }

    Monster MonsterSpawn(string name, int level = 1, Stat rate = null){
        
        Vector3 pos = new Vector3(0, Random.Range(8f, 12f), 0f);
        pos = Quaternion.Euler(0f, 0f, Random.Range(0f,360f)) * pos;
        pos += player.transform.position;

        return monsterPool.MonsterSpawn(name, level, pos, rate);
    }


    public override float NextPointCheat(){
        //if(stageInfo.stagePoint[stagePointIdx].time<(now-gameStartTime))

        gameStartTime = Time.time-stageInfo.stagePoint[stagePointIdx].time;
        return gameStartTime;
    }

    public override float NextBossCheat(){
        while(true){
            if(stageInfo.stagePoint[stagePointIdx].type == StagePoint.Type.Boss){
                gameStartTime = Time.time-stageInfo.stagePoint[stagePointIdx].time;
                return gameStartTime;
            }
            if(stagePointIdx >= stageInfo.stagePoint.Length)
                return 0f;
            stagePointIdx++;
        }
    }

    
}
