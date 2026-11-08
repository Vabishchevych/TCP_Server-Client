using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server;

class Program
{

    static public List<TcpClient> clients = new List<TcpClient>();
    static public List<string> clientNames = new List<string>();
    static async Task Main()
    {
        // init and start server
        int port = 45445;
        TcpListener listener = new TcpListener(IPAddress.Any, port);

        Console.WriteLine("Start server");

        listener.Start();

        while(true)
        {
             // accept client
            var client = await listener.AcceptTcpClientAsync();

            _ = MessageFromClients(client);   
        }
    }

    static async Task MessageFromClients(TcpClient client)
    {

        var stream = client.GetStream();

        // add client's name
        var bufferName = new byte[1024];
        int nameByte;
        string name = "";

        while(true)
        {
            bufferName = new byte[1024];
            nameByte = await stream.ReadAsync(bufferName, 0, bufferName.Length);
            if(nameByte == 0) return;
            name = Encoding.UTF8.GetString(bufferName, 0, nameByte);
            bool isName = true;
            foreach(var i in clientNames)
            {
                if(i == name)
                {
                    await stream.WriteAsync([0],0,1);
                    isName = false;
                    break;
                }
            } 
            if(isName)
            {
                clients.Add(client);
                clientNames.Add(name);
                await stream.WriteAsync([1],0,1);
                break;
            } 
        }

        while(true)
        {
            // Buffering and message processing cycle
            var buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            if(bytesRead == 0) break;

            var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine(message);

            if(message == "names") // Temporarily
            {
                foreach(var i in clientNames) Console.Write(i + " ");
                Console.WriteLine("");
            }

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
        clientNames.Remove(name);
        client.Close();
    }
}