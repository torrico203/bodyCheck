//BOMIN
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class InGameItem : PoolingObject
{
    [SerializeField]
    private CircleCollider2D collider;

    public Type type = Type.exp;

    public float value = 0;
    public string infoKey = "";

    [SerializeField]
    private SpriteRenderer icon,shadow;

    public bool isAbsorb = false, forcedAbsorb = false;

    [SerializeField]
    private bool forcedAbsorbable = true;

    private bool ready = false;

    private float acceleration = 10f;

    private float velocity = 0f;

    public Vector3 dest = Vector3.zero;

    private Vector3 dir{
        get{
            if(UI.Game.Manager.Player == null) return Vector3.zero;
            return (UI.Game.Manager.Player.transform.position - transform.position).normalized;
        }
    }

    private bool pickedUp = false;

    protected override void OnInit(){
        forcedAbsorb = false;
        isAbsorb = false;
        pickedUp = false;
        SetRadius();
        velocity = 0f;
        ready = false;

        if(type == Type.artifact){
            var artifacts = Info.Artifact.Values.ToArray();
            ArtifactInfo selectedArtifact = artifacts[Random.Range(0, artifacts.Length)];
            infoKey = selectedArtifact.name;
            icon.sprite = selectedArtifact.icon;
            shadow.sprite = selectedArtifact.icon;
        }


        
        float time = Random.Range(0.5f,0.8f);
        transform.DOMove(transform.position+dest,time).SetEase(Ease.OutQuart);
        icon.transform.localPosition = new Vector3(0f,0.5f,0f);
        icon.transform.DOLocalMoveY(0f,time).SetEase(Ease.OutBounce).OnComplete(()=>{
            dest = Vector3.zero;
            ready = true;
        });;
       
    }

    

    
    protected override void OnReturn(){
        
    }

    void SetRadius(){
        if(type == Type.exp) {
            if(UI.Game.Manager.Player.NStat.expRange != collider.radius)
                collider.radius = UI.Game.Manager.Player.NStat.expRange;
        }
        else collider.radius = 0.5f;
    }

    void Update(){
        if(UI.Game.GameOver) return;
        if(pickedUp) return;
        if(!ready) return;
        SetRadius();

        if(isAbsorb || forcedAbsorb){
            
            transform.position += (dir*velocity*Time.deltaTime);
            

            if(Vector2.Distance(transform.position, UI.Game.Manager.Player.transform.position) < velocity*Time.deltaTime){
                PickUp();
            }

            velocity += acceleration*Time.deltaTime;
            if(velocity > 100f) velocity = 100f;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(isAbsorb) return;
        //if(!ready) return;
        //if(!other.isTrigger) return;
        if(other.gameObject == UI.Game.Manager.Player.gameObject)
        {
            //PickUp();
            Absorb();
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if(!isAbsorb) return;
        if(ready) return;
        //if(!ready) return;
        //if(!other.isTrigger) return;
        if(other.gameObject == UI.Game.Manager.Player.gameObject)
        {
            //PickUp();
            isAbsorb = false;
            velocity = 0f;
        }
    }

    public void Absorb(bool forced = false){
        if(isAbsorb) return;
        if(forcedAbsorb) return;
        if(forced) {
            if(!forcedAbsorbable) return;
            forcedAbsorb = true;
        }
        else isAbsorb = true;
        velocity = -3f;
    }

    public void PickUp(){
        if (pickedUp) return;
        //if(!ready) return;
        pickedUp = true;
        if(UI.Game.GameOver) 
        {
            ReturnObject();
            return;
        }
        if(type == Type.exp)
        {

            int addExp = UI.Game.Manager.AddExp(Mathf.FloorToInt(value));


            DamageText dmgtxt = Pool.GetObject<DamageText>("DamageText");
            dmgtxt.transform.position = transform.position;
            dmgtxt.text = "<sprite name=InGameExp>+"+Mathf.FloorToInt(addExp);
            dmgtxt.floor = 0;
            dmgtxt.color = Color.green;
            dmgtxt.scale = 0.25f;
            dmgtxt.startY = 0f;
            dmgtxt.InitObject();

            //Sound.PlaySFX("Wealth");
            Effect.Play("ExpHit", transform.position);



            //UI.Game.Manager.Player.Exp += Mathf.FloorToInt(value);
            
        }
        else if(type == Type.hp)
        {
            int hhp = Mathf.FloorToInt(UI.Game.Manager.Player.NStat.hp*value);
            
            UI.Game.Manager.Player.Heal(hhp);
        }
        else if(type == Type.absorb)
        {

            UI.Game.Manager.Absorb();
        }
        else if(type == Type.coin){

            int addGold = UI.Game.Manager.AddGold(Mathf.FloorToInt(value));


            DamageText dmgtxt = Pool.GetObject<DamageText>("DamageText");
            dmgtxt.transform.position = transform.position;
            dmgtxt.text = "<sprite name=gold>+"+Mathf.FloorToInt(addGold);
            dmgtxt.floor = 0;
            dmgtxt.color = Color.yellow;
            dmgtxt.scale = 0.25f;
            dmgtxt.startY = 0f;
            dmgtxt.InitObject();

            //Sound.PlaySFX("Wealth");
            Effect.Play("GoldEffect", transform.position);

            //UI.Game.Manager.TotalCoin += Mathf.FloorToInt(value);
        }
        else if(type == Type.silver){

            int addSilver = UI.Game.Manager.AddSilver(Mathf.FloorToInt(value));


            DamageText dmgtxt = Pool.GetObject<DamageText>("DamageText");
            dmgtxt.transform.position = transform.position;
            dmgtxt.text = "<sprite name=silver>+"+Mathf.FloorToInt(addSilver);
            dmgtxt.floor = 0;
            dmgtxt.color = Color.white;
            dmgtxt.scale = 0.25f;
            dmgtxt.startY = 0f;
            dmgtxt.InitObject();

            //Sound.PlaySFX("Wealth");
            Effect.Play("SilverEffect", transform.position);

            //UI.Game.Manager.TotalCoin += Mathf.FloorToInt(value);
        }
        else if(type == Type.luckyBox){
            UI.OpenPopup<SlotMachinePopup>("SlotMachine");
        }
        else if(type == Type.artifact){
            UI.Game.Manager.AddArtifact(infoKey);
            DungeonPurchasePopup popup = UI.OpenPopup<DungeonPurchasePopup>("DungeonPurchase");
            popup.ArtifactInfoInfo = Info.Artifact[infoKey];
            popup.PurchaseButton.gameObject.SetActive(false);
        }
        else if(type == Type.heart){
            UI.Game.Player.HPUP(Mathf.FloorToInt(UI.Game.Player.NStat.hp * value));
            //UI.Game.Manager.Player.Heal(Mathf.FloorToInt(UI.Game.Manager.Player.NStat.hp));
        }
        else if(type == Type.skillBox){
            LevelupPopup popup = UI.OpenPopup<LevelupPopup>("InGameLevelup");
            popup.IsSkillBox = true;
        }
    

        
        UI.Game.Manager.PickUp(this);
        ReturnObject();
        
    }

    public enum Type{
        exp = 0,
        hp = 1,
        absorb = 2,
        coin,
        luckyBox,
        silver,
        artifact,
        heart,
        skillBox
    }
}
