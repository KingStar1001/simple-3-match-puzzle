using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
  public static GameManager instance;
  public int rowCount;
  public int columnCount;
  public int colorCount;
  void Awake()
  {
    instance = this;
  }

  public void StartGame(int _row, int _column, int _color)
  {
    rowCount = _row;
    columnCount = _column;
    colorCount = _color;
    SceneManager.LoadScene("Main");
  }

  public void EndGame()
  {
    GameObject obj = GameObject.Find("GlobalManagers");
    if (obj != null)
    {
      Destroy(obj);
    }
    SceneManager.LoadScene("Lobby");
  }
}
