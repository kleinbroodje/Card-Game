#nullable enable

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Card : Sprite2D
{
	public static List<Card> selectedQueue = new List<Card>();
	public Suit suit;
	public Value value;
	public float xPos;
	public float yPos;
	public float originalYPos;
	public float yMax;
	public string cardName;
	
	public Card(Value value, Suit suit, float xPos, float yPos)
	{
		this.suit = suit;
		this.value = value;
		this.xPos = xPos;
		this.yPos = yPos;	
		cardName = $"{this.value}_of_{this.suit}";
		originalYPos = yPos;
		yMax = originalYPos;
		Texture = GD.Load<Texture2D>($"res://assets/cards/{value}_of_{suit}.png");
		Position = new Vector2(xPos, yPos);
		Scale = new Vector2(0.75F, 0.75F);
		ZIndex = 6;
	}

	//adding cards to selectedQueue when clicked
	public override void _Input(InputEvent @event)
	{
    	if (@event is InputEventMouseButton mouseEvent && mouseEvent.IsPressed() && mouseEvent.ButtonIndex == MouseButton.Left && !PlayArea.lastPlayedCards.Values.Contains(cardName))
    	{
			if (GetRect().HasPoint(ToLocal(mouseEvent.GlobalPosition)) && !Hand.selectedCards.Contains(this))
			{
				selectedQueue.Add(this);
			}

    	}
	}

	public void PopUp()
	{
		yMax = originalYPos - 120;
	}

	public void DropDown()
	{
		yMax = originalYPos;
	}

	public void Update()
	{
		if(GetParent() == GetNode("/root/Game/Hand"))
			//getting position of mouse for hover
			yPos += (yMax - yPos) * 0.2F;
			if (GetRect().HasPoint(ToLocal(GetViewport().GetMousePosition())) && !Hand.selectedCards.Contains(this))
			{
				Hand.currentHoveringCards.Add(this);
			} 
			else
			{
				DropDown();
			}
			if (!Hand.selectedCards.Contains(this))
			{
				Rotation = 0; 
				ZIndex = 6;
				Position = new Vector2(xPos, yPos);

		}
	}
}

public partial class Hand : Node
{
	// Called when the node enters the scene tree for the first time.
	public static List<Card> selectedCards = new List<Card>();
	public static List<Card> currentHoveringCards = new List<Card>();

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

			Card newCard = new Card((Value)values.GetValue(randomValue.Next(values.Length)), (Suit)suits.GetValue(randomSuit.Next(suits.Length)), x, 2100);
			AddChild(newCard);

			x += 200;
		}
		#nullable enable
	}

	public override void _Input(InputEvent @event)
	{
    	if (@event is InputEventMouseButton mouseEvent && mouseEvent.IsPressed() && mouseEvent.ButtonIndex == MouseButton.Right && selectedCards.Count != 0)
    	{	
			selectedCards.RemoveAt(selectedCards.Count - 1);
    	}
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    	currentHoveringCards.Clear();
		
		//checking which card is on top of other when selecting
		if (Card.selectedQueue.Any())
		{
			float currentMax = 0;
			Card? currentCard = null;
			foreach (Card card in Card.selectedQueue)
			{
				if (selectedCards.Contains(card) == false)
				{
					if (card.xPos > currentMax)
					{
						currentMax = card.xPos;
						currentCard = card;
					}
				}
			}
			
			if (currentCard != null && selectedCards.Count < 5)
			{
				selectedCards.Add(currentCard);
			}
			Card.selectedQueue.Clear();
		}

		if (selectedCards.Any())
		{
			float offsetX = 0;
			foreach (Card card in selectedCards)
			{	
				card.ZIndex = selectedCards.IndexOf(card) + 1;

				float handRatio = 0.5F;
				if (selectedCards.Count > 1)
				{
					handRatio = selectedCards.IndexOf(card)/((float)selectedCards.Count-1);
				}
				
				float heightMult = -4 * handRatio * handRatio + 4 * handRatio;
				if (selectedCards.Count == 1)
				{
					heightMult = 0;
				}

				float angleMult = 2/(1+(float)Math.Pow(Math.E, -5*(handRatio-0.5)))-1;

				card.Position = DisplayServer.MouseGetPosition() + new Vector2(250 *(handRatio - 0.5F), -50 * heightMult); 
				card.RotationDegrees = angleMult * 20;
				offsetX += 100; 
				
			}
		}

		foreach (Card card in GetChildren())
		{
			card.Update();
		}
		
		//moving cards when mouse hovers over
		if (currentHoveringCards.Count == 2)
		{
			// currentHoveringCards.Last().PopUp();
			if (currentHoveringCards[0].xPos >= currentHoveringCards[1].xPos)
			{
				currentHoveringCards[0].PopUp();
				currentHoveringCards[1].DropDown();
			} 

			else
			{
				currentHoveringCards[1].PopUp();
				currentHoveringCards[0].DropDown();
			}
		} 

		else if (currentHoveringCards.Count == 1)
		{
			currentHoveringCards[0].PopUp();
		}
	}
}
