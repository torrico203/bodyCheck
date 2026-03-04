using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Lofelt.NiceVibrations;


public class Game : MonoBehaviour
{
    
    [SerializeField] 
    protected Player player;
    public Player Player { get => player; }

    [SerializeField]
    protected GameObject PlayerTarget;

    [SerializeField]
    protected MonsterPool monsterPool;
    public MonsterPool MonsterPool{
        get => monsterPool;
    }
    public int monsterCount{
        get => monsterPool.Monsters.Count;
    }
    public List<Monster> monsters{
        get => monsterPool.Monsters;
    }

    [SerializeField]
    protected Transform fieldParent;

    protected int level = 1;
    public int Level { get => level; set => level = value; }

    protected Dungeon dungeon;
    public Dungeon Dungeon{
        get => dungeon;
    }

    

    protected List<Monster> boss = new List<Monster>();
    public List<Monster> Boss { get => boss; }


    protected float targetSearchTime = 0f;
    protected float targetSearchDelay = 1f;

    protected bool forcedTarget = false;

    protected List<InGameItem> inGameItems = new List<InGameItem>();


    protected float gameStartTime = 0f;

    protected int totalCoin = 0;
    public int TotalCoin { get => totalCoin; set => totalCoin = value; }

    public int exp = 0;

    [SerializeField]
    protected Light2D globalLight;

    protected int barrelCount = 0;
    protected float barrelTime = 0f;

    protected int killCount = 0;
    public int KillCount { get => killCount; }

    protected Type gameType;
    public Type GameType{
        get => gameType;
    }

    protected void Awake()
    {
        UI.Game.Manager = this;
        MainCamera.Target = player;
        MainCamera.InFocus = false;
        gameStartTime = Time.time;
    }

    protected void Start()
    {
        //게임 시작

        Data.Player.wealth[(int)Wealth.Type.silver] = 0;


        player.Equip = Data.Player.equip;
        //player.Stat = Data.Player.stat;
        player.Stat = Data.ResultStat();
    }


    protected void Update()
    {
        if(UI.Game.GameOver) return;
        //player.MoveDir = GameUI.MoveDir;
        float now = Time.time;


        //보스 죽었을때 데이터 정리
        for(int i=0;i<boss.Count;i++){
            if(boss[i].InDeath){
                
                if(boss.Count == 1){//보스가 하나 남았을때
                    Time.timeScale = 0.2f;
                    MainCamera.Focusing(boss[i]);
                }

                //if(stagePointIdx<stageInfo.stagePoint.Length){
                    //보스용 럭키박스 이거 지급 조건에 대해서 수정 요
                    // InGameItem box = Pool.GetObject<InGameItem>("LuckyBox");
                    // box.transform.position = boss[i].transform.position;
                    // box.InitObject();
                    // inGameItems.Add(box);
                    
                //}

                boss.RemoveAt(i);
                i--;
            }
        }

        
        //게임오버 조건
        if(player.InDeath){
            //if(!UI.Game.GameOver)
            UI.Game.SetGameOver();
            return;
        }




        //타겟 서치
        if(player.Target == null){
            targetSearchTime = now - targetSearchDelay-1f;
        }
        if(now - targetSearchTime > targetSearchDelay){
            targetSearchTime = now;
            if(!forcedTarget){
                player.Target = monsterPool.GetNearMonster(player.transform.position);
            }            
        }

        //충돌문제해결전까지임시로꺼둠
        // if(Input.GetMouseButtonDown(0)){
        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //     RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
        //     if(hit.collider != null){
        //         if(hit.collider.CompareTag("Monster")){
        //             Actor target = hit.collider.GetComponent<Actor>();
        //             player.Target = target;
        //             forcedTarget = true;
        //             Debug.Log("Target : "+target.name);
        //         }
               
        //     }
        // }


        //플레이어 타겟
        if(player.Target != null){
            if(!PlayerTarget.activeSelf)
                PlayerTarget.SetActive(true);
            PlayerTarget.transform.position = player.Target.transform.position;
        }
        else{
            forcedTarget = false;
            if(PlayerTarget.activeSelf)
                PlayerTarget.SetActive(false);
            
        }
    }

    public void AddArtifact(string infoKey){
        ArtifactInfo artifactInfo = Info.Artifact[infoKey];
        if(artifactInfo.addition)
            player.VStat += (artifactInfo.stat);
        else{
            Stat value = (player.NStat*(artifactInfo.stat));
            for(int i=0;i<value.Count();i++)
            {
                if(value[i]!=0){
                    
                    if(Info.IntStat[i]!=0) {
                        value[i] = Mathf.Round(value[i]);
                        if(value[i]<1) value[i] = 1;
                    }
                }
            }
            player.VStat += value;
        }
    }

    public void AddPassive(string infokey){
        for(int i=0;i<player.Passive.Count;i++){
            if(player.Passive[i].infoKey == infokey){
                player.Passive[i].level++;
                return;
            }
        }
        PassiveData data = new PassiveData();
        data.infoKey = infokey;
        data.level = 1;
        player.Passive.Add(data);
    }

    public int GetPassiveLevel(string infoKey){
        for(int x=0;x<player.Passive.Count;x++){
            if(player.Passive[x].infoKey == infoKey)
                return player.Passive[x].level;
        }
        return 0;
    }

    public void DamageText(string dmg, Vector3 pos, int floor = 0,bool isPlayer = false, bool isCrit = false){
        
        DamageText dmgtxt = Pool.GetObject<DamageText>("DamageText");
        dmgtxt.transform.position = pos;
        dmgtxt.text = dmg;
        dmgtxt.floor = floor;
        dmgtxt.startY = 1f;
        //dmgtxt.isPlayer = isPlayer;
        if(isPlayer){
            dmgtxt.color = Color.red;
            dmgtxt.scale = 0.6f;

            Haptic.Play(HapticPatterns.PresetType.RigidImpact);
        }
        else{
            if(isCrit){
                dmgtxt.color = Color.yellow;
                dmgtxt.scale = 0.7f;
            }
            else{
                dmgtxt.color = Color.grey;
                dmgtxt.scale = 0.5f;
            }
        }
        dmgtxt.InitObject();
    }

    public int AddExp(int exp){
        int addExp = exp + Mathf.FloorToInt(exp*player.NStat.extraExp);
        player.Exp += addExp;
        return addExp;
    }
    public int AddGold(int gold){
        int addGold = gold + Mathf.FloorToInt(gold*player.NStat.extraGold);
        totalCoin += addGold;
        return addGold;
    }
    public int AddSilver(int silver){
        int addSilver = silver + Mathf.FloorToInt(silver*player.NStat.extraSilver);
        //totalCoin += addSilver;
        Data.AddWealth(Wealth.Type.silver,addSilver,false);
        return addSilver;
    }

    public void MonsterDrop(Vector3 pos, bool isBarrel = false, bool isMBoss = false,bool isElite = false){

        if(isBarrel){
            float rate = Random.Range(0f,1f);
            //Debug.Log("Rate : "+rate);
            if(rate < 0.2f)
                CreateInGameItem("ExpAbsorber",pos);
            else if(rate < 0.6f)
                CreateInGameItem("Meat",pos);
            else if(rate < 0.9f)
                CreateInGameItem("CoinBundle",pos);
            else
                CreateInGameItem("Coin",pos);
            
        }
        else if(isMBoss){
            CreateInGameItem("Artifact",pos);

            for(int i=0;i<5;i++)
                CreateInGameItem("Silver",pos);
            
        }
        else if(isElite){
            for(int i=0;i<3;i++)
                CreateInGameItem("ExpBall",pos);
            
        }
        else{
            float rate = Random.Range(0f,1f);
            if(rate < 0.1f){
                CreateInGameItem("Silver",pos);
                if(rate <= 0.01f)
                    CreateInGameItem("Meat",pos);
                
            }
            else{
                CreateInGameItem("ExpBall",pos);
            }

            killCount++;
        }
    }

    protected void CreateInGameItem(string name, Vector3 pos, float force = 0.5f){
        InGameItem item = Pool.GetObject<InGameItem>(name);
        Vector3 _pos = new Vector3(0, Random.Range(0f, force), 0f);
        _pos = Quaternion.Euler(0f, 0f, Random.Range(0f,360f)) * _pos;
        item.transform.position = pos;
        item.dest = _pos;
        item.InitObject();
        inGameItems.Add(item);
    }

    public void Absorb(bool truely = false){
        for(int i = 0; i < inGameItems.Count; i++){
            if(truely){
                //if(inGameItems[i].type != InGameItem.Type.luckyBox){
                    inGameItems[i].Absorb(true);
                //}
            }
            else{
                if(inGameItems[i].type == InGameItem.Type.exp){
                    inGameItems[i].Absorb(true);
                }
            }
        }
    }



    public void PickUp(InGameItem item){
        inGameItems.Remove(item);
    }


    
    

    protected void OnDestroy()
    {
        for(int i = 0; i < inGameItems.Count; i++){
            inGameItems[i].ReturnObject();
        }
        // for(int i = 0; i < monsters.Count; i++){
        //     monsters[i].gameObject.SetActive(false);
        // }
    }


    public void ReturnMonster(Monster monster){
        if(monster.SubType == Monster.Type.Barrel){
            barrelCount--;
        }
        monsterPool.ReturnMonster(monster);
    }

    public virtual void GateIn(DungeonRoom.Type type){}

    public virtual float NextPointCheat(){
       return 0f;
    }

    public virtual float NextBossCheat(){
        
       return 0f;
    }


    public enum Type{
        Field = 0,
        Dungeon
    }
    
}
