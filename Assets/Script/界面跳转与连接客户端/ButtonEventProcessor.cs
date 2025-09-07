using UnityEngine;
using UnityEngine.UI;

public class ButtonEventProcessor : MonoBehaviour
{
    // 引用抖音互动工具
    public DouyinInteractiveTool douyinTool;

    // 8个礼物按钮（A方）
    public Button[] partyAGiftButtons;

    // 8个礼物按钮（B方）
    public Button[] partyBGiftButtons;

    private void Start()
    {
        // 如果没有指定抖音工具，尝试查找
        if (douyinTool == null)
        {
            douyinTool = FindObjectOfType<DouyinInteractiveTool>();
        }

        // 为礼物按钮注册事件
        RegisterGiftButtons();
    }

    // 注册礼物按钮事件
    private void RegisterGiftButtons()
    {
        // 注册A方礼物按钮
        if (partyAGiftButtons != null && partyAGiftButtons.Length > 0)
        {
            for (int i = 0; i < partyAGiftButtons.Length && i < 8; i++)
            {
                int index = i; // 捕获当前索引
                partyAGiftButtons[i].onClick.AddListener(() =>
                {
                    if (douyinTool != null)
                    {
                        douyinTool.OnPartyAGiftButtonClicked(index);
                    }
                });
            }
        }

        // 注册B方礼物按钮
        if (partyBGiftButtons != null && partyBGiftButtons.Length > 0)
        {
            for (int i = 0; i < partyBGiftButtons.Length && i < 8; i++)
            {
                int index = i; // 捕获当前索引
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

    // A方加入按钮点击
    public void OnPartyAJoinClicked()
    {
        if (douyinTool != null)
        {
            douyinTool.OnPartyAJoinButtonClicked();
        }
    }

    // A方评论按钮点击
    public void OnPartyACommentClicked()
    {
        if (douyinTool != null)
        {
            douyinTool.OnPartyACommentButtonClicked();
        }
    }

    // A方点赞按钮点击
    public void OnPartyALikeClicked()
    {
        if (douyinTool != null)
        {
            douyinTool.OnPartyALikeButtonClicked();
        }
    }

    // B方加入按钮点击
    public void OnPartyBJoinClicked()
    {
        if (douyinTool != null)
        {
            douyinTool.OnPartyBJoinButtonClicked();
        }
    }

    // B方评论按钮点击
    public void OnPartyBCommentClicked()
    {
        if (douyinTool != null)
        {
            douyinTool.OnPartyBCommentButtonClicked();
        }
    }

    // B方点赞按钮点击
    public void OnPartyBLikeClicked()
    {
        if (douyinTool != null)
        {
            douyinTool.OnPartyBLikeButtonClicked();
        }
    }
}

