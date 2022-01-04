using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyDestroyType { Kill = 0, Arrive }

public class Enemy : MonoBehaviour
{
    private int wayPointCount;         //�̵� ��� ����
    private Transform[] wayPoints;     //�̵� ��� ����
    private int currentIndex = 0;      //���� ��ǥ���� �ε���
    private Movement2D movement2D;     //������Ʈ �̵� ����
    private EnemySpawner enemySpawner; //���� ������ ������ ���� �ʰ� EnemySpaner�� �˷��� ����
    [SerializeField]
    private int gold = 10; //적 사망시 획득 가능한 골드

    public void Setup(EnemySpawner enemySpawner, Transform[] wayPoints)
    {
        movement2D = GetComponent<Movement2D>();
        this.enemySpawner = enemySpawner;

        // �� �̵� ��� wayPoints ���� ����
        wayPointCount = wayPoints.Length;
        this.wayPoints = new Transform[wayPointCount];
        this.wayPoints = wayPoints;

        // ���� ��ġ�� ù��° wayPoint ��ġ�� ����
        transform.position = wayPoints[currentIndex].position;

        // �� �̵�/��ǥ���� ���� �ڷ�ƾ �Լ� ����
        StartCoroutine("OnMove");
    }

    private IEnumerator OnMove()
    {
        //���� �̵� ���� ����
        NextMoveTo();

        while (true)
        {
            //�� ������Ʈ ȸ��
            transform.Rotate(Vector3.forward * 10);

            // ���� ������ġ�� ��ǥ��ġ�� �Ÿ��� 0.02 * movemet2D.MoveSpeed ���� ���� �� if ���ǹ� ����
            // movement2D ���� �����ִ� ������ �ӵ��� ������ �� �����ӿ� 0.02���� ũ�� �����̱� ������if ���ǹ��� �ɸ��� �ʰ� ��θ� Ż���ϴ� ������Ʈ�� �߻� ����
            if (Vector3.Distance(transform.position, wayPoints[currentIndex].position) < 0.02f * movement2D.MoveSpeed)
            {
                //���� �̵����� ����
                NextMoveTo();
            }

            yield return null;
        }
    }

    private void NextMoveTo()
    {
        // ���� �̵��� wayPoints�� �����ִٸ�
        if (currentIndex < wayPointCount - 1)
        {
            //���� ��ġ�� ��Ȯ�ϰ� ��ǥ��ġ�� ����
            transform.position = wayPoints[currentIndex].position;
            //�̵� ���� ���� => ���� ��ǥ����(wayPoints)
            currentIndex++;
            Vector3 direction = (wayPoints[currentIndex].position-transform.position).normalized;
            movement2D.MoveTo(direction);
        }
        // ���� ��ġ�� ������ wayPoints���
        else
        {
            //목표 지점에 도달해서 사망할 때는 돈을 주지 않도록
            gold = 0;
            //�� ������Ʈ ����
            //Destroy(gameObject);
            OnDie(EnemyDestroyType.Arrive);
        }
    }

    public void OnDie(EnemyDestroyType type)
    {
        // EnemySpawner���� ����Ʈ�� �� ������ �����ϱ� ������ Destory()�� �������� �ʰ�
        // EnemySpawner���� ������ ������ �� �ʿ��� ó���� �ϵ��� DestoryEnemy() �Լ� ȣ��
        enemySpawner.DestroyEnemy(type, this, gold);
    }
}
