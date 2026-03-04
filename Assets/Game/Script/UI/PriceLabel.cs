using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PriceLabel : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI priceText, wealthIcon;

    [SerializeField]
    private Wealth.Type type;
    public Wealth.Type Type
    {
        get { return type; }
        set
        {
            type = value;
            Set();
        }
    }
    [SerializeField]
    private int price;
    public int Price
    {
        get { return price; }
        set
        {
            price = value;
            Set();
        }
    }

    [SerializeField]
    private bool currency = false;
    
    void Awake()
    {
        Set();
    }

    void Set()
    {
        if(currency) wealthIcon.text = "₩";
        else wealthIcon.text = "<sprite name="+type.ToString()+">";
        if (price > 0)
        {
            priceText.text = price.ToString("N0");
        }
        else
        {
            priceText.text = "0";
        }
    }
    

}