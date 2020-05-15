using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishGenerator : MonoBehaviour
{
    public int count;
    public GameObject fish;
    public List<FishMind> fishes = new List<FishMind>();
    [HideInInspector]public float cameraSize;
    public float radiusForCentreOfMass = 1;
    public float radiusForPush = 1;
    public float radiusForGoWith = 1;
    public float radiusForPushFromCursor = 1;
    [Range(0, 1)]
    public float centreMassCoef;
    [Range(0, 1)]
    public float mouseCoef;
    [Range(0, 1)]
    public float pushCoef;
    [Range(0, 1)]
    public float goWithCoef;
    [Range(0, 1)]
    public float pushFromCursorCoef;
    [Range(0, 2)]
    public float currentDirectionCoef;
    [Range(0, 2)]
    public float wortexCoef;

    public List<Wortex> wortexes = new List<Wortex>();
    public GameObject wortex;
    public float wortexCooldown;
    public float wertexRadius;
    float currentWortexCooldown = 10;
    public float wortexCooldownDispersion;

    public GameObject fishPlus;
    public float fishPlusCooldown;
    float currentFishPlusCooldown = 5;
    public float fishPlusCooldownDispersion;

    public GameObject predator;
    public float predatorCooldown;
    float currentPredatorCooldown = 5;
    public float predatorCooldownDispersion;

    public Text count1;
    public Text count2;

    public GameObject ok;
    void Start()
    {
        cameraSize = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().orthographicSize;
        ok.transform.position = new Vector3(((float)Screen.width / Screen.height) * cameraSize - 3
            , cameraSize - 3, 0);
        for (int t = 0; t < count; t++)
        {
            fishes.Add(Instantiate(fish.gameObject, new Vector3(Random.Range(-0.1f, 0.1f),
                Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f)) + transform.position,
                Quaternion.identity).GetComponent<FishMind>());
            fishes[t].SetFishGenerator(this);
            fishes[t].GetComponent<SpriteRenderer>().sortingOrder = Random.Range(10, 39);
        }
    }

    private void Update()
    {
        count1.text = " " +fishes.Count + "/100" + " ";
        count2.text = " " + fishes.Count + "/100" + " ";
        if (fishes.Count > 99) {
            ok.SetActive(true);
        }
        if (fishes.Count == 0) {
            for (int t = 0; t < count; t++)
            {
                fishes.Add(Instantiate(fish.gameObject, new Vector3(Random.Range(-0.1f, 0.1f),
                    Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f)) + transform.position,
                    Quaternion.identity).GetComponent<FishMind>());
                fishes[t].SetFishGenerator(this);
                fishes[t].GetComponent<SpriteRenderer>().sortingOrder = Random.Range(10, 39);
            }
        }
        //WertexSpawner
        currentWortexCooldown -= Time.deltaTime;
        if (currentWortexCooldown < 0) {
            Instantiate(wortex, new Vector3((-(float)Screen.width / Screen.height) * cameraSize - 10
                , Random.Range((-(float)Screen.height / Screen.width) * cameraSize, ((float)Screen.width / Screen.height) * cameraSize)
                , 0), Quaternion.identity);
            currentWortexCooldown = wortexCooldown + Random.Range(-wortexCooldownDispersion, wortexCooldownDispersion);
        }

        //fishPlusSpawner
        currentFishPlusCooldown -= Time.deltaTime;
        if (currentFishPlusCooldown < 0)
        {
            Instantiate(fishPlus, new Vector3((-(float)Screen.width / Screen.height) * cameraSize - 10
                , Random.Range((-(float)Screen.height / Screen.width) * cameraSize, ((float)Screen.width / Screen.height) * cameraSize)
                , 0), Quaternion.identity);
            currentFishPlusCooldown = fishPlusCooldown + Random.Range(-fishPlusCooldownDispersion, fishPlusCooldownDispersion);
        }

        //PredatorSpawner
        currentPredatorCooldown -= Time.deltaTime;
        if (currentPredatorCooldown < 0)
        {
            float angle = Random.Range(0, 360);
            Vector3 direction = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle), 0);
            Instantiate(predator
                , direction * (((float)Screen.width / Screen.height) * cameraSize + 10)
                , Quaternion.identity);
            currentPredatorCooldown = predatorCooldown + Random.Range(-predatorCooldownDispersion, predatorCooldownDispersion);
        }
    }

    public Vector3 GetVectorToMove(Vector3 _position, Vector3 _currentDirection, FishMind _fish)
    {
        Vector3 result;
        Vector3 cursorPosition = GetCursorPosition();
        if (isSomeBodyClosed(_position, out result))
        {
            result *= pushCoef;
        }
        result += (FindCentreOfMassInRadiusOfSprites(_position) - _position).normalized * centreMassCoef
            + (cursorPosition - _position).normalized * mouseCoef
            + PushFromCursor(_position, cursorPosition) * pushFromCursorCoef
            + MovingDirectionofFlock(_position, radiusForGoWith) * goWithCoef 
            + PushToWertex(_position, _fish) * wortexCoef;
        result = result.normalized;
        result += _currentDirection * currentDirectionCoef;
        result = result.normalized;
        return result;
    }

    Vector3 PushToWertex(Vector3 _position, FishMind _fish) {
        Vector3 result = Vector3.zero;
        for (int t = 0; t < wortexes.Count; t++) {
            float distance = sqrDistanceFromPointToPoint(wortexes[t].transform.position, _position);
            if (distance < wertexRadius * wertexRadius) {
                result += wortexes[t].transform.position - _position;
                if (distance < 1) {
                    fishes.Remove(_fish);
                    Destroy(_fish.gameObject);
                }
            }
        }
        result = result.normalized;
        return result;
    }

    Vector3 PushFromCursor(Vector3 _position, Vector3 _cursorPosition)
    {
        Vector3 result = Vector3.zero;
        if (sqrDistanceFromPointToPoint(_position, _cursorPosition) < radiusForPushFromCursor * radiusForPushFromCursor)
        {
            result = (_position - _cursorPosition).normalized;
        }
        return result;
    }

    Vector3 MovingDirectionofFlock(Vector3 _position, float _radius)
    {
        Vector3 result = Vector3.zero;
        int count = 0;
        for (int t = 0; t < fishes.Count; t++)
        {
            if (sqrDistanceFromPointToPoint(_position, fishes[t].gameObject.transform.position) < _radius * _radius)
            {
                result += fishes[t].gameObject.transform.right;
                count++;
            }
        }
        result = result.normalized;
        return result;
    }

    Vector3 GetCursorPosition()
    {
        return new Vector3(
            (cameraSize * 2 / Screen.width) * ((float)Screen.width / Screen.height)
            * (Input.mousePosition.x - Screen.width / 2),
            (cameraSize * 2 / Screen.height)
            * (Input.mousePosition.y - Screen.height / 2), 0);
    }

    public Vector3 FindCentreOfMassOfFishes()
    {
        Vector3 result = Vector3.zero;
        for (int t = 0; t < fishes.Count; t++)
        {
            result += fishes[t].transform.position;
        }
        result /= fishes.Count;
        return result;
    }

    Vector3 FindCentreOfMassInRadiusOfSprites(Vector3 _position)
    {
        Vector3 result = Vector3.zero;
        for (int t = 0; t < fishes.Count; t++)
        {
            if (sqrDistanceFromPointToPoint(fishes[t].transform.position, _position)
                < radiusForCentreOfMass * radiusForCentreOfMass)
            {
                result += fishes[t].transform.position;
            }
        }
        result /= fishes.Count;
        return result;
    }

    bool isSomeBodyClosed(Vector3 _position, out Vector3 _moveingVector)
    {
        _moveingVector = Vector3.zero;
        for (int t = 0; t < fishes.Count; t++)
        {
            if (_position != fishes[t].transform.position)
            {
                if (sqrDistanceFromPointToPoint(_position, fishes[t].transform.position) < radiusForPush * radiusForPush)
                {
                    _moveingVector += (_position - fishes[t].transform.position).normalized;
                }
            }
        }
        if (_moveingVector == Vector3.zero)
        {
            _moveingVector = _moveingVector.normalized;
            return false;
        }
        else
        {
            _moveingVector = _moveingVector.normalized;
            return true;
        }
    }

    public float sqrDistanceFromPointToPoint(Vector3 _firstPoint, Vector3 _secondPoint)
    {
        _firstPoint = new Vector3((_firstPoint.x - _secondPoint.x), (_firstPoint.y - _secondPoint.y), (_firstPoint.z - _secondPoint.z));
        return _firstPoint.x * _firstPoint.x + _firstPoint.y * _firstPoint.y + _firstPoint.z * _firstPoint.z;
    }
}
