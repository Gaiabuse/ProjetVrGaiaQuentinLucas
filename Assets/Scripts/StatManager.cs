using System;
using UnityEngine;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;


public class StatManager : MonoBehaviour
{
   private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
      private SheetsService service;
      
      public string spreadsheetId = "1jDJKW1bMuYCcQfJ5Ozomasuv2psPPmtFEKVgBrD-3zk";  // ID Google Sheets
      public string sheetName = "Feuille_1";

      public static StatManager Instance;

      private void Awake()
      {
          if (Instance != null)
          {
              Destroy(this);
              return;
          }

          DontDestroyOnLoad(this);
          Instance = this;
      }

      private async void Start()
      {
          await InitService();
      }
      
      private async Task InitService()
      {
          TextAsset jsonFile = Resources.Load<TextAsset>("service-account");
          GoogleCredential credential;
  
          using (var stream = new MemoryStream(jsonFile.bytes))
          {
              credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
          }
  
          service = new SheetsService(new BaseClientService.Initializer()
          {
              HttpClientInitializer = credential,
              ApplicationName = "UnityGoogleSheets",
          });
  
          Debug.Log("Google Sheets prêt !");
      }

      public async Task UpdateCell(string cell, object newValue)
      {
          var valueRange = new ValueRange
          {
              Values = new List<IList<object>>
              {
                  new List<object> { newValue }
              }
          };

          string range = $"{sheetName}!{cell}";

          var updateRequest = service.Spreadsheets.Values.Update(
              valueRange,
              spreadsheetId,
              range
          );

          updateRequest.ValueInputOption =
              SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

          await updateRequest.ExecuteAsync();

          Debug.Log($"Cellule {cell} mise à jour !");
      }
      
      public async Task<int> ReadIntCell(string cell)
      {
          string range = $"{sheetName}!{cell}";

          var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
          var response = await request.ExecuteAsync();

          if (response.Values == null || response.Values.Count == 0)
          {
              Debug.LogWarning($"Cellule {cell} vide, valeur = 0");
              return 0;
          }

          string rawValue = response.Values[0][0].ToString();

          if (int.TryParse(rawValue, out int result))
              return result;

          Debug.LogWarning($"Impossible de parser {rawValue}, valeur = 0");
          return 0;
      }
      
      public async Task IncrementCell(string cell, int increment = 1)
      {
          int currentValue = await ReadIntCell(cell);
          int newValue = currentValue + increment;

          await UpdateCell(cell, newValue);

          Debug.Log($"Cellule {cell} : {currentValue} → {newValue}");
      }
      private async Task<int> AddLevelToEndOfColumn(string column, string levelName)
      {
          string range = $"{sheetName}!{column}:{column}";

          var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
          var response = await request.ExecuteAsync();

          int nextRow = 1;

          if (response.Values != null)
              nextRow = response.Values.Count + 1; // prochaine ligne libre

          string cell = $"{column}{nextRow}";

          await UpdateCell(cell, levelName);

          Debug.Log($"Level {levelName} ajouté en {cell}");

          return nextRow; 
      }

      public async Task IncrementLevelWin(string column, string level, bool isWin)
      {
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
          string range = $"{sheetName}!{column}:{column}";
          var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
          var response = await request.ExecuteAsync();

          if (response.Values == null)
              return -1;

          for (int i = 0; i < response.Values.Count; i++)
          {
              if (response.Values[i].Count == 0) continue;

              if (response.Values[i][0].ToString()
                  .Equals(levelName, StringComparison.OrdinalIgnoreCase))
                  return i + 1;
          }

          return -1;
      }

      private async Task<bool> LevelExistsInColumn(string column, string levelName)
      {
          string range = $"{sheetName}!{column}:{column}";

          var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
          var response = await request.ExecuteAsync();

          if (response.Values == null || response.Values.Count == 0)
              return false;

          foreach (var row in response.Values)
          {
              if (row.Count == 0) continue;

              string cellValue = row[0].ToString();

              if (cellValue.Equals(levelName, StringComparison.OrdinalIgnoreCase))
                  return true;
          }

          return false;
      }
      
}
