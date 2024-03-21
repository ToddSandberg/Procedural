using Godot;
using System;
using System.Collections.Generic;

public class Race {
    // Keep track of each origin and the amount of influence
    public Dictionary<Origin, int> origins = new Dictionary<Origin, int>();

    public void AddOrigins(Dictionary<Origin, int> originsToAdd) {
        foreach(KeyValuePair<Origin, int> entry in originsToAdd) {
            if (origins.ContainsKey(entry.Key)) {
                origins.Add(entry.Key, origins[entry.Key] + entry.Value);
            }
        }
    }
}
