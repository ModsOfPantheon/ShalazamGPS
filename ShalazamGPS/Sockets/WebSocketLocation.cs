namespace ShalazamGPS.Sockets;

public class WebSocketLocation
{
    public float? LocX { get; set; }
    public float? LocY { get; set; }
    public float? LocZ { get; set; }
    public string ClientState { get; set; }
    public float? Heading { get; set; }
}