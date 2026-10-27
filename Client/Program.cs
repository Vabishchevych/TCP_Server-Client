using System;
using System.Net.Sockets;
using System.Text;

namespace Client;

class Program
{
    static void Main()
    {
        // connect to server
        string server = "127.0.0.1";
        int port = 45445;

        var client = new TcpClient(server, port);
        var stream = client.GetStream();

        Console.WriteLine("Connect to server.");

        // send and give message
        while(true)
        {
            var message = Console.ReadLine();
            if(message == "close") break;

            // send message
            var buffer = Encoding.UTF8.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);

            // give answer
            var responseBuffer = new byte[1024];
            var bytesRead = stream.Read(responseBuffer, 0, responseBuffer.Length);
            var response = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);
            Console.WriteLine(response);
        }
    }
}