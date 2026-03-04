using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUI : MonoBehaviour
{
    [SerializeField]
    private MainEquipUI equip;
    public MainEquipUI Equip { get => equip; }



    void Awake(){
        
    }

    void Update(){

    }

    void OnEnable(){
        MainCamera.CurtainOut(0f);
        Time.timeScale = 1f;

        string sceneName = SceneManager.GetActiveScene().name;
        if(sceneName == "Main") Sound.PlayBGM("Main");
    }

    public void StageStart(){
        Loader.LoadScene("Game");
        //Loader.LoadScene("Dungeon");
    }

    public void DungeonStart(){
        if(Data.UseWealth(Wealth.Type.meat,1)){
            Loader.LoadScene("Dungeon");
        }
    }

    public void Option(){
        
        UI.OpenPopupSimple("Option");
        
    }
}