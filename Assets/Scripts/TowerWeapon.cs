using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponState { SearchTarget = 0, AttackToTarget }

public class TowerWeapon : MonoBehaviour
{
    [SerializeField]
    private TowerTemplate towerTemplate;  //타워 정보 (공격력, 공격속도 등)
    [SerializeField]
    private GameObject projectilePrefab; //�߻�ü ������
    [SerializeField]
    private Transform spawnPoint; //�߻�ü ���� ��ġ
    //[SerializeField]
    //private float attackRate = 0.5f; //���ݼӵ�
    //[SerializeField]
    //private float attackRange = 2.0f; //���� ����
    //[SerializeField]
    //private int attakDamage = 1; //공격력
    private int level = 0; //타워 레벨
    private WeaponState weaponState = WeaponState.SearchTarget; //Ÿ�������� ����
    private Transform attackTarget = null; //���� ���
    private SpriteRenderer spriteRenderer; //타워 오브젝트 이미지 변경용
    private EnemySpawner enemySpawner; // ���ӿ� �����ϴ� �� ���� ȹ���
    private PlayerGold playerGold; //플레이어의 골드 정보 획득 및 설정
    private Tile ownerTile; //현재 타워가 배치되어 있는 타일

    public Sprite TowerSprite => towerTemplate.weapon[level].sprite;
    //public float Damage => attakDamage;
    public float Damage => towerTemplate.weapon[level].damage;
    //public float Rate => attackRate;
    public float Rate => towerTemplate.weapon[level].rate;
    //public float Range => attackRange;
    public float Range => towerTemplate.weapon[level].range;
    public int Level => level + 1;
    public int MaxLevel => towerTemplate.weapon.Length;

    public void Setup(EnemySpawner enemySpawner, PlayerGold playerGold, Tile ownerTile)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.enemySpawner = enemySpawner;
        this.playerGold = playerGold;
        this.ownerTile = ownerTile;

        // ���� ���¸� WeaponState.SearchTarget���� ����
        ChangeState(WeaponState.SearchTarget);
    }

    public void ChangeState(WeaponState newState)
    {
        // ������ ������̴� ���� ����
        StopCoroutine(weaponState.ToString());
        // ���� ����
        weaponState = newState;
        // ���ο� ���� ���
        StartCoroutine(weaponState.ToString());
    }

    private void Update()
    {
        if (attackTarget != null)
        {
            RotateToTarget();
        }
    }

    private void RotateToTarget()
    {
        // �������κ����� �Ÿ��� ���������κ����� ������ �̿��� ��ġ�� ���ϴ� �� ��ǥ�� �̿�
        // ���� = arctan(y/x)
        // x, y ������ ���ϱ�
        float dx = attackTarget.position.x - transform.position.x;
        float dy = attackTarget.position.y - transform.position.y;
        // x, y �������� �������� ���� ���ϱ�
        // ������ radian ������ ������ Mathf.Rad2Deg�� ���� �� ������ ����
        float degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, degree);
    }

    private IEnumerator SearchTarget()
    {
        while (true)
        {
            // ���� ������ �ִ� ���� ã�� ���� ���� �Ÿ��� �ִ��� ũ�� ����
            float closeDistSqr = Mathf.Infinity;
            // EnemySpawner�� EnemyList�� ���� �ʿ� �����ϴ� ��� �� �˻�
            for (int i = 0; i < enemySpawner.EnemyList.Count; ++i)
            {
                float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);
                // ���� �˻����� ������ �Ÿ��� ���ݹ��� ���� �ְ�, ������� �˻��� ������ �Ÿ��� ������
                //if (distance <= attackRange && distance <= closeDistSqr)
                // 현재 검사중인 적과의 거리가 공격범위 내에 있고, 현재까지 검사한 적보다 거리가 가까우면
                if ( distance <= towerTemplate.weapon[level].range && distance <= closeDistSqr)
                {
                    closeDistSqr = distance;
                    attackTarget = enemySpawner.EnemyList[i].transform;
                }
            }

            if (attackTarget != null)
            {
                ChangeState(WeaponState.AttackToTarget);
            }

            yield return null;
        }
    }

    private IEnumerator AttackToTarget()
    {
        while (true)
        {
            // 1. target�� �ִ��� �˻� (�ٸ� �߻�ü�� ���� ����, Goal �������� �̵��� ���� ��)
            if (attackTarget == null)
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            // 2. target�� ���ݹ��� �ȿ� �ִ��� �˻� (���� ������ ����� ���ο� �� Ž��)
            float distance = Vector3.Distance(attackTarget.position, transform.position);
            //if (distance > attackRange)
            if ( distance > towerTemplate.weapon[level].range)
            {
                attackTarget = null;
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            // 3. attackRate �ð���ŭ ���
            //yield return new WaitForSeconds(attackRate);
            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);

            // 4. ����(�߻�ü ����)
            SpawnProjectile();
        }
    }

    private void SpawnProjectile()
    {
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        // ������ �߻�ü���� ���ݴ��(attackTarget) ���� ����
        //clone.GetComponent<Projectile>().Setup(attackTarget, attakDamage);
        clone.GetComponent<Projectile>().Setup(attackTarget, towerTemplate.weapon[level].damage);
    }

    public bool Upgrade()
    {
        // 타워 업그레이드에 필요한 골드가 충분한지 검사
        if ( playerGold.CurrentGold < towerTemplate.weapon[level+1].cost)
        {
            return false;
        }

        // 타워 레벨 증가
        level ++;
        // 타워 외형 변경 (Sprite)
        spriteRenderer.sprite = towerTemplate.weapon[level].sprite;
        // 골드 차감
        playerGold.CurrentGold -= towerTemplate.weapon[level].cost;

        return true;
    }

    public void Sell()
    {
        // 골드 증가
        playerGold.CurrentGold += towerTemplate.weapon[level].sell;
        // 현재 타일에 다시 타워 건설이 가능해지도록 설정
        ownerTile.IsBuildTower = false;
        // 타워 파괴
        Destroy(gameObject);
    }
}
