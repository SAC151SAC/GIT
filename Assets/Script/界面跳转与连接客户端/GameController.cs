using UnityEngine;

public class GameController : MonoBehaviour
{
    // ����UI������
    public UIManager uiManager;

    // ���ö�����������
    public DouyinInteractiveTool douyinTool;

    private void Start()
    {
        // ȷ��������ȷ
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }

        if (douyinTool == null)
        {
            douyinTool = FindObjectOfType<DouyinInteractiveTool>();
        }

        // ��ʼ����Ϸ״̬
        InitializeGame();
    }

    private void InitializeGame()
    {
        // ���������������Ϸ��ʼ���߼�
        Debug.Log("��Ϸ��ʼ�����");
    }

    // �������������Ϸ�߼�����
    public void RestartGame()
    {
        // ������Ϸ״̬�����ؿ�ʼ����
        uiManager.ShowStartUI();
    }
}

