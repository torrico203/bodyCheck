using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TitleUI : MonoBehaviour
{


    void Awake(){
        
    }

    void Start(){
        
        Sound.PlayBGM("Title");
    }

    void Update(){

    }

    
    public void GameStart(){
        Data.Load(()=>{
            Loader.LoadScene("Main");
        });

        //Loader.LoadScene("Main");
    }

}