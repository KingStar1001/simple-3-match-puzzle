using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IntegerInput : MonoBehaviour
{
  public int min;
  public int max;
  public TMP_Text validationLabel;
  private TMP_InputField input;
  void Start()
  {
    input = GetComponent<TMP_InputField>();
  }

  public void Validate()
  {
    int number = 0;
    int.TryParse(input.text, out number);

    if (number < min || number > max)
    {
      validationLabel.text = string.Format("value must be between {0} and {1}", min, max);
      validationLabel.gameObject.SetActive(true);
    }
    else
    {
      validationLabel.gameObject.SetActive(false);
    }
  }

  public bool ValidateInput()
  {
    int number = 0;
    int.TryParse(input.text, out number);

    if (number < min || number > max)
    {
      validationLabel.text = string.Format("value must be between {0} and {1}", min, max);
      validationLabel.gameObject.SetActive(true);
      return false;
    }

    validationLabel.gameObject.SetActive(false);
    return true;
  }

  public int GetValue()
  {
    int number = 0;
    int.TryParse(input.text, out number);
    return number;
  }
}
