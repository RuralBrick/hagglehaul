using EphemeralMongo;
using MongoDB.Driver;

namespace hagglehaul.Tests.ServiceTests;

public class ServiceTestsBase
{
    protected IMongoDatabase _database = null;
    private IMongoRunner _runner = null;

    [OneTimeSetUp]
    public void ServiceTestsBaseSetup()
    {
        var options = new MongoRunnerOptions()
        {
            KillMongoProcessesWhenCurrentProcessExits = true
        };

        _runner = MongoRunner.Run(options);
        
        _database = new MongoClient(_runner.ConnectionString).GetDatabase("default");
    }
    
    [OneTimeTearDown]
    public void ServiceTestsBaseTearDown()
    {
        _runner.Dispose();
    }
}