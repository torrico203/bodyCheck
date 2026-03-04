using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class PreProjectile : PoolingObject
{
    //프로젝타일필수
    [HideInInspector]
    public Actor owner,target;
    [HideInInspector]
    public Transform fromTransform;
    [HideInInspector]
    public Deal deal;
    [HideInInspector]
    public float delay = 0f,chainRange = 0f,angle = 0f,range = 0f,spasticity = 0f;
    [HideInInspector]
    public Vector3 dest;
    [HideInInspector]
    public bool isRide = false;
    [HideInInspector]
    public ConditionData condition = null;
    [HideInInspector]
    public int maxChain = 0, chain = 0,chainCount = 0;


    
    public float duration;
    public Type type = Type.Square;
    public string motion = "";

    private bool startFromOwner = true;
    public bool StartFromOwner{
        get => startFromOwner;
        set => startFromOwner = value;
    }

    [SerializeField]
    private string projectileKey = "";
    public string ProjectileKey
    {
        get => projectileKey;
        set => projectileKey = value;
    }

    [SerializeField]
    private Transform square, squareFill, circle, circleFill;

    [SerializeField]
    private SpriteRenderer squareRenderer, circleRenderer;

    [SerializeField]
    private Color hostileColor, friendlyColor;
    

    void Awake()
    {

    }

    void Update()
    {
        if(startFromOwner){
            if(owner != null){
                this.transform.position = owner.transform.position;
                
            }
        }
    }

    
    void End(){

        this.ReturnObject();
    }

    protected override void OnInit(){
        Projectile pInfo = Pool.GetObjectInfo<Projectile>(projectileKey);
        float radius = pInfo.GetComponent<CircleCollider2D>().radius;

        switch(pInfo.type){
            case Projectile.Type.Be:
            case Projectile.Type.Self:
            case Projectile.Type.Random:
                type = Type.Circle;
                break;
            default:
                type = Type.Square;
                break;
        }
        //startFromOwner = pInfo.StartFromOwner;
        float projSize = 1f;
        if(owner != null){
            if(owner is Player){
                squareRenderer.color = friendlyColor;
                circleRenderer.color = friendlyColor;
            }
            else{
                squareRenderer.color = hostileColor;
                circleRenderer.color = hostileColor;
            }
            projSize = 1f+owner.NStat.projSize;

        }
        else{
            squareRenderer.color = hostileColor;
            circleRenderer.color = hostileColor;    
        }

        if(type == Type.Square){
            square.gameObject.SetActive(true);
            squareFill.gameObject.SetActive(true);
            circle.gameObject.SetActive(false);
            circleFill.gameObject.SetActive(false);
            float dist = Vector3.Distance(dest,Vector3.zero);
        
            if(pInfo.type == Projectile.Type.Line)
                dist = pInfo.Speed * pInfo.Duration;
            

            Vector3 v = dest - Vector3.zero;
            square.localRotation = Quaternion.Euler(0f,0f,Vector2.SignedAngle(Vector2.up,v));
            square.localScale = new Vector3(radius, dist/2f, 1f);
            square.localScale *= projSize;
            squareFill.localScale = new Vector3(projSize, 0f, projSize);

            squareFill.DOScaleY(projSize, duration).SetEase(Ease.Linear).OnComplete(End);
        }
        else if(type == Type.Circle){
            square.gameObject.SetActive(false);
            squareFill.gameObject.SetActive(false);
            circle.gameObject.SetActive(true);
            circleFill.gameObject.SetActive(true);
            if(pInfo.type == Projectile.Type.Self){
                circle.transform.localPosition = Vector3.zero;
            }
            else{
                circle.transform.localPosition = dest;
            }
            circle.localScale = new Vector3(radius, radius, 1f);
            circle.localScale *= projSize;
            circleFill.localScale = new Vector3(0f, 0f, 1f);
            circleFill.DOScale(projSize, duration).SetEase(Ease.Linear).OnComplete(End);
        }

        
    }
    protected override void OnReturn(){
        
        if(this.motion != ""){
            if(this.owner != null){
                //Debug.Log("Play Animation : " + this.motion);
                this.owner.PlayAnimation(this.motion);
            }
        }

        Projectile p = Pool.GetObject<Projectile>(projectileKey);
        p.owner = this.owner;
        p.fromTransform = this.fromTransform;
        p.target = this.target;
        p.transform.position = this.transform.position;
        p.gameObject.tag = this.gameObject.tag;
        p.condition = this.condition;
        p.deal = this.deal;
        p.dest = this.dest;
        p.range = this.range;
        p.spasticity = this.spasticity;
        p.delay = this.delay;
        p.isRide = this.isRide;
        p.chainRange = this.chainRange;
        p.maxChain = this.maxChain;
        p.chain = this.chain;
        p.chainCount = this.chainCount;
        p.StartFromOwner = this.startFromOwner;
        p.InitObject();
        
        
    }





    public enum Type{
        Square = 0,
        Circle
    }

}
