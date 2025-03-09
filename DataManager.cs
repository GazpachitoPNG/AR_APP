using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    [SerializeField] private List<Item> items = new List<Item>();
    [SerializeField] private GameObject buttonContainer;
    [SerializeField] private ItemButtomManager itemButtonManager;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.OnItemsMenu += CreateButtons; 
    }

    private void CreateButtons()
    {
      foreach (var item in items)
      {
            ItemButtomManager itemButton;
            itemButton = Instantiate(itemButtonManager, buttonContainer.transform);
            itemButton.ItemName = item.ItemName;
            itemButton.ItemImage = item.ItemImage;
            itemButton.Item3DModel = item.Item3DModel;
            itemButton.name = item.ItemName;

      }

        GameManager.instance.OnItemsMenu -= CreateButtons;

    }


}
