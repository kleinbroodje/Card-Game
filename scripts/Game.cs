using Godot;
using System;
using System.Data;
using System.Threading;

public partial class Game : Node
{
	const string HOST = "127.0.0.1";
	const int PORT = 7878;
	static StreamPeerTcp client;
	Thread recvThread;
	static bool connected = false;

	public enum Suit 
    {
        diamonds, clubs, hearts, spades,
    }

	public enum Value
    {
        three, four, five, six, seven, eight, nine, ten, jack, queen, king, ace, two
    }

	static void Recv()
	{
		while (true)
		{
			client.Poll();
			if (!connected) 	
			{
				if (client.GetStatus() == StreamPeerTcp.Status.Connected)
				{
					Console.WriteLine("Connected to server");
					connected = true;
				}
			}

			var data = client.GetUtf8String(2048);
			Console.WriteLine(Convert.ToString(data));
		}	
	}

	public override void _Ready()
	{ 
		client  = new StreamPeerTcp();
		client.ConnectToHost(HOST, PORT);
		recvThread = new Thread(Recv);
		recvThread.Start();
	}

	public override void _Process(double delta)
	{
		Hand.Update();
	}
}	
