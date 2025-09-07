using UnityEngine;
using UnityEngine.UI;

public class ButtonEventProcessor : MonoBehaviour
{
    // ���ö�����������
    public DouyinInteractiveTool douyinTool;

    // 8�����ﰴť��A����
    public Button[] partyAGiftButtons;

    // 8�����ﰴť��B����
    public Button[] partyBGiftButtons;

    private void Start()
    {
        // ���û��ָ���������ߣ����Բ���
        if (douyinTool == null)
        {
            douyinTool = FindObjectOfType<DouyinInteractiveTool>();
        }

        // Ϊ���ﰴťע���¼�
        RegisterGiftButtons();
    }

    // ע�����ﰴť�¼�
    private void RegisterGiftButtons()
    {
        // ע��A�����ﰴť
        if (partyAGiftButtons != null && partyAGiftButtons.Length > 0)
        {
            for (int i = 0; i < partyAGiftButtons.Length && i < 8; i++)
            {
                int index = i; // ����ǰ����
                partyAGiftButtons[i].onClick.AddListener(() =>
                {
                    if (douyinTool != null)
                    {
                        douyinTool.OnPartyAGiftButtonClicked(index);
                    }
                });
            }
        }

        // ע��B�����ﰴť
        if (partyBGiftButtons != null && partyBGiftButtons.Length > 0)
        {
            for (int i = 0; i < partyBGiftButtons.Length && i < 8; i++)
            {
                int index = i; // ����ǰ����
                partyBGiftButtons[i].onClick.AddListener(() =>
                {
                    if (douyinTool != null)
                    {
                        douyinTool.OnPartyBGiftButtonClicked(index);
                    }
                });
            }
        }
    }

    // A�����밴ť���
    public void OnPartyAJoinClicked()
    {
        if (douyinTool != null)
        {
            douyinTool.OnPartyAJoinButtonClicked();
        }
    }

    // A�����۰�ť���
    public void OnPartyACommentClicked()
    {
        if (douyinTool != null)
        {
            douyinTool.OnPartyACommentButtonClicked();
        }
    }

    // A�����ް�ť���
    public void OnPartyALikeClicked()
    {
        if (douyinTool != null)
        {
            douyinTool.OnPartyALikeButtonClicked();
        }
    }

    // B�����밴ť���
    public void OnPartyBJoinClicked()
    {
        if (douyinTool != null)
        {
            douyinTool.OnPartyBJoinButtonClicked();
        }
    }

    // B�����۰�ť���
    public void OnPartyBCommentClicked()
    {
        if (douyinTool != null)
        {
            douyinTool.OnPartyBCommentButtonClicked();
        }
    }

    // B�����ް�ť���
    public void OnPartyBLikeClicked()
    {
        if (douyinTool != null)
        {
            douyinTool.OnPartyBLikeButtonClicked();
        }
    }
}

