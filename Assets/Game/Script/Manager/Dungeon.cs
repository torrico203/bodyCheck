using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Lofelt.NiceVibrations;


public class Dungeon : Game
{
    private DungeonRoom room = null;

    private DungeonInfo dungeonInfo;
    private DungeonRoomInfo roomInfo;

    private int eventIdx = 0;
    [SerializeField]
    private int roomIdx = 0;
    public int RoomIdx{
        get => roomIdx;
    }
    private bool roomClear = false;
    private bool gateOpen = false;

    private bool roomLoad = false;
    

    private bool inGate = false;
    public bool InGate{
        get{
            return inGate;
        }
    }

    private bool roomRinse = false;

    private GameObject shop = null;

    public DungeonRoom.Type nextType = DungeonRoom.Type.None;

    void Awake()
    {
        gameType = Game.Type.Dungeon;
        dungeon = this;
        base.Awake();
    }

    void Start()
    {
        base.Start();

        string dungeonName = Data.DungeonName;
        dungeonInfo = Info.Dungeon[dungeonName];
        //Debug.Log("Dungeon : "+dungeonName);
        
        exp = 0;
        totalCoin = 0;

        globalLight.enabled = false;
        
        
        monsterPool.InitPool(dungeonInfo.monster);

        Sound.PlayBGM(Data.DungeonName);

    }


    void Update()
    {
        base.Update();

        
        if(roomLoad){
            if(monsterPool.Monsters.Count<=0){ //몬스터 없을떄
                eventIdx++;

                

                if(eventIdx>roomInfo.count){
                    if(!roomClear){
                        Absorb(true);
                        roomClear = true;

                        //방 리워드 지급
                        switch(room.type){
                            case DungeonRoom.Type.LuckyBox:
                                Effect.Play("RewardSummon", room.RewardTransform.position);
                                CreateInGameItem("LuckyBox",room.RewardTransform.position,0f);
                            break;
                            case DungeonRoom.Type.Heart:
                                CreateInGameItem("Heart",room.RewardTransform.position,0f);
                            break;
                            case DungeonRoom.Type.MiddleBoss:
                                MonsterSpawns(dungeonInfo.middleBoss,true,false,true);
                            break;
                            case DungeonRoom.Type.Boss:
                                //클리어 조건
                                UI.Game.SetStageClear();
                            break;
                            case DungeonRoom.Type.Elite:
                                MonsterSpawns(null,false,true);
                            break;
                            case DungeonRoom.Type.Silver:
                                Effect.Play("SilverDropAlot", room.RewardTransform.position);
                                for(int i=0;i<10;i++)
                                    CreateInGameItem("Silver",room.RewardTransform.position);
                            break;
                            case DungeonRoom.Type.SkillBox:
                                Effect.Play("RewardSummon", room.RewardTransform.position);
                                CreateInGameItem("SkillBox",room.RewardTransform.position,0f);
                            break;
                        }
                        
                    }
                    if(!gateOpen){
                        if(monsterPool.Monsters.Count == 0){
                            if(!roomRinse){
                                if(room.type != DungeonRoom.Type.Silver && room.type != DungeonRoom.Type.MiddleBoss)
                                    Absorb(true);
                                roomRinse = true;
                            }
                            if(inGameItems.Count == 0){
                                if(roomIdx == dungeonInfo.rooms.Length-2)
                                    room.GateOpen(true);    //마지막 전에는 무조건 보스방
                                else
                                    room.GateOpen();
                                gateOpen = true;
                            }
                        }
                    }
                }
                else{

                    MonsterSpawns(null,room.type == DungeonRoom.Type.Boss,false);
                    
                    
                }


            }
        }
        else
        {
            if(room == null)
            {
                // dungeonInfo의 maps에서 랜덤으로 맵 선택
                if (dungeonInfo.maps != null && dungeonInfo.maps.Length > 0)
                {   
                    //방 생성
                    int randomIndex = Random.Range(0, dungeonInfo.maps.Length-1);//마지막건 빼고(보스방이니까)
                    if(roomIdx == dungeonInfo.rooms.Length-2){
                        //마지막 전 방
                        for(int i=0;i<dungeonInfo.maps.Length;i++){
                            if(dungeonInfo.maps[i].GetComponent<DungeonRoom>().Gates.Length == 1){
                                randomIndex = i;
                            }
                        }
                    }
                    if(roomIdx == dungeonInfo.rooms.Length-1){
                        randomIndex = dungeonInfo.maps.Length-1;
                    }
                    GameObject roomObj = Instantiate(dungeonInfo.maps[randomIndex]);
                    room = roomObj.GetComponent<DungeonRoom>();
                    if(nextType == DungeonRoom.Type.None){
                        //첫번째 룸은 무조건 럭키박스
                        room.type = DungeonRoom.Type.LuckyBox;
                    }
                    else{
                        room.type = nextType;
                    }
                    
                    roomInfo = dungeonInfo.rooms[roomIdx];
                    

                    //플레이어 초기 위치 설정
                    Transform entry = room.Entries[Random.Range(0, room.Entries.Count)];
                    player.transform.position = entry.position;


                    //방 입장
                    //지역 이동시 hp 회복 적용
                    player.Heal(Mathf.RoundToInt(player.NStat.hp * player.NStat.hpRecovery));

                    eventIdx = 0;
                    roomClear = false;
                    gateOpen = false;
                    inGate = false;
                    roomRinse = false;
                    if(this.shop != null){
                        Destroy(shop);
                        shop = null;
                    }

                    if(room.type == DungeonRoom.Type.Shop){
                        //상점은 몬스터가 없음
                        Assets.CreateAsset("DungeonShop.prefab",(shop)=>{
                            this.shop = shop;
                        });
                        eventIdx = roomInfo.count+1;
                    }
                    Debug.Log("게이트 밖입니다.");

                    MainCamera.SetPosition(player.transform.position);
                    MainCamera.CurtainOut(2f, ()=>{
                        roomLoad = true;
                    });

                }
            }
            
        }
    }

    public override void GateIn(DungeonRoom.Type type){
        if(inGate) return;
        inGate = true;
        
        Debug.Log("게이트 안입니다.");

        //MainCamera.SetPosition(player.transform.position);
        MainCamera.CurtainIn(1f, ()=>{
            nextType = type;
            Destroy(room.gameObject);
            room = null;
            roomLoad = false;
            if(type != DungeonRoom.Type.Shop){ //상점은 방 안올라감
                roomIdx ++;
                exp ++;
                player.VStat.extraExp += 0.2f;
            }
        },false);

        
    }

    void MonsterSpawns(MonsterRate[] monsters = null,bool boss = false,bool elite = false,bool mBoss = false){
        if(monsters == null) monsters = roomInfo.monsters;
        float total = 0f;
        int i;
        for(i=0;i<monsters.Length;i++)
            total += monsters[i].rate;
        int monsterCount = 5;   //일단 임의로 5마리씩
        if(boss) monsterCount = 1;

        for(i=0;i<monsterCount;i++){
            
            float r = Random.Range(0,total);
            
            for(int j=0;j<monsters.Length;j++){
                if(r<=monsters[j].rate){
                    if(boss){
                        Stat rate = new Stat()+10f;
                        rate.att = 3f;
                        Monster bossMonster = BossSpawn(monsters[j].name,dungeonInfo.level,roomInfo.rate*rate,mBoss);
                        MainCamera.Focusing(bossMonster);
                    }
                    else{
                        if(elite){
                            Stat rate = new Stat() + 3f;
                            rate.att = 2f;
                            MonsterSpawn(monsters[j].name,dungeonInfo.level,roomInfo.rate*rate,false,true);
                        }
                        else
                            MonsterSpawn(monsters[j].name,dungeonInfo.level,roomInfo.rate);
                    }
                    break;
                }
                r -= monsters[j].rate;
            }
        }
    }

    Monster BossSpawn(string name,int level, Stat rate,bool isMBoss = false){
        Monster mBoss = MonsterSpawn(name,level,rate,isMBoss);
        boss.Add(mBoss);
        Sound.PlaySFX("BossAppear");
        Sound.PlayBGM("Boss");
        return mBoss;
    }
    Monster MonsterSpawn(string name,int level, Stat rate,bool isMBoss = false,bool isElite = false){
        Transform spawnPoint = room.SpawnPoints[Random.Range(0, room.SpawnPoints.Count)];
        Vector3 pos = new Vector3(0, Random.Range(0f, 1f), 0f);
        pos = Quaternion.Euler(0f, 0f, Random.Range(0f,360f)) * pos;
        return monsterPool.MonsterSpawn(name, level, spawnPoint.position+pos, rate,isMBoss,isElite);
    }

    
}
