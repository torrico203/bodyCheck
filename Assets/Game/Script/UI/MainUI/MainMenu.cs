using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private MainMenuButton[] buttons;
    [SerializeField]
    private MainMenuUI[] menus;

    [SerializeField]
    private MainMenuBoard board;
    private string menu = "Adventure";
    public string Menu { get => menu; }

    
    void Start(){
        SetMenu();
    }

    public void OnClick(string menu){
        UnSetMenu();
        this.menu = menu;
        SetMenu();
    }

    private void UnSetMenu(){
        for(int i = 0; i < buttons.Length; i++){
            if(buttons[i].Button.name == menu){
                menus[i].Unload();
            }
        }
    }

    private void SetMenu(){
        for(int i = 0; i < buttons.Length; i++){
            if(buttons[i].Button.name == menu){
                buttons[i].LayoutElement.preferredWidth = 250;
                buttons[i].Button.Disable = true;
                board.SetPosition(i);
                menus[i].Load();
            }else{
                buttons[i].LayoutElement.preferredWidth = 180;
                buttons[i].Button.Disable = false;
            }
        }
    }
}
