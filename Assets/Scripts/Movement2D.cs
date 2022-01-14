using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement2D : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 0.0f;
    [SerializeField]
    private Vector3 moveDirection = Vector3.zero;

    public float distance = 0;

    public float MoveSpeed{
        set => moveSpeed = value;
        get => moveSpeed;
    }

    private void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        distance += moveSpeed * Time.deltaTime;
    }

    public void MoveTo(Vector3 direction)
    {
        moveDirection = direction;
    }
}
