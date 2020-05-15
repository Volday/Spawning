using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMovement : MonoBehaviour
{
    public float speed;
    float currentOffset;
    public float scale;
    bool getCoppy = false;

    private void Update()
    {
        currentOffset = Time.deltaTime * speed;
        transform.position += new Vector3(currentOffset, 0, 0);

        if (transform.position.x > 0 && !getCoppy)
        {
            Instantiate(gameObject, transform.position + new Vector3(-scale, 0, 0)
            , Quaternion.identity).GetComponent<WaterMovement>();
            getCoppy = true;
        }

        if (transform.position.x > scale)
        {
            Destroy(gameObject);
        }
    }
}
