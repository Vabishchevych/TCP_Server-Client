using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server;

class Program
{
    static void Main()
    {
        // init and start server
        int port = 45445;
        TcpListener listener = new TcpListener(IPAddress.Any, port);

        Console.WriteLine("Start server");

        listener.Start(1);

        // accept client
        var client = listener.AcceptTcpClient();
        var stream = client.GetStream();

        // Buffering and message processing cycle
        var buffer = new byte[1024];
        int bytesRead;

        while(true)
        {
            bytesRead = stream.Read(buffer, 0, buffer.Length);
            if(bytesRead == 0) break;

            var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine(message);

            var response = Encoding.UTF8.GetBytes("The server received a message.");
            stream.Write(response, 0, response.Length);
        }
        // close working with client
        client.Close();
    }
}