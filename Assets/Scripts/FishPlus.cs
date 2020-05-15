using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishPlus : MonoBehaviour
{
    FishGenerator fishGenerator;
    public float pickUpRadius;
    public int fishCount;
    public float speed;

    void Start()
    {
        fishGenerator = GameObject.FindGameObjectWithTag("Respawn").GetComponent<FishGenerator>();
    }

    void Update()
    {
        bool destroy = false;
        for (int t = 0; t < fishGenerator.fishes.Count; t++) {
            if (fishGenerator.sqrDistanceFromPointToPoint(fishGenerator.fishes[t].transform.position, transform.position) 
                < pickUpRadius * pickUpRadius) {
                destroy = true;
                break;
            }
        }

        if (destroy) {
            for (int i = 0; i < fishCount; i++)
            {
                GameObject newFish = Instantiate(fishGenerator.fish, transform.position
                    + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f))
                    , Quaternion.identity);
                newFish.GetComponent<FishMind>().SetFishGenerator(fishGenerator);
                newFish.GetComponent<SpriteRenderer>().sortingOrder = Random.Range(10, 39);
                fishGenerator.fishes.Add(newFish.GetComponent<FishMind>());
            }
            Destroy(gameObject);
        }

        transform.position += Vector3.right * speed * Time.deltaTime;

        if (((float)Screen.width / Screen.height) * fishGenerator.cameraSize + 10 < transform.position.x)
        {
            Destroy(gameObject);
        }
    }
}
