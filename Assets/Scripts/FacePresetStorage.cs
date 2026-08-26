using System;
using System.IO;
using UnityEngine;

public static class FacePresetStorage
{
    public const int CustomSlotCount = 10;

    private const int CurrentDataVersion = 1;
    private const string FileName = "face_presets.json";

    [Serializable]
    private class SaveFile
    {
        public int dataVersion = CurrentDataVersion;
        public string[] slots = new string[CustomSlotCount];
    }

    private static SaveFile loadedData;
    private static bool hasLoaded = false;

    private static string FilePath
    {
        get
        {
            return Path.Combine(
                Application.persistentDataPath,
                FileName
            );
        }
    }

    private static void EnsureLoaded()
    {
        if (hasLoaded)
            return;

        hasLoaded = true;

        if (!File.Exists(FilePath))
        {
            CreateEmptySave();
            SaveToDisk();
            return;
        }

        try
        {
            string json = File.ReadAllText(FilePath);

            loadedData =
                JsonUtility.FromJson<SaveFile>(json);

            if (loadedData == null)
            {
                CreateEmptySave();
            }

            EnsureSlotArray();
        }
        catch (Exception exception)
        {
            Debug.LogWarning(
                "Could not load face presets. Creating new preset data.\n" +
                exception.Message
            );

            CreateEmptySave();
        }
    }

    private static void CreateEmptySave()
    {
        loadedData = new SaveFile();

        loadedData.dataVersion =
            CurrentDataVersion;

        loadedData.slots =
            new string[CustomSlotCount];

        for (int i = 0; i < loadedData.slots.Length; i++)
        {
            loadedData.slots[i] = "";
        }
    }

    private static void EnsureSlotArray()
    {
        if (loadedData.slots == null)
        {
            loadedData.slots =
                new string[CustomSlotCount];

            return;
        }

        if (loadedData.slots.Length == CustomSlotCount)
            return;

        string[] oldSlots = loadedData.slots;

        loadedData.slots =
            new string[CustomSlotCount];

        int amountToCopy =
            Mathf.Min(
                oldSlots.Length,
                CustomSlotCount
            );

        for (int i = 0; i < amountToCopy; i++)
        {
            loadedData.slots[i] =
                oldSlots[i];
        }
    }

    public static bool HasPreset(int slotIndex)
    {
        EnsureLoaded();

        if (!IsValidSlot(slotIndex))
            return false;

        return !string.IsNullOrEmpty(
            loadedData.slots[slotIndex]
        );
    }

    public static byte[] LoadPreset(int slotIndex)
    {
        EnsureLoaded();

        if (!IsValidSlot(slotIndex))
            return null;

        string storedData =
            loadedData.slots[slotIndex];

        if (string.IsNullOrEmpty(storedData))
            return null;

        try
        {
            return Convert.FromBase64String(storedData);
        }
        catch
        {
            Debug.LogWarning(
                "Preset slot " +
                slotIndex +
                " contained invalid data."
            );

            return null;
        }
    }

    public static void SavePreset(
        int slotIndex,
        byte[] faceData
    )
    {
        EnsureLoaded();

        if (!IsValidSlot(slotIndex))
            return;

        if (faceData == null)
            return;

        loadedData.slots[slotIndex] =
            Convert.ToBase64String(faceData);

        SaveToDisk();
    }

    public static void DeletePreset(
        int slotIndex
    )
    {
        EnsureLoaded();

        if (!IsValidSlot(slotIndex))
            return;

        loadedData.slots[slotIndex] = "";

        SaveToDisk();
    }

    private static void SaveToDisk()
    {
        try
        {
            string json =
                JsonUtility.ToJson(
                    loadedData,
                    true
                );

            File.WriteAllText(
                FilePath,
                json
            );
        }
        catch (Exception exception)
        {
            Debug.LogError(
                "Could not save face presets.\n" +
                exception.Message
            );
        }
    }

    private static bool IsValidSlot(
        int slotIndex
    )
    {
        return
            slotIndex >= 0 &&
            slotIndex < CustomSlotCount;
    }
}