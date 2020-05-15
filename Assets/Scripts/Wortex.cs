using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wortex : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    FishGenerator fishGenerator;

    private void Awake()
    {
        fishGenerator = GameObject.FindGameObjectWithTag("Respawn").GetComponent<FishGenerator>();
        fishGenerator.wortexes.Add(this);
    }
    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
        transform.Rotate(new Vector3(0, 0, -1) * rotationSpeed * Time.deltaTime);
        if (((float)Screen.width / Screen.height) * fishGenerator.cameraSize + 10 < transform.position.x) {
            fishGenerator.wortexes.Remove(this);
            Destroy(gameObject);
        }
    }
}
