using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerGenerator : MonoBehaviour
{
    [SerializeField]
    private TowerTemplate towerTemplate; // 타워 정보 (공격력, 공격속도 등)
    [SerializeField]
    private GameObject spawnPoint;
    [SerializeField]
    private float spawnTime;
    [SerializeField]
    private GameObject belt;

    private void Start()
    {
        StartCoroutine("towerSpawn");
    }

    private IEnumerator towerSpawn()
    {
        BeltUpgrade beltUpgrade = belt.GetComponent<BeltUpgrade>();
        while (true)
        {
            spawnTime = beltUpgrade.spawnTime[beltUpgrade.speedLevel];
            Instantiate(towerTemplate.towerPrefab, spawnPoint.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(spawnTime);
        }
    }
}
