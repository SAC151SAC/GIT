using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleUIHandler : MonoBehaviour
{
    // 战斗系统引用
    private BattleSystem _battleSystem;

    // ==================== A方UI元素 ====================
    // 评论框
    public TMP_InputField partyACommentInput;
    // 点赞按钮
    public Button partyALikeButton;
    // 评论按钮（点击后发送评论）
    public Button partyACommentButton;
    // 礼物按钮组（8个，对应8种礼物小兵）
    public Button[] partyAGiftButtons = new Button[8];

    // ==================== B方UI元素 ====================
    // 评论框
    public TMP_InputField partyBCommentInput;
    // 点赞按钮
    public Button partyBLikeButton;
    // 评论按钮（点击后发送评论）
    public Button partyBCommentButton;
    // 礼物按钮组（8个，对应8种礼物小兵）
    public Button[] partyBGiftButtons = new Button[8];

    private void Start()
    {
        _battleSystem = BattleSystem.Instance;
        if (_battleSystem == null)
        {
            Debug.LogError("找不到BattleSystem实例!");
            return;
        }

        // 自动绑定按钮事件（可选，也可在编辑器中手动绑定）
        BindButtonEvents();
    }

    // 自动绑定按钮点击事件（避免手动绑定的繁琐）
    private void BindButtonEvents()
    {
        // A方按钮绑定
        if (partyALikeButton != null)
            partyALikeButton.onClick.AddListener(OnPartyALikeButtonClicked);

        if (partyACommentButton != null)
            partyACommentButton.onClick.AddListener(OnPartyACommentButtonClicked);

        for (int i = 0; i < partyAGiftButtons.Length; i++)
        {
            int index = i + 1; // 礼物索引从1开始
            if (partyAGiftButtons[i] != null)
                partyAGiftButtons[i].onClick.AddListener(() => OnPartyAGiftButtonClicked(index));
        }

        // B方按钮绑定
        if (partyBLikeButton != null)
            partyBLikeButton.onClick.AddListener(OnPartyBLikeButtonClicked);

        if (partyBCommentButton != null)
            partyBCommentButton.onClick.AddListener(OnPartyBCommentButtonClicked);

        for (int i = 0; i < partyBGiftButtons.Length; i++)
        {
            int index = i + 1; // 礼物索引从1开始
            if (partyBGiftButtons[i] != null)
                partyBGiftButtons[i].onClick.AddListener(() => OnPartyBGiftButtonClicked(index));
        }
    }

    // 以下是原有事件处理方法（逻辑不变）
    public void OnPartyALikeButtonClicked()
    {
        if (_battleSystem != null)
        {
            _battleSystem.OnLikeButtonPressed(CampType.PartyA);
            Debug.Log("A方点赞按钮被点击");
        }
    }

    public void OnPartyACommentButtonClicked()
    {
        if (_battleSystem != null && !string.IsNullOrEmpty(partyACommentInput.text))
        {
            _battleSystem.OnCommentButtonPressed(CampType.PartyA, partyACommentInput.text);
            partyACommentInput.text = "";
            Debug.Log("A方评论按钮被点击");
        }
        else if (string.IsNullOrEmpty(partyACommentInput.text))
        {
            Debug.LogError("A方评论不能为空!");
        }
    }

    public void OnPartyAGiftButtonClicked(int giftIndex)
    {
        if (_battleSystem != null)
        {
            _battleSystem.OnGiftButtonPressed(CampType.PartyA, giftIndex);
            Debug.Log($"A方礼物按钮 {giftIndex} 被点击");
        }
    }

    public void OnPartyBLikeButtonClicked()
    {
        if (_battleSystem != null)
        {
            _battleSystem.OnLikeButtonPressed(CampType.PartyB);
            Debug.Log("B方点赞按钮被点击");
        }
    }

    public void OnPartyBCommentButtonClicked()
    {
        if (_battleSystem != null && !string.IsNullOrEmpty(partyBCommentInput.text))
        {
            _battleSystem.OnCommentButtonPressed(CampType.PartyB, partyBCommentInput.text);
            partyBCommentInput.text = "";
            Debug.Log("B方评论按钮被点击");
        }
        else if (string.IsNullOrEmpty(partyBCommentInput.text))
        {
            Debug.LogError("B方评论不能为空!");
        }
    }

    public void OnPartyBGiftButtonClicked(int giftIndex)
    {
        if (_battleSystem != null)
        {
            _battleSystem.OnGiftButtonPressed(CampType.PartyB, giftIndex);
            Debug.Log($"B方礼物按钮 {giftIndex} 被点击");
        }
    }
}


