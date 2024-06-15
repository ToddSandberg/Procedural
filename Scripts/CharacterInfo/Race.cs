using Godot;
using System;
using System.Collections.Generic;

public class Race {
    // Keep track of each origin and the amount of influence
    public Dictionary<string, int> originAmounts { get; set; }
    // TODO maybe have a global map for this
    public Dictionary<string, Origin> originMap { get; set; }

    public Race() {
        originAmounts = new Dictionary<string, int>();
        originMap = new Dictionary<string, Origin>();
    }

    public void AddOrigins(Origin origin, int amount) {
        if (!originMap.ContainsKey(origin.name)) {
            originMap.Add(origin.name, origin);
        }

        if (originAmounts.ContainsKey(origin.name)) {
            originAmounts.Add(origin.name, originAmounts[origin.name] + amount);
        } else {
            originAmounts.Add(origin.name, amount);
        }
    }

    public void AddOrigins(Dictionary<Origin, int> originsToAdd) {
        foreach(KeyValuePair<Origin, int> entry in originsToAdd) {
            if (!originMap.ContainsKey(entry.Key.name)) {
                originMap.Add(entry.Key.name, entry.Key);
            }

            if (originAmounts.ContainsKey(entry.Key.name)) {
                originAmounts[entry.Key.name] = originAmounts[entry.Key.name] + entry.Value;
            } else {
                originAmounts.Add(entry.Key.name, entry.Value);
            }
        }
    }

    public Dictionary<Origin, int> GetOrigins() {
        Dictionary<Origin, int> origins = new Dictionary<Origin, int>();
        foreach(KeyValuePair<string, Origin> entry in originMap) {
            origins.Add(entry.Value, originAmounts[entry.Key]);
        }
        return origins;
    }
}
