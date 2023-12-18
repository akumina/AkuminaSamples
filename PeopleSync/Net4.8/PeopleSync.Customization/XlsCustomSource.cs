using Akumina.PeopleSync.Core;
using Akumina.PeopleSync.Core.Entities;
using Akumina.PeopleSync.Core.Enums;
using Akumina.PeopleSync.Core.Implementation;
using Akumina.PeopleSync.Core.Interfaces;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PeopleSync.Customization
{
    public class XlsCustomSource : SyncBase, ISyncSource
    {
        private static readonly Dictionary<string, string> MappingFields = new Dictionary<string, string>()
        {
            {"CostCenter","Customattribute1"},
            {"HRCostCenterDescription","CustomAttribute2"},
            {"WorkLocation","WorkLocation"},
            {"State","state"},
            {"EmployeeID","employeeId"},
            {"FirstName","givenName"},
            {"LastName","surname"},
            {"PrimaryTelephoneNumber(Work)","telephoneNumber"},
            {"Email","mail"},
            {"PrimaryIndustry","CustomAttribute4"},
            {"SecondaryIndustry","CustomAttribute5"},
            {"ServiceLine","CustomAttribute6"},
            {"JobFamily","CustomAttribute7"},
            {"IsCPA","CustomAttribute8"},
            {"Region","CustomAttribute9"},
            {"Subregion","CustomAttribute10"},
            {"SkillType","CustomAttribute11"},
            {"JobTitle","jobTitle"},
            {"PreferredName","displayName"},
            {"Manager","manager"},
            {"Address1","StreetAddress"},
            {"City","city"},
            {"Zip","PostalCode"},
            {"FaxNumber","FacsimileTelephoneNumber"},
            {"Department","department"},
           };


        public XlsCustomSource(SyncRequest syncRequest)
        {
            Configuration = syncRequest;
        }
        public QueryResult QueryUsers(string resourceSet, string nextLink, ICollection<string> objectClassList, ICollection<string> propertyList)
        {
            var localFile = "C:\\temp\\sample.xlsx";

            var ftpConfig = Path.Combine(Directory.GetCurrentDirectory(), "ftp.json");
            if (File.Exists(ftpConfig))
            {
                var ftpContent = File.ReadAllText(ftpConfig);
                var ftpconfig = JsonConvert.DeserializeObject<FtpConfig>(ftpContent);
                var localfolder = ftpconfig.LocalFolder;
                DownloadFtpFile(ftpconfig.FtpUri, localfolder, ftpconfig.FtpUsername, ftpconfig.FtpPassword, ftpconfig.FileName);
                localFile = localfolder + "/" + ftpconfig.FileName;
            }
            var changes = ReadFile(localFile);
            var array = JArray.FromObject(changes);
            return new QueryResult(array, false, "");
        }

        public QueryResult QueryGroups(string resourceSet, string nextLink, ICollection<string> objectClassList, ICollection<string> propertyList)
        {
            return null;
        }

        public string FetchGroupExtensions(string lastExtensionLink, GroupExtensionTarget targets, List<KeyValuePair<string, (string CreationDate, string Visibility, string Tags, string Types)>> groupExtensions)
        {
            return "";
        }

        public Dictionary<string, string> GetData(string objectId, SyncTask.ProcessTask callback)
        {
            return new Dictionary<string, string>();
        }

        public async Task<Dictionary<string, string>> GetAssignedLicenses(string objectId)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }

        public async Task<Dictionary<string, string>> GetAppRoleAssignments(string objectId)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }

        public async Task<Dictionary<string, string>> GetDirectReports(string objectId)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }

        public async Task<Dictionary<string, string>> GetOwnedDevices(string objectId)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }

        public async Task<Dictionary<string, string>> GetUserGroups(string objectId)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }
        
        public async Task<Dictionary<string, string>> GetRegisteredDevices(string objectId)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }

        public async Task<Dictionary<string, string>> GetExtendedProperties(string objectId)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }

        public async Task<Dictionary<string, string>> GetManager(string objectId)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }

        public async Task<QueryResult> GetGroupMembers(string objectId, string nextLink)
        {
            return await Task.Run(() => new QueryResult(new Dictionary<string, string>(), false, nextLink));
        }

        public async Task<Dictionary<string, string>> GetExtendedPropertiesForGroup(string objectId)
        {
            return await Task.Run(() => new Dictionary<string, string>());
        }

        public async Task<Dictionary<string, string>> OnPremisesExtensionAttributes(string objectId)
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
                    var cells = SpreadSheetHelper.GetRowCells(row);
                    var index = 0;
                    foreach (var cell in cells)
                    {
                        if (index >= header.Descendants<Cell>().Count()) break;

                        var key = GetCellValue(spreadSheetDocument, header.Descendants<Cell>().ElementAt(index));
                        var val = GetCellValue(spreadSheetDocument, cell);

                        if (MappingFields.TryGetValue(key, out var mappedKey))
                        {
                            key = mappedKey;
                        }
                        entry.Add(key.Replace("(", "").Replace(")", "").Replace(" ", ""), val);
                        index++;
                    }

                    entry.Add("objectId", entry["employeeId"]);
                    entry.Add("odata.type", "Microsoft.DirectoryServices.User");
                    entry.Add("AccountEnabled", "True");
                    entry.Add("CompanyName", "CLA");
                    entry.Add("DirSyncEnabled", "True");
                    entry.Add("MailNickName", entry["mail"].Split('@')[0]);
                    entry.Add("userPrincipalName", entry["mail"]);
                    entry.Add("ObjectType", "User");
                    entry.Add("UserType", "Member");
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
        private static void DownloadFtpFile(string ftpFile, string localFile, string userName, string password, string fname)
        {
            try
            {
                var request = (FtpWebRequest)WebRequest.Create(ftpFile + "/" + fname);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(userName, password);
                request.UsePassive = true;
                request.UseBinary = true;
                request.EnableSsl = false;

                var response = (FtpWebResponse)request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (Stream fileStream = new FileStream(@localFile + "/" + fname, FileMode.CreateNew))
                    {
                        responseStream.CopyTo(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
    public class FtpConfig
    {
        public string FtpUri { get; set; }
        public string FileName { get; set; }
        public string LocalFolder { get; set; }
        public string FtpUsername { get; set; }
        public string FtpPassword { get; set; }

    }
    //https://stackoverflow.com/questions/3837981/reading-excel-open-xml-is-ignoring-blank-cells/3981249
    public class SpreadSheetHelper
    {
        ///<summary>returns an empty cell when a blank cell is encountered
        ///</summary>
        public static IEnumerable<Cell> GetRowCells(Row row)
        {
            var currentCount = 0;
            foreach (var cell in row.Descendants<Cell>())
            {
                var columnName = GetColumnName(cell.CellReference);
                var currentColumnIndex = ConvertColumnNameToNumber(columnName);
                for (; currentCount < currentColumnIndex; currentCount++)
                {
                    yield return new Cell();
                }
                yield return cell;
                currentCount++;
            }
        }

        /// <summary>
        /// Given a cell name, parses the specified cell to get the column name.
        /// </summary>
        /// <param name="cellReference">Address of the cell (ie. B2)</param>
        /// <returns>Column Name (ie. B)</returns>
        public static string GetColumnName(string cellReference)
        {
            // Match the column name portion of the cell name.
            var regex = new Regex("[A-Za-z]+");
            var match = regex.Match(cellReference);
            return match.Value;
        }

        /// <summary>
        /// Given just the column name (no row index),
        /// it will return the zero based column index.
        /// </summary>
        /// <param name="columnName">Column Name (ie. A or AB)</param>
        /// <returns>Zero based index if the conversion was successful</returns>
        /// <exception cref="ArgumentException">thrown if the given string
        /// contains characters other than uppercase letters</exception>
        public static int ConvertColumnNameToNumber(string columnName)
        {
            var alpha = new Regex("^[A-Z]+$");
            if (!alpha.IsMatch(columnName)) throw new ArgumentException();
            var colLetters = columnName.ToCharArray();
            Array.Reverse(colLetters);
            return colLetters.Select((letter, i) => i == 0 ? letter - 65 : letter - 64).Select((current, i) => current * (int)Math.Pow(26, i)).Sum();
        }
    }
}