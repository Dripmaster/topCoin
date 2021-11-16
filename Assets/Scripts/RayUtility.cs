using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/*
 레이케스트를 도와주는 클래스
 */
public class RayUtility
{
    private static ARRaycastManager raycastManager;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    public static bool isOnUI()
    {//터치 포인트가 UI위에 있는지 확인해준다.
        List<RaycastResult> results = new List<RaycastResult>();
        PointerEventData ped = new PointerEventData(GameObject.Find("EventSystem").GetComponent<EventSystem>());
        ped.position = Input.GetTouch(0).position;
        GameObject.Find("Canvas").GetComponent<GraphicRaycaster>().Raycast(ped, results);
        if (results.Count > 0)
        {
            return true;
        }
        return false;
    }

    static RayUtility()
    {//참조 초기화
        raycastManager = GameObject.FindObjectOfType<ARRaycastManager>();
    }
    public static bool ARRaycast(Vector2 screenPosition, out ARRaycastHit hit)
    {//AR 레이캐스트
        if (raycastManager.Raycast(screenPosition, hits, TrackableType.Planes))
        {
            hit = hits[0];
            return true;
        }
        else
        {
            hit = new ARRaycastHit();
            return false;
        }
    }
    public static bool Raycast(Vector2 screenPosition, out RaycastHit hit)
    {//보통 레이캐스트
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out hit, 100000, ~(1 << LayerMask.NameToLayer("myAvatar"))))
        {
            return true;
        }
        return false;
    }
    public static bool TryGetInputPosition(out Vector2 touchPosition)
    {//터치가 있었는지 확인해준다.
        touchPosition = Vector2.zero;

        if (Input.touchCount == 0)
        {
            return false;
        }

        touchPosition = Input.GetTouch(0).position;
        return true;
    }
}
