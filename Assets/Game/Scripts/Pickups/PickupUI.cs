using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PickupUI : MonoBehaviour
{
    public string name;
    public ItemType type;
    public ItemRarity rarity;
    public string description;

    private TextMeshPro nameText;
    private TextMeshPro typeText;
    private TextMeshPro descriptionText;

    [SerializeField] Color commonColor;
    [SerializeField] Color uncommonColor;
    [SerializeField] Color rareColor;

    public void Awake()
    {
        nameText = this.transform.GetChild(0).GetComponent<TextMeshPro>();
        typeText = this.transform.GetChild(1).GetComponent<TextMeshPro>();
        descriptionText = this.transform.GetChild(2).GetComponent<TextMeshPro>();
    }

    public void Start()
    {
        Color rarityColor = commonColor;
        switch (rarity)
        {
            case ItemRarity.COMMON:
                rarityColor = commonColor;
                break;
            case ItemRarity.UNCOMMON:
                rarityColor = uncommonColor;
                break;
            case ItemRarity.RARE:
                rarityColor = rareColor;
                break;
        }

        nameText.text = "NAME: " + "<color=#" + ColorUtility.ToHtmlStringRGB(rarityColor) + ">" + name + "</color>";
        typeText.text = "TYPE: " + type;
        descriptionText.text = description;
    }

    public void updateValues(string newName, ItemType newType, ItemRarity newRarity, string newDescription)
    {
        name = newName;
        type = newType;
        rarity = newRarity;
        description = newDescription;
    }
}
