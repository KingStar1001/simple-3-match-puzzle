using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
  public static BoardManager instance;

  float boardWidth = 1300f;
  float boardHeight = 1300f;

  public int rowCount;
  public int columnCount;
  public int colorCount;
  BoardTile[,] tiles;

  public BoardTile currentTile = null;
  public BoardTile targetTile = null;

  [Header("Assets")]
  public Sprite jewelSprite;
  public List<Sprite> cellSprites;

  public GameObject boardCellPrefab;
  public GameObject boardTilePrefab;

  [Header("Game UIs")]
  public Transform board;
  public Transform tileContainer;
  public GameObject pointer;

  [Header("Game Informations")]
  public List<Color> colors;

  void Awake()
  {
    instance = this;
  }

  void Start()
  {
    StartGame();
  }

  public void StartGame()
  {
    rowCount = GameManager.instance.rowCount;
    columnCount = GameManager.instance.columnCount;
    colorCount = GameManager.instance.colorCount;

    GenerateRandomColors();
    InitBoard();
    InitTiles();
  }

  public void EndGame()
  {
    GameManager.instance.EndGame();
  }

  public void InitBoard()
  {
    Utils.DestroyChildren(board);
    float cellWidth = boardWidth / (float)rowCount;
    float cellHeight = boardHeight / (float)columnCount;

    pointer.GetComponent<RectTransform>().sizeDelta = new Vector2(cellWidth, cellHeight);

    for (int i = 0; i < rowCount; i++)
    {
      for (int j = 0; j < columnCount; j++)
      {
        int cellIcon = (i + j) % 2;
        GameObject obj = Instantiate(boardCellPrefab, board);
        BoardCell cell = obj.GetComponent<BoardCell>();
        cell.icon.sprite = cellSprites[cellIcon];
        cell.GetComponent<RectTransform>().sizeDelta = new Vector2(cellWidth, cellHeight);
        cell.GetComponent<RectTransform>().anchoredPosition = new Vector2(-boardWidth / 2f + cellWidth * (float)i + cellWidth / 2f, -boardHeight / 2f + cellHeight * (float)j + cellHeight / 2f);
      }
    }
  }

  public void InitTiles()
  {
    tiles = new BoardTile[rowCount, columnCount];

    Utils.DestroyChildren(tileContainer);
    float cellWidth = boardWidth / (float)rowCount;
    float cellHeight = boardHeight / (float)columnCount;
    for (int i = 0; i < rowCount; i++)
    {
      for (int j = 0; j < columnCount; j++)
      {
        int type = GetRandomTile(i, j);

        GameObject obj = Instantiate(boardTilePrefab, tileContainer);
        BoardTile tile = obj.GetComponent<BoardTile>();
        tile.InitItem(type, i, j, cellWidth, cellHeight);
        tiles[i, j] = tile;
      }
    }
  }

  public void GenerateRandomColors()
  {
    colors.Clear();
    for (int i = 0; i < colorCount; i++)
    {
      colors.Add(RandomBrightColor());
    }
  }

  private Color RandomBrightColor()
  {
    // Generate random RGB values between 0 and 1
    float r = Random.Range(0f, 1f);
    float g = Random.Range(0f, 1f);
    float b = Random.Range(0f, 1f);

    // Set the RGB values to their maximum to create a bright color
    float max = Mathf.Max(r, Mathf.Max(g, b));
    r = Mathf.Clamp01(r / max);
    g = Mathf.Clamp01(g / max);
    b = Mathf.Clamp01(b / max);

    return new Color(r, g, b);
  }

  int GetRandomTile(int x, int y)
  {
    int r = -1;
    int dem = 0;
    while (true)
    {
      r = Random.Range(0, colorCount);
      if (r == colorCount) r--;
      if (y < 2 || tiles[x, y - 1].type != r || tiles[x, y - 2].type != r)
      {
        if (x < 2 || tiles[x - 1, y].type != r || tiles[x - 2, y].type != r)
        {
          return r;
        }
      }
      dem++;
      if (dem > 200)
      {
        Debug.LogError("random error");
        return 0;
      }
    }
  }

  public Vector2 GetPosition(int x, int y)
  {
    float cellWidth = boardWidth / (float)rowCount;
    float cellHeight = boardHeight / (float)columnCount;
    return new Vector2(-boardWidth / 2f + cellWidth * (float)x + cellWidth / 2f, boardHeight / 2f - cellHeight * (float)y - cellHeight / 2f);
  }

  public bool isMoving()
  {
    foreach (BoardTile tile in tiles)
    {
      if (tile == null || tile.isMoving)
      {
        return true;
      }
    }
    return false;
  }

  public void SetCurrentTile(Vector2Int pos)
  {
    foreach (BoardTile tile in tiles)
    {
      if (tile.x == pos.x && tile.y == pos.y)
      {
        currentTile = tile;
        pointer.GetComponent<RectTransform>().anchoredPosition = GetPosition(pos.x, pos.y);
        pointer.SetActive(true);
      }
    }
  }

  public void ClearCurrentTile()
  {
    currentTile = null;
    pointer.SetActive(false);
  }

  public void HideSelection()
  {
    pointer.SetActive(false);
  }

  public void SwapTile(int targetX, int targetY)
  {
    if (currentTile != null)
    {
      int offset = Mathf.Abs(targetX - currentTile.x) + Mathf.Abs(targetY - currentTile.y);
      if (offset == 1)
      {
        foreach (BoardTile tile in tiles)
        {
          if (tile.x == targetX && tile.y == targetY)
          {
            targetTile = tile;
          }
        }
        if (targetTile != null)
        {
          currentTile.Swap(targetTile.x, targetTile.y, OnFinishSwap);
          targetTile.Swap(currentTile.x, currentTile.y);
        }
        else
        {
          ClearCurrentTile();
        }
      }
      else
      {
        ClearCurrentTile();
      }
    }
    HideSelection();
  }

  public void OnFinishSwap()
  {
    SwapTiles(currentTile.x, currentTile.y, targetTile.x, targetTile.y);
    List<Vector2Int> matches = CheckTiles();

    if (matches.FindIndex(match => match.x == currentTile.x && match.y == currentTile.y) == -1 && matches.FindIndex(match => match.x == targetTile.x && match.y == targetTile.y) == -1)
    {
      SwapTiles(currentTile.x, currentTile.y, targetTile.x, targetTile.y);
      currentTile.GoToOriginalPosition();
      targetTile.GoToOriginalPosition();
    }
    else
    {
      DestroyTiles(matches);
      RefillTiles();
    }

    targetTile = null;
    ClearCurrentTile();
  }

  public List<Vector2Int> CheckTiles()
  {
    List<Vector2Int> matches = new List<Vector2Int>();
    List<Vector2Int> matchesInRow = CheckInRow();
    if (matchesInRow.Count > 0)
    {
      matches.AddRange(matchesInRow);
    }
    List<Vector2Int> matchesInColumn = CheckInColumn();
    if (matchesInColumn.Count > 0)
    {
      matches.AddRange(matchesInColumn);
    }
    return matches;
  }

  public List<Vector2Int> CheckInRow()
  {
    List<Vector2Int> list = new List<Vector2Int>();
    List<Vector2Int> matches = new List<Vector2Int>();
    for (int j = 0; j < columnCount; j++)
    {
      matches.Clear();
      matches.Add(new Vector2Int(0, j));
      for (int i = 0; i < rowCount - 1; i++)
      {
        if (tiles[i, j].type == tiles[i + 1, j].type)
        {
          matches.Add(new Vector2Int(i + 1, j));
        }
        else
        {
          if (matches.Count >= 3)
          {
            list.AddRange(matches);
          }
          matches.Clear();
          matches.Add(new Vector2Int(i + 1, j));
        }
      }
      if (matches.Count >= 3)
      {
        list.AddRange(matches);
      }
    }
    return list;
  }

  public List<Vector2Int> CheckInColumn()
  {
    List<Vector2Int> list = new List<Vector2Int>();
    List<Vector2Int> matches = new List<Vector2Int>();
    for (int i = 0; i < rowCount; i++)
    {
      matches.Clear();
      matches.Add(new Vector2Int(i, 0));
      for (int j = 0; j < columnCount - 1; j++)
      {
        if (tiles[i, j].type == tiles[i, j + 1].type)
        {
          matches.Add(new Vector2Int(i, j + 1));
        }
        else
        {
          if (matches.Count >= 3)
          {
            list.AddRange(matches);
          }
          matches.Clear();
          matches.Add(new Vector2Int(i, j + 1));
        }
      }
      if (matches.Count >= 3)
      {
        list.AddRange(matches);
      }
    }
    return list;
  }

  public void SwapTiles(int originX, int originY, int targetX, int targetY)
  {
    BoardTile tmpTile = tiles[originX, originY];
    tiles[originX, originY] = tiles[targetX, targetY];
    tiles[targetX, targetY] = tmpTile;

    tiles[originX, originY].x = originX;
    tiles[originX, originY].y = originY;
    tiles[targetX, targetY].x = targetX;
    tiles[targetX, targetY].y = targetY;
  }

  public void DestroyTiles(List<Vector2Int> matches)
  {
    foreach (Vector2Int match in matches)
    {
      if (tiles[match.x, match.y] != null)
      {
        Destroy(tiles[match.x, match.y].gameObject);
        tiles[match.x, match.y] = null;
      }
    }
  }

  public void RefillTiles()
  {
    for (int i = 0; i < rowCount; i++)
    {
      for (int j = columnCount - 1; j >= 0; j--)
      {
        if (tiles[i, j] != null)
        {
          int to = j + 1;
          while (to < columnCount && tiles[i, to] == null)
          {
            to++;
          }
          to = to - 1;
          if (to < columnCount && tiles[i, to] == null)
          {
            tiles[i, j].MoveTo(i, to);
            tiles[i, to] = tiles[i, j];
            tiles[i, j] = null;
          }
        }
      }
      int step = 0;
      for (int j = columnCount - 1; j >= 0; j--)
      {
        if (tiles[i, j] == null)
        {
          if (step == 0)
          {
            step = j;
          }

          float cellWidth = boardWidth / (float)rowCount;
          float cellHeight = boardHeight / (float)columnCount;
          int type = Random.Range(0, colorCount);
          if (type == colorCount) type--;

          GameObject obj = Instantiate(boardTilePrefab, tileContainer);
          BoardTile tile = obj.GetComponent<BoardTile>();
          tile.InitItem(type, i, j, cellWidth, cellHeight, false, step - j + 1);
          tiles[i, j] = tile;
        }
      }
    }
  }

  public Vector2Int GetTilePositionFromOffset(Vector2 pos)
  {
    int row = 0;
    int column = 0;

    float cellWidth = boardWidth / (float)rowCount;
    float cellHeight = boardHeight / (float)columnCount;
    row = (int)((pos.x + boardWidth / 2f) / cellWidth);
    column = (int)((-pos.y + boardHeight / 2f) / cellHeight);
    return new Vector2Int(row, column);
  }
}
