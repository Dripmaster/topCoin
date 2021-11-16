using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PutCoin : MonoBehaviour
{
    public GameObject goldCoinPrefab;
    public GameObject silverPrefab;
    GameObject coinPrefab;
    Vector3 tmpScale;
    float scaler;

    public float scaleSpeed =1f;

    float range;

    // Start is called before the first frame update
    void Awake()
    {
        tmpScale = transform.localScale;
        scaler = 1f;
        range = transform.GetChild(0).localScale.x;
    }
    // Update is called once per frame
    void Update()
    {
        scaler += Time.deltaTime* scaleSpeed;
        if (scaler >= 1.5f)
        {
            scaler = 1.5f;
            scaleSpeed *= -1;
        }
        if (scaler <= 0.1f)
        {
            scaler = 0.1f;
            scaleSpeed *= -1;
        }

        transform.localScale = tmpScale * scaler;

    }
    public void putCoin()
    {

        if (PhotonNetwork.IsMasterClient)
            coinPrefab = goldCoinPrefab;
        else
            coinPrefab = silverPrefab;

        float r = range * scaler;
        Vector3 pos = new Vector3(Random.Range(-r,r),0.2f, Random.Range(-r, r));

        object[] data = new object[1];
        data[0] = pos;

        PhotonNetwork.Instantiate(coinPrefab.name,Vector3.zero,Quaternion.identity,0,data);

        GameManager.instance.turnOff();
    }
}