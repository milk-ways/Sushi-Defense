using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField]
    private TowerTemplate towerTemplate; // 타워 정보 (공격력, 공격속도 등)
    //[SerializeField]
    //private GameObject towerPrefab;
    //[SerializeField]
    //private int towerBuildGold = 50;  // 타워 건설에 사용되는 골드
    [SerializeField]
    private EnemySpawner enemySpawner; // ���� �ʿ� �����Ѵ� �� ����Ʈ ���� ȹ��
    [SerializeField]
    private PlayerGold playerGold;  // 타워 건설 시 골드 감소를 위해
    [SerializeField]
    private SystemTextViewer systemTextViewer; // 돈부족, 건설불가와 같은 시스템 메시지 출력

    public void SpawnTower(Transform tileTransform)
    {
        //타워를 건설한 만큼 돈이 없으면 타워 건설 X
        //if ( towerBuildGold > playerGold.CurrentGold )
        if ( towerTemplate.weapon[0].cost > playerGold.CurrentGold)
        {
            //골드가 부족해서 타워 건설이 불가능하다고 출력
            systemTextViewer.PrintText(SystemType.Money);
            return;
        }

        Tile tile = tileTransform.GetComponent<Tile>();

        // Ÿ�� �Ǽ� ���� ���� Ȯ��
        // 1. ���� Ÿ���� ��ġ�� �̹� Ÿ���� �Ǽ��Ǿ� ������ Ÿ�� �Ǽ� X
        if (tile.IsBuildTower == true)
        {
            //현재 위치에 타워 건설이 불가능하다고 출력
            systemTextViewer.PrintText(SystemType.Build);
            return;
        }

        // Ÿ���� �Ǽ��Ǿ� �������� ����
        tile.IsBuildTower = true;
        // 타워 건설에 필요한 골드만큼 감소
        //playerGold.CurrentGold -= towerBuildGold;
        playerGold.CurrentGold -= towerTemplate.weapon[0].cost;
        // ������ Ÿ���� ��ġ�� Ÿ�� �Ǽ�
        // 선택한 타일의 위치에 타워 건설 (타일보다 z축 -1의 위치에 배치)
        Vector3 position = tileTransform.position + Vector3.back;
        //GameObject clone = Instantiate(towerPrefab, position, Quaternion.identity);
        GameObject clone = Instantiate(towerTemplate.towerPrefab, position, Quaternion.identity);
        // Ÿ�� ���⿡ enemySpawner ���� ����
        clone.GetComponent<TowerWeapon>().Setup(enemySpawner, playerGold, tile);
    }
}
