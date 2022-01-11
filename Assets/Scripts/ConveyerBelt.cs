using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerBelt : MonoBehaviour
{
    public float speed;

    private void Update()
    {
        if (transform.CompareTag("TowerBelt"))
        {
            transform.position += speed * (new Vector3(0.1f, 0, 0));
            if (transform.position.x > 6.5f) Destroy(gameObject);
        }
    }
}
