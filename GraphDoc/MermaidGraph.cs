using System.Text.Json.Serialization;

namespace GraphDoc;

public class MermaidGraph
{
    [JsonPropertyName("code")] public string Code { get; set; } = "";
    [JsonPropertyName("mermaid")] public string Mermaid { get; set; } = @"{""theme"": ""dark""}";
    [JsonPropertyName("autoSync")] public bool AutoSync { get; set; } = true;
    [JsonPropertyName("updateDiagram")] public bool UpdateDiagram { get; set; } = false;
    [JsonPropertyName("panZoom")] public bool PanZoom { get; set; } = true;
    [JsonPropertyName("zoom")] public double Zoom { get; set; } = 1;
    [JsonPropertyName("pan")] public Coordinate Pan { get; set; } = new();
}
public class Coordinate
{
    [JsonPropertyName("x")] public double X { get; set; } = 0;
    [JsonPropertyName("y")] public double Y { get; set; } = 0;
}