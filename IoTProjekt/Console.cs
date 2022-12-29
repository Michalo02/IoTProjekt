using Device;      //zmodyfikowac tak zeby laczylo z iotsim i ua i petla zeby 
using Microsoft.Azure.Devices.Client;
using Opc.UaFx;
using Opc.UaFx.Client;

internal class Program
{
    private static async Task Main(string[] args)
    {
        string deviceConnectionString = "HostName=PrzykladoweCentrumIoT.azure-devices.net;DeviceId=id-13102022;SharedAccessKey=TMC3nAGfSzGxOCy18yq8swX8hSOOquLTVCUDkqzP2YI=";

        using var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Mqtt);
        await deviceClient.OpenAsync();
        var device = new Device(deviceClient);

        await device.InitializeHandlers();
        await device.UpdateTwinAsync();

        Console.WriteLine($"Connection success!");

        await device.SendMessages(10, 1000);

        Console.WriteLine("Finished! Press Enter to close...");
        Console.ReadLine();

        using (var client = new OpcClient("opc.tcp://localhost:4840/"))
        {
            client.Connect();

            OpcReadNode[] commands = new OpcReadNode[]
            {
        new OpcReadNode("ns=2;s=Device 1/ProductionStatus", OpcAttribute.DisplayName),
        new OpcReadNode("ns=2;s=Device 1/ProductionStatus"),
        new OpcReadNode("ns=2;s=Device 1/ProductionRate"),
        new OpcReadNode("ns=2;s=Device 1/WorkorderId", OpcAttribute.DisplayName),
        new OpcReadNode("ns=2;s=Device 1/WorkorderId"),
        new OpcReadNode("ns=2;s=Device 1/Temperature", OpcAttribute.DisplayName),
        new OpcReadNode("ns=2;s=Device 1/Temperature"),
        new OpcReadNode("ns=2;s=Device 1/GoodCount", OpcAttribute.DisplayName),
        new OpcReadNode("ns=2;s=Device 1/GoodCount"),
        new OpcReadNode("ns=2;s=Device 1/BadCount", OpcAttribute.DisplayName),
        new OpcReadNode("ns=2;s=Device 1/BadCount"),
        new OpcReadNode("ns=2;s=Device 1/DeviceError", OpcAttribute.DisplayName),
        new OpcReadNode("ns=2;s=Device 1/DeviceError"),

            };

            IEnumerable<OpcValue> job = client.ReadNodes(commands);

            foreach (var item in job)
            {
                Console.WriteLine(item.Value);
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            OpcNodeInfo machineNode = client.BrowseNode("ns=2;s=Device 1");

            foreach (var chlidNode in machineNode.Children())
            {
                var isExecutable = chlidNode.Attribute(OpcAttribute.Executable)?.Value;
                Console.WriteLine($"{chlidNode.DisplayName}{isExecutable}");
            }
        }
    }
}