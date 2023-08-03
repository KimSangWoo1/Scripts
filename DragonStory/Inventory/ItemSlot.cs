using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Components;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    // UI Component
    [SerializeField]
    private Outline outline;
    [SerializeField]
    private Outline quickOutline;
    [SerializeField]
    private Image image;
    [SerializeField]
    private TextMeshProUGUI tmp;
    [SerializeField]
    private LocalizeStringEvent localStringEvent;

    // Variable Component
    private InventoryManagerSO.ItemBase itemBase;
    // Variable
    private string guid; // Inventory Guid
    private string entry; // Local Entry (Key)
    public bool haveItem; // slot에 아이템 유무

    private void OnDisable()
    {
        Reset();
    }

    public void SetItem(InventoryManagerSO.ItemBase _itemBase)
    {
        itemBase = _itemBase;
        guid = itemBase.guid;
        haveItem = true;

        image.sprite = itemBase.item.itemImage;
        image.enabled = true;

        // Local Text Setting
        StringTable table = LocalizationSettings.StringDatabase.GetTableAsync(itemBase.item.localName.TableReference).Result;
        entry = table.SharedData.GetEntryFromReference(itemBase.item.localName.TableEntryReference).Key;
        localStringEvent.SetEntry(entry);
        tmp.text = itemBase.item.localName.GetLocalizedString();
    }
    public ItemBaseSO GetItem()
    {
        return itemBase.item;
    }

    private void Reset()
    {
        guid = "";
        entry = "";
        image.enabled = false;
        localStringEvent.SetEntry(entry);
        tmp.text = "";
        itemBase = null;
        haveItem = false;
        QuickOutline(false);
        UnSelected();
    }

    public void Selected()
    {
        outline.enabled = true;
    }

    public void UnSelected()
    {
        outline.enabled = false;
    }

    public void QuickOutline(bool line)
    {
        quickOutline.enabled = line;
    }
    public Sprite GetImage()
    {
        return image.sprite;
    }
    public string GetEntry()
    {
        return entry;
    }
    public string GetGuid()
    {
        return guid;
    }
}
