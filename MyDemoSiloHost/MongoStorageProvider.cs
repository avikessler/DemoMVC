using Orleans.Storage;
using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;
using Newtonsoft.Json;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace MyDemoSiloHost
{
  class MongoStorageProvider : IStorageProvider
  {
    public Logger Log { get; set; }

    public string Name { get; set; }

    private MongoClient client { get; set; }

    public IMongoDatabase database { get; set; }

    public IMongoCollection<BsonDocument> collection { get; set; }
    public async Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
    {
      string key = grainReference.GetPrimaryKeyString() + "." + grainState.State.GetType().Name;
      FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("key", key);
      await collection.DeleteOneAsync(filter);

    }

    public Task Close()
    {
      return Task.CompletedTask;
    }

    public Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
    {
      this.Name = name;
      client = new MongoClient(config.Properties["ConnectionString"]);
      database = client.GetDatabase("GrainsStateStore");
      collection = database.GetCollection<BsonDocument>("Data");
      return Task.CompletedTask;
    }

    public async Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
    {


      string key = grainReference.GetPrimaryKeyString() + "." + grainState.State.GetType().Name;
      FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("key", key);
      BsonDocument document = await collection.Find(filter).FirstOrDefaultAsync();
      if (document != null) grainState.State = BsonSerializer.Deserialize(document["state"].AsBsonDocument, grainState.State.GetType());

    }

    public async Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
    {
      try
      {
        string key = grainReference.GetPrimaryKeyString() + "." + grainState.State.GetType().Name;

        BsonDocument storedData = new BsonDocument
           {
          { "key", key },
          { "state", grainState.State.ToBsonDocument() }
      };
        FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("key", key);
        UpdateOptions updateOptions = new UpdateOptions();
        updateOptions.IsUpsert = true;
     
        await collection.ReplaceOneAsync(filter, storedData, updateOptions);
      }
      catch (Exception ex)
      {

        throw;
      }
      


    }
  }
}
