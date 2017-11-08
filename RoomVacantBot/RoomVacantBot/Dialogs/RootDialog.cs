using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;
using System.Collections.Generic;

namespace RoomVacantBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        readonly static string azureStorageAccount = "YourAzureStorageAccount";
        readonly static string azureStorageKey = "YourAzureStorageKey";
        readonly static string azureStorageConnStr = "DefaultEndpointsProtocol=https;AccountName="+ azureStorageAccount + ";AccountKey=" + azureStorageKey +";EndpointSuffix=core.windows.net";

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var tableResultList = await GetTableResult();
            var roomList = await GetRoomList(tableResultList);
            PromptDialog.Choice(context, RoomStatusAsync,roomList, "空きを調べたい会議室を選択してください。");

        }

        public class resultEntity : TableEntity
        {
            public resultEntity(string roomId, string modifiedTime)
            {
                PartitionKey = roomId;
                RowKey = modifiedTime;
            }
            public resultEntity() { }
            public string occupied { get; set; }
            public string vacant { get; set; }
        }

        private async Task<List<resultEntity>> GetTableResult()
        {
            var storageAccount = CloudStorageAccount.Parse(azureStorageConnStr);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("analyzedTable");
            var query = new TableQuery<resultEntity>();
            var tableResult = table.ExecuteQuery(query);
            var tableResultList = table.ExecuteQuery(query).ToList();

            return tableResultList;
        }
        private async Task<List<string>> GetRoomList(List<resultEntity> tableResultList)
        {
            var roomList = new List<string>();

            foreach (var item in tableResultList)
            {
                roomList.Add(item.PartitionKey.ToString().Replace(".jpg",""));
            }
            return roomList;
        }

        private async Task RoomStatusAsync(IDialogContext context, IAwaitable<string> result)
        {
            var roomId = await result;

            var storageAccount = CloudStorageAccount.Parse(azureStorageConnStr);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("analyzedTable");
            var query = new TableQuery<resultEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, roomId+".jpg" ));
            var tableResultList = table.ExecuteQuery(query).ToList();

            var roomStatusMsg = $"{roomId} の利用状況を判定できませんでした。";
            foreach (var item in tableResultList)
            {
                if (item.PartitionKey == roomId + ".jpg")
                {
                    var occupied = Double.Parse(item.occupied);
                    var vacant = Double.Parse(item.vacant);
                    if ( occupied > 0.7 && occupied > vacant )
                    {
                        roomStatusMsg = $"{roomId} は使用中です。";
                    }
                    else if ( vacant > 0.7 && vacant > occupied )
                    {
                        roomStatusMsg = $"{roomId} は未使用です。";
                    }
                }
            }

            var message = context.MakeMessage();
            message.Text = roomStatusMsg;
            message.Attachments.Add((new Attachment()
            {
                ContentUrl = "https://" + azureStorageAccount + ".blob.core.windows.net/images/" + roomId +".jpg",
                ContentType = "image/jpg",
            }));

            await context.PostAsync(message);

            context.Done<object>(null);
        }
    }
}