using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAttackRange : MonoBehaviour
{
    private Camera mainCamera;
    private Ray ray;
    private RaycastHit hit;
    private bool isMoveRange = false;

    private void Awake()
    {
        OffAttackRange();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (isMoveRange)
        {
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            transform.Translate(mousePosition, .0f);
        }
    }

    public void OnAttackRange(Vector3 position, float range)
    {
        gameObject.SetActive(true);

        //공격 범위 크기
        float diameter = range * 2.0f;
        transform.localScale = Vector3.one * diameter;
        //공격 범위 위치
        transform.position = position;
    }

    public void OnAttackRangeMove(float range)
    {
        gameObject.SetActive(true);

        //공격 범위 크기
        float diameter = range * 2.0f;
        transform.localScale = Vector3.one * diameter;
        //공격 범위 따라니기
        isMoveRange = true;
    }

    public void OffAttackRange()
    {
        gameObject.SetActive(false);
        isMoveRange = false;
    }
}
