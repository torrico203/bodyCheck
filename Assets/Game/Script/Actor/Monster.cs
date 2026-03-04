using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class Monster : Actor
{

    // private Player player;
    // public Player Player { get => player; set => player = value; }


    public Stat rate = new Stat();
    public Stat lStat = new Stat();

    [SerializeField]
    private Type subType = Type.Monster;
    public Type SubType { get => subType; }

    public bool middleBoss = false;
    public bool elite = false;
    [SerializeField]
    private GameObject eliteMark = null;

    [SerializeField]
    private string deathEffect = "";


    [SerializeField]
    private int initCount = 5;
    public int InitCount{
        get => initCount;
        set => initCount = value;
    }

    
    public void Init(){
        this.inDeath = false;
        this.actorRoot.SetAlpha(1f);
        this.vStat = this.lStat * (this.level-1);
        this.vStat += this.MStat * rate;
        hp = Mathf.FloorToInt(NStat.hp);
        this.stun = false;
        this.inCast = false;
        if(elite) {
            eliteMark.SetActive(true);
            this.vStat.reduceSpasticity = 1f;
        }
        else eliteMark.SetActive(false);
        NStat = this.MStat;
        life = 0;

        //스킬 사용시간 초기화
        for(int i=0;i<this.active.Count;i++){
            float coolTime = 1f/NStat.attackSpeed;
            if(Info.Active[active[i].infoKey].type != ActiveType.DefaultAttack)
                coolTime = Info.Active[active[i].infoKey].coolTime * (1f - NStat.coolRdx);
            this.active[i].useTime = Time.time - coolTime;
            if(this.active[i].delay == 0f)
                this.active[i].delay = Random.Range(1f,coolTime+1f);
            //if(now - this.active[i].useTime - this.active[i].delay > coolTime){
        }
         
    }


    
    private void Awake() {
        base.Awake();
    }

    void Update()
    {
        if(UI.Game.GameOver) return;

        base.Update();
        

        if(this.hp>0&&!this.inDeath&&!this.stun&&!this.inCast&&!this.isFixed&&this.spasticity<=0f){
            if(this.target != null){
                if(this.target.InDeath || this.target.hp<1){
                    this.target = null;
                }
            }
            else{
                if(!UI.Game.Player.InDeath && UI.Game.Player.hp>0){
                    this.target = UI.Game.Player;
                }
                // if(!player.InDeath)
                //     this.target = player;
            }

            if(this.target != null){
                bool aConfirm = false;
                for(int i = 0; i < this.active.Count; i++){
                    ActiveErr err = this.UseActive(i);
                    if(err == ActiveErr.Range) aConfirm = true;
                    if(err == ActiveErr.None) break;
                }
                if(aConfirm){
                    Vector2 dir = this.target.transform.position - this.transform.position;
                    this.movedir = dir.normalized;
                }
                else{
                    this.movedir = Vector2.zero;
                }             
            }
            else{
                this.movedir = Vector2.zero;
            }

            float diffx = transform.position.x - UI.Game.Player.transform.position.x;
            float diffy = transform.position.y - UI.Game.Player.transform.position.y;
            if(diffx > 25f){
                transform.position = new Vector3(transform.position.x-50f,transform.position.y,transform.position.z);
            }
            else if(diffx < -25f){
                transform.position = new Vector3(transform.position.x+50f,transform.position.y,transform.position.z);
            }
            if(diffy > 12.5f){
                transform.position = new Vector3(transform.position.x,transform.position.y-25f,transform.position.z);
            }
            else if(diffy < -12.5f){
                transform.position = new Vector3(transform.position.x,transform.position.y+25f,transform.position.z);
            }
        }
    }

    protected override void OnDeath(){
        
        if(deathEffect != ""){
            Effect.Play(deathEffect, center.transform.position);
            UI.Game.Manager.ReturnMonster(this);
        }
        else{
            this.actorRoot.SetAlpha(0f,2f,()=>{
                UI.Game.Manager.ReturnMonster(this);
            },Ease.InExpo);
        }
        UI.Game.Manager.MonsterDrop(this.transform.position, this.subType == Type.Barrel,middleBoss,elite);

        
    }

    public enum Type{
        Monster = 0,
        Barrel
    }

}