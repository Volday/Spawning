using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predator : MonoBehaviour
{
    FishGenerator fishGenerator;
    public float pickUpRadius;
    bool getTarget = false;
    public float getTargetRadius;
    public float smoothing;
    Rigidbody2D rigidbody;
    public float speed;
    Vector3 targetPosition;
    float deathRadius;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        fishGenerator = GameObject.FindGameObjectWithTag("Respawn").GetComponent<FishGenerator>();
        deathRadius = (((float)Screen.width / Screen.height) * fishGenerator.cameraSize + 40)
            * (((float)Screen.width / Screen.height) * fishGenerator.cameraSize + 40);
        Vector3 target = fishGenerator.FindCentreOfMassOfFishes();
        Vector3 direction = target - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, transform.forward);
        targetPosition = transform.position;
    }
    void FixedUpdate()
    {
        if (!getTarget)
        {
            Vector3 target = fishGenerator.FindCentreOfMassOfFishes();
            Vector3 direction = (target - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, transform.forward);

            targetPosition += direction * speed * Time.fixedDeltaTime;
            Vector3 vectorToMove = Vector3.Lerp(transform.position, targetPosition, smoothing);
            rigidbody.MovePosition(vectorToMove);

            if (fishGenerator.sqrDistanceFromPointToPoint(transform.position, target)
            < getTargetRadius * getTargetRadius)
            {
                getTarget = true;
            }
        }
        else {
            Vector3 targetVector = transform.position + transform.right * speed * Time.fixedDeltaTime;
            Debug.Log(transform.right);
            rigidbody.MovePosition(targetVector);
        }

        if (fishGenerator.sqrDistanceFromPointToPoint(transform.position, Vector3.zero) 
            > deathRadius) {
            Destroy(gameObject);
        }

        for (int t = 0; t < fishGenerator.fishes.Count; t++)
        {
            if (fishGenerator.sqrDistanceFromPointToPoint(fishGenerator.fishes[t].transform.position, transform.position)
                < pickUpRadius * pickUpRadius)
            {
                FishMind fishMind = fishGenerator.fishes[t];
                fishGenerator.fishes.Remove(fishMind);
                Destroy(fishMind.gameObject);
            }
        }
    }
}
