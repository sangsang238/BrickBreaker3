using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class CSVLoader
{
    private static Dictionary<string, string> errorMessages = new Dictionary<string, string>();

    public static void LoadErrorMessages(string csvFilePath)
    {
        try
        {
            string[] lines = File.ReadAllLines(csvFilePath);

            foreach (string line in lines)
            {
                string[] parts = line.Split(';');
                if (parts.Length == 2)
                {
                    string errorCode = parts[0];
                    string errorMessage = parts[1].Replace("\\n", Environment.NewLine); // Replace "\\n" with newline
                    errorMessages[errorCode] = errorMessage;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error loading error messages: " + e.Message);
        }
    }

    public static string GetErrorMessage(string errorCode)
    {
        if (errorMessages.TryGetValue(errorCode, out string errorMessage))
        {
            return errorMessage;
        }
        else
        {
            //return "An unknown error occurred.";
            return "SUCCESS!";
        }
    }
}
