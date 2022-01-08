using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectDetector : MonoBehaviour//, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private TowerSpawner towerSpawner;

    private Vector3 previousPosition;
    private SpriteRenderer spriteRenderer;

    private bool isDragging;

    private Camera mainCamera;
    private Ray ray;
    private RaycastHit hit;

    private void Awake()
    {
        // "MainCamera" 태그를 가지고 있는 오브젝트 탐색후 Camera 컴포넌트 정보 전달
        // GameObject.FindGameOjbectWithTag("MainCamera").GetComponent<Camera>();�� ����
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /*private void Update()
    {
        // 마우스 왼쪽 버튼을 눌렀을 때
        if (Input.GetMouseButtonDown(0))
        {
            // 카메라 위치에서 화면의 마우스 위치를 관통하는 광선 생성
            // ray.origin : 광선의 시작위치(=카메라 위치)
            // ray.direction : 광선의 진행방향
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // 2D 모니터를 통해 3D월드의 오브젝트를 마우스로 선택하는 방법
            // 광선에 부딪히는 오브젝트 hit에 저장
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                // 광선에 부딪힌 오브젝트의 태그가 "Tile"이면
                if (hit.transform.CompareTag("Tile"))
                {
                    // 타워를 생성하는 SpawnTower() 호출
                    towerSpawner.SpawnTower(hit.transform);
                }
            }
        }
    }*/
    /*public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 직전에 있던 위치 정보 저장
        previousPosition = transform.position;

        // 오브젝트의 투명도 조절
        Color color = spriteRenderer.color;
        color.a = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 현재 스크린상의 마우스 위치를 오브젝트 위치로 설정
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // 2D 모니터를 통해 3D월드의 오브젝트를 마우스로 선택하는 방법
        // 광선에 부딪히는 오브젝트 hit에 저장
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // 광선에 부딪힌 오브젝트의 태그가 "Tile"이 아니면
            if (!(hit.transform.CompareTag("Tile")))
            {
                transform.position = previousPosition;
                Color color = spriteRenderer.color;
                color.a = 1.0f;
            }
        }
    }*/
    
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
    }
}
