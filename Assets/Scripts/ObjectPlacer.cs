using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    private List<(GameObject, Color,int)?> _placedObjects = new();
    private List<Material> _placedObjectMaterials = new();
    public IReadOnlyCollection<(GameObject, Color, int)?> GetPlacedObjects() => _placedObjects;
    public int PlaceObject(GameObject prefab, Transform preview, Color color, int id)
    {
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = preview.position;
        newObject.transform.rotation = preview.rotation;
        newObject.layer = LayerMask.NameToLayer("Furniture");
        var children = newObject.GetComponentsInChildren<Transform>(true).Select(t => t.gameObject).ToList();
        children.ForEach(c => c.layer = newObject.layer);

        ChangeColor(newObject, color);
        _placedObjects.Add((newObject,color,id));
        return _placedObjects.Count-1;
    }
    public void RemoveObjectAt(int selectedObjectIndex)
    {
        if(_placedObjects.Count <= selectedObjectIndex || !_placedObjects[selectedObjectIndex].HasValue)
            return;
        Destroy(_placedObjects[selectedObjectIndex].Value.Item1);
        _placedObjects[selectedObjectIndex] = null;
    }
    public void LoadObjects(SaveData data, ObjectDatabaseSO database)
    {
        RemoveAllObjects();
        var placedData = data.placedObjectsData;
        foreach (var obj in placedData)
        {
            int index = database.objectsData.FindIndex((x) => obj.Id == x.Id);
            GameObject gameObject = database.objectsData[index].Prefab;
            PlaceObject(gameObject, obj.position, obj.rotation, obj.Color, obj.Id);
        }
    }
    private void PlaceObject(GameObject prefab, Vector3 position, Quaternion rotation, Color color, int id)
    {
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = position;
        newObject.transform.rotation = rotation;
        newObject.layer = LayerMask.NameToLayer("Furniture");
        var children = newObject.GetComponentsInChildren<Transform>(true).Select(t => t.gameObject).ToList();
        children.ForEach(c => c.layer = newObject.layer);
        ChangeColor(newObject, color);
        _placedObjects.Add((newObject,color,id));
    }
    private void ChangeColor(GameObject gameObject, Color color)
    {
        Material selected = null;
        foreach (var material in _placedObjectMaterials)
            if (material.color == color)
            {
                selected = material;
                break;
            }
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        if (selected == null)
        {
            selected = new Material(renderers[0].material)
            {
                color = color
            };
        }

        foreach (var renderer in renderers)
            renderer.materials = renderer.materials.Select((_) => selected).ToArray();

        _placedObjectMaterials.Add(selected);
    }
    private void RemoveAllObjects()
    {
        foreach(var obj in _placedObjects)
        {
            if(obj.HasValue)
                Destroy(obj.Value.Item1);
        }
        _placedObjects = new();
    }
}
