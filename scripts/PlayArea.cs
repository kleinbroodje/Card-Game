using Godot;
using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

public partial class PlayArea : Sprite2D
{
	public static Dictionary<string, object>  lastPlayedCards = new Dictionary<string, object>();
	int[] cardsCount = new int[13]; 
	bool straight;
	bool flush;

	public override void _Ready()
	{
		var screenSize = GetViewportRect().Size;
		Position = new Vector2(screenSize.X/2, screenSize.Y/2);
		ZIndex = -1;	
	}

	//playing cards when clicked on playArea
	public override void _Input(InputEvent @event)
	{
    	if (@event is InputEventMouseButton mouseEvent && mouseEvent.IsPressed() && mouseEvent.ButtonIndex == MouseButton.Left)
    	{	
			if (GetRect().HasPoint(ToLocal(mouseEvent.GlobalPosition)) && Hand.selectedCards.Any())
			{

				switch(Hand.selectedCards.Count)
				{
					case 1:
						lastPlayedCards.Clear();
						lastPlayedCards.Add("type", (int)Combinations.single);
						break;

					case 2:
						lastPlayedCards.Clear();
						lastPlayedCards.Add("type", (int)Combinations.pair);
						break;

					case 3:
						lastPlayedCards.Clear();
						lastPlayedCards.Add("type", (int)Combinations.triple);
						break;

					case 5:	
						lastPlayedCards.Clear();
						Hand.selectedCards.OrderBy(card => (int)card.value);
						Array.Fill(cardsCount, 0);
						flush = true;

						if (Hand.selectedCards.Any(o => o.suit != Hand.selectedCards[0].suit))
						{
							flush = false;
						}
						
						for (int i = 0; i < 5; i++)
						{	
							cardsCount[(int)Hand.selectedCards[i].value] += 1;
							if (i > 0)
							{
								
								if ((int)Hand.selectedCards[i].value == (int)Hand.selectedCards[i-1].value + 1)
								{
									straight = true;
								}
								else
								{
									straight = false;
								}
							}
						}
						
						if (cardsCount.Contains(4))
						{
							lastPlayedCards.Add("type", (int)Combinations.fourOfKind);	
						}
						else if (cardsCount.Contains(2) && cardsCount.Contains(3))
						{
							lastPlayedCards.Add("type", (int)Combinations.fullHouse);	
						}
						else if (straight == true && flush == true)
						{
							lastPlayedCards.Add("type", (int)Combinations.straightFlush);
						}
						else if (straight == true)
						{
							lastPlayedCards.Add("type", (int)Combinations.straight);
						}
						else if (flush == true)
						{
							lastPlayedCards.Add("type", (int)Combinations.flush);
						}

						break;
					}
			
				foreach (Sprite2D card in GetChildren())
				{
					card.QueueFree();	
				}

				foreach (Card card in Hand.selectedCards)
					{	
						lastPlayedCards.Add($"card{Hand.selectedCards.IndexOf(card)}", card.cardName);
						var newCard = new Sprite2D();
						newCard.Texture = GD.Load<Texture2D>($"res://assets/cards/{card.cardName}.png");
						AddChild(newCard);
						card.QueueFree();
					}
					Hand.selectedCards.Clear();
					var data = JsonSerializer.Serialize(lastPlayedCards);
					Client.client.PutData($"play-{data}".ToUtf8Buffer());
			}
    	}
	}
    public override void _Process(double delta)
    {
		var screenSize = GetViewportRect().Size;

		if (Client.data != null)
		{
			if (Client.data.StartsWith("play"))
			{
				foreach(var child in GetChildren())
				{
					child.QueueFree();
				}
				
				var raw = Client.data.Split("-")[1];
				var data = JsonSerializer.Deserialize<Dictionary<string, object>>(raw);
				Console.WriteLine(data);
				foreach (KeyValuePair<string, object> pair in data)
				{
					if (pair.Key.StartsWith("card"))
					{
						var name = pair.Value.ToString();
						var newCard = new Sprite2D();
						newCard.Texture = GD.Load<Texture2D>($"res://assets/cards/{name}.png");
						AddChild(newCard);
					}		
				}	
				
			}
		}

		foreach(Sprite2D card in GetChildren())
		{
			float cardRatio = 0F;
			if (GetChildren().Count > 1)
			{
				cardRatio = GetChildren().IndexOf(card)/((float)GetChildren().Count-1)-0.5F;
			}
			card.GlobalPosition = new Vector2(screenSize.X/2 + cardRatio * (GetChildren().Count-1) * 400, screenSize.Y/2);
			card.ZIndex = 0;
			card.Rotation = 0; 
		}
		
    }
}
