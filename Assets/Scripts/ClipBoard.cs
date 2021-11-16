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
        //AR Component�� �޾ƿ´�.
        rayManager = FindObjectOfType<ARRaycastManager>();
        visual = transform.GetChild(0).gameObject;

        //indicatior�� deactivate�Ѵ�.
        visual.SetActive(false);
        range.SetActive(false);
    }

    void Update()
    {
        if (!canMove) return;
        // ȭ�� �߾ӿ��� ray�� ���
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        rayManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes);


        if (hits.Count > 0) //���� ray��  ar plane�� �ε�ģ�ٸ� position�� rotation�� ��´�
        {
            transform.position = hits[0].pose.position;
            transform.rotation = hits[0].pose.rotation;

            //���� active�� �����ִٸ� true�� �ٲ۴�.
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
