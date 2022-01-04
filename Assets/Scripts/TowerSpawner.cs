using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject towerPrefab;
    [SerializeField]
    private int towerBuildGold = 50;  // 타워 건설에 사용되는 골드
    [SerializeField]
    private EnemySpawner enemySpawner; // ���� �ʿ� �����Ѵ� �� ����Ʈ ���� ȹ��
    [SerializeField]
    private PlayerGold playerGold;  // 타워 건설 시 골드 감소를 위해

    public void SpawnTower(Transform tileTransform)
    {
        //타워를 건설한 만큼 돈이 없으면 타워 건설 X
        if ( towerBuildGold > playerGold.CurrentGold )
        {
            return;
        }

        Tile tile = tileTransform.GetComponent<Tile>();

        // Ÿ�� �Ǽ� ���� ���� Ȯ��
        // 1. ���� Ÿ���� ��ġ�� �̹� Ÿ���� �Ǽ��Ǿ� ������ Ÿ�� �Ǽ� X
        if (tile.IsBuildTower == true)
        {
            return;
        }

        // Ÿ���� �Ǽ��Ǿ� �������� ����
        tile.IsBuildTower = true;
        // 타워 건설에 필요한 골드만큼 감소
        playerGold.CurrentGold -= towerBuildGold;
        // ������ Ÿ���� ��ġ�� Ÿ�� �Ǽ�
        GameObject clone = Instantiate(towerPrefab, tileTransform.position, Quaternion.identity);
        // Ÿ�� ���⿡ enemySpawner ���� ����
        clone.GetComponent<TowerWeapon>().Setup(enemySpawner);
    }
}
