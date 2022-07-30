using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Helper
{

    #region Main Camera Refrencing

    private static Camera mainCamera;
    public static Camera MainCamera
    {
        get
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
            return mainCamera;
        }
    }

    #endregion

    #region Getting Await Time To Reduce Garbage Allocation

    private static readonly Dictionary<float, WaitForSeconds> waitDict
         = new Dictionary<float, WaitForSeconds>();
    public static WaitForSeconds WaitFor(float time)
    {
        if (waitDict.TryGetValue(time, out var await)) return await;

        waitDict[time] = new WaitForSeconds(time);
        return waitDict[time];
    }

    #endregion

    #region Checking If Cursor Is Over Any UI

    public static PointerEventData eventData;
    public static List<RaycastResult> results;

    public static bool IsCursorOverUI()
    {
        eventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    #endregion

    #region Getting World Position With Respect To UI Element

    public static Vector2 GetWorlPosition(RectTransform rectTransform)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, 
            rectTransform.position, MainCamera, out var worldPos);
        return worldPos;
    }

    #endregion

    #region Deleting All Childrens Of Any Transform

    public static void ClearChildren(this Transform itemTransform)
    {
        foreach(Transform child in itemTransform)
        {
            Object.Destroy(child.gameObject);
        }
    }

    #endregion

    #region Make Object Look Towards Other Object 

    public static void LookRotation(this Transform source, Transform target, float lookSpeed)
    {
        source.rotation = Quaternion.Slerp(source.rotation, Quaternion.LookRotation
            (target.position - source.position, Vector3.up), Time.deltaTime * lookSpeed);
    }

    public static void LookRotationX(this Transform source, Transform target, float lookSpeed)
    {
        Vector3 originalRotation = source.eulerAngles;
        source.rotation = Quaternion.Slerp(source.rotation, Quaternion.LookRotation
            (target.position - source.position, Vector3.up), Time.deltaTime * lookSpeed);
        source.eulerAngles = new Vector3(source.eulerAngles.x, originalRotation.y, originalRotation.z);
    }

    public static void LookRotationY(this Transform source, Transform target, float lookSpeed)
    {
        Vector3 originalRotation = source.eulerAngles;
        source.rotation = Quaternion.Slerp(source.rotation, Quaternion.LookRotation
            (target.position - source.position, Vector3.up), Time.deltaTime * lookSpeed);
        source.eulerAngles = new Vector3(originalRotation.x, source.eulerAngles.y, originalRotation.z);
    }

    public static void LookRotationZ(this Transform source, Transform target, float lookSpeed)
    {
        Vector3 originalRotation = source.eulerAngles;
        source.rotation = Quaternion.Slerp(source.rotation, Quaternion.LookRotation
            (target.position - source.position, Vector3.up), Time.deltaTime * lookSpeed);
        source.eulerAngles = new Vector3(originalRotation.x, originalRotation.y, source.eulerAngles.z);
    }

    #endregion

    #region Make Object Rotate Towards Other Object

    public static void RotateTowardsTarget(this Transform source, Transform target, float speed)
    {
        Vector3 newDirection = Vector3.RotateTowards(source.forward,
            target.position - source.position, speed * Time.deltaTime, 0.0f);
        source.rotation = Quaternion.LookRotation(newDirection);
    }

    public static void RotateTowardsTargetX(this Transform source, Transform target, float speed)
    {
        Vector3 originalRotation = source.eulerAngles;
        Vector3 newDirection = Vector3.RotateTowards(source.forward,
            target.position - source.position, speed * Time.deltaTime, 0.0f);
        source.rotation = Quaternion.LookRotation(newDirection);
        source.eulerAngles = new Vector3(source.eulerAngles.x, originalRotation.y, originalRotation.z);
    }

    public static void RotateTowardsTargetY(this Transform source, Transform target, float speed)
    {
        Vector3 originalRotation = source.eulerAngles;
        Vector3 newDirection = Vector3.RotateTowards(source.forward,
            target.position - source.position, speed * Time.deltaTime, 0.0f);
        source.rotation = Quaternion.LookRotation(newDirection);
        source.eulerAngles = new Vector3(source.eulerAngles.x, originalRotation.y, originalRotation.z);
    }

    public static void RotateTowardsTargetZ(this Transform source, Transform target, float speed)
    {
        Vector3 originalRotation = source.eulerAngles;
        Vector3 newDirection = Vector3.RotateTowards(source.forward,
            target.position - source.position, speed * Time.deltaTime, 0.0f);
        source.rotation = Quaternion.LookRotation(newDirection);
        source.eulerAngles = new Vector3(source.eulerAngles.x, originalRotation.y, originalRotation.z);
    }

    #endregion

    #region Check If Object Looking Towards Other

    public static bool IsLooking(this Transform source, Transform target)
    {
        return !(Vector3.Dot(source.position, target.position) < 0.7f);
    }

    #endregion

}

public class Generics<T>
{

    #region Randomizing List

    public static List<T> Randomize(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;
    }

    #endregion

}