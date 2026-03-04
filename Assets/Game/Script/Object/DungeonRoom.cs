using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public class DungeonRoom : MonoBehaviour
{
    [SerializeField]
    private DungeonGate[] gates;
    public DungeonGate[] Gates{
        get => gates;
    }

    [SerializeField]
    private Transform spawnPointParent;

    private List<Transform> spawnPoints = new List<Transform>();
    public List<Transform> SpawnPoints{
        get => spawnPoints;
    }

    [SerializeField]
    private Transform entryParent;

    private List<Transform> entries = new List<Transform>();
    public List<Transform> Entries{
        get => entries;
    }

    [SerializeField]
    private Transform rewardTransform;
    public Transform RewardTransform{
        get => rewardTransform;
    }

    [SerializeField]
    private Transform gridTransform;

    [SerializeField]
    private GameObject[] objects,tiles;
    



    public Type type = Type.None;

    [System.Serializable]
    public class typeDic : CustomDictionary.SerializableDictionary<Type, float>{}
    [SerializeField]
    private typeDic typeRate;

    public bool forUI = false;

    //public InGameItem.Type reward = ;
    

    void Awake()
    {
        foreach(var gate in gates){
            gate.gameObject.SetActive(false);
        }

        // spawnPointParent의 모든 자식들을 spawnPoints 리스트에 추가
        for (int i = 0; i < spawnPointParent.childCount; i++)
        {
            spawnPoints.Add(spawnPointParent.GetChild(i));
        }

        // entryParent의 모든 자식들을 entries 리스트에 추가
        for (int i = 0; i < entryParent.childCount; i++)
        {
            entries.Add(entryParent.GetChild(i));
        }

        Instantiate(tiles[UnityEngine.Random.Range(0,tiles.Length)],gridTransform);
        Instantiate(objects[UnityEngine.Random.Range(0,objects.Length)],gridTransform);
        
    }

    void Start(){
        if(forUI){
            SetLayerRecursively(gameObject, LayerMask.NameToLayer("UI"));
        }
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (child == null) continue;
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public void GateOpen(bool isBoss = false){
        float total = 0f;
        foreach(var row in typeRate){
            total += row.Value;
        }


        foreach(var gate in gates){
            gate.gameObject.SetActive(true);

            if(isBoss){
                gate.Type = Type.Boss;
            }
            else{

                float r = UnityEngine.Random.Range(0,total);
                        
                foreach(var row in typeRate){
                    if(r<=row.Value){
                        gate.Type = row.Key;
                        break;
                    }
                    r -= row.Value;
                }




            }
        }
    }



    void Update()
    {
    }

    public enum Type{
        None = 0,
        Monster,
        LuckyBox,
        MiddleBoss,
        Boss,
        Shop,
        Elite,
        Silver,
        Heart,
        SkillBox
    }
}
