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

    // 아이템 선택 상세 창
    [SerializeField]
    private Image selectedImage; // 선택 이미지
    [SerializeField]
    private TextMeshProUGUI selectedName; // TMP 이름
    [SerializeField]
    private LocalizeStringEvent localName; // Local 이름
    [SerializeField]
    private TextMeshProUGUI selectedDescript; //TMP 설명
    [SerializeField]
    private LocalizeStringEvent localDescript; //Local 설명

    // Variable
    private int index;
    private int originIndex;

    private Mode beforeMode;
    private void OnEnable()
    {
        beforeMode = GameManager.mode;
        GameManager.ChangeMode(Mode.INVENTORY);

        Init(); //인벤토리 매니저에 저장된 아이템으로 인벤토리 셋팅하기
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
    // Quick Slot에 Item 장착
    private void QuickItemSetting()
    {
        if (InputManager.Instance.QuickSlotAdd())
        {
            //이미 퀵 슬롯에 장착에 되어 있는지 판단
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
        //위
        if (InputManager.Instance.UiUp())
        {
            index -= 4;
            if (index <= -1)
            {
                index = originIndex;
            }
        }
        //아래
        else if (InputManager.Instance.UiDown())
        {
            index += 4;
            if (index >= 12)
            {
                index = originIndex;
            }
        }
        // 좌
        else if (InputManager.Instance.UiLeft())
        {
            index--;
        }
        //우
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
            // 아이템 상세 창 설정
            selectedImage.enabled = false;
            localName.SetEntry("NULL");
            localDescript.SetEntry("NULL");
            return;
        }

        // Item 가지고 있는 Slot으로만 이동하기
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
        // Index 복구
        else
        {
            index = originIndex;
        }
    }
}
