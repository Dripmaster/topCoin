

using Photon.Pun;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    int stageState;
    public static GameManager instance;
    public ClipBoard clipBoard;
    public PutCoin putCoin;
    public GameObject turnPanel;
    public GameObject[] buttons;
    public GameObject LastCoin;

    public GameObject WinObject;
    public GameObject LoseObject;

    public void Awake()
    {
        instance = this;
    }

    public void Update()
    {
        if (turnPanel.gameObject.activeInHierarchy)
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    putCoin.putCoin();
                }
            }

            if (LastCoin != null)
                putCoin.transform.position = LastCoin.transform.GetChild(0).position;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            putCoin.putCoin();
        }
    }

    [PunRPC]
    public void StartStage()
    {
        stageState = 1;
        clipBoard.onGameStart();
        foreach (var item in buttons)
        {
            item.gameObject.SetActive(false);
        }
        putCoin.transform.localPosition = new Vector3(0,0.02f,0);
    }
    [PunRPC]
    public void EndStage()
    {
        stageState = 0;
        clipBoard.onGameEnd();
        turnPanel.SetActive(false);
        foreach (var item in buttons)
        {
            item.gameObject.SetActive(true);
        }

        var Coins = GameObject.FindGameObjectsWithTag("Coin");
        foreach (var item in Coins)
        {
            Destroy(item);
        }
    }

    [PunRPC]
    public void turnOn()
    {
        putCoin.gameObject.SetActive(true);
        turnPanel.SetActive(true);
    }
    
    public void putted()
    {
    }

    public void turnOff()
    {
        turnPanel.SetActive(false);
        putCoin.gameObject.SetActive(false);

        PhotonView photonView = PhotonView.Get(GameManager.instance);
        photonView.RPC("turnOn", RpcTarget.Others);
    }

    public void Lose()
    {
        if (stageState == 0)
            return;
        PhotonView photonView = PhotonView.Get(GameManager.instance);
        photonView.RPC("EndStage", RpcTarget.All);
        LoseObject.SetActive(true);
        StartCoroutine(hidePanel());
    }
    [PunRPC]
    public void Win()
    {
        if (stageState == 0)
            return;
        WinObject.SetActive(true);
        StartCoroutine(hidePanel());
    }


    IEnumerator hidePanel()
    {
        yield return new WaitForSeconds(3);
        WinObject.SetActive(false);
        LoseObject.SetActive(false);
    }
}