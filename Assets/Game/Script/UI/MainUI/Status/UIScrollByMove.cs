
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIScrollByMove : MonoBehaviour
{
    public float scrollSpeed = 30f;
    public float repeatTerm = 341.33f; // 임의의 반복 기준

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        rectTransform.anchoredPosition += new Vector2(scrollSpeed * Time.deltaTime, scrollSpeed * Time.deltaTime);

        // Reset position if needed (looping effect)
        if (rectTransform.anchoredPosition.x >= repeatTerm) // 임의의 반복 기준
        {
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}