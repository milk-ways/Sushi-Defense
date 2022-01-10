using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerGenerator : MonoBehaviour
{
    [SerializeField]
    private TowerTemplate towerTemplate; // Ÿ�� ���� (���ݷ�, ���ݼӵ� ��)
    [SerializeField]
    private GameObject spawnPoint;
    [SerializeField]
    private float spawnTime; 

    private void Start()
    {
        StartCoroutine("towerSpawn");
    }

    private IEnumerator towerSpawn()
    {
        while (true)
        {
            Instantiate(towerTemplate.towerPrefab, spawnPoint.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(spawnTime);
        }
    }
}
