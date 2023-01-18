using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirtableApiClient;
using social_bot.User;

namespace social_bot.SpreadSheetSaver
{
    /// <summary>
    /// менеджер сохранения в таблицу airtable
    /// </summary>
    public class AirTableSaver: ISpreadSheetSaver
    {
        private readonly string baseId = "apprjtivvPIJcIZO8";
        private readonly string apiKey = "keyteL0nsmllOVAF8";

        private string tableName = "blanks";

        private static AirTableSaver instance;

        public static AirTableSaver Instance => instance ??= new AirTableSaver();

        public void Save()
        {
            AsyncSave();
        }

        public void Load()
        {
            AsyncLoad(FillRecords);
        }

        private async void AsyncSave()
        {
            using (AirtableBase airtableBase = new AirtableBase(apiKey, baseId))
            {
                foreach (UserBlank userBlank in BlankManager.Instance.Blanks)
                {
                    if (userBlank.ID == 0)
                        continue;

                    Fields fields = new Fields();
                    fields.AddField("Telegram ID", userBlank.ID.ToString());
                    fields.AddField("Telegram User Name", userBlank.UserName);
                    fields.AddField("Status", userBlank.Status.ToString());
                    fields.AddField("Introducing", userBlank.Description);
                    foreach (QuestionBase question in userBlank.Questions)
                    {
                        fields.AddField(question.ColumnName, question.GetAnswerText());
                    }

                    if (string.IsNullOrEmpty(userBlank.RowId))
                    {
                        AirtableCreateUpdateReplaceRecordResponse result =
                            await airtableBase.CreateRecord(tableName, fields);
                        if (result.Success)
                        {
                            userBlank.RowId = result.Record.Id;
                        }
                    }
                    else await airtableBase.UpdateRecord(tableName, fields, userBlank.RowId);
                }
            }
        }

        private async void AsyncLoad(Action<List<AirtableRecord>> _onFillRecords)
        { 
            List<AirtableRecord> records = new List<AirtableRecord>(); 
            string offset = null;

            using (AirtableBase airtableBase = new AirtableBase(apiKey, baseId))
            {
                do
                {
                    Task<AirtableListRecordsResponse> task = airtableBase.ListRecords(tableName);

                    AirtableListRecordsResponse response = await task;

                    if (response.Success)
                    {
                        records.AddRange(response.Records.ToList());
                        offset = response.Offset;
                    }
                    else if (response.AirtableApiError is AirtableApiException)
                    {
                        string errorMessage = response.AirtableApiError.ErrorMessage;
                        if (response.AirtableApiError is AirtableInvalidRequestException)
                        {
                            errorMessage += "\nDetailed error message: ";
                            errorMessage += response.AirtableApiError.DetailedErrorMessage;
                        }
                        Console.WriteLine(errorMessage);
                        break;
                    }
                    else
                    {
                        Console.WriteLine( "Unknown error while load table");
                        break;
                    }
                } while (offset != null);
                _onFillRecords.Invoke(records);
            }
        }

        public void FillRecords(List<AirtableRecord> _records)
        {
            BlankManager.Instance.Blanks.RemoveRange(1, BlankManager.Instance.Blanks.Count-1);
            UserBlank tempale = BlankManager.Instance.Template;
            foreach (AirtableRecord record in _records)
            { 
                UserBlank userBlank = new UserBlank();
                userBlank.ID = long.Parse(record.GetField("Telegram ID")?.ToString());
                if (userBlank.ID == 0)
                    continue;

                userBlank.RowId = record.Id;
                userBlank.UserName = record.GetField("Telegram User Name")?.ToString();
                userBlank.Status = Enum.Parse<UserStatus>(record.GetField("Status")?.ToString());
                userBlank.Description = record.GetField("Introducing")?.ToString();
                userBlank.Questions = new QuestionBase[tempale.Questions.Length];
                for (int i = 0; i < userBlank.Questions.Length; i++)
                {
                    userBlank.Questions[i] = new QuestionText();
                    userBlank.Questions[i].ColumnName = tempale.Questions[i].ColumnName;
                    object value = record.GetField(userBlank.Questions[i].ColumnName);
                    if (value != null)
                        (userBlank.Questions[i] as QuestionText).SetAnswer(value.ToString());
                    userBlank.Questions[i].Complete();
                }
                BlankManager.Instance.Blanks.Add(userBlank);
            }
        }
    }
}
