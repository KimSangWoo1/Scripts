using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using System;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    MapSO _mapSO;
    [SerializeField]
    TileGenerator _tileGenerator;
    [SerializeField]
    RoomManager _roomManager;

    private ChpaterMapInfoSO _chpaterMapInfoSO;
    
    private List<Room> _rooms;
    private List<bool> _specialRoomCheckList;

    private Vector3Int[,] _map;
    private Vector2Int _currentPosition;
    
    private int _roomCount;
    private int _maxSteps = 10;

    [SerializeField]
    private bool _mapDraw;

    private void OnEnable()
    {
        _chpaterMapInfoSO = _mapSO.ChapterInfo[GameManager.Instance.Chatper - 1];
#if UNITY_EDITOR
        EventManager.Instance.Notify(EventType.AddKeyPress, new EventData.InputKeyData(eActionMapType.Dev, eDevKeyType.RandomMap.ToString(), AA));
#endif
    }
#if UNITY_EDITOR
    private void AA(InputAction.CallbackContext context)
    {
        GenerateMap();
    }
#endif

    // 맵 생성
    private async void GenerateMap()
    {
        List<RoomControl> roomControls = new List<RoomControl>();
        _rooms = new List<Room>();
        CheckSpecialAddRoom();
        Vector2Int startRoomIndex = GenerateRoom();
        MapFloodFlowDistance(startRoomIndex);

        Vector2Int bossRoomIndex = EssentialRoomTypeSetting();
        SpecialRoomSetting(startRoomIndex, bossRoomIndex);

        List<int> normalRoomList = EventRoomTypeSetting();
        MonsterRoomSetting(normalRoomList);

        for (int i = 0; i < _rooms.Count; i++)
        {
            _tileGenerator.TileGenerate(_rooms[i], _chpaterMapInfoSO.ChpaterRuleTiles, i); //타일 그리기
            Vector3 roomToCellCenter = _tileGenerator.Tilemap.CellToWorld(_tileGenerator.RoomCenter); // 좌표 셋팅
            _rooms[i].Center = roomToCellCenter;

            RoomControl roomControl = await CreateRoom(roomToCellCenter); // Room 객체 생성
            roomControl.Room = _rooms[i];
            roomControl.TileMap = _tileGenerator.Tilemap;
            roomControl.Room.GetCreateAbleRegion = _tileGenerator.GetNonCollisionRegions;
            roomControl.Room.Seed = (Time.time + i).ToString();
            roomControl.ChpaterMapInfoSO = _chpaterMapInfoSO;
            roomControl.DrawRoom = _tileGenerator.DrawTile;
            roomControl.ClearRoom = _tileGenerator.ClearTile;
            ConnectRoom(roomControl, i);

            if (roomControl.Room.RoomType == eRoomType.Event)
                await CreateSpecificObject(roomControl);
            // 장애물 생성
            roomControl.Room.CreateAbleRegion = _tileGenerator.GetNonCollisionRegions(roomControl.Room.Map,0);
            roomControl.Room.Props = await CreateProp(roomControl);
            roomControl.Room.Trabs = await CreateTrab(roomControl);
            roomControl.CreateMonster = CreateMonster;
            roomControl.CreateSpecificMonster = CreateSpecificMonster;
            roomControl.RoomReady(false);
            roomControls.Add(roomControl);
        }
        _roomManager.Rooms = roomControls;
        _roomManager.StartRoom();
    }
    
    // 전체 Map 거리 계산
    private void MapFloodFlowDistance(Vector2Int startRoomIndex)
    {
        int[,] mapFlags = new int[_map.GetLength(0), _map.GetLength(1)];

        Queue<TileGenerator.SpecificCoord> queue = new Queue<TileGenerator.SpecificCoord>();
        queue.Enqueue(new TileGenerator.SpecificCoord(startRoomIndex.x, startRoomIndex.y, 1)); // 시작방 z 값과 맞춰야 함
        mapFlags[startRoomIndex.x, startRoomIndex.y] = 1;

        while (queue.Count > 0)
        {
            TileGenerator.SpecificCoord tile = queue.Dequeue();
            int distance = tile.Value;
            for (int x = tile.TileX - 1; x <= tile.TileX + 1; x++)
            {
                for (int y = tile.TileY - 1; y <= tile.TileY + 1; y++)
                {
                    if ((x >= 0 && x < _map.GetLength(0) && y >= 0 && y < _map.GetLength(1)) && (y == tile.TileY || x == tile.TileX))
                    {
                        if (_map[x, y].z != 0)
                        {
                            if (mapFlags[x, y] == 0)
                            {
                                mapFlags[x, y] = 1;
                                _map[x, y].z = distance + 1;
                                queue.Enqueue(new TileGenerator.SpecificCoord(x, y, distance + 1));
                            }
                        }
                    }
                }
            }
        }
    }

    // Special방 확률에 따른 총 방 갯수 추가
    private void CheckSpecialAddRoom()
    {
        string seed = (GameManager.Instance.Chatper - 1 + Time.time).ToString();
        System.Random chapterRadom = new System.Random(seed.GetHashCode());
        
        _specialRoomCheckList = new List<bool>(_chpaterMapInfoSO.SpecialRoomInfos.Count);
        int addRoomCount = 0;

        for(int i=0; i<_chpaterMapInfoSO.SpecialRoomInfos.Count; i++)
        {
            SpecialRoomInfo specialRoomInfo = _chpaterMapInfoSO.SpecialRoomInfos[i];

            if (!specialRoomInfo.IsIncluded)
            {
                bool include = false;
                
                float r = chapterRadom.Next(0, 101) * (float)chapterRadom.NextDouble();
                if(r <= specialRoomInfo.RandomValue)
                {
                    include = true;
                    addRoomCount++;
                }
                _specialRoomCheckList.Add(include);
            }
            else
            {
                _specialRoomCheckList.Add(specialRoomInfo.IsIncluded);
            }
        }
        _roomCount += addRoomCount;
    }

    // 시작방, 보스 입장방 설정
    private Vector2Int EssentialRoomTypeSetting()
    {
        int maxDitance = 0;
        Vector2Int boosRoomIndex = new();
        for (int i = 0; i < _map.GetLength(0); i++)
        {
            for (int j = 0; j < _map.GetLength(1); j++)
            {
                if (_map[i, j].z == 1)
                {
                    var startRoom = _rooms.FirstOrDefault(r => r.Pos == (Vector2Int)_map[i, j]);
                    if (startRoom != null)
                    {
                        startRoom.RoomType = eRoomType.Event;
                        startRoom.EventRoomType = eEventRoomType.Start;
                    }
                }
                else if(_map[i, j].z >= 2)
                {
                    int compareDistance = _map[i, j].z;
                    if (maxDitance < compareDistance)
                    {
                        maxDitance = compareDistance;
                        boosRoomIndex.x = i;
                        boosRoomIndex.y = j;
                    }
                }
            }
        }

        var bossRoom = _rooms.FirstOrDefault(r => r.Pos == (Vector2Int)_map[boosRoomIndex.x, boosRoomIndex.y]);
        if (bossRoom != null)
        {
            bossRoom.RoomType = eRoomType.Event;
            bossRoom.EventRoomType = eEventRoomType.BossEntrance;
        }
        return boosRoomIndex;
    }

    // 스페셜방 설정
    private void SpecialRoomSetting(Vector2Int startRoomIndex, Vector2Int bossRoomIndex)
    {
        for(int i=0; i < _chpaterMapInfoSO.SpecialRoomInfos.Count; i++)
        {
            var specialRoomInfo = _chpaterMapInfoSO.SpecialRoomInfos[i];
            Room specialRoom = null;
            // 이번 챕터에 방이 포함되어 있는지 여부
            if (_specialRoomCheckList[i])
            {
                if(specialRoomInfo.CriteriaRoomType == eEventRoomType.Start)
                {
                    specialRoom = SelectFloodFlowRoom(startRoomIndex, specialRoomInfo.Distance);
                }
                else if(specialRoomInfo.CriteriaRoomType == eEventRoomType.BossEntrance)
                {
                    specialRoom = SelectFloodFlowRoom(bossRoomIndex, specialRoomInfo.Distance);
                }

                if(specialRoom != null)
                {
                    specialRoom.RoomType = eRoomType.Special;
                    specialRoom.SpecialRoomType = specialRoomInfo.SpecialRoomType;
                    specialRoom.monsterRoomType = specialRoomInfo.monsterRoomType;
                }
            }
        }
    }

    // 나머지 이벤트방 설정
    private List<int> EventRoomTypeSetting()
    {
        List<int> normalRoomsList = new List<int>();
        for(int i=0; i<_rooms.Count; i++)
        {
            if(_rooms[i].RoomType == eRoomType.Normal)
            {
                normalRoomsList.Add(i);
            }
        }
        //시작방, 보스 입장방 2개가 무조건 있어야 한다.
        List<int> selectNumberList = SelectRandomNumbers(normalRoomsList, _chpaterMapInfoSO.EventRooms.Count - 2);
        for (int j = 0,  eventIndex = 2; j < selectNumberList.Count; j++, eventIndex++)
        {
            int index = selectNumberList[j];
            _rooms[index].RoomType = eRoomType.Event;
            _rooms[index].EventRoomType = _chpaterMapInfoSO.EventRooms[eventIndex].EventRoomType;
        }
        return normalRoomsList;
    }

    // 몬스터 방 설정
    private void MonsterRoomSetting(List<int> normalRoomList)
    {
        List<int> monsterRoomsIndex = new List<int>();
        for(int i=0; i< _chpaterMapInfoSO.MonsterRooms.Count; i++)
        {
            for(int j =0; j< _chpaterMapInfoSO.MonsterRooms[i].RoomCreateCount; j++)
            {
                monsterRoomsIndex.Add(i);
            }
        }

        var randomRoomsList = SelectRandomNumbers(monsterRoomsIndex, monsterRoomsIndex.Count);
        for(int i=0; i<normalRoomList.Count; i++)
        {
            var monsterRoom = _rooms[normalRoomList[i]];
            monsterRoom.monsterRoomType = _chpaterMapInfoSO.MonsterRooms[randomRoomsList[0]].monsterRoomType;
            randomRoomsList.RemoveAt(0);

            if (randomRoomsList.Count == 0)
            {
                randomRoomsList = Enumerable.Range(0, _chpaterMapInfoSO.MonsterRooms.Count).ToList();
            }
        }
        randomRoomsList.Clear();
        monsterRoomsIndex.Clear();
    }

    // 랜덤 Index 뽑기
    private List<int> SelectRandomNumbers(List<int> numbers, int count)
    {
        System.Random random = new System.Random();
        List<int> selectedNumbers = new List<int>();

        for (int i = 0; i < count; i++)
        {
            // numbers 리스트에서 임의의 인덱스를 선택
            int randomIndex = random.Next(0, numbers.Count);

            // 선택된 숫자를 결과 리스트에 추가하고 numbers 리스트에서 제거
            selectedNumbers.Add(numbers[randomIndex]);
            numbers.RemoveAt(randomIndex);
        }
        return selectedNumbers;
    }

    // 특수 지점 찾기 (홍수 알고리즘 이용)
    private Room SelectFloodFlowRoom(Vector2Int startPos, int maxDistance)
    {
        int[,] mapFlags = new int[_map.GetLength(0), _map.GetLength(1)];

        Queue<TileGenerator.SpecificCoord> queue = new Queue<TileGenerator.SpecificCoord>();
        queue.Enqueue(new TileGenerator.SpecificCoord(startPos.x, startPos.y, 0));
        mapFlags[startPos.x, startPos.y] = 1;
        
        Room room = null;
        while (queue.Count > 0)
        {
            TileGenerator.SpecificCoord tile = queue.Dequeue();
            int distance = tile.Value;
            for (int x = tile.TileX - 1; x <= tile.TileX + 1; x++)
            {
                for (int y = tile.TileY - 1; y <= tile.TileY + 1; y++)
                {
                    if ((x >= 0 && x < _map.GetLength(0) && y >= 0 && y < _map.GetLength(1)) && (y == tile.TileY || x == tile.TileX))
                    {
                        if (mapFlags[x, y] == 0)
                        {
                            if (distance + 1 >= maxDistance)
                            {
                                room = _rooms.FirstOrDefault(r => r.Pos == (Vector2Int)_map[x, y]);
                                if(room.RoomType == eRoomType.Normal)
                                {
                                    return room;
                                }
                            }
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new TileGenerator.SpecificCoord(x, y, distance + 1));

                        }
                    }
                }
            }
        }
        return room;
    }

    #region  Room
    // Drunked Walk 알고리즘 방 생성
    private Vector2Int GenerateRoom()
    {
        _roomCount += _chpaterMapInfoSO.RoomCount;
        _currentPosition = new Vector2Int(0, 0); // 시작 지점은 그리드 중앙
        _maxSteps = _roomCount * 10;

        List<Vector2Int> roomTiles = new List<Vector2Int>();
        roomTiles.Add(_currentPosition);

        int xMin = 0;
        int xMax = 0;
        int yMin = 0;
        int yMax = 0;

        while (roomTiles.Count < _roomCount)
        {
            // 무작위 방향 선택 (상, 하, 좌, 우)
            int randomDirection = UnityEngine.Random.Range(0, 4);

            // 선택된 방향으로 이동
            switch (randomDirection)
            {
                case 0:
                    _currentPosition += Vector2Int.up;
                    break;
                case 1:
                    _currentPosition += Vector2Int.down;
                    break;
                case 2:
                    _currentPosition += Vector2Int.left;
                    break;
                case 3:
                    _currentPosition += Vector2Int.right;
                    break;
            }

            if (!roomTiles.Contains(_currentPosition))
            {
                roomTiles.Add(_currentPosition);
            }

            // Map 크기 구하기
            if (xMin > _currentPosition.x)
            {
                xMin = _currentPosition.x;
            }
            else if (xMax < _currentPosition.x)
            {
                xMax = _currentPosition.x;
            }
            if (yMin > _currentPosition.y)
            {
                yMin = _currentPosition.y;
            }
            else if (yMax < _currentPosition.y)
            {
                yMax = _currentPosition.y;
            }
        }
        int xSzie = Math.Abs(xMin) + xMax + 1; // 0 이 들어가기 때문
        int ySize = Math.Abs(yMin) + yMax + 1;

        Vector2Int startRoomIndex = new();
        _map = new Vector3Int[xSzie, ySize];

        for (int i = 0; i < roomTiles.Count; i++)
        {
            Room room = new Room();
            room.Pos = roomTiles[i];
            _rooms.Add(room);

            int indexX = roomTiles[i].x - xMin;
            int indexY = roomTiles[i].y - yMin;
            _map[indexX, indexY] = new Vector3Int(roomTiles[i].x, roomTiles[i].y, 2);

            if (roomTiles[i].x == 0 && roomTiles[i].y == 0)
            {
                startRoomIndex.x = indexX;
                startRoomIndex.y = indexY;
                _map[startRoomIndex.x, startRoomIndex.y] = new Vector3Int(roomTiles[i].x, roomTiles[i].y, 1);
            }
        }
        return startRoomIndex;
    }

    // 방 객체 생성
    private async UniTask<RoomControl> CreateRoom(Vector3 roomCenter)
    {
        var room = await AddressableManager.Instance.InstanceObject<GameObject>(_chpaterMapInfoSO.ChapterRoom.name.ToString());
        //var room = Instantiate(_chpaterMapInfoSO.ChapterRoom);
        room.transform.position = roomCenter;
        RoomControl roomControl = room.GetComponent<RoomControl>();
        return roomControl;
    }

    // 방 to 방 연결
    private void ConnectRoom(RoomControl roomControl, int index)
    {
        int connectCount = 0;
        for (int i = 0; i < _rooms.Count; i++)
        {
            if (connectCount >= 4) break;

            if (i != index)
            {
                var directPos = roomControl.Room.Pos;
                var targetPos = _rooms[i].Pos;

                if ((directPos + Vector2Int.up) == targetPos)
                {
                    roomControl.IsConnectTop = true;
                    connectCount++;
                }
                else if ((directPos + Vector2Int.down) == targetPos)
                {
                    roomControl.IsConnectBottom = true;
                    connectCount++;
                }
                else if ((directPos + Vector2Int.left) == targetPos)
                {
                    roomControl.IsConnectLeft = true;
                    connectCount++;
                }
                else if ((directPos + Vector2Int.right) == targetPos)
                {
                    roomControl.IsConnectRight = true;
                    connectCount++;
                }
            }
        }
    }
    #endregion

    #region  UniTask Create
    private async UniTask CreateSpecificObject(RoomControl roomControl)
    {
        for (int j = 0; j < _chpaterMapInfoSO.EventRooms.Count; j++)
        {
            if (roomControl.Room.EventRoomType == _chpaterMapInfoSO.EventRooms[j].EventRoomType)
            {
                var eventRoomInfo = _chpaterMapInfoSO.EventRooms[j];
                List<GameObject> npcs = await CreateNpc(eventRoomInfo.Npcs, roomControl.transform, roomControl.Room.Center);
                List<GameObject> props = await CreateEventRoomProp(eventRoomInfo.Props, roomControl.transform, roomControl.Room.Center);
                roomControl.Room.Npcs = npcs;
                roomControl.Room.SpecificProps = props;
                return;
            }
        }
    }

    private async UniTask<List<GameObject>> CreateNpc(List<GameObject> npcs, Transform parent, Vector3 roomCenter)
    {
        List<GameObject> npcObjects = new List<GameObject>(npcs.Count);
        for (int i = 0; i < npcs.Count; i++)
        {
            var npc = await AddressableManager.Instance.InstanceObject<GameObject>(npcs[i].name.ToString(), parent);
            //npc.transform.position += roomCenter;
            npc.transform.parent = parent;
            npcObjects.Add(npc);
        }
        return npcObjects;
    }

    private async UniTask<List<GameObject>> CreateTrab(RoomControl roomControl)
    {
        System.Random pseudoRandom = new System.Random(roomControl.Room.Seed.GetHashCode());
        int trabCreateCount = pseudoRandom.Next(_chpaterMapInfoSO.ChapterRoomInfo.TrapCostMin, _chpaterMapInfoSO.ChapterRoomInfo.TrapCostMax + 1);

        List<GameObject> trabs = new List<GameObject>(trabCreateCount);
        for (int i = 0; i < trabCreateCount; i++)
        {
            int trabIndex = pseudoRandom.Next(0, _chpaterMapInfoSO.Trabs.Count);
            var trab = await AddressableManager.Instance.InstanceObject<GameObject>(_chpaterMapInfoSO.Trabs[trabIndex].name.ToString(), roomControl.transform);

            int posIndex = pseudoRandom.Next(0, roomControl.Room.CreateAbleRegion.Count);
            TileGenerator.Coord coord = roomControl.Room.GetCreatePos(posIndex);
            trab.transform.position = _tileGenerator.Tilemap.CellToWorld(roomControl.Room.Map[coord.TileX, coord.TileY]);
            trabs.Add(trab);
        }
        return trabs;
    }

    private async UniTask<List<GameObject>> CreateProp(RoomControl roomControl)
    {
        System.Random pseudoRandom = new System.Random(roomControl.Room.Seed.GetHashCode());
        int propCreateCount = pseudoRandom.Next(_chpaterMapInfoSO.ChapterRoomInfo.PropCostMin, _chpaterMapInfoSO.ChapterRoomInfo.PropCostMax + 1);

        List<GameObject> props = new List<GameObject>(propCreateCount);
        for (int i = 0; i < propCreateCount; i++)
        {
            int propIndex = pseudoRandom.Next(0, _chpaterMapInfoSO.Props.Count);
            var prop = await AddressableManager.Instance.InstanceObject<GameObject>(_chpaterMapInfoSO.Props[propIndex].name.ToString(), roomControl.transform);

            int posIndex = pseudoRandom.Next(0, roomControl.Room.CreateAbleRegion.Count);
            TileGenerator.Coord coord = roomControl.Room.GetCreatePos(posIndex);
            prop.transform.position = _tileGenerator.Tilemap.CellToWorld(roomControl.Room.Map[coord.TileX, coord.TileY]);
            props.Add(prop);
        }
        return props;
    }

    private async UniTask<List<GameObject>> CreateEventRoomProp(List<GameObject> props, Transform parent, Vector3 roomCenter)
    {
        List<GameObject> propObjects = new List<GameObject>(props.Count);
        for (int i = 0; i < props.Count; i++)
        {
            var prop = await AddressableManager.Instance.InstanceObject<GameObject>(props[i].name.ToString(), parent);
            //prop.transform.position += roomCenter;
            prop.transform.parent = parent;
            propObjects.Add(prop);
        }
        return propObjects;
    }

    private async UniTask<List<GameObject>> CreateMonster(RoomControl roomControl)
    {
        if (roomControl.Room.monsterRoomType == eMonsterRoomType.None) 
            return null;

        System.Random pseudoRandom = new System.Random(roomControl.Room.Seed.GetHashCode());
        int monsterCreateCount = pseudoRandom.Next(_chpaterMapInfoSO.ChapterRoomInfo.MonsterCostMin, _chpaterMapInfoSO.ChapterRoomInfo.MonsterCostMax + 1);

        List<GameObject> monsters = new List<GameObject>(monsterCreateCount);
        List<GameObject> roomMonstersInfo = _chpaterMapInfoSO.GetMonsters(roomControl.Room.monsterRoomType);
        for(int i = 0; i < monsterCreateCount; i++)
        {
            int monsterIndex = pseudoRandom.Next(0, roomMonstersInfo.Count);
            var monster = await AddressableManager.Instance.InstanceObject<GameObject>(roomMonstersInfo[monsterIndex].name.ToString(), roomControl.transform);

            int posIndex = pseudoRandom.Next(0, roomControl.Room.CreateAbleRegion.Count);
            TileGenerator.Coord coord = roomControl.Room.GetCreatePos(posIndex);
            monster.transform.position = _tileGenerator.Tilemap.CellToWorld(_tileGenerator.Map[coord.TileX, coord.TileY]);
            monsters.Add(monster);
        }
        return monsters;
    }

    private async UniTask<List<GameObject>> CreateSpecificMonster(RoomControl roomControl)
    {
        System.Random pseudoRandom = new System.Random(roomControl.Room.Seed.GetHashCode());

        var specialRoomInfo = _chpaterMapInfoSO.SpecialRoomInfos.FirstOrDefault(r => r.SpecialRoomType == roomControl.Room.SpecialRoomType);
        List<GameObject> monsters = new List<GameObject>();

        for (int i = 0; i < specialRoomInfo.SpecialMonsters.Count; i++)
        {
            for(int j=0; j< specialRoomInfo.SpecialMonsters[i].Count; j++)
            {
                var monster = await AddressableManager.Instance.InstanceObject<GameObject>(specialRoomInfo.SpecialMonsters[j].Monster.name.ToString(), roomControl.transform);

                int posIndex = pseudoRandom.Next(0, roomControl.Room.CreateAbleRegion.Count);
                TileGenerator.Coord coord = roomControl.Room.GetCreatePos(posIndex);
                monster.transform.position = _tileGenerator.Tilemap.CellToWorld(_tileGenerator.Map[coord.TileX, coord.TileY]);
                monsters.Add(monster);
            }
        }
        return monsters;
    }
    #endregion

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (_mapDraw)
        {
            DrawGUIRegions();
        }
    }

    private void DrawGUIRegions()
    {
        GUIStyle _currentStyle = new GUIStyle(GUI.skin.box);
        GUIContent _guiContent = new GUIContent();

        if (_map != null)
        {
            for (int i = 0; i < _map.GetLength(0); i++)
            {
                for (int j = 0; j < _map.GetLength(1); j++)
                {
                    Vector3 worldPos = _map[i, j];
                    Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
                    screenPos.y = Screen.height - screenPos.y - 32;

                    if (_map[i, j].z == 0)
                    {
                        _currentStyle.normal.background = Texture2D.whiteTexture;
                    }
                    else
                    {
                        _currentStyle.normal.background = Texture2D.grayTexture;
                    }
                    _guiContent.text = _map[i, j].z.ToString();
                    GUI.Box(new Rect(screenPos.x, screenPos.y, 32 - 2, 32 - 2), _guiContent, _currentStyle);
                }
            }
        }
    }
#endif
}


[System.Serializable]
public class Room
{
    public eRoomType RoomType;
    public eMonsterRoomType monsterRoomType;
    public eEventRoomType EventRoomType;
    public eSpecialRoomType SpecialRoomType;

    public Vector2Int Pos;
    public Vector3 Center;
    public string Seed;
    public Vector3Int[,] Map;
    public List<TileGenerator.SpecificCoord> SpecifcCoords;
    public List<TileGenerator.Coord> CreateAbleRegion;
    public List<GameObject> Npcs;
    public List<GameObject> Trabs;
    public List<GameObject> Props;
    public List<GameObject> SpecificProps;
    public List<GameObject> Monsters;
    public List<GameObject> SpecificMonsters;

    public Func<Vector3Int[,], int, List<TileGenerator.Coord>> GetCreateAbleRegion;

    public TileGenerator.Coord GetCreatePos(int index)
    {
        TileGenerator.Coord coord = CreateAbleRegion[index];
        RemoveArea(index);
        return coord;
    }

    private void RemoveArea(int index)
    {
        var coord = CreateAbleRegion[index];
        for (int x = coord.TileX - 1; x<= coord.TileX; x++)
        {
            for (int y = coord.TileY-1; y <= coord.TileY; y++)
            {
                if(x>=0 && y>=0)
                {
                    TileGenerator.Coord neighbourCoord = coord;
                    neighbourCoord.TileX = x;
                    neighbourCoord.TileY = y;
                    CreateAbleRegion.Remove(neighbourCoord);
                }
            }
        }
    }
}