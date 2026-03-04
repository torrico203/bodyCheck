using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StarLevel : MonoBehaviour
{
    [SerializeField]
    private GameObject starPrefab;


    private int maxLevel = 0;
    public int MaxLevel{
        get{return maxLevel;}
        set{
            maxLevel = value;
            if(maxLevel < 1) maxLevel = 1;
        }
    }
    private int level = 0;
    public int Level{
        get{return level;}
        set{
            level = value;
            if(level > maxLevel) level = maxLevel;
            if(level < 0) level = 0;
            SetUp();
        }
    }
    

    void SetUp(){
        foreach(Transform child in transform){
            Destroy(child.gameObject);
        }

        for(int i = 0; i < maxLevel; i++){
            GameObject star = Instantiate(starPrefab,transform);
            Color color = Color.white;
            if(i < level){
                
            }
            else{
                color = new Color(0,0,0,0.6f);
            }
            star.GetComponent<Image>().color = color;
            star.transform.SetAsLastSibling();
        }
    }


}
