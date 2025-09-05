using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel; // ��Ϸ�������
    [SerializeField] private TMP_Text resultText;     // �����ʾ�ı�
    [SerializeField] private Button restartButton;     // ���¿�ʼ��ť
    [SerializeField] private Button quitButton;        // �˳���ť

    private void Awake()
    {
        // ȷ����ʼ״̬����
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // �󶨰�ť�¼�
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
    }

    // ��ʾ��Ϸ��������
    public void ShowGameOver(string resultMessage)
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (resultText != null)
            resultText.text = resultMessage;

        // ��ͣ��Ϸ
        Time.timeScale = 0;
    }

    // ���¿�ʼ��ť���
    private void OnRestartClicked()
    {
        // �ָ�ʱ������
        Time.timeScale = 1;
        // ���¼��ص�ǰ��������Ҫ��Build Settings����ӳ�����
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    // �˳���ť���
    private void OnQuitClicked()
    {
        // �˳���Ϸ
        Application.Quit();
#if UNITY_EDITOR
        // �ڱ༭����ֹͣ����
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

