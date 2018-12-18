using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akumina.PeopleSync.Core;
using Akumina.PeopleSync.Core.Entities;
using Akumina.PeopleSync.Core.Implementation;
using Akumina.PeopleSync.Core.Interfaces;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json.Linq;

namespace PeopleSync.Customization
{
    public class XlsCustomSource : AadSyncBase, ISyncSource
    {
        public XlsCustomSource(SyncRequest syncRequest)
        {
            Configuration = syncRequest;
        }
        public QueryResult QueryUsers(string resourceSet, string nextLink, ICollection<string> objectClassList, ICollection<string> propertyList)
        {
            var changes = ReadFile("C:\\temp\\sample.xlsx");
            var array = JArray.FromObject(changes);
            return new QueryResult(array, false, "");
        }

        public QueryResult QueryGroups(string resourceSet, string nextLink, ICollection<string> objectClassList, ICollection<string> propertyList)
        {
            return null;
        }

        public string FetchGroupExtensions(string lastExtensionLink, List<KeyValuePair<string, (string CreationDate, string Visibility, string Tags, string Types)>> groupExtensions)
        {
            return "";
        }

        public Dictionary<string, string> GetData(string objectId, dynamic client, SyncTask.ProcessTask callback)
        {
            return new Dictionary<string, string>();
        }

        public async Task<Dictionary<string, string>> GetAssignedLicenses(string objectId, dynamic client)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }

        public async Task<Dictionary<string, string>> GetAppRoleAssignments(string objectId, dynamic client)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }

        public async Task<Dictionary<string, string>> GetDirectReports(string objectId, dynamic client)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }

        public async Task<Dictionary<string, string>> GetOwnedDevices(string objectId, dynamic client)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }

        public async Task<Dictionary<string, string>> GetRegisteredDevices(string objectId, dynamic client)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }

        public async Task<Dictionary<string, string>> GetExtendedProperties(string objectId, dynamic client)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }

        public async Task<Dictionary<string, string>> GetManager(string objectId, dynamic client)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }

        public async Task<Dictionary<string, string>> GetGroupMembers(string objectId, dynamic client)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }

        public async Task<Dictionary<string, string>> GetExtendedPropertiesForGroup(string objectId, dynamic client)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }

        public async Task<Dictionary<string, string>> OnPremisesExtensionAttributes(string objectId, dynamic client)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }

        private static List<Dictionary<string, string>> ReadFile(string fileName)
        {
            var result = new List<Dictionary<string, string>>();
            using (var spreadSheetDocument = SpreadsheetDocument.Open(fileName, false))
            {
                var sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                var relationshipId = sheets.First().Id.Value;
                var worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                var workSheet = worksheetPart.Worksheet;
                var sheetData = workSheet.GetFirstChild<SheetData>();
                var rows = sheetData.Descendants<Row>();
                if (!(rows != null && rows.Any())) return result;
                var header = rows.ElementAt(0);
                foreach (var row in rows)
                {
                    if (row.RowIndex.Value == 1) continue;
                    var entry = new Dictionary<string, string>();
                    for (var i = 0; i < row.Descendants<Cell>().Count(); i++)
                    {
                        var key = GetCellValue(spreadSheetDocument, header.Descendants<Cell>().ElementAt(i));
                        var val = GetCellValue(spreadSheetDocument, row.Descendants<Cell>().ElementAt(i));
                        entry.Add(key.Replace("(", "").Replace(")", ""), val);
                    }
                    entry.Add("objectId", entry["EmployeeID"]);
                    entry.Add("odata.type", "Microsoft.DirectoryServices.User");
                    result.Add(entry);
                }
            }
            return result;
        }

        private static string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            var stringTablePart = document.WorkbookPart.SharedStringTablePart;
            var value = cell.CellValue == null || string.IsNullOrEmpty(cell.CellValue.InnerXml) ? "" : cell.CellValue.InnerXml;
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
            }
            return value;
        }
    }
}
