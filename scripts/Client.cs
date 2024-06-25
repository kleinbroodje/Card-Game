using Godot;
using System;

public enum Suit 
    {
        diamonds, clubs, hearts, spades,
    }

public enum Value
    {
        three, four, five, six, seven, eight, nine, ten, jack, queen, king, ace, two
    }

public enum Combinations
	{
		single, pair, triple, straight, flush, fullHouse, fourOfKind, straightFlush
	}

public partial class Client : Node
{
	const string HOST = "127.0.0.1";
	const int PORT = 7878;
	public static StreamPeerTcp client;
	bool connected = false;
	public static Godot.Collections.Array dataRaw;
	public static String data;

	//make socket and connect to server
	public override void _Ready()
	{ 
		client  = new StreamPeerTcp();
		client.ConnectToHost(HOST, PORT);
	}

	//process server requests 
    public override async void _Process(double delta)
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

		var bytes = client.GetAvailableBytes();
		dataRaw = client.GetPartialData(bytes);
		data = System.Text.Encoding.UTF8.GetString((byte[])dataRaw[1]);
	}

}	
