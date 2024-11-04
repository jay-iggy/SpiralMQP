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

    [SerializeField] Color commonColor;
    [SerializeField] Color uncommonColor;
    [SerializeField] Color rareColor;

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
            case ItemRarity.UNCOMMON:
                rarityColor = uncommonColor;
                break;
            case ItemRarity.RARE:
                rarityColor = rareColor;
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
