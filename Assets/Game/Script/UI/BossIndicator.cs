using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BossIndicator : MonoBehaviour
{
    [SerializeField] private RectTransform indicatorRect; // UI 표시기
    [SerializeField] private Image indicatorImage; // UI 이미지
    [SerializeField] private float screenMargin = 80f; // 화면 테두리 여백
    [SerializeField] private float transitionDuration = 0.3f; // 전환 시간

    private Transform bossTransform;
    private Camera mainCamera;
    private RectTransform canvasRect;
    private CanvasScaler canvasScaler;
    private bool isOnScreen = false;
    private Canvas canvas;

    private void Start()
    {
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();
        canvasScaler = GetComponentInParent<CanvasScaler>();
    }

    public void SetBoss(Transform boss)
    {
        bossTransform = boss;
    }

    private void Update()
    {
        if (bossTransform == null) return;

        // 보스의 월드 좌표를 스크린 좌표로 변환
        Vector3 screenPos = mainCamera.WorldToScreenPoint(bossTransform.position);

        // 보스가 카메라 앞에 있는지 확인
        bool isInFront = screenPos.z > 0;

        // 보스가 화면 안에 있는지 확인
        bool wasOnScreen = isOnScreen;
        isOnScreen = isInFront && 
                    screenPos.x >= 0 && screenPos.x <= Screen.width &&
                    screenPos.y >= 0 && screenPos.y <= Screen.height;

        if (isInFront)
        {
            float scaleFactor = canvasScaler.scaleFactor;
            float scaledMargin = screenMargin * scaleFactor;

            if (isOnScreen)
            {
                // 화면 안에 있을 때는 스크린 좌표를 Canvas 좌표로 정확하게 변환
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect, screenPos, canvas.worldCamera, out localPoint);
                indicatorRect.anchoredPosition = localPoint;
                indicatorImage.color = new Color(1, 1, 1, 1);
            }
            else
            {
                // 화면 밖에 있을 때는 테두리에 표시
                float screenWidth = Screen.width;
                float screenHeight = Screen.height;
                
                // 화면 가장자리에 도달했을 때의 위치 계산
                float x = Mathf.Clamp(screenPos.x, scaledMargin, screenWidth - scaledMargin);
                float y = Mathf.Clamp(screenPos.y, scaledMargin, screenHeight - scaledMargin);
                
                // 화면 좌표를 Canvas 좌표로 변환
                Vector2 clampedScreenPos = new Vector2(x, y);
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect, clampedScreenPos, canvas.worldCamera, out localPoint);
                
                indicatorRect.anchoredPosition = localPoint;

                // 방향 표시를 위한 회전
                //Vector2 direction = new Vector2(screenPos.x - x, screenPos.y - y).normalized;
                //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                //indicatorRect.rotation = Quaternion.Euler(0, 0, angle);
                indicatorImage.color = new Color(1, 1, 1, 0.7f);
            }
        }
        else
        {
            // 카메라 뒤에 있을 때는 화면 중앙에 표시
            indicatorRect.anchoredPosition = Vector2.zero;
            indicatorImage.color = new Color(1, 1, 1, 0.5f);
        }

        // 화면 안/밖 전환 시 부드러운 효과
        if (wasOnScreen != isOnScreen)
        {
            indicatorRect.DOScale(isOnScreen ? 1f : 0.8f, transitionDuration);
        }
    }
} 