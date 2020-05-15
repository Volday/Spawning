using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMind : MonoBehaviour
{
    FishGenerator fishGenerator;
    Rigidbody2D rigidbody;
    public float speed;
    Vector3 targetPosition;
    [Range(0, 1)]
    public float smoothing;

    public float flowShift;
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        targetPosition = transform.position;
    }

    private void FixedUpdate()
    {
        Vector3 direction = fishGenerator.GetVectorToMove(transform.position, transform.forward, this);
        targetPosition += direction * speed * Time.fixedDeltaTime;
        targetPosition += Vector3.right * flowShift * Time.deltaTime;
        Vector3 vectorToMove = Vector3.Lerp(transform.position, targetPosition, smoothing);
        rigidbody.MovePosition(vectorToMove);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, transform.forward);
    }

    public void SetFishGenerator(FishGenerator _fishGenerator)
    {
        fishGenerator = _fishGenerator;
    }
}
