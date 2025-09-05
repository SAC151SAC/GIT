using UnityEngine;

public class GameController : MonoBehaviour
{
    // 引用UI管理器
    public UIManager uiManager;

    // 引用抖音互动工具
    public DouyinInteractiveTool douyinTool;

    private void Start()
    {
        // 确保引用正确
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }

        if (douyinTool == null)
        {
            douyinTool = FindObjectOfType<DouyinInteractiveTool>();
        }

        // 初始化游戏状态
        InitializeGame();
    }

    private void InitializeGame()
    {
        // 可以在这里添加游戏初始化逻辑
        Debug.Log("游戏初始化完成");
    }

    // 可以添加其他游戏逻辑方法
    public void RestartGame()
    {
        // 重置游戏状态并返回开始界面
        uiManager.ShowStartUI();
    }
}

