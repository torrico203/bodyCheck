using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Lofelt.NiceVibrations;


public class MonsterPool : MonoBehaviour
{
    protected List<Monster> monsters = new List<Monster>();
    public List<Monster> Monsters { get => monsters; }


    [SerializeField]
    protected Transform poolParent;


    protected float monsterSpawnTime = 0f;
    protected int maxMonster = 20;

    protected List<Monster> boss = new List<Monster>();
    public List<Monster> Boss { get => boss; }

    protected List<Monster> aggressive = new List<Monster>();
    public List<Monster> Aggressive { get => aggressive; }

    protected List<InGameItem> inGameItems = new List<InGameItem>();

    protected Dictionary<string, Monster> monsterDic = new Dictionary<string, Monster>();
    protected Dictionary<string, Transform> monsterPoolDic = new Dictionary<string, Transform>();

    [SerializeField]
    protected GameObject barrel;

    protected int barrelCount = 0;
    protected float barrelTime = 0f;


    protected void Awake()
    {
       
    }

    protected void Start()
    {
       
    }


    protected void Update()
    {
        
    }

    public void InitPool(GameObject[] monsterList)
    {
        foreach(GameObject monster in monsterList){
            CreateMonsterPool(monster,monster.GetComponent<Monster>().InitCount);
        }
        CreateMonsterPool(barrel);
    }

    protected void CreateMonsterPool(GameObject monster,int count = 5){
        if(monster == null) return;
        if(!monsterPoolDic.ContainsKey(monster.name)){
            monsterDic.Add(monster.name, monster.GetComponent<Monster>());

            GameObject pool = new GameObject();
            pool.name = monster.name;
            pool.transform.SetParent(poolParent);
            monsterPoolDic.Add(monster.name, pool.transform);

            for(int i=0;i<count;i++){
                GameObject mon = Instantiate(monster, pool.transform);
                mon.SetActive(false);
                mon.name = monster.name;
            }
        }
    }

    public Monster MonsterSpawn(string name, int level = 1, Vector3 pos = default, Stat rate = null,bool isMBoss = false, bool isElite = false){
        if(rate == null) rate = new Stat();
        if(monsterPoolDic.ContainsKey(name)){
            Transform pool = monsterPoolDic[name];
            // Vector3 pos = new Vector3(0, Random.Range(8f, 12f), 0f);
            // pos = Quaternion.Euler(0f, 0f, Random.Range(0f,360f)) * pos;
            // pos += player.transform.position;
            
            Effect.Play("Summon", pos);
            if(pool.childCount>1){
                Monster monster = pool.GetChild(0).GetComponent<Monster>();
                monster.transform.SetParent(null);
                
                monster.transform.position = pos;
                monsters.Add(monster);
                //monster.gameObject.SetActive(true);
                monster.level = level;
                monster.rate = rate;
                monster.middleBoss = isMBoss;
                monster.elite = isElite;
                monster.Init();
                monsterSpawnTime = Time.time;

                StartCoroutine(SetMonsterActive(monster));
                monsters.Add(monster);
                return monster;
            }
            else{
                GameObject mon = Instantiate(monsterDic[name].gameObject, pool);
                mon.SetActive(false);
                Monster monster = mon.GetComponent<Monster>();
                monster.transform.SetParent(null);
                monster.transform.position = pos;
                monsters.Add(monster);
                //monster.gameObject.SetActive(true);
                monster.gameObject.name = name;
                monster.level = level;
                monster.rate = rate;
                monster.middleBoss = isMBoss;
                monster.elite = isElite;
                monster.Init();
                monsterSpawnTime = Time.time;
                
                StartCoroutine(SetMonsterActive(monster));
                monsters.Add(monster);
                return monster;
            }

         
        }
        return null;
    }

    IEnumerator SetMonsterActive(Monster monster){
        yield return new WaitForSeconds(0.6f);
        monster.gameObject.SetActive(true);
    }
    
    public Actor GetNearMonster(Vector3 pos){
        float minDist = 100f;
        Actor target = null;
        for(int i = 0; i < monsters.Count; i++){
            if(monsters[i].hp > 0){
                float dist = Vector3.Distance(pos, monsters[i].transform.position);
                if(dist < minDist){
                    minDist = dist;
                    target = monsters[i];
                }
            }
            else{
                monsters.RemoveAt(i);
                i--;
            }
        }
        return target;
    }


    public void ReturnMonster(Monster monster){
        // if(monster.SubType == Monster.Type.Barrel){
        //     barrelCount--;
        // }
        monster.transform.SetParent(monsterPoolDic[monster.name]);
        monster.gameObject.SetActive(false);
        monsters.Remove(monster);
    }

    
}
