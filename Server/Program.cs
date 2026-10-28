using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server;

class Program
{

    static public List<TcpClient> clients = new List<TcpClient>();
    static async Task Main()
    {
        // init and start server
        int port = 45445;
        TcpListener listener = new TcpListener(IPAddress.Any, port);

        Console.WriteLine("Start server");

        listener.Start(2);

        while(true)
        {
             // accept client
            var client = await listener.AcceptTcpClientAsync();
            clients.Add(client);

            _ = MessageFromClients(client);   
        }
    }

    static async Task MessageFromClients(TcpClient client)
    {

        var stream = client.GetStream();
        while(true)
        {
            // Buffering and message processing cycle
            var buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            if(bytesRead == 0) break;

            var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine(message);

            var response = Encoding.UTF8.GetBytes("The server received a message.");
            await stream.WriteAsync(response, 0, response.Length);

            List<TcpClient> clientsCopy = new List<TcpClient>(clients);
            foreach(TcpClient i in clientsCopy)
            {
                var streamSend = i.GetStream();
                if(streamSend != stream) await streamSend.WriteAsync(buffer, 0, buffer.Length);
            }
        }
        clients.Remove(client);
        client.Close();
    }
}