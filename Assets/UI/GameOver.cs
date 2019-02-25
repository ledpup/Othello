using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {
  
    public void OnPointerDown()
    {
        transform.gameObject.SetActive(false);
    }
}
