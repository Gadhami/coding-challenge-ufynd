﻿namespace Hotels.Infrastructure.Persistence.Configuration;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}