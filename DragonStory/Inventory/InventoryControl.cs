using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using TMPro;
public class InventoryControl : MonoBehaviour
{
    // Manager , Channel
    [SerializeField]
    ItemChannelSO itemChannelSO;
    [SerializeField]
    AudioChannelSO audioChannelSO;
    [SerializeField]
    InventoryManagerSO inventoryManagerSO;
    
    // Sciprt Component
    [SerializeField]
    private List<ItemSlot> itemSlots;

    // ������ ���� �� â
    [SerializeField]
    private Image selectedImage; // ���� �̹���
    [SerializeField]
    private TextMeshProUGUI selectedName; // TMP �̸�
    [SerializeField]
    private LocalizeStringEvent localName; // Local �̸�
    [SerializeField]
    private TextMeshProUGUI selectedDescript; //TMP ����
    [SerializeField]
    private LocalizeStringEvent localDescript; //Local ����

    // Variable
    private int index;
    private int originIndex;

    private Mode beforeMode;
    private void OnEnable()
    {
        beforeMode = GameManager.mode;
        GameManager.ChangeMode(Mode.INVENTORY);

        Init(); //�κ��丮 �Ŵ����� ����� ���������� �κ��丮 �����ϱ�
        SelectItem(itemSlots[index], itemSlots[index]);
        audioChannelSO.UiSoundEvent(AudioManager.UiSoundType.OPEN);//Sound
    }

    private void OnDisable()
    {
        GameManager.ChangeMode(beforeMode);
        audioChannelSO.UiSoundEvent(AudioManager.UiSoundType.CLOSE);//Sound
    }

    void Update()
    {
        if (InputManager.Instance.ShowInventory())
        {
            ScreenUIManager.Instance.ShowOffInventory();
        }

        if(InputManager.Instance.UiUp() || InputManager.Instance.UiDown() || InputManager.Instance.UiLeft() || InputManager.Instance.UiRight())
        {
            SelectMoveItem();
        }

        QuickItemSetting();
    }

    private void Init()
    {
        for(int i=0; i<inventoryManagerSO.itemList.Count; i++)
        {
            itemSlots[i].SetItem(inventoryManagerSO.itemList[i]);
            itemSlots[i].haveItem = true;
        }

        for(int i =0; i< itemSlots.Count; i++)
        {
            if (!itemSlots[i].haveItem) break;
            for(int j=0; j< inventoryManagerSO.quickItemList.Count; j++)
            {
                if(itemSlots[i].GetGuid().Equals(inventoryManagerSO.quickItemList[j].guid))
                {
                    itemSlots[i].QuickOutline(true);
                }
            }
        }
        index = 0;
    }
    // Quick Slot�� Item ����
    private void QuickItemSetting()
    {
        if (InputManager.Instance.QuickSlotAdd())
        {
            //�̹� �� ���Կ� ������ �Ǿ� �ִ��� �Ǵ�
            if(!inventoryManagerSO.itemList[index].isQuick)
            {
                if (inventoryManagerSO.quickItemList.Count >=3)
                {
                    InventoryUpdate();
                }
                itemChannelSO.QuickItemEvent(inventoryManagerSO.itemList[index]);
                itemSlots[index].QuickOutline(true);
            }
        }
    }

    private void InventoryUpdate()
    {
        for(int i =0; i<itemSlots.Count; i++)
        {
            if(itemSlots[i].GetGuid().Equals(inventoryManagerSO.quickItemList[0].guid))
            {
                itemSlots[i].QuickOutline(false);
                break;
            }
        }
    }

    private void SelectMoveItem() {

        originIndex = index;
        //��
        if (InputManager.Instance.UiUp())
        {
            index -= 4;
            if (index <= -1)
            {
                index = originIndex;
            }
        }
        //�Ʒ�
        else if (InputManager.Instance.UiDown())
        {
            index += 4;
            if (index >= 12)
            {
                index = originIndex;
            }
        }
        // ��
        else if (InputManager.Instance.UiLeft())
        {
            index--;
        }
        //��
        else if (InputManager.Instance.UiRight())
        {
            index++;
        }
        index = Mathf.Clamp(index, 0, 11);

        SelectItem(itemSlots[originIndex], itemSlots[index]);
    }

    private void SelectItem(ItemSlot beforeSlot, ItemSlot currentSlot)
    {
        if (inventoryManagerSO.itemList.Count == 0)
        {
            // ������ �� â ����
            selectedImage.enabled = false;
            localName.SetEntry("NULL");
            localDescript.SetEntry("NULL");
            return;
        }

        // Item ������ �ִ� Slot���θ� �̵��ϱ�
        if (currentSlot.haveItem)
        {
            beforeSlot.UnSelected();
            currentSlot.Selected();

            selectedImage.enabled = true;
            selectedImage.sprite = currentSlot.GetImage();
            localName.SetEntry(currentSlot.GetEntry());
            localDescript.SetEntry(currentSlot.GetEntry());

            if(currentSlot != beforeSlot)
            {
                audioChannelSO.UiSoundEvent(AudioManager.UiSoundType.MOVE);//Sound
            }
        }
        // Index ����
        else
        {
            index = originIndex;
        }
    }
}
