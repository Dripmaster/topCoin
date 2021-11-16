using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Coin : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback, IPunObservable
{
    bool firstCoin;
    public bool lastCoin;
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = this.gameObject.GetPhotonView().InstantiationData;

        Vector3 pos = (Vector3)data[0];

        Vector3 myPos = GameManager.instance.putCoin.transform.position + pos;
        transform.position = myPos;
        gameObject.SetActive(true);
        firstCoin = false;
        lastCoin = true;
        if (GameManager.instance.LastCoin == null)
            firstCoin = true;
        else
        {
            GameManager.instance.LastCoin.GetComponent<Coin>().lastCoin = false;
        }
        GameManager.instance.LastCoin = gameObject;
        GameManager.instance.putted();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.name == "Board"&& !firstCoin && lastCoin)
        {
            if (photonView.IsMine)
            {

                PhotonView photonView = PhotonView.Get(GameManager.instance);
                photonView.RPC("Win", RpcTarget.Others);

                GameManager.instance.Lose();
            }
            else
            {
            }
        }
    }
}
