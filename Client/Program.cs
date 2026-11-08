using System;
using System.Net.Sockets;
using System.Text;

namespace Client;

class Program
{
    static async Task Main()
    {
        // connect to server
        string server = "127.0.0.1";
        int port = 45445;
        
        var client = new TcpClient(server, port);
        var stream = client.GetStream();

        // client is writing name
        var bufferOk = new byte[1];
        while(bufferOk[0] == 0)
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();
            if(!(name == "" || name == " "))
            {
                var nameByte = Encoding.UTF8.GetBytes(name);
                stream.Write(nameByte, 0, nameByte.Length); 
                var bytesRead = stream.Read(bufferOk, 0, bufferOk.Length);
            }
        }

        Console.WriteLine("Connect to server.");

        _ = GiveMessage(stream);

        // send message
        while(true)
        {
            var message = await Task.Run(() => Console.ReadLine());
            if(message == "close") break;

            // send message
            var buffer = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(buffer, 0, buffer.Length);
        }
    }

    static async Task GiveMessage(NetworkStream stream)
    {
        while(true)
        {
            var responseBuffer = new byte[1024];
            var bytesRead =  await stream.ReadAsync(responseBuffer, 0, responseBuffer.Length);
            if(bytesRead == 0) break;
            var response = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);
            Console.WriteLine(response);
        }
    }
}