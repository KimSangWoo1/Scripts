using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class TileGenerator : MonoBehaviour
{
	[SerializeField]
	Tilemap _tilemap;
	[SerializeField]
	Tilemap _notWalkTilemap;
	RuleTile[] _ruleTiles;

	[Header("Smooth Map")]
	[SerializeField]
	private int _smoothCount;
	[SerializeField]
	private int _waterThresholdSize;
	[SerializeField]
	private int _walkThresholdSize;

	[Header("Random")]
	[SerializeField]
	private string seed;
	[SerializeField]
	private bool useRandomSeed;
	[SerializeField, Range(0, 100)]
	private int randomFillPercent;

	private List<SpecificCoord> _specificCoords = new List<SpecificCoord>();
	private Vector3Int[,] _map;
	private BoundsInt _roomSizeBound;
	private BoundsInt _roomDrawBound; // TileMap Cell 기준 값 
	private BoundsInt _tileMapDrawBound; // _map[,] 기준 값  _roomDrawBound와 의미 동일

	[SerializeField]
	private int _roomPadding;

	private int _roomWidth;
	private int _roomHeight;
	private int _roomNumber;

	//RoomSize에서 Offset에 따른 WalkAble 고정 영역 정해주기
	private const int _offsetL = 8;
	private const int _offsetR = 8;
	private const int _offsetT = 6;
	private const int _offsetB = 6;

	//맵 (방향별)타일 갯수 
	private const int XMin = -25;
	private const int XMax = 25;
	private const int YMin = -11;
	private const int YMax = 11;

	// RoomSize안에 CreateAble 영역 경계 정해주기 (벽 안)
	private const int OffsetMinL = 4;
	private const int OffsetMinR = 4;
	private const int OffsetMinT = 4;
	private const int OffsetMinB = 5;

	public Tilemap Tilemap => _tilemap;
	public Vector3Int RoomCenter => new Vector3Int((int)_roomSizeBound.center.x, (int)_roomSizeBound.center.y, (int)_roomSizeBound.center.z);
	public Vector3Int[,] Map  { get { return _map; } set { _map = value; } }
	public Vector2Int RoomSize { get; set; }
	public int RoomWidth => _roomWidth;
	public int RoomHeight => _roomHeight;

	public struct SpecificCoord
	{
		public int TileX;
		public int TileY;
		public int Value;
		public SpecificCoord(int x, int y, int count)
		{
			TileX = x;
			TileY = y;
			Value = count;
		}
	}

	public struct Coord
	{
		public int TileX;
		public int TileY;

		public Coord(int x, int y)
		{
			TileX = x;
			TileY = y;
		}
	}

    public void Init()
	{
		_roomWidth = Mathf.Abs(XMin) + Mathf.Abs(XMax);
		_roomHeight = Mathf.Abs(YMin) + Mathf.Abs(YMax);
		RoomSize = new Vector2Int(_roomWidth+1, _roomHeight+1); // -1, 0 ,1  0이 포함되기 때문
	}

	public void TileGenerate(Room room, RuleTile[] ruleTiles, int roomNumber)
	{
		_ruleTiles = ruleTiles;
		_roomNumber = roomNumber;

		_specificCoords = new List<SpecificCoord>();
		room.SpecifcCoords = _specificCoords;
		
		_map = new Vector3Int[RoomSize.x, RoomSize.y];
		room.Map = _map;
		SetRoomSize(room.Pos.x,room.Pos.y);

		if(room.RoomType == eRoomType.Normal)
        {
			ProceduralGenerate();
		}
        else
        {
			SimpleGenerate();
        }
	}

	// 규격에 맞게 절차없이 맵 생성
	private void SimpleGenerate()
    {
		GroundFillMap();
		//DrawTile();
	}

	// 규격에 맞게 절차적으로 Tile 그리며 맵 생성
	private void ProceduralGenerate()
    {
		RandomFillMap();

		for(int i=1; i<=_smoothCount; i++)
        {
			SmoothMap();
        }
		ProcessMap();
		SmoothTile();
		//DrawTile();
	}

	// 방 사이즈 설정
	private void SetRoomSize(int posX, int posY)
	{
		int signX = 0;
		int signY = 0;

        if (_roomNumber != 0)
        {
            if (posX != 0)
            {
				signX = XMin + (RoomSize.x * posX) > 0 ? 1 : -1;
				signX *= Mathf.Abs(posX);
			}
            if (posY != 0)
            {
				signY = YMin + (RoomSize.y * posY) > 0 ? 1 : -1;
				signY *= Mathf.Abs(posY);
			}
        }
		
		_roomSizeBound.xMin = XMin + (RoomSize.x * posX) + (signX * _roomPadding);  //Room Size Boundary for TileMap Grid
		_roomSizeBound.xMax = XMax + (RoomSize.x * posX) + (signX * _roomPadding);

		_roomSizeBound.yMin = YMin + (RoomSize.y * posY) + (signY * _roomPadding);
		_roomSizeBound.yMax = YMax + (RoomSize.y * posY) + (signY * _roomPadding);

		_roomDrawBound.xMin = _roomSizeBound.xMin + _offsetL; // Room Draw Boundary for TileMap Grid
		_roomDrawBound.xMax = _roomSizeBound.xMax - _offsetR;
		_roomDrawBound.yMin = _roomSizeBound.yMin + _offsetB;
		_roomDrawBound.yMax = _roomSizeBound.yMax - _offsetT;

		_tileMapDrawBound.xMin = _offsetL;    // Room Draw Boundary for [,] 
		_tileMapDrawBound.xMax = _map.GetLength(0) - _offsetR;
		_tileMapDrawBound.yMin = _offsetT;
		_tileMapDrawBound.yMax = _map.GetLength(1) - _offsetB;
	}
	
	// 물 타일 리소스 특성상 이상한 곳 주변 물 타일 채우기
	private void SmoothTile()
    {
		foreach (var specificCoord in _specificCoords)
        {
			bool done = false;
			for (int neighbourX = specificCoord.TileX - 1; neighbourX <= specificCoord.TileX + 1; neighbourX++)
			{
				for (int neighbourY = specificCoord.TileY - 1; neighbourY <= specificCoord.TileY + 1; neighbourY++)
				{
                    if (IsOutTileMapDrawBound(neighbourX, neighbourY))
                    {
						continue;
                    }

					if (neighbourX != specificCoord.TileX || neighbourY != specificCoord.TileY)
					{
                        if (_map[neighbourX, neighbourY].z == 1)
                        {
							_map[specificCoord.TileX, specificCoord.TileY].z = int.MaxValue;
							int connectWallCount = FindConnectedWallCount(neighbourX, neighbourY, specificCoord.TileX - 1, specificCoord.TileY - 1, specificCoord.TileX + 1, specificCoord.TileY + 1);
							if (specificCoord.Value > connectWallCount)
                            {
								FillWall3X3(specificCoord.TileX, specificCoord.TileY);
								done = true;
								break;
                            }
                            else
                            {
								done = true;
							}
						}
					}
				}
				_map[specificCoord.TileX, specificCoord.TileY].z = 1;
				if (done) break;
			}
		}
	}

	//홍수 흐름 알고리즘 (전부 찾을 필요 없음 Wall이 끉어졌을때 WallCount 값과 비교만 하면 되기 때문)
	private int FindConnectedWallCount(int gridX, int gridY, int minX, int minY, int maxX, int maxY)
    {
		Queue<Coord> wallQue = new Queue<Coord>();
		bool[,] visited = new bool[3,3];
		int connectWallCount = 1;

		wallQue.Enqueue(new Coord(gridX, gridY));
		while (wallQue.Count > 0)
        {
			var wallTile = wallQue.Dequeue();
			visited[maxX - wallTile.TileX, maxY - wallTile.TileY] = true;
			for (int x = wallTile.TileX - 1; x <= wallTile.TileX + 1; x++)
			{
				for (int y = wallTile.TileY - 1; y <= wallTile.TileY + 1; y++)
				{
                    if (x< minX || x>maxX || y < minY || y >maxY)
                    {
						continue;
                    }
					if (IsOutTileMapDrawBound(x, y))
					{
						continue;
					}

					if (y == wallTile.TileY || x == wallTile.TileX)
                    {
						if (_map[x, y].z == 1 && !visited[maxX - x, maxY - y])
                        {
							wallQue.Enqueue(new Coord(x, y));
							connectWallCount++;
						}
					}
				}
			}
        }
		return connectWallCount;
    }

	// Tile 그리기
	public void DrawTile(Vector3Int[,] map = null)
	{
		if (map != null) 
			_map = map;
		
		for (int x = 0; x < _map.GetLength(0); x++)
		{
			for (int y = 0; y < _map.GetLength(1); y++)
			{
                if (IsOutTileMapDrawBound(x, y))
                {
					//_tilemap.SetTile(_map[x, y], _ruleTiles[0]);
					_notWalkTilemap.SetTile(_map[x, y], _ruleTiles[0]);
				}
                else
                {
					if (_map[x, y].z == 0)
					{
						_tilemap.SetTile(_map[x, y], _ruleTiles[_map[x, y].z]);
					}
					else
					{
						_notWalkTilemap.SetTile(_map[x, y], _ruleTiles[_map[x, y].z]);

					}
				}
			}
		}
	}

	// Tile 지우기
	public void ClearTile(Vector3Int[,] map = null)
	{
		if (map != null)
			_map = map;

		for (int x = 0; x < _map.GetLength(0); x++)
		{
			for (int y = 0; y < _map.GetLength(1); y++)
			{

				_tilemap.SetTile(_map[x, y], null);
				_tilemap.RefreshTile(_map[x, y]);

				_notWalkTilemap.SetTile(_map[x, y], null);
				_notWalkTilemap.RefreshTile(_map[x, y]);

			}
		}
	}

	// 규격에 맞게 땅 Tile로만 채움
	private void GroundFillMap()
    {
		for (int x = 0, tileX = _roomSizeBound.xMin; tileX <= _roomSizeBound.xMax; x++, tileX++)
		{
			for (int y = 0, tileY = _roomSizeBound.yMin; tileY <= _roomSizeBound.yMax; y++, tileY++)
			{
				_map[x, y] = new Vector3Int(tileX, tileY, 0);
			}
		}
	}

	#region Cellular Automata - Cave

	// Cellular Automata - Cave 공통 랜덤 맵 생성
	private void RandomFillMap()
	{
		System.Random pseudoRandom = new System.Random(seed.GetHashCode() + _roomNumber);

		if (useRandomSeed)
		{
			seed = Time.time.ToString();
		}

		for (int x = 0,  tileX = _roomSizeBound.xMin; tileX <= _roomSizeBound.xMax; x++ , tileX++)
		{
			for (int y = 0, tileY = _roomSizeBound.yMin; tileY <= _roomSizeBound.yMax; y++, tileY++)
			{
				if (tileX <= _roomDrawBound.xMin || tileX >= _roomDrawBound.xMax || tileY <= _roomDrawBound.yMin || tileY >= _roomDrawBound.yMax)
				{
					_map[x, y] = new Vector3Int(tileX, tileY, 0);
				}
				else
				{
					int z = pseudoRandom.Next(0, 100) < randomFillPercent ? 1 : 0;

                    if (z==0)
                    {
						if(_map[x, y].z != 1)
                        {
							_map[x, y] = new Vector3Int(tileX, tileY, z);
						}
					}
					else
                    {
						_map[x, y] = new Vector3Int(tileX, tileY, z);
						//RandomFillRactangle(x, y, tileX, tileY, 1);
					}
				}
			}
		}
	}

	// Cellular Automata Smooth
	private void SmoothMap()
	{
		for (int x = 0; x < _map.GetLength(0); x++)
		{
			for (int y = 0; y < _map.GetLength(1); y++)
			{
				if (IsOutTileMapDrawBound(x,y))
                {
					continue;
                }

				int neighbourWallTiles = GetSurroundingWallCount(x, y);
				if (neighbourWallTiles > 4)
                {
					_map[x, y].z = 1;
				}
				else if (neighbourWallTiles < 4)
					_map[x, y].z = 0;
			}
		}
	}

	// 주위 벽 갯수 체크
	int GetSurroundingWallCount(int gridX, int gridY, bool isOutSideAdd = false)
	{
		int wallCount = 0;
		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
		{
			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
			{
				if (IsInRoomRange(neighbourX, neighbourY))
				{
					if (neighbourX != gridX || neighbourY != gridY)
					{
						wallCount += _map[neighbourX, neighbourY].z;
					}
				}
				else
				{
					if(isOutSideAdd)
						wallCount++;
				}
			}
		}
		return wallCount;
	}

	#region Regions Control - Flood flow Algorithm
	// Regions에 타일 갯수가 작으면 없애기
	void ProcessMap()
	{
		List<List<Coord>> wallRegions = GetRegions(1);
		List<List<Coord>> remainRegions = new();
		foreach (List<Coord> wallRegion in wallRegions)
		{
			if (wallRegion.Count < _waterThresholdSize)
			{
				foreach (Coord tile in wallRegion)
				{
					_map[tile.TileX, tile.TileY].z = 0;
				}
            }
            else
            {
				remainRegions.Add(wallRegion);
			}
		}

		List<List<Coord>> roomRegions = GetRegions(0);

		foreach (List<Coord> roomRegion in roomRegions)
		{
			if (roomRegion.Count < _walkThresholdSize)
			{
				foreach (Coord tile in roomRegion)
				{
					_map[tile.TileX, tile.TileY].z = 1;
				}
			}
		}

		foreach (List<Coord> remainRegion in remainRegions)
		{
			foreach (Coord tile in remainRegion)
			{
				int wallCount = GetSurroundingWallCount(tile.TileX, tile.TileY, true);
				if (_map[tile.TileX, tile.TileY].z == 1 && wallCount >= 4 && wallCount < 8)
				{
					_specificCoords.Add(new SpecificCoord(tile.TileX, tile.TileY, wallCount));
				}else if (wallCount<=2)
                {
					FillWall3X3(tile.TileX, tile.TileY);
                }
			}
		}
	}

	// 같은 타일 Regions 반환
	List<List<Coord>> GetRegions(int tileType)
	{
		List<List<Coord>> regions = new List<List<Coord>>();
		int[,] mapFlags = new int[RoomSize.x, RoomSize.y];

		for (int x = 0; x < RoomSize.x; x++)
		{
			for (int y = 0; y < RoomSize.y; y++)
			{
				if (mapFlags[x, y] == 0 && _map[x, y].z == tileType)
				{
					List<Coord> newRegion = GetRegionTiles(x, y);
					regions.Add(newRegion);

					foreach (Coord tile in newRegion)
					{
						mapFlags[tile.TileX, tile.TileY] = 1;
					}
				}
			}
		}
		return regions;
	}

	// Flood flow (홍수 흐름 알고리즘)
	List<Coord> GetRegionTiles(int startX, int startY)
	{
		List<Coord> tiles = new List<Coord>();
		int[,] mapFlags = new int[RoomSize.x, RoomSize.y];
		int tileType = _map[startX, startY].z;

		Queue<Coord> queue = new Queue<Coord>();
		queue.Enqueue(new Coord(startX, startY));
		mapFlags[startX, startY] = 1;

		while (queue.Count > 0)
		{
			Coord tile = queue.Dequeue();
			int connectCount = 0;
			for (int x = tile.TileX - 1; x <= tile.TileX + 1; x++)
			{
				for (int y = tile.TileY - 1; y <= tile.TileY + 1; y++)
				{
					if (IsInRoomRange(x, y) && (y == tile.TileY || x == tile.TileX))
					{
						if (mapFlags[x, y] == 0 && _map[x, y].z == tileType)
						{
							mapFlags[x, y] = 1;
							queue.Enqueue(new Coord(x, y));
						}

						if (_map[x, y].z == tileType)
						{
							connectCount++;
						}
					}
				}
			}

			// 리소스 타일 특성 때문에 Rule Tile에 위반됨 
			if(tileType == 1 && connectCount <= 2)
            {
				_map[tile.TileX, tile.TileY].z = 0;
            }
            else
            {
				tiles.Add(tile);
			}
		}
		return tiles;
	}

	// 같은 타일 Regions 반환
	public List<Coord> GetNonCollisionRegions(Vector3Int[,] map,int tileType)
	{
		List<Coord> region = new List<Coord>();
		_map = map;

		for (int x = 0; x < RoomSize.x; x++)
		{
			for (int y = 0; y < RoomSize.y; y++)
			{
				var CellPos = _map[x, y];
				if (_map[x, y].z == tileType)
				{
                    if (IsInWallRnage(x, y))
                    {
						var CellToWorldPos = _tilemap.CellToWorld(CellPos);
						var hit = Physics2D.BoxCast(CellToWorldPos, Vector2.one, 0f, Vector2.zero, 1f, LayerMask.GetMask("Default") | LayerMask.GetMask("Map"));
						if (hit.transform == null)
						{
							region.Add(new Coord(x, y));
						}
					}
                }
			}
		}
		return region;
	}

	bool IsInRoomRange(int x, int y)
	{
		return x >= 0 && x < RoomSize.x && y >= 0 && y < RoomSize.y;
	}

	bool IsOutTileMapDrawBound(int x, int y)
    {
		return x <= _tileMapDrawBound.xMin || x >= _tileMapDrawBound.xMax || y <= _tileMapDrawBound.yMin || y >= _tileMapDrawBound.yMax;
	}

	bool IsInWallRnage(int x, int y)
    {
		return x >= OffsetMinL && x < RoomSize.x - OffsetMinR && y >= OffsetMinB && y < RoomSize.y - OffsetMinT;
	}

	#endregion
	#endregion

	// 물 타일 그리기 3x3 (리소스 및 타일 규칙 제한 때문)
	private void FillWall3X3(int x, int y)
    {
		for (int neighbourX = x - 1; neighbourX <= x + 1; neighbourX++)
		{
			for (int neighbourY = y - 1; neighbourY <= y + 1; neighbourY++)
			{
				if (!IsOutTileMapDrawBound(neighbourX, neighbourY))
				{
					_map[neighbourX, neighbourY].z = 1;
				}
			}
		}
	}

	// 물 타일 단위 그리기 2x2
	private void RandomFillRactangle(int x , int y, int tileX, int tileY, int tileType)
    {
		int midX = _roomWidth / 2;
		int midY = _roomHeight / 2;
		
		if (x < midX) 
		{
            if (y < midY)
            {
				// 오른 아래 대각 그리기
				FillRectangle(x,y,1,1,tileX, tileY, tileType);
			}
			else
            {
				// 왼 위 대각 그리기
				FillRectangle(x,y,-1,-1, tileX, tileY, tileType);
			}
		}
		else 
		{
			if (y < midY)
			{
				// 왼 아래 대각 그리기
				FillRectangle(x,y,-1,1, tileX, tileY, tileType);
			}
			else
			{
				// 오른 위 대각 그리기
				FillRectangle(x,y,1,-1, tileX, tileY, tileType);
			}
		}
		
	}

	// 타일 그리기 2X2
	private void FillRectangle(int centralX, int centralY, int xRange, int yRange, int tileX, int tileY, int tileType=1)
	{
		int startX = (xRange > 0) ? centralX : centralX + xRange;
		int endX = (xRange > 0) ? centralX + xRange : centralX;
		int startY = (yRange > 0) ? centralY : centralY + yRange;
		int endY = (yRange > 0) ? centralY + yRange : centralY;

		tileX = (xRange > 0) ? tileX : tileX + xRange;
		tileY = (yRange > 0) ? tileY : tileY + yRange;

		for (int x = startX, gridX = tileX; x <= endX; x++, gridX++)
		{
			for (int y = startY, gridY = tileY; y <= endY; y++, gridY++)
			{
				_map[x, y] = new Vector3Int(gridX, gridY, tileType);
			}
		}
	}
}
