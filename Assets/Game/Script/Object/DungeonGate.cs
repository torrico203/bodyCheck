using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class DungeonGate : MonoBehaviour
{
    
    private DungeonRoom.Type type = DungeonRoom.Type.None;
    public DungeonRoom.Type Type{
        get => type;
        set {
            type = value;
            icon.sprite = icons[(int)type];
        }
    }

    [SerializeField]
    private Sprite[] icons;

    [SerializeField]
    private SpriteRenderer icon;

    void Awake()
    {
        
    }

    void Start()
    {
        
    }


    void Update()
    {
    }

    void OnEnable()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one,0.6f);

    }
    

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject == UI.Game.Manager.Player.gameObject)
        {
            //Debug.Log("Player In : "+type.ToString());
            //UI.Game.Player.ClearProjectile();
            UI.Game.Manager.GateIn(type);
        }
    }
    
}
