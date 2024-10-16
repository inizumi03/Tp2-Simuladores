using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string savePath = Application.persistentDataPath + "/save";
    private static readonly string listPath = Application.persistentDataPath + "/savesList.json";

    // Guardar una partida
    public static void SavePlayer(Player player, int dropdownValue, int saveID)
    {
        string path = savePath + saveID + ".json"; // Cambiamos la extensión a .json
        PlayerData data = new PlayerData(player, dropdownValue, saveID);

        // Serializamos a JSON y escribimos en el archivo
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(path, json);

        SaveSavesList(saveID); // Guardar la lista de partidas
    }

    // Cargar una partida
    public static PlayerData LoadPlayer(int saveID)
    {
        string path = savePath + saveID + ".json"; // Cambiamos la extensión a .json
        if (File.Exists(path))
        {
            // Leemos el archivo de texto con JSON y lo deserializamos
            string json = File.ReadAllText(path);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            return data;
        }
        else
        {
            Debug.LogError("Archivo de guardado no encontrado en " + path);
            return null;
        }
    }

    // Guardar la lista de IDs de las partidas
    private static void SaveSavesList(int saveID)
    {
        List<int> savesList = LoadSavesList() ?? new List<int>();
        if (!savesList.Contains(saveID))
        {
            savesList.Add(saveID);
            if (savesList.Count > 10) // Limitar a 10 partidas
            {
                savesList.RemoveAt(0);
            }
        }

        // Serializamos la lista de IDs a JSON usando el envoltorio ListWrapper
        string json = JsonUtility.ToJson(new ListWrapper<int> { list = savesList });
        File.WriteAllText(listPath, json); // Guardamos el JSON en el archivo
    }

    // Cargar la lista de IDs de partidas guardadas
    public static List<int> LoadSavesList()
    {
        if (File.Exists(listPath))
        {
            // Leemos el archivo de texto con JSON y lo deserializamos
            string json = File.ReadAllText(listPath);
            List<int> savesList = JsonUtility.FromJson<ListWrapper<int>>(json).list;
            return savesList;
        }
        else
        {
            return new List<int>(); // Si no existe el archivo, devolver una lista vacía
        }
    }
}

// Clase auxiliar para serializar/deserializar listas en JSON
[System.Serializable]
public class ListWrapper<T>
{
    public List<T> list;
}