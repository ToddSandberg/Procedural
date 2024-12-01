using Godot;
using System;
using System.Collections.Generic;

public class House {
    
	public string id { get; set; } = System.Guid.NewGuid().ToString();
    public List<string> household { get; set; } = new List<string>();
    public float positionx { get; set; }
    public float positiony { get; set; }
    public float positionz { get; set; }
    public string town { get; set; }
}
