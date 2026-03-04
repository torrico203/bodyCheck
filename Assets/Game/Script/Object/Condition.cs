using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Condition : PoolingObject
{
    [SerializeField]
    private Stat stat = new Stat();
    [SerializeField]
    private Stat lStat = new Stat();

    [SerializeField]
    private Deal deal = new Deal();
    public Deal Deal{
        get => deal;
        set => deal=value;
    }

    [SerializeField]
    private Deal lDeal = new Deal();

    private Deal _deal = new Deal();

    [SerializeField]
    private float duration = 0f;
    public float Duration { get => duration; set => duration = value; }

    [SerializeField]
    private float term = 0f;

    [SerializeField]
    private bool isStun = false,isCast = false, addition = false, heal = false;

    [SerializeField]
    private Color color = Color.white;

    [SerializeField]
    private int activeCount = 0;
    public int ActiveCount { get => activeCount; }

    private int aCount = 0;
    public int ACount { get => aCount; set => aCount = value; }

    private Dictionary<string,Color> _color = new Dictionary<string, Color>();

    private string infoKey;
    public string InfoKey { get => infoKey; set => infoKey = value; }

    public int level = 1;

    private float startTime = 0f;
    public float StartTime { get => startTime; set => startTime = value; }

    private Actor owner;
    public Actor Owner { get => owner; set => owner = value; }

    private Actor attacker;
    public Actor Attacker { get => attacker; set => attacker = value; }

    
    private Stat _stat = new Stat();

    private float termTime = 0f;


    protected override void OnInit(){
        startTime = Time.time;
        if(owner != null){

            if(isStun) owner.Stun = true;
            if(isCast) owner.InCast = true;
            
            _stat = this.stat + this.lStat * (this.level - 1);
            if(!addition) _stat = owner.MStat * _stat;
            owner.CStat += _stat;
            Debug.Log("컨디션 붙임 : "+_stat.ToString());

            _deal = deal + lDeal * (level - 1);

            if(color != Color.white){
                //_color = owner.ActorRoot.GetColor();
                Dictionary<string,Color> c = owner.ActorRoot.GetColor();
                _color.Clear();
                foreach (var renderer in c){
                    if(_color.ContainsKey(renderer.Key)){
                        _color[renderer.Key] = renderer.Value;
                    }
                    else{
                        _color.Add(renderer.Key,renderer.Value);
                    }
                }
                owner.ActorRoot.SetColor(color);
            }
            
            aCount = 0;
        }
    }

    void Update(){
        float now = Time.time;

        if(owner != null){
            if(now - startTime > duration){
                owner.DelCondition(infoKey);  
            }

            if(this.owner.hp <= 0f){
                owner.DelCondition(infoKey);
            }

            // term 값마다 실행하는 조건 처리
            if(term > 0f)
            {
                if(now - termTime > term)
                {
                    if(heal){
                        this.owner.Heal(_deal);
                    }
                    else{
                        this.owner.Hit(_deal,attacker);
                    }

                    termTime = now;
                }
            }
        }
        else{
            ReturnObject();
        }
        
    }

    protected override void OnReturn(){

        if(isStun) owner.Stun = false;
        if(isCast) owner.InCast = false;

        if(owner != null){
            owner.CStat -= _stat;
            Debug.Log("컨디션 뗌 : "+_stat.ToString());
        }

        if(color != Color.white){
            foreach (var renderer in _color){
                owner.ActorRoot.SetColor(renderer.Key,renderer.Value);
            }
        }
    }


}
