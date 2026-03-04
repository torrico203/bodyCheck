using System.Collections;
using UnityEngine;
using DG.Tweening;

public class ColorEffect : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Color color1 = Color.white, color2 = Color.white;

    void Start(){
        Effect();
    }

    void Effect(){
        spriteRenderer.DOColor(color1, 0.5f).OnComplete(()=>{
            spriteRenderer.DOColor(color2, 0.5f).OnComplete(()=>{
                Effect();
            });
        });
    }
}
