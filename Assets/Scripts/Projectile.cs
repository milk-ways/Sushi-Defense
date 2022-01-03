using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Movement2D movement2D;
    private Transform target;

    public void Setup(Transform target)
    {
        movement2D = GetComponent<Movement2D>();
        this.target = target;   // Ÿ���� �������� target
    }

    // Update is called once per frame
    private void Update()
    {
        if (target != null) // target�� �����ϸ�
        {
            // �߻�ü�� target�� ��ġ�� �̵�
            Vector3 direction = (target.position - transform.position).normalized;
            movement2D.MoveTo(direction);
        }
        else  // ���� ������ target�� �������
        {
            // �߻�ü ������Ʈ ����
            Destroy(gameObject);
        }
        if (movement2D.distance >= 2) // �����Ÿ� �̻� ���� �� ���� (�̰� ���߿� Ÿ�� �����Ÿ��� ����)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;  //���� �ƴ� ���� �ε�����
        if (collision.transform != target) return;   //���� target�� ���� �ƴ� ��

        collision.GetComponent<Enemy>().OnDie();     //�� ��� �Լ� ȣ��
        Destroy(gameObject);                         //�߻�ü ������Ʈ ����
    }
}
