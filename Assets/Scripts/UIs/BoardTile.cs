using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BoardTile : MonoBehaviour
{
  public Image icon;
  public int type;
  public int x;
  public int y;
  public bool isMoving = false;
  private Action onFinishSwap = null;

  public void InitItem(int _type, int _x, int _y, float width, float height, bool isFirst = true, int step = 0)
  {
    type = _type;
    x = _x;
    y = _y;

    icon.color = BoardManager.instance.colors[type];
    GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
    GetComponent<RectTransform>().anchoredPosition = BoardManager.instance.GetPosition(x, -1);
    Vector2 toPosition = BoardManager.instance.GetPosition(x, y);
    float delay = 0.14f * (float)(BoardManager.instance.columnCount - y + 1) + 0.01f * x;
    StopCoroutine("MoveToTarget");
    if (isFirst)
    {
      StartCoroutine(MoveToTarget(toPosition, 2000f, delay));
    }
    else
    {
      StartCoroutine(MoveToTarget(toPosition, 2000f, 0.14f * step));
    }
  }

  private IEnumerator MoveToTarget(Vector2 to, float speed, float delay = 0f)
  {
    isMoving = true;
    if (delay > 0)
      yield return new WaitForSeconds(delay);
    Vector2 from = GetComponent<RectTransform>().anchoredPosition;
    float journeyLength = Vector2.Distance(from, to);
    float startTime = Time.time;
    Vector2 currentPos = GetComponent<RectTransform>().anchoredPosition;

    while (Vector2.Distance(currentPos, to) > 0.1f)
    {
      float distCovered = (Time.time - startTime) * speed;
      float fractionOfJourney = distCovered / journeyLength;
      GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(from, to, fractionOfJourney);
      yield return null;

      currentPos = GetComponent<RectTransform>().anchoredPosition;
    }

    // Ensure the object reaches the exact target position
    GetComponent<RectTransform>().anchoredPosition = to;
    isMoving = false;
    if (onFinishSwap != null) onFinishSwap();
    onFinishSwap = null;
  }

  public void Swap(int targetX, int targetY, Action _onFinishSwap = null)
  {
    onFinishSwap = _onFinishSwap;
    Vector2 toPosition = BoardManager.instance.GetPosition(targetX, targetY);
    StopCoroutine("MoveToTarget");
    StartCoroutine(MoveToTarget(toPosition, 500f, 0f));
  }

  public void GoToOriginalPosition()
  {
    Vector2 toPosition = BoardManager.instance.GetPosition(x, y);
    StopCoroutine("MoveToTarget");
    StartCoroutine(MoveToTarget(toPosition, 500f, 0f));
  }

  public void MoveTo(int targetX, int targetY)
  {
    x = targetX;
    y = targetY;
    Vector2 toPosition = BoardManager.instance.GetPosition(targetX, targetY);
    StopCoroutine("MoveToTarget");
    StartCoroutine(MoveToTarget(toPosition, 1000f, 0f));
  }
}
