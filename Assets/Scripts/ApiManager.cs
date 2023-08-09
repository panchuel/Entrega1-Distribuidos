using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class ApiManager : MonoBehaviour
{
    public List<Character> characters { get; private set; }
    public UnityEvent charactersLoaded;
    public static ApiManager manager;
    public int[] deck = { 1, 32, 358, 578, 601 };

    [SerializeField] private TMP_InputField field;
    [SerializeField] private TMP_Text errorMessage, userName;
    private string api = "https://rickandmortyapi.com/api/character/", server = "https://my-json-server.typicode.com/panchuel/Entrega1-Distribuidos/users/";
    private int index = 0;
    private string id;

    private void Awake()
    {
        characters = new List<Character>();
        manager = this;
    }

    #region CardRelated

    public void Request()
    {
        errorMessage.text = "";
        characters.Clear();
        GetCharactersMethod(deck);
    }

    public void GetCharactersMethod(int[] cards)
    {
        StartCoroutine(GetCharacter(cards));
    }

    public void SetImage(int index, RawImage img)
    {
        StartCoroutine(DownloadImage(characters.ElementAt(index).image, img));
    }
    
    IEnumerator GetCharacter(int[] cards)
    {
        errorMessage.text = "";
        
        UnityWebRequest www = UnityWebRequest.Get(api + cards[index]);
        yield return www.SendWebRequest();
        print(www);
        if(www.result != UnityWebRequest.Result.Success) 
        {
            Debug.Log("Connection Error: " + www.error);
            errorMessage.text = "Connection Error: " + www.error;
        }
        else 
        {
            if (www.responseCode == 200)
            {
                //string responseText = www.downloadHandler.text;

                Character character = JsonUtility.FromJson<Character>(www.downloadHandler.text);
                characters.Add(character);
            }
            else
            {
                string message = "Status: " + www.responseCode;
                message += "\nContent-type: " + www.GetResponseHeader("content-type");
                message += "\nError: " + www.error;
                errorMessage.text = message;
            }
        }

        if (index < 4)
        {
            index++;
            StartCoroutine(GetCharacter(cards));
        }
        else
        {
            index = 0;
            charactersLoaded.Invoke();
            StopCoroutine(GetCharacter(deck));
        }
    }
    
    IEnumerator DownloadImage(string url, RawImage targetTxt)
    {
        errorMessage.text = "";
        
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            targetTxt.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }

    #endregion

    #region UserRelated

    public void LoadUser()
    {
        
        if(!string.IsNullOrEmpty(field.text) && int.TryParse(id, out int numericValue)) StartCoroutine(GetUser(int.Parse(id)));
        else if (string.IsNullOrEmpty(field.text)) errorMessage.text = "Type an user id to search";
        else if (!int.TryParse(id, out int numericValue1)) errorMessage.text = "Using wrong format"; 
        
    }

    public void IdToCheck()
    {
        id = field.text;
    }
    
    IEnumerator GetUser(int id)
    {
        errorMessage.text = "";
        
        UnityWebRequest www = UnityWebRequest.Get(server + id);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Connection Error: " + www.error);
            errorMessage.text = "Connection Error: " + www.error;
        }
        else
        {
            if (www.responseCode == 200)
            {
                string responseText = www.downloadHandler.text;

                User user = JsonUtility.FromJson<User>(responseText);

                deck = user.deck;
                userName.text = user.name;
                
                Request();
            }
            else
            {
                string message = "Status: " + www.responseCode;
                message += "\nContent-type: " + www.GetResponseHeader("content-type");
                message += "\nError: " + www.error;
                errorMessage.text = message;
            }
        }
    }

    #endregion
}

[System.Serializable]
public class Character
{
    public int id;
    public string name;
    public string species;
    public string image;
}

[System.Serializable]
public class User
{
    public int id;
    public string name;
    public int[] deck;
}
