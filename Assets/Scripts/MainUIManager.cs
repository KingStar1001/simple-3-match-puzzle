using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainUIManager : MonoBehaviour
{
  [Header("Game Base Information")]
  public int minRowCount;
  public int maxRowCount;
  public int minColumnCount;
  public int maxColumnCount;
  public int minColorCount;
  public int maxColorCount;

  [Header("UI Elements")]
  public IntegerInput rowInput;
  public IntegerInput columnInput;
  public IntegerInput colorInput;

  void Start()
  {
    rowInput.min = minRowCount;
    rowInput.max = maxRowCount;
    columnInput.min = minColumnCount;
    columnInput.max = maxColumnCount;
    colorInput.min = minColorCount;
    colorInput.max = maxColorCount;
  }

  public void StartGame()
  {
    rowInput.Validate();
    columnInput.Validate();
    colorInput.Validate();

    if (rowInput.ValidateInput() && columnInput.ValidateInput() && colorInput.ValidateInput())
    {
      GameManager.instance.StartGame(rowInput.GetValue(), columnInput.GetValue(), colorInput.GetValue());
    }
  }
}
