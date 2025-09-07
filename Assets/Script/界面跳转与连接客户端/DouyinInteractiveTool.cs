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
    // ���ò�����˫������
    public TMP_InputField nicknameInput;      // �����ǳ�
    public TMP_InputField roomIdInput;        // ���÷����
    public TMP_InputField urlInputField;      // ��������

    // ˫������
    public PartySettings partyA;
    public PartySettings partyB;

    public Button partyAJoinButton;  // A�����밴ť
    public Button partyBJoinButton;  // B�����밴ť

    // ��ʱ���ã��룩
    public int timeoutSeconds = 10;
    // ����ģʽ����
    public bool testMode = true;

    // 8�����ﰴť��Ӧ�Ĺ̶���ֵ���ӵ͵��ߣ���λ���֣�
    private int[] giftValues = { 10, 20, 30, 40, 50, 60, 70, 80 };

    // Ԥ������ͷ��URL�б�
    public List<string> randomAvatarUrls = new List<string>
    {
        "http://192.168.66.136:9091/client/dyLogin"
    };

    // ���ﰴť״̬����
    private int partyAGiftClickCount = 0;
    private int partyBGiftClickCount = 0;
    private bool isVideoPlaying = false; // ��Ƶ����״̬

    void Start()
    {
        // ��ʼ��Ĭ��ֵΪָ������
        if (urlInputField != null && string.IsNullOrEmpty(urlInputField.text))
        {
            urlInputField.text = "http://192.168.66.136:9091/client/dyLogin";
            Log("������Ĭ��URL");
        }

        // ȷ����Ĭ��ͷ��URL
        if (randomAvatarUrls.Count == 0)
        {
            randomAvatarUrls.Add("http://192.168.66.136:529/client/gift_callback");
            Log("�����Ĭ��ͷ��URL");
        }

        // �Զ��󶨼��밴ť�¼�
        if (partyAJoinButton != null)
        {
            partyAJoinButton.onClick.RemoveAllListeners();
            partyAJoinButton.onClick.AddListener(OnPartyAJoinButtonClicked);
            Log("�Ѱ�A�����밴ť�¼�");
        }
        else
        {
            LogError("partyAJoinButtonδ��Inspector�и�ֵ");
        }

        if (partyBJoinButton != null)
        {
            partyBJoinButton.onClick.RemoveAllListeners();
            partyBJoinButton.onClick.AddListener(OnPartyBJoinButtonClicked);
            Log("�Ѱ�B�����밴ť�¼�");
        }
        else
        {
            LogError("partyBJoinButtonδ��Inspector�и�ֵ");
        }
    }

    //�ṩ��ȡ��������ķ���
    public int[] GetGiftValues()
    {
        return giftValues;
    }

    //��Ƶ���Ʒ���
    private void ToggleVideo()
    {
        isVideoPlaying = !isVideoPlaying;

        if (isVideoPlaying)
        {
            Log("������Ƶ");
            // ʵ����Ŀ����ӿ�����Ƶ�Ĵ���
        }
        else
        {
            Log("�ر���Ƶ");
            // ʵ����Ŀ����ӹر���Ƶ�Ĵ���
        }
    }

    // ��ȡ��ǰ���뼶ʱ���
    private long GetCurrentTimestamp()
    {
        return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds;
    }

    // �������msg_id
    private string GenerateMsgId()
    {
        return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);
    }

    // �������sec_openid
    private string GenerateSecOpenId()
    {
        return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 12);
    }

    // A�����밴ť����¼�
    public void OnPartyAJoinButtonClicked()
    {
        Log("A�����밴ť�����");
        if (partyA == null)
        {
            LogError("partyAδ��Inspector������");
            return;
        }
        SendJoinRequest(partyA);
    }

    // A�����۰�ť����¼�
    public void OnPartyACommentButtonClicked()
    {
        if (string.IsNullOrEmpty(partyA.commentInput.text))
        {
            LogError($"{partyA.partyName} �������ݲ���Ϊ��");
            return;
        }
        SendCommentRequest(partyA);
    }

    // A�����ް�ť����¼�
    public void OnPartyALikeButtonClicked()
    {
        SendLikeRequest(partyA);
    }

    // A�����ﰴť����¼������հ�ť������
    public void OnPartyAGiftButtonClicked(int buttonIndex)
    {
        partyAGiftClickCount++;

        // ��������������ݲ�ȷ����Ƶ����
        if (partyAGiftClickCount % 2 == 1)
        {
            SendGiftRequest(partyA, buttonIndex);

            if (!isVideoPlaying)
            {
                ToggleVideo(); // ������Ƶ
            }
        }
        // ż�����ֻ�ر���Ƶ����������
        else
        {
            if (isVideoPlaying)
            {
                ToggleVideo(); // �ر���Ƶ
            }
        }
    }

    // B�����밴ť����¼�
    public void OnPartyBJoinButtonClicked()
    {
        Log("B�����밴ť�����");
        if (partyB == null)
        {
            LogError("partyBδ��Inspector������");
            return;
        }
        SendJoinRequest(partyB);
    }

    // B�����۰�ť����¼�
    public void OnPartyBCommentButtonClicked()
    {
        if (string.IsNullOrEmpty(partyB.commentInput.text))
        {
            LogError($"{partyB.partyName} �������ݲ���Ϊ��");
            return;
        }
        SendCommentRequest(partyB);
    }

    // B�����ް�ť����¼�
    public void OnPartyBLikeButtonClicked()
    {
        SendLikeRequest(partyB);
    }

    // B�����ﰴť����¼������հ�ť������
    public void OnPartyBGiftButtonClicked(int buttonIndex)
    {
        partyBGiftClickCount++;

        // ��������������ݲ�ȷ����Ƶ����
        if (partyBGiftClickCount % 2 == 1)
        {
            SendGiftRequest(partyB, buttonIndex);

            if (!isVideoPlaying)
            {
                ToggleVideo(); // ������Ƶ
            }
        }
        // ż�����ֻ�ر���Ƶ����������
        else
        {
            if (isVideoPlaying)
            {
                ToggleVideo(); // �ر���Ƶ
            }
        }
    }

    // ���ͼ�������
    private void SendJoinRequest(PartySettings party)
    {
        Log($"׼������{party.partyName}��������");

        // ��֤��Ҫ����
        if (string.IsNullOrEmpty(nicknameInput.text))
        {
            LogError("�ǳƲ���Ϊ��");
            return;
        }

        if (string.IsNullOrEmpty(roomIdInput.text))
        {
            LogError("����Ų���Ϊ��");
            return;
        }

        if (string.IsNullOrEmpty(urlInputField.text))
        {
            LogError("URL����Ϊ��");
            return;
        }

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

    // ������������
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

    // ���͵�������
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

        // ����ս��ϵͳ����С��
        CampType camp = party == partyA ? CampType.PartyA : CampType.PartyB;
        // ע�⣺ȷ��BattleSystem.Instance����ȷ��ʼ��
        if (BattleSystem.Instance != null)
        {
            BattleSystem.Instance.OnLikeButtonPressed(camp);
        }
        else
        {
            LogError("BattleSystem.InstanceΪnull");
        }
    }

    // �����������������̶�Ϊ1����ֵ����ť�����ӵ͵��ߣ�
    private void SendGiftRequest(PartySettings party, int buttonIndex)
    {
        // ȷ����������Ч��Χ��
        if (buttonIndex < 0 || buttonIndex >= giftValues.Length)
        {
            LogError($"{party.partyName} ��Ч�����ﰴť����: {buttonIndex}");
            return;
        }

        // ���ɻ�������
        string msgId = GenerateMsgId();
        string secOpenId = GenerateSecOpenId();
        string avatarUrl = randomAvatarUrls[UnityEngine.Random.Range(0, randomAvatarUrls.Count)];

        // ��������������̶�Ϊ1����ֵ���ݰ�ť������ȡ
        int giftNum = 1; // �̶�����Ϊ1
        int giftValue = giftValues[buttonIndex]; // ��������ȡ��Ӧ��ֵ
        string secGiftId = (buttonIndex + 1).ToString(); // ����ID�밴ť����������1-8��
        string audienceSecOpenId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 12);

        // ������������
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

        // ���ڲ���ģʽ�����test�ֶ�
        if (testMode)
        {
            data["test"] = true;
        }

        SendRequest(data, party.partyName);

        // ����ս��ϵͳ����С����ע������+1��Ӧ1-8��
        CampType camp = party == partyA ? CampType.PartyA : CampType.PartyB;
        if (BattleSystem.Instance != null)
        {
            BattleSystem.Instance.OnGiftButtonPressed(camp, buttonIndex + 1);
        }
        else
        {
            LogError("BattleSystem.InstanceΪnull");
        }
    }

    // ����HTTP����
    private void SendRequest(Dictionary<string, object> data, string partyName)
    {
        Log($"��ʼ����{partyName}������");

        if (urlInputField == null)
        {
            LogError("urlInputFieldδ��Inspector�и�ֵ");
            return;
        }

        string url = urlInputField.text;
        if (string.IsNullOrEmpty(url))
        {
            LogError("����������URL");
            return;
        }

        if (string.IsNullOrEmpty(nicknameInput.text))
        {
            LogError("�������ǳ�");
            return;
        }

        if (string.IsNullOrEmpty(roomIdInput.text))
        {
            LogError("������roomId");
            return;
        }

        Log($"��ʼ����{partyName}����: {url}");
        StartCoroutine(PostRequest(url, data, partyName));
    }

    // ����POST����
    // ����POST�����޸��棩
    private IEnumerator PostRequest(string url, Dictionary<string, object> data, string partyName)
    {
        UnityWebRequest request = null;
        string formattedJson = "";

        try
        {
            List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>> { data };
            string jsonData = ConvertListToJson(dataList);
            formattedJson = ConvertListToJson(dataList, true);

            Log($"{partyName} ׼�����͵�JSON����: {jsonData}");

            request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = timeoutSeconds;

            Log($"{partyName} ��ʼ��������...");
        }
        catch (Exception ex)
        {
            LogError($"{partyName} ����׼���׶η����쳣: {ex.Message}\n��ջ����: {ex.StackTrace}");
            yield break; // �˳�Э��
        }

        // ��yield return�Ƶ�try/catch�ⲿ
        yield return request.SendWebRequest();

        // ����������
        try
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                Log($"{partyName} ����ɹ������͵�����:\n{formattedJson}\n��������Ӧ: {request.downloadHandler.text}");
            }
            else
            {
                LogError($"{partyName} ����ʧ��: {request.error} ��Ӧ��: {(int)request.responseCode}\n���Է��͵�����:\n{formattedJson}");
            }
        }
        catch (Exception ex)
        {
            LogError($"{partyName} ������Ӧʱ�����쳣: {ex.Message}\n��ջ����: {ex.StackTrace}");
        }
        finally
        {
            // ȷ���ͷ���Դ
            if (request != null)
            {
                request.Dispose();
            }
        }
    }


    // JSONת������־�ȸ�������
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
        string logMessage = $"[{DateTime.Now:HH:mm:ss}] ����: {message}";
        Debug.LogError(logMessage);
    }
}

[Serializable]
public class PartySettings
{
    public string partyName;
    public TMP_InputField commentInput;
}

