using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectDetector : MonoBehaviour//, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private TowerSpawner towerSpawner;
    [SerializeField]
    private TowerDataViewer towerDataViewer;

    private Vector3 previousPosition;
    private GameObject hitGameObject; 
    private SpriteRenderer spriteRenderer;
    private Color color;

    private bool isDragging;

    private Camera mainCamera;
    private Ray ray;
    private RaycastHit hit;

    private void Awake()
    {
        // "MainCamera" 태그를 가지고 있는 오브젝트 탐색후 Camera 컴포넌트 정보 전달
        // GameObject.FindGameOjbectWithTag("MainCamera").GetComponent<Camera>();�� ����
        mainCamera = Camera.main;
    }

    private void Update()
    {
        ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if ((hit.transform.CompareTag("TowerUI")))
                {
                    isDragging = true;
                    hitGameObject = hit.transform.gameObject;
                    previousPosition = hit.transform.position;
                    spriteRenderer = hitGameObject.GetComponent<SpriteRenderer>();
                    color = spriteRenderer.color;
                    //towerDataViewer.OnPanel();
                }
            }
        }
        if (isDragging)
        {
            color.a = 0.3f;
            spriteRenderer.color = color;
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition) - hitGameObject.transform.position;
            hitGameObject.transform.Translate(mousePosition,.0f);
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;

                // 2D 모니터를 통해 3D월드의 오브젝트를 마우스로 선택하는 방법
                // 광선에 부딪히는 오브젝트 hit에 저장
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    // 광선에 부딪힌 오브젝트의 태그가 "Tile"이 아니면
                    if (!(hit.transform.CompareTag("Tile")))
                    {
                        hitGameObject.transform.position = previousPosition;
                    }
                    else
                    {
                        Destroy(hitGameObject);
                        towerSpawner.SpawnTower(hit.transform);
                        towerDataViewer.OffPanel();
                    }
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.transform.CompareTag("Tower"))
                {
                    towerDataViewer.OnPanel(hit.transform);
                }
                else
                {
                    towerDataViewer.OffPanel();
                }
            }
        }
    }
    /*
    public void OnMouseDown()
    {
        isDragging = true;
        previousPosition = transform.position;
    }
    public void OnMouseUp()
    {
        isDragging = false;
        ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // 2D 모니터를 통해 3D월드의 오브젝트를 마우스로 선택하는 방법
        // 광선에 부딪히는 오브젝트 hit에 저장
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // 광선에 부딪힌 오브젝트의 태그가 "Tile"이 아니면
            if (!(hit.transform.CompareTag("Tile")))
            {
                transform.position = previousPosition;
            }
            else
            {
                Destroy(gameObject);
                towerSpawner.SpawnTower(hit.transform);
            }
        }

    }
    private void Update()
    {
        Color color = spriteRenderer.color;
        if (isDragging) {
            color.a = 0.1f;
            spriteRenderer.color = color;
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            transform.Translate(mousePosition, .0f);
        }
        color.a = 1.0f;
        spriteRenderer.color = color;
    }*/
}
