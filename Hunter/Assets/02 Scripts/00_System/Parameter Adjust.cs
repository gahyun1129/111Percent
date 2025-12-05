using TMPro;
using UnityEngine;
using DG.Tweening;

public class ParameterAdjust : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI player_scale;
    [SerializeField] private TextMeshProUGUI enemy_scale;
    [SerializeField] private TextMeshProUGUI camera_view;
    [SerializeField] private TextMeshProUGUI player_pos;
    [SerializeField] private TextMeshProUGUI enemy_pos;
    [SerializeField] private TextMeshProUGUI player_pos_y;
    [SerializeField] private TextMeshProUGUI enemy_pos_y;
    [SerializeField] private TextMeshProUGUI player_speed;

    [Header("Follow Camera UI")]
    [SerializeField] private TextMeshProUGUI viewport_threshold;
    [SerializeField] private TextMeshProUGUI follow_speed;
    [SerializeField] private TextMeshProUGUI smooth_stop_duration;
    [SerializeField] private TextMeshProUGUI bounce_distance;
    [SerializeField] private TextMeshProUGUI bounce_duration;
    [SerializeField] private TextMeshProUGUI bounce_ease_text;

    [Header("Enemy UI")]
    [SerializeField] private TextMeshProUGUI enemy_speed;
    [SerializeField] private TextMeshProUGUI enemy_stop_distance;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject enemy;
    [SerializeField] private Camera cam;
    [SerializeField] private FollowCamera followCamera;

    private GameData gameData;

    private void Start()
    {
        // GameData 가져오기
        if (gameData == null)
        {
            gameData = GameDataManager.Instance.GAMEDATA;
        }

        // 적 초기 위치: 뷰포트 (0.8, 0.3)
        Vector3 enemyViewportPos = new Vector3(0.77f, 0.15f, 0);
        Vector3 enemyWorldPos = cam.ViewportToWorldPoint(enemyViewportPos);
        enemyWorldPos.z = enemy.transform.position.z; // z값 유지
        enemy.transform.position = enemyWorldPos;

        // 초기 텍스트 업데이트
        player_scale.text = $"{player.transform.localScale.x:F2}";
        enemy_scale.text = $"{enemy.transform.localScale.x:F2}";
        camera_view.text = $"{cam.orthographicSize:F2}";
        player_pos.text = $"{gameData.playerViewportX:F2}";
        enemy_pos.text = $"{gameData.enemyViewportX:F2}";
        player_pos_y.text = $"{gameData.playerViewportY:F2}";
        enemy_pos_y.text = $"{gameData.enemyViewportY:F2}";
        player_speed.text = $"{gameData.playerSpeed:F2}";

        // Follow Camera 초기 텍스트 업데이트
        if (followCamera != null)
        {
            if (viewport_threshold != null)
                viewport_threshold.text = $"{gameData.viewportThresholdX:F2}";

            if (follow_speed != null)
                follow_speed.text = $"{gameData.followSpeed:F2}";

            if (smooth_stop_duration != null)
                smooth_stop_duration.text = $"{gameData.smoothStopDuration:F2}";

            if (bounce_distance != null)
                bounce_distance.text = $"{gameData.bounceDistance:F2}";

            if (bounce_duration != null)
                bounce_duration.text = $"{gameData.bounceDuration:F2}";

            if (bounce_ease_text != null)
                bounce_ease_text.text = gameData.bounceEase.ToString();
        }

        if (enemy_stop_distance != null)
            enemy_stop_distance.text = $"{gameData.stopDistance:F2}";

        // Enemy Speed 초기 텍스트 업데이트
        if (gameData != null && enemy_speed != null)
        {
            enemy_speed.text = $"{gameData.enemySpeed:F2}";
        }
    }

    public void PlayerScale(float degree)
    {
        // 현재 스케일에 degree 더하기
        Vector3 currentScale = player.transform.localScale;
        currentScale += Vector3.one * degree;
        player.transform.localScale = currentScale;

        // 텍스트 업데이트
        player_scale.text = $"{currentScale.x:F2}";
    }

    public void EnemyScale(float degree)
    {
        // 현재 스케일에 degree 더하기
        Vector3 currentScale = enemy.transform.localScale;
        currentScale += Vector3.one * degree;
        enemy.transform.localScale = currentScale;

        // 텍스트 업데이트
        enemy_scale.text = $"{currentScale.x:F2}";
    }



    public void PlayerPos(float degree)
    {
        // 뷰포트 좌표로 현재 위치 가져오기
        Vector3 viewportPos = cam.WorldToViewportPoint(player.transform.position);

        // x 값만 degree만큼 변경 (0~1 범위)
        viewportPos.x += degree;
        viewportPos.x = Mathf.Clamp01(viewportPos.x); // 0~1 범위로 제한


        // 다시 월드 좌표로 변환
        Vector3 worldPos = cam.ViewportToWorldPoint(viewportPos);
        worldPos.z = player.transform.position.z; // z값 유지
        player.transform.position = worldPos;

        // 텍스트 업데이트
        gameData.playerViewportX = viewportPos.x;
        player_pos.text = $"{viewportPos.x:F2}";
    }

    public void EnemyPos(float degree)
    {
        // 뷰포트 좌표로 현재 위치 가져오기
        Vector3 viewportPos = cam.WorldToViewportPoint(enemy.transform.position);

        // x 값만 degree만큼 변경 (0~1 범위)
        viewportPos.x += degree;
        viewportPos.x = Mathf.Clamp01(viewportPos.x); // 0~1 범위로 제한

        // 다시 월드 좌표로 변환
        Vector3 worldPos = cam.ViewportToWorldPoint(viewportPos);
        worldPos.z = enemy.transform.position.z; // z값 유지
        enemy.transform.position = worldPos;

        // 텍스트 업데이트
        gameData.enemyViewportX = viewportPos.x;
        enemy_pos.text = $"{viewportPos.x:F2}";
    }

    public void PlayerPosY(float degree)
    {
        // 뷰포트 좌표로 현재 위치 가져오기
        Vector3 viewportPos = cam.WorldToViewportPoint(player.transform.position);

        // y 값만 degree만큼 변경 (0~1 범위)
        viewportPos.y += degree;
        viewportPos.y = Mathf.Clamp01(viewportPos.y); // 0~1 범위로 제한

        // 다시 월드 좌표로 변환
        Vector3 worldPos = cam.ViewportToWorldPoint(viewportPos);
        worldPos.z = player.transform.position.z; // z값 유지
        player.transform.position = worldPos;

        // 텍스트 업데이트
        gameData.playerViewportY = viewportPos.y;
        player_pos_y.text = $"{viewportPos.y:F2}";
    }

    public void EnemyPosY(float degree)
    {
        // 뷰포트 좌표로 현재 위치 가져오기
        Vector3 viewportPos = cam.WorldToViewportPoint(enemy.transform.position);

        // y 값만 degree만큼 변경 (0~1 범위)
        viewportPos.y += degree;
        viewportPos.y = Mathf.Clamp01(viewportPos.y); // 0~1 범위로 제한

        // 다시 월드 좌표로 변환
        Vector3 worldPos = cam.ViewportToWorldPoint(viewportPos);
        worldPos.z = enemy.transform.position.z; // z값 유지
        enemy.transform.position = worldPos;

        // 텍스트 업데이트
        gameData.enemyViewportY = viewportPos.y;
        enemy_pos_y.text = $"{viewportPos.y:F2}";
    }

    public void PlayerSpeed(float degree)
    {
        // GameData의 playerSpeed 값 변경
        if (gameData != null)
        {
            gameData.playerSpeed += degree;

            // 최소값 제한 (음수가 되지 않도록)
            if (gameData.playerSpeed < 0f)
                gameData.playerSpeed = 0f;

            // 텍스트 업데이트
            player_speed.text = $"{gameData.playerSpeed:F2}";
        }
    }

    ////////////////////////////////////////////////////////////////////////
    /// 카메라
    //////////////////////////////////////////////////////////////////////// 

    public void CameraView(float degree)
    {
        // 카메라 orthographicSize 변경
        cam.orthographicSize += degree;

        // 최소값 제한 (너무 작아지지 않도록)
        if (cam.orthographicSize < 0.1f)
            cam.orthographicSize = 0.1f;

        // 텍스트 업데이트
        camera_view.text = $"{cam.orthographicSize:F2}";
    }

    ////////////////////////////////////////////////////////////////////////
    /// Follow Camera
    ////////////////////////////////////////////////////////////////////////

    // Viewport Threshold X - 슬라이더용 (0~1 범위)
    public void ViewportThresholdSlider(float value)
    {
        if (followCamera == null) return;

        // FollowCamera의 viewportThresholdX 값 직접 설정
        gameData.viewportThresholdX = value;

        // 텍스트 업데이트
        if (viewport_threshold != null)
            viewport_threshold.text = $"{value:F2}";
    }

    // Follow Speed - Plus/Minus 버튼용
    public void FollowSpeed(float degree)
    {
        if (followCamera == null) return;

        float currentValue = gameData.followSpeed;
        currentValue += degree;

        // 최소값 제한
        if (currentValue < 0.1f)
            currentValue = 0.1f;

        gameData.followSpeed = currentValue;

        // 텍스트 업데이트
        if (follow_speed != null)
            follow_speed.text = $"{currentValue:F2}";
    }

    // Smooth Stop Duration - Plus/Minus 버튼용
    public void SmoothStopDuration(float degree)
    {
        if (followCamera == null) return;

        float currentValue = gameData.smoothStopDuration;
        currentValue += degree;

        // 최소값 제한
        if (currentValue < 0.01f)
            currentValue = 0.01f;

        gameData.smoothStopDuration = currentValue;

        // 텍스트 업데이트
        if (smooth_stop_duration != null)
            smooth_stop_duration.text = $"{currentValue:F2}";
    }

    // Bounce Distance - Plus/Minus 버튼용
    public void BounceDistance(float degree)
    {
        if (followCamera == null) return;

        float currentValue = gameData.bounceDistance;
        currentValue += degree;

        // 최소값 제한
        if (currentValue < 0f)
            currentValue = 0f;

        gameData.bounceDistance = currentValue;

        // 텍스트 업데이트
        if (bounce_distance != null)
            bounce_distance.text = $"{currentValue:F2}";
    }

    // Bounce Duration - Plus/Minus 버튼용
    public void BounceDuration(float degree)
    {
        if (followCamera == null) return;

        float currentValue = gameData.bounceDuration;
        currentValue += degree;

        // 최소값 제한
        if (currentValue < 0.01f)
            currentValue = 0.01f;

        gameData.bounceDuration = currentValue;

        // 텍스트 업데이트
        if (bounce_duration != null)
            bounce_duration.text = $"{currentValue:F2}";
    }

    // Bounce Ease - 선택용 (다음 Ease로 전환)
    public void NextBounceEase()
    {
        if (followCamera == null) return;

         Ease currentEase = gameData.bounceEase;
        Ease nextEase = GetNextEase(currentEase);

        gameData.bounceEase = nextEase;

        // 텍스트 업데이트
        if (bounce_ease_text != null)
            bounce_ease_text.text = nextEase.ToString();
    }

    // Bounce Ease - 선택용 (이전 Ease로 전환)
    public void PreviousBounceEase()
    {
        if (followCamera == null) return;

        Ease currentEase = gameData.bounceEase;
        Ease previousEase = GetPreviousEase(currentEase);

        gameData.bounceEase = previousEase;

        // 텍스트 업데이트
        if (bounce_ease_text != null)
            bounce_ease_text.text = previousEase.ToString();
    }

    ////////////////////////////////////////////////////////////////////////
    /// Enemy
    ////////////////////////////////////////////////////////////////////////

    // Enemy Speed - Plus/Minus 버튼용
    public void EnemySpeed(float degree)
    {
        if (gameData == null) return;

        gameData.enemySpeed += degree;

        // 최소값 제한
        if (gameData.enemySpeed < 0f)
            gameData.enemySpeed = 0f;

        // 텍스트 업데이트
        if (enemy_speed != null)
            enemy_speed.text = $"{gameData.enemySpeed:F2}";
    }

    // Enemy Stop Distance - Plus/Minus 버튼용
    public void EnemyStopDistance(float degree)
    {
        float currentValue = gameData.stopDistance;
        currentValue += degree;

        // 최소값 제한
        if (currentValue < 0f)
            currentValue = 0f;

        gameData.stopDistance = currentValue;

        // 텍스트 업데이트
        if (enemy_stop_distance != null)
            enemy_stop_distance.text = $"{currentValue:F2}";
    }

    ////////////////////////////////////////////////////////////////////////
    /// Helper Methods
    ////////////////////////////////////////////////////////////////////////

    // 부드러운 Ease 타입들만 순환
    private Ease GetNextEase(Ease current)
    {
        switch (current)
        {
            case Ease.Linear: return Ease.OutQuad;
            case Ease.OutQuad: return Ease.OutCubic;
            case Ease.OutCubic: return Ease.OutQuart;
            case Ease.OutQuart: return Ease.OutQuint;
            case Ease.OutQuint: return Ease.OutSine;
            case Ease.OutSine: return Ease.OutExpo;
            case Ease.OutExpo: return Ease.OutCirc;
            case Ease.OutCirc: return Ease.OutBack;
            case Ease.OutBack: return Ease.OutElastic;
            case Ease.OutElastic: return Ease.Linear;
            default: return Ease.OutCubic;
        }
    }

    private Ease GetPreviousEase(Ease current)
    {
        switch (current)
        {
            case Ease.Linear: return Ease.OutElastic;
            case Ease.OutQuad: return Ease.Linear;
            case Ease.OutCubic: return Ease.OutQuad;
            case Ease.OutQuart: return Ease.OutCubic;
            case Ease.OutQuint: return Ease.OutQuart;
            case Ease.OutSine: return Ease.OutQuint;
            case Ease.OutExpo: return Ease.OutSine;
            case Ease.OutCirc: return Ease.OutExpo;
            case Ease.OutBack: return Ease.OutCirc;
            case Ease.OutElastic: return Ease.OutBack;
            default: return Ease.OutCubic;
        }
    }
}
