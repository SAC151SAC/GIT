using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // ����UI���
    public GameObject startUI;
    public GameObject loginUI;
    public GameObject gameUI;
    public GameObject buttonsUI;// ��ťUI

    // ��¼UIԪ��
    public TMP_InputField loginRoomIdInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public Button registerButton;

    // ��ʼUI��ť
    public Button startGameButton;

    // ���ö�����������
    public DouyinInteractiveTool douyinTool;

    // ����ʵ��
    public static UIManager Instance;

    private void Awake()
    {
        // ȷ��ֻ��һ��UIManagerʵ��
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
        // ��ʼ��ʾStart UI����������UI
        ShowStartUI();

        // ע�ᰴť�¼�
        startGameButton.onClick.AddListener(OnStartGameClicked);
        loginButton.onClick.AddListener(OnLoginClicked);
        registerButton.onClick.AddListener(OnRegisterClicked);

        // Ĭ�����ذ�ťUI
        buttonsUI.SetActive(false);
    }

    private void Update()
    {
        // ����ϷUI����ʱ�����ո���л���ťUI����ʾ״̬
        if (gameUI.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            ToggleButtonsUI();
        }
    }

    // ��ʾ��ʼ����
    public void ShowStartUI()
    {
        startUI.SetActive(true);
        loginUI.SetActive(false);
        gameUI.SetActive(false);
        buttonsUI.SetActive(false);
    }

    // ��ʾ��¼����
    public void ShowLoginUI()
    {
        startUI.SetActive(false);
        loginUI.SetActive(true);
        gameUI.SetActive(false);
        buttonsUI.SetActive(false);
    }

    // ��ʾ��Ϸ����
    public void ShowGameUI()
    {
        startUI.SetActive(false);
        loginUI.SetActive(false);
        gameUI.SetActive(true);
        // ��Ϸ�����ʱĬ�����ذ�ťUI
        buttonsUI.SetActive(false);
    }

    // �л���ťUI����ʾ״̬
    public void ToggleButtonsUI()
    {
        buttonsUI.SetActive(!buttonsUI.activeSelf);
    }

    // ��ʼ��Ϸ��ť����¼�
    public void OnStartGameClicked()
    {
        ShowLoginUI();
    }

    // ��¼��ť����¼�
    public void OnLoginClicked()
    {
        // ����֤����
        if (string.IsNullOrEmpty(loginRoomIdInput.text))
        {
            Debug.LogError("�����뷿���");
            return;
        }

        if (string.IsNullOrEmpty(passwordInput.text))
        {
            Debug.LogError("����������");
            return;
        }

        // ����¼�ķ����ͬ������������
        if (douyinTool != null && douyinTool.roomIdInput != null)
        {
            douyinTool.roomIdInput.text = loginRoomIdInput.text;
        }

        // ������Ϸ����
        ShowGameUI();
    }

    // ע�ᰴť����¼�
    public void OnRegisterClicked()
    {
        // ����������ע���߼�����ǰֻ�Ǽ���ʾ��¼����
        Debug.Log("��ת��ע������...");
        // ʵ����Ŀ��Ӧ����ʾע���
        ShowLoginUI();
    }
}

