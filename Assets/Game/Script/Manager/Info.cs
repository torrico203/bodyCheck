using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.IO;
using System;

public class Info : MonoBehaviour
{
    private static Info i;
    public static Info I
    {
        get
        {
            if (i == null)
            {
                i = FindObjectOfType<Info>();
                if (i == null)
                {
                    GameObject singleton = new GameObject(typeof(Info).ToString());
                    i = singleton.AddComponent<Info>();
                    DontDestroyOnLoad(singleton);
                }
            }
            return i;
        }
    }

    
    private Dictionary<string,ActiveInfo> active;
    public static Dictionary<string,ActiveInfo> Active { get => I.active; }

    private Dictionary<string,PassiveInfo> passive;
    public static Dictionary<string,PassiveInfo> Passive { get => I.passive; }

    private Dictionary<string,EquipmentInfo> equipment;
    public static Dictionary<string,EquipmentInfo> Equipment { get => I.equipment; }

    private Dictionary<string,StageInfo> stage;
    public static Dictionary<string,StageInfo> Stage { get => I.stage; }

    private Dictionary<string,DungeonInfo> dungeon;
    public static Dictionary<string,DungeonInfo> Dungeon { get => I.dungeon; }

    private Dictionary<string,RewardInfo> reward;
    public static Dictionary<string,RewardInfo> Reward { get => I.reward; }

    private Dictionary<string,ArtifactInfo> artifact;
    public static Dictionary<string,ArtifactInfo> Artifact { get => I.artifact; }

    private PlayerInfo player;
    public static PlayerInfo Player { get => I.player; }

    
    private int maxStat = 5;
    public static int MaxStat { get => I.maxStat;}
    
    [SerializeField]
    private Color[] gradeColor;
    public static Color[] GradeColor { get => I.gradeColor; }


    [System.Serializable]
    public class ColorDic : CustomDictionary.SerializableDictionary<string, Color>{}
    [SerializeField]
    private ColorDic tagColor;
    public static ColorDic TagColor { get => I.tagColor; }

    [SerializeField]
    private Stat gradeStatRate;
    public static Stat GradeStatRate { get => I.gradeStatRate; }

    [SerializeField]
    private Stat intStat;
    public static Stat IntStat { get => I.intStat; }

    [SerializeField]
    private Stat floatStat;
    public static Stat FloatStat { get => I.floatStat; }

    
    private List<string> gradeName = new List<string>();
    public static List<string> GradeName { get => I.gradeName; }

    private List<string> gradeNameColor = new List<string>();
    public static List<string> GradeNameColor { get => I.gradeNameColor; }


    [SerializeField]
    private string[] defaultEquip;
    public static string[] DefaultEquip { get => I.defaultEquip; }

    private bool loaded = false;
    public static bool Loaded { get => I.loaded; }


    void Awake(){
        DontDestroyOnLoad(gameObject);
    }

    public static void Load(){


        I.StartCoroutine(I.InfoLoad());

        
    }
    IEnumerator InfoLoad(){
        I.active = new Dictionary<string,ActiveInfo>();
        AsyncOperationHandle<IList<ActiveInfo>> activeInfoHandle = Addressables.LoadAssetsAsync<ActiveInfo>("ActiveInfo", obj =>
        {
            Debug.Log("ActiveInfo : "+obj.name);
            I.active.Add(obj.name, obj);
        });
        yield return activeInfoHandle;

        I.passive = new Dictionary<string,PassiveInfo>();
        AsyncOperationHandle<IList<PassiveInfo>> passiveInfoHandle = Addressables.LoadAssetsAsync<PassiveInfo>("PassiveInfo", obj =>
        {
            Debug.Log("PassiveInfo : "+obj.name);
            I.passive.Add(obj.name, obj);
        });

        I.equipment = new Dictionary<string,EquipmentInfo>();
        AsyncOperationHandle<IList<EquipmentInfo>> equipmentInfoHandle = Addressables.LoadAssetsAsync<EquipmentInfo>("Equipment", obj =>
        {
            Debug.Log("Equipment : "+obj.name);
            I.equipment.Add(obj.name, obj);
        });
        yield return equipmentInfoHandle;

        I.stage = new Dictionary<string,StageInfo>();
        AsyncOperationHandle<IList<StageInfo>> stageInfoHandle = Addressables.LoadAssetsAsync<StageInfo>("Stage", obj =>
        {
            Debug.Log("Stage : "+obj.name);
            I.stage.Add(obj.name, obj);
        });
        yield return stageInfoHandle;

        I.dungeon = new Dictionary<string,DungeonInfo>();
        AsyncOperationHandle<IList<DungeonInfo>> dungeonInfoHandle = Addressables.LoadAssetsAsync<DungeonInfo>("Dungeon", obj =>
        {
            Debug.Log("Dungeon : "+obj.name);
            I.dungeon.Add(obj.name, obj);
        });
        yield return dungeonInfoHandle;

        I.reward = new Dictionary<string,RewardInfo>();
        AsyncOperationHandle<IList<RewardInfo>> rewardInfoHandle = Addressables.LoadAssetsAsync<RewardInfo>("RewardInfo", obj =>
        {
            Debug.Log("RewardInfo : "+obj.name);
            I.reward.Add(obj.name, obj);
        });
        yield return rewardInfoHandle;

        I.artifact = new Dictionary<string,ArtifactInfo>();
        AsyncOperationHandle<IList<ArtifactInfo>> artifactInfoHandle = Addressables.LoadAssetsAsync<ArtifactInfo>("Artifact", obj =>
        {
            Debug.Log("ArtifactInfo : "+obj.name);
            I.artifact.Add(obj.name, obj);
        });
        yield return artifactInfoHandle;

        //I.player = new PlayerInfo();
        AsyncOperationHandle<PlayerInfo> playerInfoHandle = Addressables.LoadAssetAsync<PlayerInfo>("PlayerInfo");
        yield return playerInfoHandle;
        if(playerInfoHandle.Status == AsyncOperationStatus.Succeeded){
            Debug.Log("PlayerInfo : "+playerInfoHandle.Result.name);
            I.player = playerInfoHandle.Result;
        }
        //Addressables.Release(playerInfoHandle);


        //등급관련
        for(int i=0;i<Enum.GetNames(typeof(Grade)).Length;i++){
            I.gradeName.Add(Util.LocalStr("UI",((Grade)i).ToString()));
            //Debug.Log("GradeName : "+I.gradeName[i]);
            I.gradeNameColor.Add("<color="+Util.ColorToHex(I.gradeColor[i])+">"+I.gradeName[i]+"</color>");
            //Debug.Log("GradeNameColor : "+I.gradeNameColor[i]);
        }


        // AssetBundle localAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "activeinfo"));
        // string[] names = localAssetBundle.GetAllAssetNames();
        // I.active = new ActiveInfo[names.Length];
        // foreach (string name in names)
        // {
        //     Debug.Log(name);
        //     ActiveInfo active = localAssetBundle.LoadAsset<ActiveInfo>(name);
        //     I.active[active.no] = active;
        // }

        // localAssetBundle.Unload(false);

        I.loaded = true;
        Debug.Log("Info Loaded");
    }


    public static string RandomEquipment(){
        List<string> keys = new List<string>();
        foreach(var i in I.equipment.Keys){
            if(I.equipment[i].inUse)
                keys.Add(i);
        }

        return keys[UnityEngine.Random.Range(0,keys.Count)];
    }

}
