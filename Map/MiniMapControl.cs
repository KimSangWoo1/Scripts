using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MiniMapControl : MonoBehaviour
{
    [SerializeField] ImageAnimator _openAnimator;
    [SerializeField] ImageAnimator _closeAnimator;
    [SerializeField] List<Image> _images;

    [SerializeField] NormalMini _normalMini;
    [SerializeField] List<SpecialMIni> _specialMInis;
    [SerializeField] List<EventMini> _eventMinis;

    private Image[,] _cells;
    private Dictionary<(int x, int y), Room> _minimapDic;

    private int _mapXSize;
    private int _mapYSize;
    private bool _isOpen = true;

    private readonly int _size = 5;

    private void Awake()
    {
        Initialization();
    }

    private void Start()
    {
        EventManager.Instance.Notify(EventType.AddKeyDown, new EventData.InputKeyData(eActionMapType.Player, KeyType.MiniMap.ToString(), ControlMiniMap));
    }

    private void Initialization()
    {
        _minimapDic = new Dictionary<(int x, int y), Room>();
        _cells = new Image[_size, _size];

        int x = 0;
        int y = 4;

        for (int i = 0; i < _images.Count; i++)
        {
            if (i % _size == 0 && i != 0)
            {
                x = 0;
                y--;
            }
            _cells[x, y] = _images[i];
            x++;
        }
    }

    private void MiniMapReset()
    {
        for (int i = 0; i < _cells.GetLength(1); i++)
        {
            for (int j = 0; j < _cells.GetLength(0); j++)
            {
                _cells[i, j].sprite = GetResetIcon();
                _cells[i, j].enabled = false;
            }
        }
    }

    public void UpdateMiniMap()
    {
        MiniMapReset();
        Vector2Int playerWorldPos = DataManager.Instance.GameUtilsData.PlayerWorldPos;

        bool[,] miniMapFlags = new bool[_size, _size];

        Queue<(int mapX, int mapY, int cellX, int cellY, int distance)> queue = new();
        queue.Enqueue((playerWorldPos.x, playerWorldPos.y, 2, 2, 0)); // 5,5 크기의 index 중앙 값은 2,2

        while (queue.Count > 0)
        {
            var pos = queue.Dequeue();
            int distance = pos.distance++;

            for (int mY = pos.mapY - 1, cY = pos.cellY - 1; mY <= pos.mapY + 1; mY++, cY++)
            {
                for (int mX = pos.mapX - 1, cX = pos.cellX - 1; mX <= pos.mapX + 1; mX++, cX++)
                {
                    if (cX >= 0 && cX < _cells.GetLength(0) && cY >= 0 && cY < _cells.GetLength(1) && (mY == pos.mapY || mX == pos.mapX))
                    {
                        if (!miniMapFlags[cX, cY])
                        {
                            miniMapFlags[cX, cY] = true;

                            Room room;
                            if (_minimapDic.TryGetValue((mX, mY), out room))
                            {
                                _cells[cX, cY].enabled = true;
                                if (playerWorldPos.x == mX && playerWorldPos.y == mY)
                                {
                                    _cells[cX, cY].sprite = GetMinimapIcon(room.RoomType, room.EventRoomType, room.SpecialRoomType, room.IsRoomClear, true);
                                }
                                else
                                {
                                    _cells[cX, cY].sprite = GetMinimapIcon(room.RoomType, room.EventRoomType, room.SpecialRoomType, room.IsRoomClear, false);
                                    if (distance < 3 && room.IsRoomClear)
                                    {
                                        queue.Enqueue((mX, mY, cX, cY, distance));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    #region Callbacks
    //Image Animator Callback
    public void MiniMapStateUpdate(bool value)
    {
        _isOpen = value;
    }

    // Input Callback
    private void ControlMiniMap(InputAction.CallbackContext ctx)
    {
        if (_isOpen)
        {
            _openAnimator.gameObject.SetActive(false);

            if (!_closeAnimator.gameObject.activeSelf)
            {
                _closeAnimator.gameObject.SetActive(true);
            }
        }
        else
        {
            _closeAnimator.gameObject.SetActive(false);

            if (!_openAnimator.gameObject.activeSelf)
            {
                _openAnimator.gameObject.SetActive(true);
            }
        }
    }
    #endregion

    #region Set Map
    public void SetWorldMapSize(int xSize, int ySize)
    {
        _mapXSize = xSize;
        _mapYSize = ySize;
    }

    public void SetMiniMapCell(int x, int y, Room room)
    {
        _minimapDic.Add((x, y), room);
    }
    #endregion

    #region Get Icon
    private Sprite GetMinimapIcon(eRoomType roomType, eEventRoomType eventRoomType, eSpecialRoomType specialRoomType, bool isRoomClear, bool isPlayerInRoom = false)
    {
        if (isPlayerInRoom)
        {
            return _normalMini.Icon;
        }
        else
        {
            if (roomType == eRoomType.Event)
            {
                if (isRoomClear)
                {
                    return GetEventRoomIcon(eventRoomType);
                }
                else
                {
                    return GetResetIcon();
                }
            }
            else if (roomType == eRoomType.Special)
            {
                return GetSpecialRoomIcon(specialRoomType);
            }
            else
            {
                if (isRoomClear)
                {
                    return _normalMini.ClearIcon;
                }
                else
                {
                    return GetResetIcon();
                }
            }
        }
    }

    private Sprite GetEventRoomIcon(eEventRoomType eventRoomType)
    {
        for (int i = 0; i < _eventMinis.Count; i++)
        {
            if (_eventMinis[i].EventRoomType == eventRoomType)
            {
                return _eventMinis[i].Icon;
            }
        }
        return null;
    }

    private Sprite GetSpecialRoomIcon(eSpecialRoomType specialRoomType)
    {
        for(int i=0; i< _specialMInis.Count; i++)
        {
            if(_specialMInis[i].SpecialRoomType == specialRoomType)
            {
                return _specialMInis[i].Icon;
            }
        }
        return null;
    }

    private Sprite GetResetIcon()
    {
        return _normalMini.UnClearIcon;
    }
    #endregion

    #region Minimap Icons
    [System.Serializable]
    class MiniMapIcon
    {
        public Sprite Icon;
    }

    [System.Serializable]
    class SpecialMIni : MiniMapIcon
    {
        public eSpecialRoomType SpecialRoomType;
    }

    [System.Serializable]
    class EventMini : MiniMapIcon
    {
        public eEventRoomType EventRoomType;
    }

    [System.Serializable]
    class NormalMini : MiniMapIcon
    {
        public Sprite ClearIcon;
        public Sprite UnClearIcon;
    }
    #endregion
}
