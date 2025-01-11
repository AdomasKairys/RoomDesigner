using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    private List<(GameObject, Color, int)?> _placedObjects = new();

    public IReadOnlyCollection<(GameObject, Color, int)?> GetPlacedObjects() => _placedObjects;
    public int PlaceObject(GameObject prefab, Vector3 position, Quaternion rotation, Color color, int id)
    {
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = position;
        newObject.transform.rotation = rotation;
        newObject.layer = LayerMask.NameToLayer("Furniture");
        var children = newObject.GetComponentsInChildren<Transform>(true).Select(t => t.gameObject).ToList();
        children.ForEach(c => c.layer = newObject.layer);
        ChangeColor(newObject, color);
        _placedObjects.Add((newObject, color, id));
        return _placedObjects.Count - 1;
    }
    public void RemoveObjectAt(int index)
    {
        if (!IsIndexValid(index, $"Game object not found at index: {index}"))
            return;

        Destroy(_placedObjects[index].Value.Item1);

        _placedObjects[index] = null;
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
    public void RemoveAllObjects()
    {
        foreach (var obj in _placedObjects)
        {
            if (obj.HasValue)
                Destroy(obj.Value.Item1);
        }
        _placedObjects = new();
    }
    public void ChangeObjectLayerByIndex(int index, int layer)
    {
        if (!IsIndexValid(index, $"Game object not found at index: {index}"))
            return;

        _placedObjects[index].Value.Item1.layer = layer;
        var children = _placedObjects[index].Value.Item1.GetComponentsInChildren<Transform>(true).Select(t => t.gameObject).ToList();
        children.ForEach(c => c.layer = layer);
    }
    public Color GetColorByIndex(int index)
    {
        if(!IsIndexValid(index, $"Game object color not found at index: {index}"))
            return Color.black;

        return _placedObjects[index].Value.Item2;
    }
    public GameObject GetGameObjectByIndex(int index)
    {
        if (!IsIndexValid(index, $"Game object not found at index: {index}"))
            return null;

        return _placedObjects[index].Value.Item1;
    }
    public void ChangeColorByIndex(int index, Color color)
    {
        if (!IsIndexValid(index, $"Game object not found at index: {index}"))
            return;

        _placedObjects[index] = (_placedObjects[index].Value.Item1,color, _placedObjects[index].Value.Item3);
        ChangeColor(_placedObjects[index].Value.Item1, color);
    }
    private bool IsIndexValid(int index, string errorMessage = "")
    {
        if (index < 0 || _placedObjects.Count <= index || !_placedObjects[index].HasValue)
        {
            Debug.LogError(errorMessage);
            return false;
        }
        return true;
    }
    
    private void ChangeColor(GameObject gameObject, Color color)
    {
        MaterialPropertyBlock selected = new MaterialPropertyBlock();
        selected.SetColor("_BaseColor", color);
        MeshRenderer[] renderers = gameObject.GetComponentsInChildren<MeshRenderer>();

        foreach (var renderer in renderers)
            renderer.SetPropertyBlock(selected);
    }
    
}
