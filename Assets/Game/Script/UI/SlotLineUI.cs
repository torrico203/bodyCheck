using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SlotLineUI : MonoBehaviour
{


    [SerializeField]
    private GameObject iconPrefab;

    [SerializeField]
    private Transform body;

    [SerializeField]
    private Button stopButton;

    private List<Image> icons = new List<Image>();

    [SerializeField]
    private float term = 220f;

    public bool isRolling = false;
    
    private float y = 0f, _y = -100f, height = 0f;

    private SlotMachineInfo[] infos;

    public int idx{
        get{
            return Mathf.RoundToInt(y/term)%infos.Length;
        }
        set{
            y = value * term;
        }
    }

    public int _idx = 0;

    void CleanUp(){
        // body에 있는 모든 게임 오브젝트를 제거합니다.
        foreach (Transform child in body)
        {
            Destroy(child.gameObject);
        }
        icons.Clear();
    }
    public void Setup(SlotMachineInfo[] infos)
    {
        this.CleanUp();
        this.infos = infos;
        height = term * infos.Length * 2f;
        float y = infos.Length * term;
        for(int j=0;j<2;j++){
            for(int i = 0; i < infos.Length; i++){
                GameObject icon = Instantiate(iconPrefab, body);
                Image img = icon.GetComponent<Image>();
                icon.transform.localPosition = new Vector3(0, y, 0);
                y -= term;
                img.sprite = infos[i].icon;
                icons.Add(img);
                SetAlpha(i,y);
            }
        }
        idx = Random.Range(0,infos.Length);

        _y = -100f;
        y = 0f;
        
    }

    void SetAlpha(int idx,float pos){
        if(-term*2f < pos && pos < term*2f){
            icons[idx].color = new Color(1, 1, 1, 1f - Mathf.Abs(pos) / (term*2f));
        }else{
            icons[idx].color = new Color(1, 1, 1, 1f - Mathf.Abs(pos) / (term*2f));
        }
    }

    void Update(){
        if(_y != y){
            _y = Mathf.Lerp(_y, y, 0.5f);
            for(int i = 0; i < icons.Count; i++){
                float pos = infos.Length * term - term * i;
                pos += _y;
                pos = (pos+((infos.Length-1)*term))%height - ((infos.Length-1)*term);
                icons[i].transform.localPosition = new Vector3(0, pos, 0);
                SetAlpha(i,pos);
            }
        }

        if(isRolling){
            y += term;
            y = y%height;
        }

        //_idx = idx;
    }

    public void Stop()
    {
        isRolling = false;
        stopButton.gameObject.SetActive(false);
        //Effect.Play("SlotStop",Vector3.zero,this.transform);
        Effect.Play("SlotStop",this.transform.position,UI.I.transform);
    }
    public void Roll(){
        isRolling = true;
        stopButton.gameObject.SetActive(true);
    }



}