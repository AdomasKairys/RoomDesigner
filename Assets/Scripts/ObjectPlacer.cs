using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    private List<GameObject> _placedObjects = new();

    public int PlaceObject(GameObject prefab, Transform preview)
    {
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = preview.position;
        newObject.transform.rotation = preview.rotation;
        newObject.layer = LayerMask.NameToLayer("Furniture");
        var children = newObject.GetComponentsInChildren<Transform>(true).Select(t => t.gameObject).ToList();
        children.ForEach(c => c.layer = newObject.layer);

        _placedObjects.Add(newObject);
        return _placedObjects.Count-1;
    }

    internal void RemoveObjectAt(int selectedObjectIndex)
    {
        if(_placedObjects.Count <= selectedObjectIndex || _placedObjects[selectedObjectIndex] == null)
            return;
        Destroy(_placedObjects[selectedObjectIndex]);
        _placedObjects[selectedObjectIndex] = null;
    }
}
