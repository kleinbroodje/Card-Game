#nullable enable

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public enum Suit 
    {
        diamonds, clubs, hearts, spades,
    }

public enum Value
    {
        three, four, five, six, seven, eight, nine, ten, jack, queen, king, ace, two
    }

public partial class Card : Sprite2D
{
	public static List<Card> selectedQueue = new List<Card>();
	public Suit suit;
	public Value value;
	public float xPos;
	public float yPos;
	public bool selected = false;
	
	public Card(Value value, Suit suit, float xPos, float yPos)
	{
		this.suit = suit;
		this.value = value;
		this.xPos = xPos;
		this.yPos = yPos;	
	}

	public override void _Input(InputEvent @event)
	{
    	if (@event is InputEventMouseButton mouseEvent && mouseEvent.IsPressed() && mouseEvent.ButtonIndex == MouseButton.Left)
    	{
			if (selected)
			{
				selected = false;
			}
			else if (GetRect().HasPoint(ToLocal(mouseEvent.GlobalPosition)))
			{
				selectedQueue.Add(this);
				
			}
    	}
	}

	public override void _Ready()
	{
		Texture = GD.Load<Texture2D>($"res://assets/cards/{value}_of_{suit}.png");
		Position = new Vector2(xPos, yPos);
		Scale = new Vector2(0.75F, 0.75F);
	}


	public override void _Process(double delta)
	{
		if (selected)
		{
			Position = GetGlobalMousePosition();
		}

		else
		{
			Position = new Vector2(xPos, yPos);
		}
	}

}

public partial class Hand : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{ 
		#nullable disable
		Array values = Enum.GetValues(typeof(Value));
		Array suits = Enum.GetValues(typeof(Suit));
		float x = 525;
		for (int i = 0; i < 13; i++)
		{
			Random randomValue = new Random();
			Random randomSuit = new Random();

			Card newCard = new Card((Value)values.GetValue(randomValue.Next(values.Length)), (Suit)suits.GetValue(randomSuit.Next(suits.Length)), x, 1500);
			AddChild(newCard);

			x += 200;
		}
		#nullable enable

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Card.selectedQueue.Any())
		{
			float currentMax = 0;
			Card? currentCard = null;
			foreach (Card card in Card.selectedQueue)
			{
				if (card.xPos > currentMax)
				{
					currentMax = card.xPos;
					currentCard = card;
				}
			}
			if (currentCard != null)
			{
				currentCard.selected = true;
				Card.selectedQueue.Clear();
			}
		}
	}
}
