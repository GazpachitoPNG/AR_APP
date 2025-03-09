using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ItemButtomManager : MonoBehaviour
{
    private string itemName;
    private string itemDescription;
    private Sprite itemImage;
    private GameObject item3DModel;
    private ARInteractionsManager interactionsManager;
    private string urlBundleModel;
    private RawImage imageBundle;


    public string ItemName { 
        set
        {
            itemName = value;
        }
    }
 

    public string ItemDescription { set => itemDescription = value; }
    public Sprite ItemImage { set => itemImage = value; }
    public GameObject Item3DModel { set => item3DModel = value; }
    public string URLBundleModel { set => urlBundleModel = value; }

    public RawImage ImageBundle { get => imageBundle; set => imageBundle = value; }

    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).GetComponent<TMP_Text>().text = itemName;
        //transform.GetChild(1).GetComponent<RawImage>().texture = itemImage.texture;
        imageBundle = transform.GetChild(1).GetComponent<RawImage>();
        transform.GetChild(2).GetComponent<TMP_Text>().text = itemDescription;

        var button = GetComponent<Button>();
        button.onClick.AddListener(GameManager.instance.ARPosition);
        button.onClick.AddListener(Create3DModel);

        interactionsManager = FindObjectOfType<ARInteractionsManager>();
    }

    private void Create3DModel()
    {
        //interactionsManager.Item3DModel = Instantiate(item3DModel);
        StartCoroutine(DownLoadAssetBundle(urlBundleModel));
    }

    IEnumerator DownLoadAssetBundle(string urlAssetBundle)
    {
        Debug.Log("Iniciando descarga del AssetBundle: " + urlAssetBundle);
        UnityWebRequest serverRequest = UnityWebRequestAssetBundle.GetAssetBundle(urlAssetBundle);
        yield return serverRequest.SendWebRequest();

        if (serverRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("AssetBundle descargado correctamente.");
            AssetBundle model3D = DownloadHandlerAssetBundle.GetContent(serverRequest);
            if (model3D != null)
            {
                Debug.Log("AssetBundle cargado correctamente.");
                string[] assetNames = model3D.GetAllAssetNames();
                if (assetNames.Length > 0)
                {
                    GameObject model = model3D.LoadAsset<GameObject>(assetNames[0]);
                    if (model != null)
                    {
                        Debug.Log("Modelo cargado correctamente desde el AssetBundle.");
                        GameObject modelInstance = Instantiate(model);

                        // Ajustar posición, escala y rotación del modelo
                        modelInstance.transform.position = new Vector3(0, 0, 2); // Coloca el modelo frente a la cámara
                        modelInstance.transform.localScale = Vector3.one; // Ajusta la escala si es necesario
                        modelInstance.transform.rotation = Quaternion.identity; // Ajusta la rotación si es necesario

                        // Asignar el modelo al ARInteractionsManager
                        interactionsManager.Item3DModel = modelInstance;
                        Debug.Log("Modelo instanciado y asignado al ARInteractionsManager.");
                    }
                    else
                    {
                        Debug.LogError("No se pudo cargar el modelo desde el AssetBundle.");
                    }
                }
                else
                {
                    Debug.LogError("El AssetBundle está vacío.");
                }

                // Liberar el AssetBundle de memoria
                model3D.Unload(false);
            }
            else
            {
                Debug.LogError("El AssetBundle está vacío o no es válido.");
            }
        }
        else
        {
            Debug.LogError("Error al descargar el AssetBundle: " + serverRequest.error);
        }
    }

}


