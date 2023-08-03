using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotControl : MonoBehaviour
{
    [SerializeField]
    private InventoryManagerSO inventoryManagerSO;
    [SerializeField]
    private ItemChannelSO itemChannelSO;

    public List<QuickSlot> quickSlotList;
    
    private int index;

    private void OnEnable()
    {
        itemChannelSO.OnQuickUpdateRequested += QuickSlotUpdate;
        //Invoke(nameof(QuickSlotUpdate),0.5f);
    }

    private void Start()
    {
        QuickSlotUpdate();
    }

    private void OnDisable()
    {
        itemChannelSO.OnQuickUpdateRequested -= QuickSlotUpdate;
    }

    private void QuickSlotUpdate()
    {
        for (int i = 0; i < quickSlotList.Count; i++)
        {
            if (i <= inventoryManagerSO.quickItemList.Count-1)
            {
                quickSlotList[i].SetQuickItem(inventoryManagerSO.quickItemList[i].item);
                inventoryManagerSO.quickItemList[i].quickNumber = i;
            }
            else
            {
                quickSlotList[i].Reset();
            }
        }
    }
}

