using System.Net;
using System.Net.Sockets;
using System.Text;

var host = Dns.GetHostName();
var localhost = await Dns.GetHostEntryAsync(host);
var localIpAddress = localhost.AddressList[0];

var ipEndPoint = new IPEndPoint(localIpAddress, 80);

if (args.Length == 2) {
    ipEndPoint = new IPEndPoint(IPAddress.Parse(args[0]), Convert.ToInt32(args[1]));
}

using var client = new TcpClient();
client.Connect(ipEndPoint);

var stream = client.GetStream();

var buff = new byte[32];
var received = stream.Read(buff);

var msg = Encoding.ASCII.GetString(buff, 0, received);
Console.WriteLine(msg);

var receiverTask = new Task(async () => {
    while (true) {
        var recBuff = new byte[128];
        var recCount = await stream.ReadAsync(recBuff);

        var recMsg = Encoding.ASCII.GetString(recBuff, 0, recCount);
        Console.WriteLine(recMsg);
    }
});

receiverTask.Start();

while (true) {
    var msgOut = Console.ReadLine() ?? string.Empty;
    stream.Write(Encoding.ASCII.GetBytes(msgOut));
}