using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
    private SheetsService service;

    [Header("Google Sheets")]
    public string spreadsheetId = "1jDJKW1bMuYCcQfJ5Ozomasuv2psPPmtFEKVgBrD-3zk";
    public string sheetName = "Feuille_1";

    public static StatManager Instance;

    private Task _initTask;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        _initTask = InitService();
    }

    private async Task EnsureReady()
    {
        if (_initTask == null) _initTask = InitService();
        await _initTask;

        if (service == null)
            throw new InvalidOperationException("Google Sheets service is null (InitService a échoué).");
    }

    private async Task InitService()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("service-account");
        if (jsonFile == null)
        {
            Debug.LogError("service-account introuvable. Mets le fichier dans Assets/Resources/service-account.json");
            return;
        }

        GoogleCredential credential;
        using (var stream = new MemoryStream(jsonFile.bytes))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
        }

        service = new SheetsService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "UnityGoogleSheets",
        });

        Debug.Log("Google Sheets prêt !");
        await Task.CompletedTask;
    }

    public async Task UpdateCell(string cell, object newValue)
    {
        await EnsureReady();

        var valueRange = new ValueRange
        {
            Values = new List<IList<object>> { new List<object> { newValue } }
        };

        string range = $"{sheetName}!{cell}";

        var updateRequest = service.Spreadsheets.Values.Update(valueRange, spreadsheetId, range);
        updateRequest.ValueInputOption =
            SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

        await updateRequest.ExecuteAsync();
        Debug.Log($"Cellule {cell} mise à jour !");
    }

    public async Task<int> ReadIntCell(string cell)
    {
        await EnsureReady();

        string range = $"{sheetName}!{cell}";
        var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
        var response = await request.ExecuteAsync();

        if (response.Values == null || response.Values.Count == 0)
            return 0;

        string rawValue = response.Values[0][0]?.ToString() ?? "0";
        return int.TryParse(rawValue, out int result) ? result : 0;
    }

    public async Task IncrementCell(string cell, int increment = 1)
    {
        await EnsureReady();

        int currentValue = await ReadIntCell(cell);
        int newValue = currentValue + increment;
        await UpdateCell(cell, newValue);
    }

    private async Task<int> AddLevelToEndOfColumn(string column, string levelName)
    {
        await EnsureReady();

        string range = $"{sheetName}!{column}:{column}";
        var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
        var response = await request.ExecuteAsync();

        int nextRow = (response.Values != null) ? response.Values.Count + 1 : 1;
        string cell = $"{column}{nextRow}";
        await UpdateCell(cell, levelName);
        return nextRow;
    }

    public async Task IncrementLevelWin(string column, string level, bool isWin)
    {
        await EnsureReady();

        int row = await GetLevelRow(column, level);
        if (row == -1)
        {
            row = await AddLevelToEndOfColumn(column, level);
            await UpdateCell($"B{row}", 0);
            await UpdateCell($"C{row}", 0);
        }

        string statCell = isWin ? $"B{row}" : $"C{row}";
        await IncrementCell(statCell);
    }

    private async Task<int> GetLevelRow(string column, string levelName)
    {
        await EnsureReady();

        string range = $"{sheetName}!{column}:{column}";
        var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
        var response = await request.ExecuteAsync();

        if (response.Values == null) return -1;

        for (int i = 0; i < response.Values.Count; i++)
        {
            if (response.Values[i].Count == 0) continue;
            if (response.Values[i][0].ToString().Equals(levelName, StringComparison.OrdinalIgnoreCase))
                return i + 1;
        }
        return -1;
    }
}
