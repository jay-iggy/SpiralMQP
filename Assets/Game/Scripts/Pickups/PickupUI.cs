using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PickupUI : MonoBehaviour
{
    public string name;
    public ItemType type;
    public ItemRarity rarity;

    private TextMeshPro nameText;
    private TextMeshPro typeText;
    private TextMeshPro rarityText;

    private Color commonColor;
    private Color rareColor;
    private Color epicColor;
    private Color legendaryColor;

    public void Awake()
    {
        nameText = this.transform.GetChild(0).GetComponent<TextMeshPro>();
        typeText = this.transform.GetChild(1).GetComponent<TextMeshPro>();
        rarityText = this.transform.GetChild(2).GetComponent<TextMeshPro>();
    }

    public void Start()
    {
        nameText.text = "NAME: " + name;
        typeText.text = "TYPE: " + type;

        Color rarityColor = commonColor;
        switch (rarity)
        {
            case ItemRarity.COMMON:
                rarityColor = commonColor;
                break;
            case ItemRarity.RARE:
                rarityColor = rareColor;
                break;
            case ItemRarity.EPIC:
                rarityColor = epicColor;
                break;
            case ItemRarity.LEGENDARY:
                rarityColor = legendaryColor;
                break;
        }

        rarityText.text = "RARITY: " + "<color=#" + ColorUtility.ToHtmlStringRGB(rarityColor) + ">" + rarity + "</color>";
    }

    public void updateValues(string newName, ItemType newType, ItemRarity newRarity)
    {
        name = newName;
        type = newType;
        rarity = newRarity;
    }
}
