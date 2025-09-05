using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DouyinInteractiveTool : MonoBehaviour
{
    // 共用参数（双方共享）
    public TMP_InputField nicknameInput;      // 共用昵称
    public TMP_InputField roomIdInput;        // 共用房间号
    public TMP_InputField urlInputField;      // 共用链接

    // 双方设置
    public PartySettings partyA;
    public PartySettings partyB;

    public Button partyAJoinButton;  // A方加入按钮
    public Button partyBJoinButton;  // B方加入按钮

    // 超时设置（秒）
    public int timeoutSeconds = 10;
    // 测试模式开关
    public bool testMode = true;

    // 8个礼物按钮对应的固定价值（从低到高，单位：分）
    private int[] giftValues = { 10, 20, 30, 40, 50, 60, 70, 80 };

    // 预设的随机头像URL列表
    public List<string> randomAvatarUrls = new List<string>
    {
        "https://p6.douyinpic.com/aweme/100x100/aweme-avatar/tos-cn-avt-0015_6bc85980317f5269c48625bb4d518abb.jpeg?from=3067671334"
    };

    // 礼物按钮状态跟踪
    private int partyAGiftClickCount = 0;
    private int partyBGiftClickCount = 0;
    private bool isVideoPlaying = false; // 视频播放状态

    void Start()
    {
        // 初始化默认值为指定链接
        if (urlInputField != null && string.IsNullOrEmpty(urlInputField.text))
        {
            urlInputField.text = "http://192.168.66.136:529/client/gift_callback";
        }

        // 自动绑定加入按钮事件
        if (partyAJoinButton != null)
            partyAJoinButton.onClick.AddListener(OnPartyAJoinButtonClicked);

        if (partyBJoinButton != null)
            partyBJoinButton.onClick.AddListener(OnPartyBJoinButtonClicked);
    }

    //提供获取礼物分数的方法
    public int[] GetGiftValues()
    {
        return giftValues;
    }

    //视频控制方法
    private void ToggleVideo()
    {
        isVideoPlaying = !isVideoPlaying;

        if (isVideoPlaying)
        {
            Debug.Log("开启视频");
            // 实际项目中添加开启视频的代码
        }
        else
        {
            Debug.Log("关闭视频");
            // 实际项目中添加关闭视频的代码
        }
    }

    // 获取当前毫秒级时间戳
    private long GetCurrentTimestamp()
    {
        return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds;
    }

    // 生成随机msg_id
    private string GenerateMsgId()
    {
        return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);
    }

    // 生成随机sec_openid
    private string GenerateSecOpenId()
    {
        return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 12);
    }

    // A方加入按钮点击事件
    public void OnPartyAJoinButtonClicked()
    {
        SendJoinRequest(partyA);
    }

    // A方评论按钮点击事件
    public void OnPartyACommentButtonClicked()
    {
        if (string.IsNullOrEmpty(partyA.commentInput.text))
        {
            LogError($"{partyA.partyName} 评论内容不能为空");
            return;
        }
        SendCommentRequest(partyA);
    }

    // A方点赞按钮点击事件
    public void OnPartyALikeButtonClicked()
    {
        SendLikeRequest(partyA);
    }

    // A方礼物按钮点击事件（接收按钮索引）
    public void OnPartyAGiftButtonClicked(int buttonIndex)
    {
        partyAGiftClickCount++;

        // 奇数点击发送数据并确保视频开启
        if (partyAGiftClickCount % 2 == 1)
        {
            SendGiftRequest(partyA, buttonIndex);

            if (!isVideoPlaying)
            {
                ToggleVideo(); // 开启视频
            }
        }
        // 偶数点击只关闭视频不发送数据
        else
        {
            if (isVideoPlaying)
            {
                ToggleVideo(); // 关闭视频
            }
        }
    }

    // B方加入按钮点击事件
    public void OnPartyBJoinButtonClicked()
    {
        SendJoinRequest(partyB);
    }

    // B方评论按钮点击事件
    public void OnPartyBCommentButtonClicked()
    {
        if (string.IsNullOrEmpty(partyB.commentInput.text))
        {
            LogError($"{partyB.partyName} 评论内容不能为空");
            return;
        }
        SendCommentRequest(partyB);
    }

    // B方点赞按钮点击事件
    public void OnPartyBLikeButtonClicked()
    {
        SendLikeRequest(partyB);
    }

    // B方礼物按钮点击事件（接收按钮索引）
    public void OnPartyBGiftButtonClicked(int buttonIndex)
    {
        partyBGiftClickCount++;

        // 奇数点击发送数据并确保视频开启
        if (partyBGiftClickCount % 2 == 1)
        {
            SendGiftRequest(partyB, buttonIndex);

            if (!isVideoPlaying)
            {
                ToggleVideo(); // 开启视频
            }
        }
        // 偶数点击只关闭视频不发送数据
        else
        {
            if (isVideoPlaying)
            {
                ToggleVideo(); // 关闭视频
            }
        }
    }

    // 发送加入请求
    private void SendJoinRequest(PartySettings party)
    {
        string msgId = GenerateMsgId();
        string secOpenId = GenerateSecOpenId();
        string avatarUrl = randomAvatarUrls[UnityEngine.Random.Range(0, randomAvatarUrls.Count)];

        var data = new Dictionary<string, object>
        {
            {"msg_id", msgId},
            {"sec_openid", secOpenId},
            {"nickname", nicknameInput.text},
            {"avatar_url", avatarUrl},
            {"roomId", roomIdInput.text},
            {"timestamp", GetCurrentTimestamp()},
            {"test", testMode}
        };

        SendRequest(data, party.partyName);
    }

    // 发送评论请求
    private void SendCommentRequest(PartySettings party)
    {
        string msgId = GenerateMsgId();
        string secOpenId = GenerateSecOpenId();
        string avatarUrl = randomAvatarUrls[UnityEngine.Random.Range(0, randomAvatarUrls.Count)];

        var data = new Dictionary<string, object>
        {
            {"msg_id", msgId},
            {"sec_openid", secOpenId},
            {"content", party.commentInput.text},
            {"avatar_url", avatarUrl},
            {"nickname", nicknameInput.text},
            {"timestamp", GetCurrentTimestamp()}
        };

        SendRequest(data, party.partyName);
    }

    // 发送点赞请求
    private void SendLikeRequest(PartySettings party)
    {
        string msgId = GenerateMsgId();
        string secOpenId = GenerateSecOpenId();
        string avatarUrl = randomAvatarUrls[UnityEngine.Random.Range(0, randomAvatarUrls.Count)];
        int likeNum = UnityEngine.Random.Range(1, 10);

        var data = new Dictionary<string, object>
        {
            {"msg_id", msgId},
            {"sec_openid", secOpenId},
            {"like_num", likeNum},
            {"avatar_url", avatarUrl},
            {"nickname", nicknameInput.text},
            {"timestamp", GetCurrentTimestamp()}
        };

        SendRequest(data, party.partyName);
    }

    // 发送礼物请求（数量固定为1，价值按按钮索引从低到高）
    private void SendGiftRequest(PartySettings party, int buttonIndex)
    {
        // 确保索引在有效范围内
        if (buttonIndex < 0 || buttonIndex >= giftValues.Length)
        {
            LogError($"{party.partyName} 无效的礼物按钮索引: {buttonIndex}");
            return;
        }

        // 生成基础参数
        string msgId = GenerateMsgId();
        string secOpenId = GenerateSecOpenId();
        string avatarUrl = randomAvatarUrls[UnityEngine.Random.Range(0, randomAvatarUrls.Count)];

        // 礼物参数：数量固定为1，价值根据按钮索引获取
        int giftNum = 1; // 固定数量为1
        int giftValue = giftValues[buttonIndex]; // 按索引获取对应价值
        string secGiftId = (buttonIndex + 1).ToString(); // 礼物ID与按钮索引关联（1-8）
        string audienceSecOpenId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 12);

        // 构建礼物数据
        var data = new Dictionary<string, object>
        {
            {"msg_id", msgId},
            {"sec_openid", secOpenId},
            {"sec_gift_id", secGiftId},
            {"gift_num", giftNum},
            {"gift_value", giftValue},
            {"avatar_url", avatarUrl},
            {"nickname", nicknameInput.text},
            {"timestamp", GetCurrentTimestamp()},
            {"audience_sec_open_id", audienceSecOpenId}
        };

        // 仅在测试模式下添加test字段
        if (testMode)
        {
            data["test"] = true;
        }

        SendRequest(data, party.partyName);
    }

    // 发送HTTP请求
    private void SendRequest(Dictionary<string, object> data, string partyName)
    {
        if (urlInputField == null || string.IsNullOrEmpty(urlInputField.text))
        {
            LogError("请输入请求URL");
            return;
        }

        if (string.IsNullOrEmpty(nicknameInput.text))
        {
            LogError("请输入昵称");
            return;
        }

        if (string.IsNullOrEmpty(roomIdInput.text))
        {
            LogError("请输入roomId");
            return;
        }

        StartCoroutine(PostRequest(urlInputField.text, data, partyName));
    }

    // 处理POST请求
    private IEnumerator PostRequest(string url, Dictionary<string, object> data, string partyName)
    {
        List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>> { data };
        string jsonData = ConvertListToJson(dataList);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        request.timeout = timeoutSeconds;

        yield return request.SendWebRequest();

        string formattedJson = ConvertListToJson(dataList, true);
        if (request.result == UnityWebRequest.Result.Success)
        {
            Log($"{partyName} 请求成功！发送的数据:\n{formattedJson}\n服务器响应: {request.downloadHandler.text}");
        }
        else
        {
            LogError($"{partyName} 请求失败: {request.error} 响应码: {(int)request.responseCode}\n尝试发送的数据:\n{formattedJson}");
        }

        request.Dispose();
    }

    // JSON转换和日志等辅助方法
    private string ConvertListToJson(List<Dictionary<string, object>> dataList, bool prettyPrint = false)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("[");

        if (prettyPrint) sb.Append("\n");

        int itemIndex = 0;
        foreach (var data in dataList)
        {
            itemIndex++;
            sb.Append(ConvertDictionaryToJson(data, prettyPrint));

            if (itemIndex < dataList.Count)
            {
                sb.Append(",");
            }

            if (prettyPrint) sb.Append("\n");
        }

        sb.Append("]");
        return sb.ToString();
    }

    private string ConvertDictionaryToJson(Dictionary<string, object> data, bool prettyPrint = false)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{");

        if (prettyPrint) sb.Append("\n");

        int index = 0;
        int total = data.Count;

        foreach (var pair in data)
        {
            index++;
            string key = $"\"{pair.Key}\":";
            string value;

            if (pair.Value is string)
            {
                value = $"\"{EscapeJson(pair.Value.ToString())}\"";
            }
            else if (pair.Value is bool)
            {
                value = pair.Value.ToString().ToLower();
            }
            else
            {
                value = pair.Value.ToString();
            }

            if (prettyPrint)
            {
                sb.Append($"    {key} {value}");
            }
            else
            {
                sb.Append($"{key}{value}");
            }

            if (index < total)
            {
                sb.Append(",");
            }

            if (prettyPrint) sb.Append("\n");
        }

        if (prettyPrint)
            sb.Append("}");
        else
            sb.Append("}");

        return sb.ToString();
    }

    private string EscapeJson(string value)
    {
        if (string.IsNullOrEmpty(value))
            return "";

        return value.Replace("\\", "\\\\")
                    .Replace("\"", "\\\"")
                    .Replace("\b", "\\b")
                    .Replace("\f", "\\f")
                    .Replace("\n", "\\n")
                    .Replace("\r", "\\r")
                    .Replace("\t", "\\t");
    }

    private void Log(string message)
    {
        string logMessage = $"[{DateTime.Now:HH:mm:ss}] {message}";
        Debug.Log(logMessage);
    }

    private void LogError(string message)
    {
        string logMessage = $"[{DateTime.Now:HH:mm:ss}] 错误: {message}";
        Debug.LogError(logMessage);
    }
}

[Serializable]
public class PartySettings
{
    public string partyName;
    public TMP_InputField commentInput;
}
