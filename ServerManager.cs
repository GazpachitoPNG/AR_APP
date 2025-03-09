using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

public class ServerManager : MonoBehaviour
{
    [SerializeField] private string jsonURL;
    [SerializeField] private ItemButtomManager itemButtonManager;
    [SerializeField] private GameObject buttonsContainer; 

    [Serializable]

    public struct Items
    {
        [Serializable]
        public struct Item
        {
            public string Name;
            public string Description;
            public string URLBundleModel;
            public string URLImageModel; 
        }

        public Item[] items; 
    }

    public Items newItemsCollection = new Items(); 
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetJsonData());
        GameManager.instance.OnItemsMenu += CreateButtons; 
    }

    private void CreateButtons()
    {
        foreach (var item in newItemsCollection.items)
        {
            ItemButtomManager itemButton;
            itemButton = Instantiate(itemButtonManager); // Instanciar sin padre
            itemButton.transform.SetParent(buttonsContainer.transform, false); //Asignar padre sin afectar escala y rotacion
            itemButton.name = item.Name;
            itemButton.ItemName = item.Name;
            itemButton.ItemDescription = item.Description;
            itemButton.URLBundleModel = item.URLBundleModel;
            StartCoroutine(GetBundleImage(item.URLImageModel, itemButton));
        }
        GameManager.instance.OnItemsMenu -= CreateButtons;
    }

 

    IEnumerator GetJsonData()
    {
        UnityWebRequest serverRequest = UnityWebRequest.Get(jsonURL);
        yield return serverRequest.SendWebRequest();

        if (serverRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("JSON received: " + serverRequest.downloadHandler.text);

            try
            {
                string jsonText = serverRequest.downloadHandler.text.Trim();
                newItemsCollection = JsonUtility.FromJson<Items>(jsonText);
                Debug.Log("JSON parsed successfully!");
            }
            catch (System.ArgumentException ex)
            {
                Debug.LogError("Error parsing JSON: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("Error downloading JSON: " + serverRequest.error);
        }
    }

    IEnumerator GetBundleImage(string urlImage, ItemButtomManager button)
    {
        UnityWebRequest serverRequest = UnityWebRequest.Get(urlImage);
        serverRequest.downloadHandler = new DownloadHandlerTexture();
        yield return serverRequest.SendWebRequest();

        if (serverRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("JSON received: " + serverRequest.downloadHandler.text);

            try
            {
                string jsonText = serverRequest.downloadHandler.text.Trim();
                //newItemsCollection = JsonUtility.FromJson<Items>(jsonText);
                button.ImageBundle.texture = ((DownloadHandlerTexture)serverRequest.downloadHandler).texture;
                Debug.Log("JSON parsed successfully!");
            }
            catch (System.ArgumentException ex)
            {
                Debug.LogError("Error parsing JSON: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("Error downloading JSON: " + serverRequest.error);
        }
    }
}
