using UnityEngine;
using UnityEngine.EventSystems;

public class BoardController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
  private RectTransform imageTransform;
  private void Awake()
  {
    // Get the required components
    imageTransform = GetComponent<RectTransform>();
  }

  public void OnPointerDown(PointerEventData eventData)
  {
    // Calculate the offset between the pointer position and the image position
    if (!BoardManager.instance.isMoving())
    {
      Vector2 offset;
      RectTransformUtility.ScreenPointToLocalPointInRectangle(imageTransform, eventData.position, eventData.pressEventCamera, out offset);
      Vector2Int pos = BoardManager.instance.GetTilePositionFromOffset(offset);
      if (BoardManager.instance.currentTile == null)
      {
        BoardManager.instance.SetCurrentTile(pos);
      }
      else
      {
        BoardManager.instance.SwapTile(pos.x, pos.y);
      }
    }
  }

  public void OnDrag(PointerEventData eventData)
  {
    // Move the image with the pointer while dragging
    if (!BoardManager.instance.isMoving() && BoardManager.instance.currentTile != null)
    {
      Vector2 offset;
      RectTransformUtility.ScreenPointToLocalPointInRectangle(imageTransform, eventData.position, eventData.pressEventCamera, out offset);
      Vector2Int pos = BoardManager.instance.GetTilePositionFromOffset(offset);
      if (BoardManager.instance.currentTile.x != pos.x || BoardManager.instance.currentTile.y != pos.y)
      {
        BoardManager.instance.SwapTile(pos.x, pos.y);
      }
    }
  }

  public void OnPointerUp(PointerEventData eventData)
  {
    // Perform any necessary actions when the dragging is released
  }
}
