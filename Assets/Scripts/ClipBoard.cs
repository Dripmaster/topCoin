using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ClipBoard : MonoBehaviour
{
    private ARRaycastManager rayManager;
    public GameObject visual;
    public GameObject range;

    bool canMove;
    void Start()
    {
        canMove = true;
        //AR Component를 받아온다.
        rayManager = FindObjectOfType<ARRaycastManager>();
        visual = transform.GetChild(0).gameObject;

        //indicatior를 deactivate한다.
        visual.SetActive(false);
        range.SetActive(false);
    }

    void Update()
    {
        if (!canMove) return;
        // 화면 중앙에서 ray를 쏜다
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        rayManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes);


        if (hits.Count > 0) //만약 ray가  ar plane과 부딪친다면 position과 rotation을 얻는다
        {
            transform.position = hits[0].pose.position;
            transform.rotation = hits[0].pose.rotation;

            //만약 active가 꺼져있다면 true로 바꾼다.
            if (!visual.activeInHierarchy)
            {
                visual.SetActive(true);
            }

        }
    }
    public void onGameStart()
    {
        canMove = false;
        range.SetActive(true);
    }
    public void onGameEnd()
    {
        canMove = true;
        range.SetActive(false);
    }
}
