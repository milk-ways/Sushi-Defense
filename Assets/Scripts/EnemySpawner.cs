using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //[SerializeField]
    //private GameObject enemyPrefab; //��������
    [SerializeField]
    private GameObject enmeyHPSliderPrefab; //적 체력을 나타내는 Slider UI 프리펩
    [SerializeField]
    private Transform canvasTransform; //UI를 표현하는 Canvas 오브젝트의 Transform
    //[SerializeField]
    //private float spawnTime; //�� �����ֱ�
    [SerializeField]
    private Transform[] wayPoints; //�̵����
    [SerializeField]
    private PlayerHP playerHP; //플레이어의 체력 컴포넌트
    [SerializeField]
    private PlayerGold playerGold; //플레이어의 골드 컴포넌트
    private Wave currentWave;  //현재 웨이브 정보
    private int currentEnemyCount;  //현재 웨이브에 남아있는 적 숫자 (웨이브 시작시 max로 설정, 적 사망 시 -1)
    private List<Enemy> enemyList; //�����ϴ� ��� ���� ����

    // ���� ������ ������ EnemySpawner���� �ϱ� ������ Set�� �ʿ����.
    public List<Enemy> EnemyList => enemyList;
    // 현재 웨이브의 남아있는 적, 최대 적 숫자
    public int CurrentEnemyCount => currentEnemyCount;
    public int MaxEnemyCount => currentWave.maxEnemyCount;

    private void Awake()
    {
        //�� ����Ʈ �޸� �Ҵ�
        enemyList = new List<Enemy>();
        //�� ���� �ڷ�ƾ �Լ� ȣ��
        //StartCoroutine("SpawnEnemy"); 
    }
    
    public void StartWave(Wave wave)
    {
        //매개변수로 받아온 웨이브 정보 저장
        currentWave = wave;
        //현재 웨이브의 최대 적 숫자를 저장
        currentEnemyCount = currentWave.maxEnemyCount;
        //현재 웨이브 시작
        StartCoroutine("SpawnEnemy");
    }

    private IEnumerator SpawnEnemy()
    {
        //현재 웨이브에서 생성한 적 숫자
        int spawnEnemyCount = 0;
        //while (true)
        //현재 웨이브에서 생성되어야 하는 적의 숫자만큼 적을 생성하고 코루틴 종료
        while ( spawnEnemyCount < currentWave.maxEnemyCount )
        {
            //GameObject clone = Instantiate(enemyPrefab); // �� ������Ʈ ����
            //웨이브에 등장하는 적의 종류가 여러 종류일 때 임의의 적이 등장하도록 설정하고, 적 오브젝트 생성
            int enemyIndex = Random.Range(0,currentWave.enemyPrefabs.Length);
            GameObject clone = Instantiate(currentWave.enemyPrefabs[enemyIndex]);
            Enemy enemy = clone.GetComponent<Enemy>();   // ��� ������ ���� Enemy ������Ʈ

            // this�� �� �ڽ�(�ڽ��� EnemySpawner ����)
            enemy.Setup(this,wayPoints);                // wayPoint ������ �Ű������� Setup() ȣ��
            enemyList.Add(enemy);                        // ����Ʈ�� ��� ������ �� ���� ����
            
            SpawnEnemyHPSlider(clone);                   // 적 체력을 나타내는 Slider UI 생성 및 설정

            // 현재 웨이브에서 생성한 적의 숫자 +1
            spawnEnemyCount ++;

            //yield return new WaitForSeconds(spawnTime);  // spawnTime �ð� ���� ���
            // 각 웨이브마다 spawnTime이 다를 수 있기 때문에 현재 웨이브의 spawnTime 사용
            yield return new WaitForSeconds(currentWave.spawnTime);
        }
    }

    public void DestroyEnemy(EnemyDestroyType type, Enemy enemy, int gold)
    {
        // 적이 목표지점까지 도착했을 때
        if ( type == EnemyDestroyType.Arrive)
        {
            // 플레이어의 체력 -1
            playerHP.TakeDamage(1);
        }
        // 적이 플레이어의 발사체에게 사망했을 때
        else if ( type == EnemyDestroyType.Kill )
        {
            // 적의 종류에 따라 사망 시 골드 획득
            playerGold.CurrentGold += gold;
        }

        // 적이 사망할 때마다 현재 웨이브의 생존 적 숫자 감소 (UI 표시용)
        currentEnemyCount --;
        // �����Ϳ��� ����ϴ� �� ���� ����
        enemyList.Remove(enemy);
        // �� ������Ʈ ����
        Destroy(enemy.gameObject);
    }

    private void SpawnEnemyHPSlider(GameObject enemy)
    {
        // 적 체력을 나타내는 Slider UI 생성
        GameObject sliderClone = Instantiate(enmeyHPSliderPrefab);
        // Slider UI 오브젝트를 parent("Canvas" 오브젝트)의 자식으로 설정
        // UI는 캔버스의 자식 오브젝트로 설정되어 있어야 화면에 보이기에
        sliderClone.transform.SetParent(canvasTransform);
        // 계층 설정으로 바뀐 크기를 다시 (1,1,1)로 설정
        sliderClone.transform.localScale = Vector3.one;

        // Slider UI가 쫓아다닐 대상을 본인으로 설정
        sliderClone.GetComponent<SliderPositionAutoSetter>().Setup(enemy.transform);
        // Slider UI에 자신의 체력 정보를 표시하도록 설정
        sliderClone.GetComponent<EnemyHPViewer>().Setup(enemy.GetComponent<EnemyHP>());
    }
}
