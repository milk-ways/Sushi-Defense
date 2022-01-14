using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponState { SearchTarget = 0, AttackToTarget }

public class TowerWeapon : MonoBehaviour
{
    [SerializeField]
    private TowerTemplate towerTemplate; // 타워 정보 (공격력, 공격속도 등)
    [SerializeField]
    private Transform spawnPoint; // 발사체 생성 위치
    [SerializeField]
    private GameObject projectilePrefab; // 발사체 프리펩
    //[SerializeField]
    //private float attackRate = 0.5f; // 공격속도
    //[SerializeField]
    //private float attackRange = 2.0f; // 공격범위
    //[SerializeField]
    //private int attackDamage = 1; // 공격력
    private int level = 0; // 타워 레벨
    private Transform attackTarget = null; // 공격 대상
    private WeaponState weaponState = WeaponState.SearchTarget; // 타워 무기의 상태
    private EnemySpawner enemySpawner; // 게임에 존재하는 적 정보 획득용
    private SpriteRenderer spriteRenderer; // 타워 오브젝트 이미지 변경용
    private Tile ownerTile; // 현재 타워가 배치되어 있는 타일

    public string TowerType => towerTemplate.weapon[level].towerType;
    public Sprite TowerSprite => towerTemplate.weapon[level].sprite;
    public float Range => towerTemplate.weapon[level].range;///attackRange;
    public float Damage => towerTemplate.weapon[level].damage;//attackDamage;
    public float Rate => towerTemplate.weapon[level].rate;//attackRate;
    public int Level => level + 1;
    public int exp = 1;
    public int xpforlv2 = 3;
    public int xpforlv3 = 9;
    
    [Header("Laser")]
    [SerializeField]
    private LineRenderer lineRenderer; //레이저로 사용되는 선(LineRenderer)
    [SerializeField]
    private Transform hitEffect; //타격 효과
    [SerializeField]
    private LayerMask targetLayer; //광선에 부딪히는 레이어 설정


    public void Setup(EnemySpawner enemySpawner, Tile ownerTile)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.enemySpawner = enemySpawner;
        this.ownerTile = ownerTile;
 
        // 최초 상태를 WeaponState.SearchTarget으로 설정
        ChangeState(WeaponState.SearchTarget);
    }

    public void ChangeState(WeaponState newState)
    {
        // 이전에 재생중이던 상태 종료
        StopCoroutine(weaponState.ToString());
        // 상태 변경
        weaponState = newState;
        // 새로운 상태 재생
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
        // 원점으로부터의 거리와 수평축으로부터의 각도를 이용해 위치를 구하는 극 좌표계 이용
        // 각도 = arctan(y/x)
        // x, y 변위값 구하기
        float dx = attackTarget.position.x - transform.position.x;
        float dy = attackTarget.position.y - transform.position.y;
        // x, y 변위값을 바탕으로 각도 구하기
        // 각도가 radian 단위이기 때문에 Mathf.Rad2Deg를 곱해 도 단위를 구함
        float degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, degree);
    }

    private IEnumerator SearchTarget()
    {
        while (true)
        {
            // 제일 가까이 있는 적을 찾기 위해 최초 거리를 최대한 크게 설정
            float closeDistSqr = Mathf.Infinity;
            // EnemySpawner의 EnemyList에 있는 현재 맵에 존재하는 모든 적 검사
            for (int i = 0; i < enemySpawner.EnemyList.Count; ++i)
            {
                float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);
                // 현재 검사중인 적과의 거리가 공격범위 내에 있고, 현재까지 검사한 적보다 거리가 가까우면
                if (distance <= towerTemplate.weapon[level].range && distance <= closeDistSqr)
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
            // target이 있는지 검사 (다른 발사체에 의해 제거, Goal 지점까지 이동해 삭제 등)
            if (attackTarget == null)
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            // target이 공격 범위 안에 있는지 검사 (공격 범위를 벗어나면 새로운 적 탐색)
            float distance = Vector3.Distance(attackTarget.position, transform.position);
            if (distance > towerTemplate.weapon[level].range)
            {
                attackTarget = null;
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            // 3. attackRate 시간만큼 대기
            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);

            // 4. 공격(발사체 생성)
            SpawnProjectile();
        }
    }

    private void SpawnProjectile()
    {
        if (TowerType == "Gun")
        {
            GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
            // 생성된 발사체에게 공격대상 정보 제공
            clone.GetComponent<Bullet>().Setup(attackTarget, towerTemplate.weapon[level].damage);
        }
        else if (TowerType == "Slow")
        {
            GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
            // 생성된 발사체에게 공격대상 정보 제공
            clone.GetComponent<SlowBullet>().Setup(attackTarget, towerTemplate.weapon[level].damage);

        }
        else if (TowerType == "Laser")
        {

            Vector3 direction = attackTarget.position - spawnPoint.position;
            RaycastHit2D[] hit = Physics2D.RaycastAll(spawnPoint.position, direction, towerTemplate.weapon[level].range, targetLayer);

            // 같은 방향으로 여러 개의 광선을 쏴서 그 중 현재 attakcTarget과 동일한 오브젝트를 검출
            for (int i = 0; i < hit.Length; ++i)
            {
                if (hit[i].transform == attackTarget)
                {
                    // 선의 시작지점
                    lineRenderer.SetPosition(0, spawnPoint.position);
                    // 선의 목표지점
                    lineRenderer.SetPosition(1, new Vector3(hit[i].point.x, hit[i].point.y, 0) + Vector3.back);
                    // 타격 효과 위치 설정
                    hitEffect.position = hit[i].point;
                    // 적 체력 감소 (1초에 damage만큼 감소)
                    //attackTarget.GetComponent<EnemyHP>().TakeDamage(towerTemplate.weapon[level].damage * Time.deltaTime);
                    // 공격력 = 타워 기본 공격력 + 버프에 의해 추가된 공격력
                    float damage = towerTemplate.weapon[level].damage;
                    attackTarget.GetComponent<EnemyHP>().TakeDamage(damage * Time.deltaTime);
                }
            }
        }
    }

    public void Sell()
    {
        // 골드 증가 코드 필요
        // 현재 타일에 다시 타워 건설이 가능하도록 설정
        ownerTile.IsBuildTower = false;
        // 타워 파괴
        Destroy(gameObject);
    }
}
