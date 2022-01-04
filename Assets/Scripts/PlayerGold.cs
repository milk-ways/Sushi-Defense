using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGold : MonoBehaviour
{
    [SerializeField]
    private int currentGold = 100;

    public int currentGold
    {
        set => currentGold = Mathf.Max(0, value);
        get => currentGold;
    }
}
