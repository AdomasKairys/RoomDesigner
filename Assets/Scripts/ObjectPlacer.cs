using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    private List<GameObject> _placedObjects = new();

    public int PlaceObject(GameObject prefab, Transform preview)
    {
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = preview.position;
        newObject.transform.rotation = preview.rotation;

        _placedObjects.Add(newObject);
        return _placedObjects.Count-1;
    }
}
