using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // 所有UI面板
    public GameObject startUI;
    public GameObject loginUI;
    public GameObject gameUI;
    public GameObject buttonsUI;// 按钮UI

    // 登录UI元素
    public TMP_InputField loginRoomIdInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public Button registerButton;

    // 开始UI按钮
    public Button startGameButton;

    // 引用抖音互动工具
    public DouyinInteractiveTool douyinTool;

    // 单例实例
    public static UIManager Instance;

    private void Awake()
    {
        // 确保只有一个UIManager实例
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 初始显示Start UI，隐藏其他UI
        ShowStartUI();

        // 注册按钮事件
        startGameButton.onClick.AddListener(OnStartGameClicked);
        loginButton.onClick.AddListener(OnLoginClicked);
        registerButton.onClick.AddListener(OnRegisterClicked);

        // 默认隐藏按钮UI
        buttonsUI.SetActive(false);
    }

    private void Update()
    {
        // 在游戏UI界面时，按空格键切换按钮UI的显示状态
        if (gameUI.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            ToggleButtonsUI();
        }
    }

    // 显示开始界面
    public void ShowStartUI()
    {
        startUI.SetActive(true);
        loginUI.SetActive(false);
        gameUI.SetActive(false);
        buttonsUI.SetActive(false);
    }

    // 显示登录界面
    public void ShowLoginUI()
    {
        startUI.SetActive(false);
        loginUI.SetActive(true);
        gameUI.SetActive(false);
        buttonsUI.SetActive(false);
    }

    // 显示游戏界面
    public void ShowGameUI()
    {
        startUI.SetActive(false);
        loginUI.SetActive(false);
        gameUI.SetActive(true);
        // 游戏界面打开时默认隐藏按钮UI
        buttonsUI.SetActive(false);
    }

    // 切换按钮UI的显示状态
    public void ToggleButtonsUI()
    {
        buttonsUI.SetActive(!buttonsUI.activeSelf);
    }

    // 开始游戏按钮点击事件
    public void OnStartGameClicked()
    {
        ShowLoginUI();
    }

    // 登录按钮点击事件
    public void OnLoginClicked()
    {
        // 简单验证输入
        if (string.IsNullOrEmpty(loginRoomIdInput.text))
        {
            Debug.LogError("请输入房间号");
            return;
        }

        if (string.IsNullOrEmpty(passwordInput.text))
        {
            Debug.LogError("请输入密码");
            return;
        }

        // 将登录的房间号同步到互动工具
        if (douyinTool != null && douyinTool.roomIdInput != null)
        {
            douyinTool.roomIdInput.text = loginRoomIdInput.text;
        }

        // 进入游戏界面
        ShowGameUI();
    }

    // 注册按钮点击事件
    public void OnRegisterClicked()
    {
        // 这里可以添加注册逻辑，当前只是简单显示登录界面
        Debug.Log("跳转到注册流程...");
        // 实际项目中应该显示注册表单
        ShowLoginUI();
    }
}

