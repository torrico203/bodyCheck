using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ImageSizeFitter : MonoBehaviour
{

    private Image image;
    
    private Vector2 _screen;

    private Vector2 originalSize;
    private RectTransform screen;

    [SerializeField]
    private Type type = Type.width;

    [SerializeField]
    private float ratio = 1f;

    void Start(){
        image = GetComponent<Image>();
        screen = UI.I.GetComponent<RectTransform>();
        
        originalSize = image.rectTransform.sizeDelta;
        Set();
    }
    
    void Update()
    {
        if(screen.sizeDelta.x != _screen.x || screen.sizeDelta.y != _screen.y){
            Set();
        }
    }

    void Set(){
        _screen = screen.sizeDelta;
        float _ratio = 1f;
     
        switch(type){
            case Type.width:
                _ratio = (_screen.x / originalSize.x);
                image.rectTransform.sizeDelta = new Vector2(_screen.x * ratio, originalSize.y * _ratio * ratio);
                break;
            case Type.height:
                _ratio = (_screen.y / originalSize.y);
                image.rectTransform.sizeDelta = new Vector2(originalSize.x * _ratio * ratio, _screen.y * ratio);
                break;
            case Type.mixed:
                if((_screen.x / originalSize.x) > (_screen.y / originalSize.y))
                {
                    _ratio = (_screen.x / originalSize.x);
                    image.rectTransform.sizeDelta = new Vector2(_screen.x * ratio, originalSize.y * _ratio * ratio);
                }
                else
                {
                    _ratio = (_screen.y / originalSize.y);
                    image.rectTransform.sizeDelta = new Vector2(originalSize.x * _ratio * ratio, _screen.y * ratio);
                }
                break;
        }
        
        
    }
    
    public enum Type{
        width = 0,
        height,
        mixed
    }

}