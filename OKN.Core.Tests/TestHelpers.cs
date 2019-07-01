using AutoMapper;
using MongoDB.Driver;
using OKN.Core.Mappings;

public static class TestHelpers
{
    public static IMapper GetDefaultMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile(typeof(MappingProfile)));
        var mapper = config.CreateMapper();

        return mapper;
    }

    public static IMongoDatabase GetDefaultDatabase(string connectionString)
    {
        var client = new MongoClient(connectionString);
        return client.GetDatabase("okn");
    }
}