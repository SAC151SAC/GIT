using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel; // 游戏结束面板
    [SerializeField] private TMP_Text resultText;     // 结果显示文本
    [SerializeField] private Button restartButton;     // 重新开始按钮
    [SerializeField] private Button quitButton;        // 退出按钮

    private void Awake()
    {
        // 确保初始状态隐藏
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // 绑定按钮事件
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
    }

    // 显示游戏结束界面
    public void ShowGameOver(string resultMessage)
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (resultText != null)
            resultText.text = resultMessage;

        // 暂停游戏
        Time.timeScale = 0;
    }

    // 重新开始按钮点击
    private void OnRestartClicked()
    {
        // 恢复时间流逝
        Time.timeScale = 1;
        // 重新加载当前场景（需要在Build Settings中添加场景）
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    // 退出按钮点击
    private void OnQuitClicked()
    {
        // 退出游戏
        Application.Quit();
#if UNITY_EDITOR
        // 在编辑器中停止播放
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

