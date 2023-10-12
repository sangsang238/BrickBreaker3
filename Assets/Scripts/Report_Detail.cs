using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Report_Detail : MonoBehaviour
{
    MongoDBManager manager = new MongoDBManager();
    public static ModelPlayer mdplayer = null;

    public TMP_Dropdown cbbRangeTime;
    public TextMeshProUGUI textMeshPro;

    public Button btnExport;
    public SpriteRenderer askingTable;
    public ScrollRect scrollView;
    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtDateTime;
    public Button btnBack;

    public Button dialog;


    // Start is called before the first frame update
    void Start()
    {
        cbbRangeTime.onValueChanged.AddListener(OnDropdownValueChanged);

        if (manager != null)
        {
            textMeshPro.text = DisplayPlayerPlayMomentsOneWeek();
        }
    }

    //-----------------------------------------Display--------------------------------------------
    private string DisplayPlayerPlayMoments()
    {
        //List<(string PlayerName, List<string> PlayMoments)> playerPlayMomentsList = manager.GetPlayerPlayMoments();
        var playerPlayMomentsList = manager.GetPlayerPlayMoments();
        string result = "";
        int i = 1;
        int j = 0;
        foreach (var kvp in playerPlayMomentsList)
        {
            result += i.ToString() + ".";
            result += $"{kvp.PlayerName}";
            //  null
            if (kvp.PlayMoments.Count == 0)
                result += " «Has_not_logged_in_yet.»\n\n\n";
            else
            {
                foreach (var playMoment in kvp.PlayMoments)
                {
                    if (j >= 1)
                        result += i.ToString() + "." + j.ToString() + "."+ $"{kvp.PlayerName}";
                    result += $" «{playMoment.Replace(" ", "_")}»\n\n\n";
                    j++;
                }
            }

            i++;
            j = 0;
        }

        return result;
    }


    private string DisplayPlayerPlayMomentsOneWeek()
    {
        DateTime currentMoment = DateTime.Now;
        string strCurrentMoment = GetDateFromInput(currentMoment.ToString("M/d/yyyy h:mm:ss tt"));

        string LastWeekDate = GetLastWeekDateFromInput(strCurrentMoment);
        string LastMonthDate = GetLastMonthDateFromInput(strCurrentMoment);

        Debug.Log("Curr: " + strCurrentMoment + "\n" +
                  "LastWeekDate: " + LastWeekDate + "\n" +
                  "LastMonthDate: " + LastMonthDate);

        var playerPlayMomentsList = manager.GetPlayerPlayMoments();
        string result = "";

        //string X = "11/30/2022"; // ngày bắt đầu
        //string Z = "1/1/2024"; // ngày kết thúc


        List<string> allPlayMoments = new List<string>();

        foreach (var kvp in playerPlayMomentsList)
        {
            allPlayMoments.AddRange(kvp.PlayMoments);
        }

        //string[] allPlayMomentsArray = allPlayMoments.ToArray();

        //List<string> filteredDates = FilterDatesWithinRange(X, Z, allPlayMomentsArray);
        //Debug.Log("Test: " + string.Join(",", filteredDates) + "\n");

        int i = 1;
        //int j = 0;
        foreach (var kvp in playerPlayMomentsList)
        {
            //if (kvp.PlayMoments.Count > 0)
            //{
            //    result += i.ToString() + ".";
            //    result += $"{kvp.PlayerName}";

            //    foreach (var playMoment in kvp.PlayMoments)
            //    {
            //        if (j >= 1)
            //            result += i.ToString() + "." + j.ToString() + "." + $"{kvp.PlayerName}";
            //        result += $" «{FilterDatesWithinRange(LastWeekDate, strCurrentMoment, GetDateFromInput(playMoment)).FirstOrDefault()}»\n\n\n";
            //        j++;
            //    }

            //    i++;
            //    j = 0;
            //}


            if (kvp.PlayMoments.Count > 0)
            {
                //result += i.ToString() + ".";
                //result += $"{kvp.PlayerName}";

                bool anyMomentDisplayed = false;

                int j = 0;
                foreach (var playMoment in kvp.PlayMoments)
                {
                    string filteredDate = FilterDatesWithinRange(LastWeekDate, strCurrentMoment, GetDateFromInput(playMoment)).FirstOrDefault();

                    if (!string.IsNullOrEmpty(filteredDate))
                    {
                        //result += i.ToString() + ".";
                        //result += $"{kvp.PlayerName}";
                        //anyMomentDisplayed = true;

                        if (j >= 1)
                            result += i.ToString() + "." + j.ToString() + "." + $"{kvp.PlayerName}";
                        else
                        {
                            result += i.ToString() + ".";
                            result += $"{kvp.PlayerName}";
                            anyMomentDisplayed = true;
                        }
                        //result += $" «{filteredDate} ({playMoment})»\n\n\n";
                        result += $" «{playMoment.Replace(" ","_")}»\n\n\n";
                        j++;
                    }
                }

                if (anyMomentDisplayed)
                {
                    i++;
                }
            }

        }

        return result;
    }

    private string DisplayPlayerPlayMomentsOneMonth()
    {
        DateTime currentMoment = DateTime.Now;
        string strCurrentMoment = GetDateFromInput(currentMoment.ToString("M/d/yyyy h:mm:ss tt"));

        string LastMonthDate = GetLastMonthDateFromInput(strCurrentMoment);

        Debug.Log("Curr: " + strCurrentMoment + "\n" +
                  "LastMonthDate: " + LastMonthDate);

        var playerPlayMomentsList = manager.GetPlayerPlayMoments();
        string result = "";

        List<string> allPlayMoments = new List<string>();

        foreach (var kvp in playerPlayMomentsList)
        {
            allPlayMoments.AddRange(kvp.PlayMoments);
        }

        int i = 1;
        foreach (var kvp in playerPlayMomentsList)
        {
            if (kvp.PlayMoments.Count > 0)
            {
                //result += i.ToString() + ".";
                //result += $"{kvp.PlayerName}";
                bool anyMomentDisplayed = false;

                int j = 0;
                foreach (var playMoment in kvp.PlayMoments)
                {
                    string filteredDate = FilterDatesWithinRange(LastMonthDate, strCurrentMoment, GetDateFromInput(playMoment)).FirstOrDefault();

                    if (!string.IsNullOrEmpty(filteredDate))
                    {


                        if (j >= 1)
                            result += i.ToString() + "." + j.ToString() + "." + $"{kvp.PlayerName}";
                        else
                        {
                            result += i.ToString() + ".";
                            result += $"{kvp.PlayerName}";
                            anyMomentDisplayed = true;
                        }

                        //result += $" «{filteredDate} ({playMoment})»\n\n\n";
                        result += $" «{playMoment.Replace(" ", "_")}»\n\n\n";
                        j++;
                    }
                }

                if (anyMomentDisplayed)
                {
                    i++;
                }
            }

        }

        return result;
    }

    //-----------------------------------------Combobox--------------------------------------------

    private void OnDropdownValueChanged(int selectedIndex)
    {
        // selectedIndex là chỉ số của lựa chọn được chọn
        switch (selectedIndex)
        {
            case 0:
                Debug.Log("OneWeek");
                textMeshPro.text = DisplayPlayerPlayMomentsOneWeek();
                break;
            case 1:
                Debug.Log("OneMonth");
                textMeshPro.text = DisplayPlayerPlayMomentsOneMonth();
                break;
            case 2:
                Debug.Log("AllTimes");
                textMeshPro.text = DisplayPlayerPlayMoments();
                break;
            case 3:

                break;
            default:
                break;
        }
    }

    //-----------------------------------------Asking Table--------------------------------------------
    private void AskingMenu()
    {
        btnExport.gameObject.SetActive(false);
        btnBack.gameObject.SetActive(false);
        cbbRangeTime.enabled = false;
        textMeshPro.enabled = false;
        btnExport.enabled = false;
        askingTable.enabled = false;
        scrollView.gameObject.SetActive(false);
        txtName.enabled = false;
        txtDateTime.enabled = false;
        cbbRangeTime.gameObject.SetActive(false);

        askingTable.gameObject.SetActive(true);
    }

    private void DetailReportMenu()
    {
        askingTable.gameObject.SetActive(false);

        btnExport.gameObject.SetActive(true);
        btnBack.gameObject.SetActive(true);
        cbbRangeTime.enabled = true;
        textMeshPro.enabled = true;
        btnExport.enabled = true;
        askingTable.enabled = true;
        scrollView.gameObject.SetActive(true);
        txtName.enabled = true;
        txtDateTime.enabled = true;
        cbbRangeTime.gameObject.SetActive(true);
    }


    public void btnExportClicked()
    {
        AskingMenu();
    }

    public void btnClose()
    {
        DetailReportMenu();
    }

    private void ExportToCSV_1Week(string data)
    {
        string csvFilePath = Path.Combine(Application.dataPath, "DetailReport_1Week.csv");

        using (StreamWriter writer = new StreamWriter(csvFilePath))
        {
            writer.Write(data);
        }

        Debug.Log("CSV export complete!");
        dialog.gameObject.SetActive(true);
    }

    private void ExportToCSV_1Month(string data)
    {
        string csvFilePath = Path.Combine(Application.dataPath, "DetailReport_1Month.csv");

        using (StreamWriter writer = new StreamWriter(csvFilePath))
        {
            writer.Write(data);
        }

        Debug.Log("CSV export complete!");
        dialog.gameObject.SetActive(true);
    }

    private void ExportToCSV_AllTimes(string data)
    {
        string csvFilePath = Path.Combine(Application.dataPath, "DetailReport_Alltimes.csv");

        using (StreamWriter writer = new StreamWriter(csvFilePath))
        {
            writer.Write(data);
        }

        Debug.Log("CSV export complete!");
        dialog.gameObject.SetActive(true);
    }


    private string ExportPlayerPlayMomentsToCSV(List<(string PlayerName, List<string> PlayMoments)> playerPlayMomentsList, string startDate, string endDate, int i, int j)
    {
        string exportData = "Index;Player;PlayMoment\n";

        foreach (var kvp in playerPlayMomentsList)
        {
            if (kvp.PlayMoments.Count > 0)
            {
                bool anyMomentDisplayed = false;
                foreach (var playMoment in kvp.PlayMoments)
                {

                    string filteredDate = FilterDatesWithinRange(startDate, endDate, GetDateFromInput(playMoment)).FirstOrDefault();

                    if (!string.IsNullOrEmpty(filteredDate))
                    {
                        anyMomentDisplayed = true;
                        exportData += $"{i}.{j};{kvp.PlayerName};{playMoment.Replace(" ", "_")}\n";
                        j++;
                    }
                }
                if (anyMomentDisplayed)
                    i++;
            }
            
            //i++;
            j = 0; // Reset j for the next player
        }

        return exportData;
    }

    private string ExportPlayerPlayMomentsToCSV_AllTimes(List<(string PlayerName, List<string> PlayMoments)> playerPlayMomentsList)
    {
        string csvData = "Index;Player;PlayMoment\n";
        int i = 1;
        int j = 0;

        foreach (var kvp in playerPlayMomentsList)
        {
            string playerName = kvp.PlayerName;
            List<string> playMoments = kvp.PlayMoments;

            if (playMoments.Count == 0)
            {
                csvData += $"{i}.0;{playerName};Has_not_logged_in_yet.\n";
            }
            else
            {
                foreach (var playMoment in playMoments)
                {
                    csvData += $"{i}.{j};{playerName};{playMoment.Replace(" ", "_")}\n";
                    j++;
                }
            }

            i++;
            j = 0;
        }

        return csvData;
    }

    public void btn1WeekClicked()
    {
        DateTime currentMoment = DateTime.Now;
        string strCurrentMoment = GetDateFromInput(currentMoment.ToString("M/d/yyyy h:mm:ss tt"));
        string LastWeekDate = GetLastWeekDateFromInput(strCurrentMoment);

        var playerPlayMomentsList = manager.GetPlayerPlayMoments();
        string dataToExport = ExportPlayerPlayMomentsToCSV(playerPlayMomentsList, LastWeekDate, strCurrentMoment, 1, 0);

        ExportToCSV_1Week(dataToExport);
        //AskingMenu();
    }

    public void btn1MonthClicked()
    {
        DateTime currentMoment = DateTime.Now;
        string strCurrentMoment = GetDateFromInput(currentMoment.ToString("M/d/yyyy h:mm:ss tt"));
        string LastMonthDate = GetLastMonthDateFromInput(strCurrentMoment);

        var playerPlayMomentsList = manager.GetPlayerPlayMoments();
        string dataToExport = ExportPlayerPlayMomentsToCSV(playerPlayMomentsList, LastMonthDate, strCurrentMoment, 1, 0);

        ExportToCSV_1Month(dataToExport);
        //AskingMenu();
    }

    public void btnAllTimesClicked()
    {
        var playerPlayMomentsList = manager.GetPlayerPlayMoments();
        string csvDataToExport = ExportPlayerPlayMomentsToCSV_AllTimes(playerPlayMomentsList);

        ExportToCSV_AllTimes(csvDataToExport);
        //AskingMenu();
    }



    //-----------------------------------------Handle DateTime--------------------------------------------
    private string GetDateFromInput(string input)
    {
        // Chuyển đổi chuỗi thành đối tượng DateTime
        DateTime dateTime;
        if (!DateTime.TryParse(input, out dateTime))
        {
            throw new ArgumentException("Invalid input format");
        }

        // Trích xuất ngày, tháng và năm
        int day = dateTime.Day;
        int month = dateTime.Month;
        int year = dateTime.Year;

        // Tạo chuỗi ngày/tháng/năm
        string dateOutput = $"{month}/{day}/{year}";

        return dateOutput;
    }

    private string GetLastWeekDateFromInput(string input)
    {
        // Chuyển đổi chuỗi thành đối tượng DateTime
        DateTime dateTime;
        if (!DateTime.TryParse(input, out dateTime))
        {
            throw new ArgumentException("Invalid input format");
        }

        // Lấy ngày 7 ngày trước tính từ ngày hiện tại
        DateTime lastWeek = dateTime.AddDays(-7);

        // Trích xuất ngày, tháng và năm của ngày 7 ngày trước
        int day = lastWeek.Day;
        int month = lastWeek.Month;
        int year = lastWeek.Year;

        // Tạo chuỗi ngày/tháng/năm
        string dateOutput = $"{month}/{day}/{year}";

        return dateOutput;
    }



    private string GetLastMonthDateFromInput(string input)
    {
        // Chuyển đổi chuỗi thành đối tượng DateTime
        DateTime dateTime;
        if (!DateTime.TryParse(input, out dateTime))
        {
            throw new ArgumentException("Invalid input format");
        }

        // Lấy ngày 1 tháng trước so với ngày hiện tại
        DateTime lastMonth = dateTime.AddMonths(-1);

        // Trích xuất ngày, tháng và năm của tháng trước
        int day = lastMonth.Day;
        int month = lastMonth.Month;
        int year = lastMonth.Year;

        // Tạo chuỗi ngày/tháng/năm
        string dateOutput = $"{month}/{day}/{year}";

        return dateOutput;
    }


    private List<string> FilterDatesWithinRange(string X, string Z, string Y)
    {
        DateTime startDate = DateTime.ParseExact(X, "M/d/yyyy", CultureInfo.InvariantCulture);
        DateTime endDate = DateTime.ParseExact(Z, "M/d/yyyy", CultureInfo.InvariantCulture);

        DateTime currentDate = DateTime.ParseExact(Y, "M/d/yyyy", CultureInfo.InvariantCulture);

        List<string> filteredDates = new List<string>();

        if (currentDate >= startDate && currentDate <= endDate)
        {
            filteredDates.Add(Y);
        }

        return filteredDates;
    }
}
