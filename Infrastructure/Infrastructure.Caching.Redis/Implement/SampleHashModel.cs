using Infrastructure.Caching.Redis.Interface;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Caching.Redis.Implement;

public record struct SampleHashModel : IRedisHashInitializable
{
    public string Name { get; set; } = string.Empty;
    
    public int Age { get; set; } = -1;

    public SampleHashModel()
    {

    }

    public string GetRedisKey()
    {
        return $"user:{Name}:{Age}";
    }

    public void LoadFromHash(HashEntry[] entries)
    {
        if (entries.Length <= 0)
        {
            return;
        }

        foreach (var entry in entries)
        {
            var value = entry.Value.ToString();

            if (string.IsNullOrEmpty(value))
                continue;

            switch (entry.Name.ToString())
            {
                case "Name":
                    Name = value;
                    break;
                case "Age":
                    if (int.TryParse(value, out var age))
                        Age = age;
                    break;
            }
        }
    }

    public HashEntry[] ConvertToHash()
    {
        var model = new HashEntry[]
        {
                new HashEntry("Name", Name ?? string.Empty),
                new HashEntry("Age", Age.ToString())
        };

        return model;
    }

}
