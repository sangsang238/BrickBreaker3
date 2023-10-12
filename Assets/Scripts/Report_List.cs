using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using OfficeOpenXml;
using System.IO;


public class Report_List : MonoBehaviour
{
    MongoDBManager manager = new MongoDBManager();
    //public static ModelPlayer mdplayer = null;

    public TextMeshProUGUI textMeshPro;
    public Button export;
    public Button dialog;
    private List<string> csvLines = new List<string>();

    void Start()
    {
        if (manager != null)
            textMeshPro.text = GetPlayerNamesAndStats();

        // Attach export button click event
        export.onClick.AddListener(ExportDataToCSV); 
    }

    public string FormatPlayTime(int totalSeconds)
    {
        int days = totalSeconds / (24 * 3600);
        int hours = (totalSeconds % (24 * 3600)) / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;

        string formattedTime = $"{days}·days·{hours}·hours·{minutes}·minutes·{seconds}·seconds";
        return formattedTime;
    }

    public static string FormatPlayTimeForCSV(int totalSeconds)
    {
        int days = totalSeconds / (24 * 3600);
        int hours = (totalSeconds % (24 * 3600)) / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;

        string formattedTime = $"{days} days, {hours} hours, {minutes} minutes and {seconds} seconds";
        return formattedTime;
    }

    public string GetPlayerNamesAndStats()
    {
        var playerStarts = manager.GetPlayerNamesAndStats();

        if (playerStarts.Count == 0)
        {
            return "No players found.";
        }

        string result = "";
        int i = 1;
        foreach (var kvp in playerStarts)
        {
            result += i.ToString() + ".";
            result += $"{kvp.Key}   {FormatPlayTime(kvp.Value.playTime)}   {kvp.Value.playSession}\n\n\n";
            // Store CSV data for export
            csvLines.Add($"{kvp.Key},{kvp.Value.playTime},{kvp.Value.playSession}");
            i++;
        }

        return result;
    }

    private void ExportDataToCSV()
    {
        string csvFilePath = Path.Combine(Application.dataPath, "PlayerData.csv");
        using (StreamWriter writer = new StreamWriter(csvFilePath))
        {
            // Write the header
            writer.WriteLine("NO;Player Names;Time Spent Playing;Games");
            int i = 1;
            // Write data
            foreach (var kvp in manager.GetPlayerNamesAndStats())
            {
                string key = kvp.Key;
                string playTime = FormatPlayTimeForCSV(kvp.Value.playTime);
                string playSession = kvp.Value.playSession.ToString();

                // Wrap data with double quotes as text qualifiers
                string csvLine = $"{i};{key};{playTime};{playSession}";
                writer.WriteLine(csvLine);
                i++;
            }
        }
        dialog.gameObject.SetActive(true);
        Debug.Log("CSV export complete!");
    }

}
