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
    [SerializeField]
    private int towerBuyGold = 50; // 타워 구매에 사용되는 골드
    [SerializeField]
    private PlayerGold playerGold; // 타워 구매시 골드 감소를 위해
    public GameObject[] slots;

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
                    towerDataViewer.OnPanel(hit.transform);
                }
                else if (hit.transform.CompareTag("TowerBelt"))
                {
                    int i;
                    // 타워를 살 만큼 돈이 없으면 타워 건설 X
                    if (towerBuyGold > playerGold.CurrentGold)
                    {
                        return;
                    }
                    for (i = 0; i < slots.Length; i++)
                    {
                        if (slots[i].GetComponent<Slot>().fullCheck == false)
                        {
                            hit.transform.SetParent(slots[i].transform);
                            slots[i].GetComponent<Slot>().fullCheck = true;
                            // 위치 이동
                            hit.transform.position = slots[i].transform.position;
                            break;
                        }
                    }
                    if (i == slots.Length)
                    {
                        return;
                    }

                    hit.transform.gameObject.tag = "TowerUI";
                    // 타워 구매에 필요한 골드만큼 감소
                    playerGold.CurrentGold -= towerBuyGold;
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
                        towerDataViewer.OffPanel();
                        color.a = 1f;
                        spriteRenderer.color = color;
                    }
                    else
                    {
                        hitGameObject.transform.parent.GetComponent<Slot>().fullCheck = false;
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
}
