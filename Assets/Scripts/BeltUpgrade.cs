using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeltUpgrade : MonoBehaviour
{
    [SerializeField]
    private PlayerGold playerGold; // 업그레이드시 골드 감소를 위해
    [SerializeField]
    private Button buttonUpgradeSpawnTime;
    public float[] spawnTime = { 3, 2, 1 };
    private int[] costUpgradeSpawnTime = { 50, 100 };
    public int speedLevel = 0;

    private void Update()
    {
        if (speedLevel == 2)
        {
            buttonUpgradeSpawnTime.interactable = false;
        }
    }

    public void OnclickEventBeltUpgradeSpeed()
    {
        if (costUpgradeSpawnTime[speedLevel] > playerGold.CurrentGold)
        {
            return;
        }
        playerGold.CurrentGold -= costUpgradeSpawnTime[speedLevel];
        speedLevel++;
    }
}
