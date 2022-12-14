
using Genesis.Commons;
using Serilog;

Logging.Start();

var test = new Test("testing baby", "description");
Log.Information("Test: {@test}", test);

var serialized = MsgPack.Serialize(test);
Log.Information("Serialized: {@se}", serialized);

var deserialized = MsgPack.Deserialize<Test>(serialized);
Log.Information("Deserialized: {@des}", deserialized);

Console.ReadLine();

public class Test
{
    public Test()
    {
        this.Name = "Default";
        this.Description = "Default";
    }

    public Test(string name, string description)
    {
        this.Name = name;
        this.Description = description;
    }

    public string Name { get; set; }
    public string Description { get; set; }
}