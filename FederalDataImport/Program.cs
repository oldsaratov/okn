using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using OKN.Core;
using OKN.Core.Mappings;
using OKN.Core.Models;
using OKN.Core.Models.Commands;
using OKN.Core.Repositories;
using Uploadcare;
using Uploadcare.Upload;

namespace FederalDataImport
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var url1 = new MongoUrl("mongodb://localhost:27017/okn?retryWrites=true");

            var client1 = new MongoClient(url1);
            var database1 = client1.GetDatabase(url1.DatabaseName);
            var collection1 = database1.GetCollection<BsonDocument>("objects");

            //var okCount = 0;

            var context = new DbContext(database1);
            var config = new MapperConfiguration(cfg => cfg.AddProfile(typeof(MappingProfile)));
            var mapper = config.CreateMapper();

            var repo1 = new ObjectsRepository(mapper, context);
            var repo2 = new ObjectsEventRepository(mapper, context);

            var client = new UploadcareClient("", "");
            var uploader = new UrlUploader(client);


            foreach (var item in collection1.AsQueryable())
            {
                if (item.Contains("events") && item["events"] != BsonNull.Value && item["events"].AsBsonArray.Any())
                {
                    if (!item.Contains("typeHistory") || item["typeHistory"] == BsonNull.Value || !item["typeHistory"].AsBsonArray.Any())
                    {
                        var objectId = item["_id"].ToString();

                        if (item["events"].AsBsonArray.Any(x => !x.AsBsonDocument.Contains("type")))
                        {
                            ;
                        }

                        var events = item["events"].AsBsonArray.Where(x => x["type"] == 1).ToList();
                        if (events.Count == 1)
                        {
                            var oknevent = events.FirstOrDefault().AsBsonDocument;


                            var updateObjectCommand = new UpdateObjectCommand(objectId)
                            {
                                TypeHistory = new OknTypeHistory
                                {
                                    OccuredAt = DateTime.Parse(oknevent["occuredAt"].ToString()),
                                    Type = Enum.Parse<EObjectType>(item["type"].ToString()),
                                    Reason = oknevent["description"].ToString()
                                }
                            };

                            await repo1.UpdateObject(updateObjectCommand, CancellationToken.None);
                        }
                        else
                        {
                            Console.WriteLine(objectId);
                        }
                    }
                }

                //if (item.Contains("federal") && item["federal"] != BsonNull.Value && (!item.Contains("mainPhoto") || item["mainPhoto"] == BsonNull.Value))
                //{
                //    var federalInfo = item["federal"].AsBsonDocument;
                //    if (federalInfo.Contains("photo") && federalInfo["photo"] != BsonNull.Value)
                //    {
                //        Console.WriteLine($"Uploading photos for {item["name"]}");

                //        var photo = federalInfo["photo"];
                //        var url = photo.AsBsonDocument["url"].AsBsonValue.ToString();

                //        var objectId = item["_id"].ToString();

                //        try
                //        {
                //            var uploaded = await uploader.Upload(url, true);

                //            var updateObjectCommand = new UpdateObjectCommand(objectId)
                //            {
                //                MainPhoto = new FileInfo
                //                {
                //                    Description = "Источник: АИС ЕГРКН",
                //                    FileId = uploaded.FileId,
                //                    Url = uploaded.Url
                //                }
                //            };

                //            await repo1.UpdateObject(updateObjectCommand, CancellationToken.None);
                //        }
                //        catch (Exception e)
                //        {
                //            Console.WriteLine(e);
                //            continue;
                //        }

                //        await Task.Delay(2000);
                //    }
                //}

                //var federalName = item["nativeName"].ToString();

                //var nameTokenFilter = Builders<BsonDocument>.Filter.Regex("name", new BsonRegularExpression(federalName, "i"));

                //var localItems = collection2.Find(nameTokenFilter).ToList();
                //if (localItems == null || !localItems.Any())
                //{
                //    Console.WriteLine($"FAIL Can't find local item for federal='{federalName}'");
                //}
                //else
                //{
                //    if (localItems.Count > 1)
                //    {
                //        Console.WriteLine($"FAIL Find many local items for federal='{federalName}';");
                //    }
                //    else
                //    {
                //        var localItem = localItems.FirstOrDefault();
                //        Console.WriteLine($"OK Find local item for federal='{federalName}'; local='{localItem["name"]}'");
                //        okCount++;
                //    }
                //}


                //UpdateDefinition<BsonDocument> up = null;


                //UpdateDefinition<BsonDocument> up = Builders<BsonDocument>.Update
                //    .Unset("longitude")
                //    .Unset("latitude");

                //if (item.Contains("federal") && item["federal"] != null && !item.Contains("events"))
                //{
                //    var federalInfo = item["federal"].AsBsonDocument;

                //    string description = null;


                //    if (federalInfo.Contains("documents"))
                //    {
                //        var documents = federalInfo["documents"].AsBsonArray;

                //        foreach (var document in documents)
                //        {
                //            var objectId = item["_id"].ToString();
                //            var eventId = Guid.NewGuid().ToString();
                //            var createObjectEventCommand = new CreateObjectEventCommand(objectId, eventId)
                //            {
                //                Name = "Постановка на учет",
                //                Type = EObjectEventType.Protection
                //            };

                //            var documentName = document.AsBsonDocument["name"].AsBsonValue;
                //            createObjectEventCommand.Description = $"{documentName}";

                //            if (document.AsBsonDocument.Contains("number"))
                //            {
                //                var documentNumber = document.AsBsonDocument["number"].AsBsonValue;
                //                createObjectEventCommand.Description += $" №{documentNumber}";
                //            }

                //            if (document.AsBsonDocument.Contains("date"))
                //            {
                //                var documentDate = DateTime.Parse(document.AsBsonDocument["date"].ToString());
                //                createObjectEventCommand.Description += $" от {documentDate:dd.MM.yyyy}";

                //                createObjectEventCommand.OccuredAt = documentDate;
                //            }

                //            try
                //            {
                //                await repo2.CreateObjectEvent(createObjectEventCommand, CancellationToken.None);
                //            }
                //            catch (Exception e)
                //            {
                //                Console.WriteLine(e);
                //                throw;
                //            }
                //        }
                //    }
                //}

                //    //    up = Builders<BsonDocument>.Update
                //    //        .Set("type", int.Parse(federalInfo["categoryType"].AsBsonDocument["id"].AsString));


                //if (item.Contains("federal") && item["federal"] != null)
                //{
                //    var federalInfo = item["federal"].AsBsonDocument;

                //    if (federalInfo.Contains("address"))
                //    {
                //        if (federalInfo["address"] == BsonNull.Value) continue;

                //        var address = federalInfo["address"].AsBsonDocument;
                //        if (address.Contains("mapPosition") &&
                //            federalInfo["address"].AsBsonDocument["mapPosition"].AsBsonDocument["type"].AsBsonValue ==
                //            "Point")
                //        {

                //            Console.WriteLine(
                //                $"lon: {address["mapPosition"].AsBsonDocument["coordinates"].AsBsonArray[0].AsBsonValue}");
                //            Console.WriteLine(
                //                $"lat: {address["mapPosition"].AsBsonDocument["coordinates"].AsBsonArray[1].AsBsonValue}");

                //            up = Builders<BsonDocument>.Update
                //                .Set("longitude",
                //                    address["mapPosition"].AsBsonDocument["coordinates"].AsBsonArray[0].AsBsonValue
                //                        .ToString())
                //                .Set("latitude",
                //                    address["mapPosition"].AsBsonDocument["coordinates"].AsBsonArray[1].AsBsonValue
                //                        .ToString());
                //        }
                //    }
                //}


                //if (up != null)
                //{
                //    var filter = Builders<BsonDocument>.Filter.Eq(x => x["_id"], item["_id"]);
                //    collection1.UpdateOne(filter, up);
                //}
            }

            Console.WriteLine($"Done!");

            Console.ReadKey();
        }
    }
}
