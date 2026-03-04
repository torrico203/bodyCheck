using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIImageAnimator : MonoBehaviour
{
    public Image targetImage; // 애니메이션을 적용할 UI Image
    public Sprite[] sprites; // 잘려진 스프라이트들
    public float frameRate = 10f; // 초당 프레임

    private int currentFrame;
    private float timer;

    void Update()
    {
        if (sprites.Length == 0 || targetImage == null) return;

        timer += Time.deltaTime;

        if (timer >= 1f / frameRate)
        {
            currentFrame = (currentFrame + 1) % sprites.Length;
            targetImage.sprite = sprites[currentFrame];
            timer = 0f;
        }
    }
}