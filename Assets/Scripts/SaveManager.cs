using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PlacedObjectData
{
    public Vector3 position;
    public Quaternion rotation;
    public Quaternion bodyRotation;
    public Color Color;
    public int Id;

    public PlacedObjectData(GameObject go, Color color, int id)
    {
        position = go.transform.position;
        rotation = go.transform.rotation;
        Color = color;
        Id = id;
        bodyRotation = Quaternion.identity;
        var previewBody = go.transform.Find("Body");
        if (previewBody != null)
            bodyRotation = previewBody.rotation;

    }
}
[System.Serializable]
public class SerializableGridData
{
    public List<Vector3Int> placementDataKey = new();
    public List<PlacementData> placementDataValue = new();
    public SerializableGridData(GridData data)
    {
        var placedObjets = data.placedObjects;
        foreach (var obj in placedObjets)
        {
            placementDataKey.Add(obj.Key);
            placementDataValue.Add(obj.Value);
        }
    }

    public GridData ToGridData()
    {
        GridData data = new();
        data.placedObjects = placementDataKey.Zip(placementDataValue, (k, v) => new KeyValuePair<Vector3Int, PlacementData>(k, v)).ToDictionary((k) => k.Key, (v) => v.Value);
        return data;
    }
}
[System.Serializable]
public class SaveData
{
    public List<PlacedObjectData> placedObjectsData;
    public SerializableGridData serializableGridData;
}
public class SaveManager : MonoBehaviour
{
    [SerializeField] PlacementStystem placementStystem;
    [SerializeField] ObjectManager objectManager;
    [SerializeField] ObjectDatabaseSO objectDatabaseSO;
    private string _thumbnailFolder;
    private string _saveFolder;

    public static SaveManager Instance { get; private set; }

    public UnityEvent<SaveData, ObjectDatabaseSO> OnLoad;

    private string _saveFileName="Save";
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
            Instance = this;

        _thumbnailFolder = Application.persistentDataPath + "/thumbnail";
        _saveFolder = Application.persistentDataPath + "/saves";
        Directory.CreateDirectory(_thumbnailFolder);
        Directory.CreateDirectory(_saveFolder);

    }
    public void SetSaveFileName(string saveFileName) => _saveFileName = saveFileName;
    public void SaveFurniture()
    {
        SaveData saveData = new();
        string thumbnailPath = _thumbnailFolder + $"/Screenshot_{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")}.png";
        ScreenCapture.CaptureScreenshot(thumbnailPath);
        saveData.serializableGridData = new(placementStystem.GetFurnitureData());
        List<PlacedObjectData> pod = new();
        foreach(var obj in objectManager.GetPlacedObjects())
        {
            if (obj.HasValue)
                pod.Add(new(obj.Value.Item1, obj.Value.Item2, obj.Value.Item3));
        }
        saveData.placedObjectsData = pod;


        string path = _saveFolder + $"/{_saveFileName}_{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")}.json";

        string data = JsonUtility.ToJson(saveData);
        File.WriteAllText(path, data);
    }
    public List<string> GetAllSaveFiles()
    {
        return Directory.EnumerateFiles(_saveFolder).Select((f)=>Path.GetFileName(f)).ToList();
    }
    public byte[] GetThumbnailBytes(string fileName)
    {
        string date = Path.GetFileNameWithoutExtension(fileName).Split('_')[1];
        string thumbnailPath = _thumbnailFolder + $"/Screenshot_{date}.png";
        if (!File.Exists(thumbnailPath))
            throw new Exception();

        return File.ReadAllBytes(thumbnailPath);
    }
    public void DeleteSaveFile(string fileName)
    {
        string filePath = _saveFolder + "/" + fileName;
        if (!File.Exists(filePath))
        {
            Debug.LogError("Path not found in " + filePath);
            return;
        }
        DeleteThumbnail(fileName);
        File.Delete(filePath);
    }
    public void LoadFurniture(string fileName)
    {
        string filePath = Application.persistentDataPath + "/" + fileName;
        if (!File.Exists(filePath))
        {
            Debug.LogError("Path not found in " + filePath);
            return;
        }
        string saveFile = File.ReadAllText(filePath);
        SaveData loadedObjects = JsonUtility.FromJson<SaveData>(saveFile);
        OnLoad?.Invoke(loadedObjects, objectDatabaseSO);
    }
    private void DeleteThumbnail(string fileName)
    {
        string date = Path.GetFileNameWithoutExtension(fileName).Split('_')[1];
        string thumbnailPath = _thumbnailFolder + $"/Screenshot_{date}.png";
        if (!File.Exists(thumbnailPath))
            return;

        File.Delete(thumbnailPath);
    }
}
