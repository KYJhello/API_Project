using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using TMPro;

[System.Serializable]
public class Root
{
    public string CharacterImage;
    public int ExpeditionLevel;
    public string PvpGradeName;
    public int TownLevel;
    public string TownName;
    public string Title;
    public string GuildMemberGrade;
    public string GuildName;
    public int UsingSkillPoint;
    public int TotalSkillPoint;
    public List<Stat> Stats;
    public List<Tendency> Tendencies;
    public string ServerName;
    public string CharacterName;
    public int CharacterLevel;
    public string CharacterClassName;
    public string ItemAvgLevel;
    public string ItemMaxLevel;
}

public class Stat
{
    public string Type;
    public string Value;
    public List<string> Tooltip;
}

public class Tendency
{
    public string Type;
    public int Point;
    public int MaxPoint;
}

public class Server : MonoBehaviour
{
    public API_Key apiKey;
    public GameObject charInfoCollection;
    public string curName;
    public TMP_InputField inputText;
    public TMP_Text uName;
    public TMP_Text uServer;
    public TMP_Text uClass;
    public TMP_Text uLevel;
    public RawImage img;

    private void Awake()
    {
        DisabeCharInfoCollection();
    }

    public void ActiveCharInfoCollection()
    {
        charInfoCollection.SetActive(true);
    }
    public void DisabeCharInfoCollection()
    {
        charInfoCollection.SetActive(false);
    }

    public void SaveUserInfoToJson(Root root)
    {
        string path = Path.Combine(Application.persistentDataPath + "/CharData.json");
        Debug.Log(path);
        if (File.Exists(path)) { File.Delete(path); }
        string json = JsonUtility.ToJson(root);
        File.WriteAllText(path, json);
    }
    public void LoadUserNameToJson()
    {
        string path = Path.Combine(Application.persistentDataPath + "/CharData.json");
        if (File.Exists(path)) {
            string json = File.ReadAllText(path);
            curName = JsonUtility.FromJson<Root>(json).CharacterName;
            StartCoroutine(CharInfoRequest(curName));
        }
        else
        {
            return;
        }
    }

    public void SetUserName()
    {
        if(curName == inputText.text)
        {
            return;
        }
        curName = inputText.text;
        OnCharSearchButton();
    }

    public void OnCharSearchButton()
    {
        if(curName == null)
        {
            return;
        }
        StartCoroutine(CharInfoRequest(curName));
    }

    IEnumerator CharInfoRequest(string userName)
    {
        ActiveCharInfoCollection();
        curName = userName;
        string url = $"https://developer-lostark.game.onstove.com/armories/characters/{userName}/profiles";
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("accept", "application/json");
        www.SetRequestHeader("authorization", "bearer " + apiKey.GetKeyData());        
        yield return www.SendWebRequest();

        if(www.error == null)
        {
            Root charData = JsonUtility.FromJson<Root>(www.downloadHandler.text);

            SaveUserInfoToJson(charData);

            uName.text = charData.CharacterName;
            uServer.text = charData.ServerName;
            uClass.text = charData.CharacterClassName;
            uLevel.text = charData.ItemMaxLevel.ToString();

            UnityWebRequest www2 = UnityWebRequestTexture.GetTexture(charData.CharacterImage);
            yield return www2.SendWebRequest();


            img.texture = ((DownloadHandlerTexture)www2.downloadHandler).texture;

        }
        else
        {
            Debug.Log("ERROR");
        }
    }
}
