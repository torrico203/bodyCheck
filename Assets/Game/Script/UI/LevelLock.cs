using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class LevelLock : MonoBehaviour
{

    [SerializeField]
    private int level = 0;
    private int _level = 0;

    [SerializeField]
    private GameObject[] actives, deactives;

    [SerializeField]
    private Button[] buttons;

    [SerializeField]
    private TextMeshProUGUI levelText;

    void Awake(){
        levelText.text = "Lv."+level.ToString();
    }
    
    void Update(){

        if(Data.Player.level != _level){
            _level = Data.Player.level;

            if(_level >= level){
                foreach(GameObject obj in actives){
                    obj.SetActive(true);
                }
                foreach(GameObject obj in deactives){
                    obj.SetActive(false);
                }
                foreach(Button button in buttons){
                    button.Disable = false;
                }
            }
            else{
                foreach(GameObject obj in actives){
                    obj.SetActive(false);
                }
                foreach(GameObject obj in deactives){
                    obj.SetActive(true);
                }
                foreach(Button button in buttons){
                    button.Disable = true;
                }
            }
            
        }
    }

    public void Alert(){
        UI.SmallNotice("ne_level",
            new[]{new{
                level = level
            }});
    }

    

}