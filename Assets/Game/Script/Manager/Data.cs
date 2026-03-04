using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;
using Unity.VisualScripting;

public class Data : MonoBehaviour
{
    private static Data i;
    public static Data I
    {
        get
        {
            if (i == null)
            {
                i = FindObjectOfType<Data>();
                if (i == null)
                {
                    GameObject singleton = new GameObject(typeof(Data).ToString());
                    i = singleton.AddComponent<Data>();
                    DontDestroyOnLoad(singleton);
                }
            }
            return i;
        }
    }

    [SerializeField]
    private PlayerData player = new PlayerData();
    public static PlayerData Player { get => I.player; }

    // private string stageName = "Stage1";
    // public static string StageName { get => I.stageName; set => I.stageName = value; }

    [SerializeField]
    private string dungeonName = "Dungeon1";
    public static string DungeonName { get => I.dungeonName; set => I.dungeonName = value; }

    private List<bool> saveFlag = new List<bool>();

    private float saveTime = 0f;

    private bool loaded = false;

    void Awake(){
        DontDestroyOnLoad(gameObject);
        
        for(int i=0;i<Enum.GetNames(typeof(PlayerData.Type)).Length;i++){
            saveFlag.Add(false);
        }
    }

    void Update(){
        if(!Info.Loaded) return;
        float now = Time.time;
        if(now - saveTime > 10f){
            Save();
            saveTime = now;
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {   //이땐 강제 저장
            //SaveAll();
            Save();
        }
    }
    

    public static void Load(Action callback = null){
        // if(!PlayerPrefs.HasKey("nickname")){
        //     I.player = Util.DeepCopy<PlayerData>(Info.Player.initial);
        //     I.SaveAll();
        // }
        // else{
        //     I.player.nickname = PlayerPrefs.GetString("nickname");
        //     I.player.level = int.Parse(PlayerPrefs.GetString("level"));
        //     I.player.wealth = JsonConvert.DeserializeObject<Wealth>(PlayerPrefs.GetString("wealth"));
        //     I.player.equip = JsonConvert.DeserializeObject<Equip>(PlayerPrefs.GetString("equip"));
        //     I.player.stat = JsonConvert.DeserializeObject<Stat>(PlayerPrefs.GetString("stat"));
        //     I.player.inventory = JsonConvert.DeserializeObject<List<EquipmentData>>(PlayerPrefs.GetString("inventory"));
        //     I.player.dungeon = JsonConvert.DeserializeObject<List<StageData>>(PlayerPrefs.GetString("dungeon"));
        // }

        PlayFabManager.GetUserData((result)=>{
            if(result == null){
                I.player = Util.DeepCopy<PlayerData>(Info.Player.initial);
                I.SaveAll();
            }
            else{
                foreach (var entry in result)
                {
                    Debug.Log($"Key: {entry.Key}, Value: {entry.Value.Value}, LastUpdated: {entry.Value.LastUpdated}");
                    switch(entry.Key){
                        case "nickname":
                            I.player.nickname = entry.Value.Value;
                            break;
                        case "level":
                            I.player.level = int.Parse(entry.Value.Value);
                            break;
                        case "wealth":
                            I.player.wealth = JsonConvert.DeserializeObject<Wealth>(entry.Value.Value);
                            break;
                        case "equip":
                            I.player.equip = JsonConvert.DeserializeObject<Equip>(entry.Value.Value);
                            break;
                        case "stat":
                            I.player.stat = JsonConvert.DeserializeObject<Stat>(entry.Value.Value);
                            break;
                        case "inventory":
                            I.player.inventory = JsonConvert.DeserializeObject<List<EquipmentData>>(entry.Value.Value);
                            break;
                        case "dungeon":
                            I.player.dungeon = JsonConvert.DeserializeObject<List<StageData>>(entry.Value.Value);
                            break;
                    }
                }
            }
            
        });

        I.loaded = true;
        callback?.Invoke();
    }
    void SaveAll(){
        for(int i=0;i<saveFlag.Count;i++){
            saveFlag[i] = true;
        }
    }

    void Save(){
        if(!PlayFabManager.IsLogin) return;
        if(!loaded) return;

        Dictionary<string,string> saveDic = new Dictionary<string,string>();
        for(int i=0;i<saveFlag.Count;i++){
            if(saveFlag[i]){
                switch((PlayerData.Type)i){
                    case PlayerData.Type.nickname:
                        //PlayerPrefs.SetString("nickname", player.nickname);
                        saveDic.Add("nickname",player.nickname);
                    break;
                    case PlayerData.Type.level:
                        //PlayerPrefs.SetString("level", player.level.ToString());
                        PlayFabManager.UpdateLeaderboard("Level",player.level);
                        saveDic.Add("level",player.level.ToString());
                    break;
                    case PlayerData.Type.wealth:
                        //PlayerPrefs.SetString("wealth", JsonConvert.SerializeObject(player.wealth));
                        saveDic.Add("wealth",JsonConvert.SerializeObject(player.wealth));
                        
                        PlayFabManager.UpdateLeaderboard("Dia",player.wealth.dia);
                        PlayFabManager.UpdateLeaderboard("Exp",player.wealth.exp);
                        PlayFabManager.UpdateLeaderboard("Gold",player.wealth.gold);
                    break;
                    case PlayerData.Type.equip:
                        //PlayerPrefs.SetString("equip", JsonConvert.SerializeObject(player.equip));
                        saveDic.Add("equip",JsonConvert.SerializeObject(player.equip));
                    break;
                    case PlayerData.Type.stat:
                        //PlayerPrefs.SetString("stat", JsonConvert.SerializeObject(player.stat));
                        saveDic.Add("stat",JsonConvert.SerializeObject(player.stat));
                    break;
                    case PlayerData.Type.inventory:
                        //PlayerPrefs.SetString("inventory", JsonConvert.SerializeObject(player.inventory));
                        saveDic.Add("inventory",JsonConvert.SerializeObject(player.inventory));
                    break;
                    case PlayerData.Type.dungeon:
                        //PlayerPrefs.SetString("dungeon", JsonConvert.SerializeObject(player.dungeon));
                        saveDic.Add("dungeon",JsonConvert.SerializeObject(player.dungeon));
                    break;
                    default:
                        Debug.LogError("Data Save Error : "+((PlayerData.Type)i).ToString());
                    break;
                }
                saveFlag[i] = false;
            }
        }
        if(saveDic.Count>0) PlayFabManager.UpdateUserData(saveDic);

    }

    public static void LevelUp(){
        bool isLevelup = true;

        for(int i=0;i<3;i++){
            if((Info.Player.initial.stat[i] + Info.Player.lStat[i]*((I.player.level)*Info.MaxStat)) > I.player.stat[i]){
                isLevelup = false;
                break;
            }
        }

        if(isLevelup){
            I.player.level++;
        }

        I.saveFlag[(int)PlayerData.Type.level] = true;
    }
    
    public static void Unequip(EquipmentType type){
        I.player.equip[(int)type] = -1;
        I.saveFlag[(int)PlayerData.Type.inventory] = true;
    }
    public static void Equip(int idx){
        if(idx<0 || idx >= I.player.inventory.Count) return;
        EquipmentData data = I.player.inventory[idx];
        if(data == null) return;
        
        I.player.equip[(int)Info.Equipment[data.infoKey].type] = idx;
        I.saveFlag[(int)PlayerData.Type.equip] = true;
    }

    public static Stat EquipmentStat(EquipmentData data){
        Stat stat = new Stat();
        if(data == null) return stat;
        
        stat = Info.Equipment[data.infoKey].stat;
        stat += Info.Equipment[data.infoKey].lStat * (data.level-1);
        stat += stat * (Info.GradeStatRate*(int)data.grade);
        
        //품질 계산
        Stat qStat = stat * Mathf.Lerp(-0.2f, 0.2f, data.quality);
        for(int i=0;i<stat.Count();i++)
            if(stat[i] < 0) qStat[i] = qStat[i] * -1f;
        
        stat += qStat;
        //stat += stat * data.rate;
        
        return stat;
    }
    public static Stat EquipmentStat(int idx){
        Stat stat = new Stat();
        if(idx<0 || idx >= I.player.inventory.Count) return stat;
        EquipmentData data = I.player.inventory[idx];
        if(data == null) return stat;
        
        return EquipmentStat(data);
    }
    public static Stat ResultStat(){
        Stat stat = new Stat();
        stat += I.player.stat;
        for(int i=0;i<I.player.equip.Count();i++){
            stat += EquipmentStat(I.player.equip[i]);
        }
        return stat;
    }

    public static void AddStat(Stat.Type type, float value){
        I.player.stat[(int)type] += value;
        I.saveFlag[(int)PlayerData.Type.stat] = true;
    }

    public static void AddWealth(Wealth.Type type, int value, bool save = true){
        if(value < 0) return;
        I.player.wealth[(int)type] += value;
        if(save) I.saveFlag[(int)PlayerData.Type.wealth] = true;
    }
    public static void AddWealth(Wealth wealth){
        Debug.Log("AddWealth");
        I.player.wealth += wealth;
        I.saveFlag[(int)PlayerData.Type.wealth] = true;
    }

    public static void AddInventory(EquipmentData data){
        if(data == null) return;
        data.flag = 1;
        I.player.inventory.Add(data);
        I.saveFlag[(int)PlayerData.Type.inventory] = true;
    }
    
    public static void RemoveInventory(int idx){
        I.player.inventory.RemoveAt(idx);
        for(int i=0;i<I.player.equip.Count();i++){
            if(I.player.equip[i] == idx){
                I.player.equip[i] = -1;
            }
            else if(I.player.equip[i] > idx){
                I.player.equip[i]--;
            }
        }
        I.saveFlag[(int)PlayerData.Type.inventory] = true;
        I.saveFlag[(int)PlayerData.Type.equip] = true;
    }

    public static void AddStuffs(List<Stuff> stuff){
        for(int i=0;i<stuff.Count;i++){
            if(stuff[i].equipment != null){
                if(stuff[i].equipment.infoKey != "")
                    AddInventory(stuff[i].equipment);
            }
            
            if(stuff[i].wealth != null){
                if(!stuff[i].wealth.IsEmpty())
                    AddWealth(stuff[i].wealth);
            }
        }
        //I.saveFlag[(int)PlayerData.Type.inventory] = true;
        //I.saveFlag[(int)PlayerData.Type.wealth] = true;
    }

    public static EquipmentData DoSynthesis(int[] offerings){
        int level = 0;
        float quality = 0f;
        Grade grade = Grade.Common;
        for(int i=0;i<offerings.Length;i++){
            if(offerings[i] < 0 || offerings[i] >= I.player.inventory.Count) return null;
            
            EquipmentData data = I.player.inventory[offerings[i]];
            level += data.level;
            quality += data.quality;
            if(data.grade > grade) grade = data.grade;
        }
        grade++;
        if(grade > Grade.Legend) grade = Grade.Legend;
        level = Mathf.FloorToInt(level / offerings.Length);
        quality = Mathf.Floor(quality / offerings.Length * 100f) / 100f;
        EquipmentData result = new EquipmentData();
        result.infoKey = Info.RandomEquipment();
        result.grade = grade;
        result.level = level;
        result.quality = quality;

        for(int i=0;i<offerings.Length;i++){
            for(int j=0;j<i;j++){
                if(offerings[j] < offerings[i])
                    offerings[i]--;
            }
            RemoveInventory(offerings[i]);
        }
        
        AddInventory(result);
        I.saveFlag[(int)PlayerData.Type.inventory] = true;

        return result;        
    }
    
    public static void EquipmentLevelup(int idx){
        if(idx<0 || idx >= I.player.inventory.Count) return;
        
        I.player.inventory[idx].level++;
        I.saveFlag[(int)PlayerData.Type.inventory] = true;
    }

    public static bool EquipmentQualityUp(int idx){
        if(idx<0 || idx >= I.player.inventory.Count) return false;
        
        float quality = Util.GenerateQuality();
            
        if(quality > I.player.inventory[idx].quality){
            I.player.inventory[idx].quality = quality;
            I.saveFlag[(int)PlayerData.Type.inventory] = true;
            return true;
        }
        else{
            UI.SmallNotice("FailQualityUp");
        }
        return false;
    }

    public static bool UseWealth(Wealth.Type type, int value, bool save = true){
        if(value < 0) return false;
        if(I.player.wealth[(int)type] < value) {
            UI.SmallNotice("ne_"+type.ToString());
            return false;
        }
        I.player.wealth[(int)type] -= value;
        if(save) I.saveFlag[(int)PlayerData.Type.wealth] = true;
        return true;
    }

    public static void AddStageData(string infoKey){
        StageData dungeon = new StageData();
        dungeon.infoKey = infoKey;
        I.player.dungeon.Add(dungeon);
    }

    public static void StageClear(string infoKey){
        for(int i=0;i<I.player.dungeon.Count;i++){
            if(I.player.dungeon[i].infoKey == infoKey){
                I.player.dungeon[i].clear = 1;
                
                I.saveFlag[(int)PlayerData.Type.dungeon] = true;

                PlayFabManager.UpdateLeaderboard("Dungeon",i+1);
                break;
            }
        }
    }
}