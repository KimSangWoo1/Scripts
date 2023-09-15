using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomControl : MonoBehaviour
{
    [SerializeField]
    private Room _room;

    [HideInInspector]
    public bool IsConnectTop;
    [HideInInspector]
    public bool IsConnectBottom;
    [HideInInspector]
    public bool IsConnectLeft;
    [HideInInspector]
    public bool IsConnectRight;

    [SerializeField]
    private Collider2D _camBound2D;
    [SerializeField]
    private GameObject _topDoor;
    [SerializeField]
    private GameObject _bottomDoor;
    [SerializeField]
    private GameObject _leftDoor;
    [SerializeField]
    private GameObject _rightDoor;

    private ChpaterMapInfoSO _chpaterMapInfoSO;

    private bool _isRoomClear;
    private bool _isEntered;

    public Room Room { get => _room; set => _room = value; }
    public ChpaterMapInfoSO ChpaterMapInfoSO { get => _chpaterMapInfoSO; set => _chpaterMapInfoSO = value; }
    public Collider2D CamBound2D => _camBound2D;
    public Tilemap TileMap { get; set; }

    public Action<Vector3Int[,]> DrawRoom;
    public Action<Vector3Int[,]> ClearRoom;

    public Func<RoomControl, UniTask<List<GameObject>>> CreateMonster;
    public Func<RoomControl, UniTask<List<GameObject>>> CreateSpecificMonster;

#if UNITY_EDITOR
    private Texture2D _whiteTexture;
	private Texture2D _redTexture;
    private Texture2D _blueTexture;

    [Header("GUI")]
	[SerializeField]
	private bool _regionDraw;
	[SerializeField]
	private bool _specificCoordDraw;
    [SerializeField]
    private bool _createAbleDraw;

    private GUIStyle _currentStyle;
    private GUIContent _guiContent;
#endif

    private void Awake()
	{
		TextureInit();
	}

	private void OnEnable()
    {
        if (_isRoomClear)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    public void OnClear()
    {
        _isRoomClear = true;
        OpenDoor();
    }

    public void OpenDoor()
    {
        if (IsConnectTop) 
            _topDoor.SetActive(false);
        if (IsConnectBottom) 
            _bottomDoor.SetActive(false);
        if (IsConnectLeft) 
            _leftDoor.SetActive(false);
        if (IsConnectRight) 
            _rightDoor.SetActive(false);
    }

    public void CloseDoor()
    {
        _topDoor.SetActive(true);
        _bottomDoor.SetActive(true);
        _leftDoor.SetActive(true);
        _rightDoor.SetActive(true);
    }

    public async void RoomOn()
    {
        RoomReady(true);
        this.gameObject.SetActive(true);

        // 타일 그리기
        DrawRoom(Room.Map);
        if (!_isEntered)
        {
            // 몬스터 생성
            Room.Monsters = await CreateMonster(this);
            if(Room.SpecialRoomType != eSpecialRoomType.None)
                Room.SpecificMonsters = await CreateSpecificMonster(this);
        }
        _isEntered = true;

        // 이벤트방 or Room Cleared 일 경우
        if (Room.RoomType == eRoomType.Event || _isRoomClear)
        {
            OpenDoor();
        }
        // 몬스터 방
        if(Room.monsterRoomType == eMonsterRoomType.None || Room.SpecialRoomType == eSpecialRoomType.None)
        {
#if UNITY_EDITOR
            if (CheatKey.IsDevOn)
            {
                OpenDoor();
                _isRoomClear = true;
            }
#endif
        }
        else
        {
            OpenDoor();
            _isRoomClear = true;
        }
    }

    public void RoomOff()
    {
        // 타일 지우기
        ClearRoom(Room.Map);
        this.gameObject.SetActive(false);
    }

    public Vector2 GetFrontDoorPosition(Vector2Int enterPos)
    {
        Vector2 readyPosition = Room.Center;
        if (enterPos == Vector2Int.up)
        {
            readyPosition = _bottomDoor.transform.position + _bottomDoor.transform.up;
        }
        if (enterPos == Vector2Int.down)
        {
            readyPosition = _topDoor.transform.position + -_topDoor.transform.up;
        }
        if (enterPos == Vector2Int.right)
        {
            readyPosition = _leftDoor.transform.position + _leftDoor.transform.right;
        }
        if (enterPos == Vector2Int.left)
        {
            readyPosition = _rightDoor.transform.position + -_rightDoor.transform.right;
        }
		return readyPosition;
    }

    public void RoomReady(bool isReady)
    {
        PropSetActive(isReady);
        SpecificPropSetActive(isReady);
        NpcSetActive(isReady);
        TrabSetActive(isReady);
    }

    private void PropSetActive(bool isActive)
    {
        if (Room.Props != null)
        {
            for (int i = 0; i < Room.Props.Count; i++)
            {
                Room.Props[i].SetActive(isActive);
            }
        }
    }

    private void TrabSetActive(bool isActive)
    {
        if (Room.Trabs != null)
        {
            for (int i = 0; i < Room.Trabs.Count; i++)
            {
                Room.Trabs[i].SetActive(isActive);
            }
        }
    }

    private void SpecificPropSetActive(bool isActive)
    {
        if (Room.SpecificProps != null)
        {
            for (int i = 0; i < Room.SpecificProps.Count; i++)
            {
                Room.SpecificProps[i].SetActive(isActive);
            }
        }
    }

    private void NpcSetActive(bool isActive)
    {
        if (Room.Npcs != null)
        {
            for (int i = 0; i < Room.Npcs.Count; i++)
            {
                Room.Npcs[i].SetActive(isActive);
            }
        }
    }

#if UNITY_EDITOR
    private void TextureInit()
	{
		_whiteTexture = MakeTex(32, 32, new Color(0, 0, 0, 0.4f));
		_redTexture = MakeTex(32, 32, new Color(1, 0, 0, 0.4f));
        _blueTexture = MakeTex(32, 32, new Color(0, 0, 1, 0.4f));
    }

	private Texture2D MakeTex(int width, int height, Color col)
	{
		Color[] pix = new Color[width * height];
		for (int i = 0; i < pix.Length; ++i)
		{
			pix[i] = col;
		}
		Texture2D result = new Texture2D(width, height);
		result.SetPixels(pix);
		result.Apply();
		return result;
	}

	private void DrawGUIRegions()
	{
        _currentStyle = new GUIStyle(GUI.skin.box);
        _guiContent = new GUIContent();

        if (Room.Map != null)
		{
			for (int i = 0; i < Room.Map.GetLength(0); i++)
			{
				for (int j = 0; j < Room.Map.GetLength(1); j++)
				{
					Vector3 worldPos = TileMap.CellToWorld(Room.Map[i, j]);
					Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
					screenPos.y = Screen.height - screenPos.y - 32;

					if (Room.Map [i, j].z == 0)
					{
                        _currentStyle.normal.background = _whiteTexture;
					}
					else
					{
                        _currentStyle.normal.background = _redTexture;
					}
					GUI.Box(new Rect(screenPos.x, screenPos.y, 32 - 2, 32 - 2), _guiContent, _currentStyle);
				}
			}
		}
	}

	private void DrawSepecifCoords()
	{
        _currentStyle = new GUIStyle(GUI.skin.box);
        _guiContent = new GUIContent();

        foreach (var specificCoord in Room.SpecifcCoords)
		{
			Vector3 worldPos = TileMap.CellToWorld(Room.Map[specificCoord.TileX, specificCoord.TileY]);
			Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
			screenPos.y = Screen.height - screenPos.y - 32;

            _currentStyle.normal.background = _blueTexture;
            _guiContent.text = specificCoord.Value.ToString();
			GUI.Box(new Rect(screenPos.x, screenPos.y, 32 - 2, 32 - 2), _guiContent, _currentStyle);
		}
	}

    private void DrawNonCollisionCoords()
    {
        _currentStyle = new GUIStyle(GUI.skin.box);
        _guiContent = new GUIContent();

        foreach (var createAbleCoord in Room.CreateAbleRegion)
        {
            Vector3 worldPos = TileMap.CellToWorld(Room.Map[createAbleCoord.TileX, createAbleCoord.TileY]);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            screenPos.y = Screen.height - screenPos.y - 32;

            _currentStyle.normal.background = _blueTexture;
            GUI.Box(new Rect(screenPos.x, screenPos.y, 32 - 2, 32 - 2), _guiContent, _currentStyle);
        }
    }

	private void OnGUI()
	{
		if (_regionDraw)
		{
			DrawGUIRegions();
		}
		else if (_specificCoordDraw)
		{
			DrawSepecifCoords();
		}else if (_createAbleDraw)
        {
            DrawNonCollisionCoords();
        }
	}
#endif
}
