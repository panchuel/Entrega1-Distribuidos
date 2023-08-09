using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardSlot : MonoBehaviour
{
    [SerializeField] private RawImage img;
    [SerializeField] private TMP_Text id, cardName, specie;
    public int index;
    
    // Start is called before the first frame update
    void Start()
    {
        ApiManager.manager.charactersLoaded.AddListener(SetImage);
    }

    void SetImage()
    {
        ApiManager.manager.SetImage(index, img);
        id.text = ApiManager.manager.characters.ElementAt(index).id.ToString();
        cardName.text = ApiManager.manager.characters.ElementAt(index).name;
        specie.text = ApiManager.manager.characters.ElementAt(index).species;
    }
}
