using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

public partial class Card : TextureButton
{
	public static Card draggedCard; 
	public Suit suit;
	public Value value;
	public float xPos;
	public float yPos;
	public string cardName;
	bool dragging = false;	
	Vector2 mouseOffset; 
	float scaleSize = 0.75f;
	Tween tweenHover;
	Tween tweenSelect;
	Vector2 startMousePos;
	bool selected = false;
	
	public Card(Value value, Suit suit, float xPos, float yPos)
	{
		this.suit = suit;
		this.value = value;
		this.yPos = yPos;	
		cardName = $"{this.value}_of_{this.suit}";
		TextureNormal = GD.Load<Texture2D>($"res://assets/cards/{cardName}.png");
	}

    public override void _Ready()
    {
		Scale = new Vector2(scaleSize, scaleSize);
		PivotOffset = Size/2;

		var button = GetNode<TextureButton>(".");
		button.ButtonDown += OnButtonDown;
		button.ButtonUp += OnButtonUp;
		button.MouseEntered += OnMouseEntered;
		button.MouseExited += OnMouseExited;
    }

	private void OnButtonDown()
    {
		startMousePos = GetViewport().GetMousePosition();

		PivotOffset = new Vector2(0, 0);
		mouseOffset = GlobalPosition - GetViewport().GetMousePosition();
		PivotOffset = Size/2;
		
		if (tweenHover != null && tweenHover.IsRunning())
			{
				tweenHover.Kill();
			}
		if (tweenSelect != null && tweenSelect.IsRunning())
			{
				tweenSelect.Kill();
			}
		Scale *= new Vector2(1.13f, 1.13f);

		draggedCard = this;
		dragging = true;

	}
	 
	private void OnButtonUp()
	{

		draggedCard = null;
		dragging = false;

		Scale = new Vector2(scaleSize, scaleSize);

		var selectForgiveness = startMousePos-GetViewport().GetMousePosition();
		if (selectForgiveness < new Vector2(10, 10) && selectForgiveness > new Vector2(-10, -10))
		{	
			if (selected)
			{
				selected = false;
			}
			else
			{
				selected = true;
			}
		}
	}

    private void OnMouseEntered()
    {
		if (!dragging)
		{
			if (tweenHover != null && tweenHover.IsRunning())
			{
				tweenHover.Kill();
			}
			
			tweenHover = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
			tweenHover.TweenProperty(this, "scale", new Vector2(scaleSize * 1.04f, scaleSize * 1.04f), 0.5f);
		}
	}

	private void OnMouseExited()
	{
		if (!dragging)
		{

			if (tweenHover != null && tweenHover.IsRunning())
			{
				tweenHover.Kill();
			}

			tweenHover = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
			tweenHover.TweenProperty(this, "scale", new Vector2(scaleSize, scaleSize), 0.5f);
		}

	}

	public override void _Process(double delta)
	{
		var screenSize = GetViewportRect().Size;
      	float handRatio = 0.5f;
		if (GetNode("..").GetChildren().Count > 1)
		{
			handRatio = GetIndex()/(float)(GetNode("..").GetChildren().Count-1f);
		}
		xPos = screenSize.X/2 - Size.X/2 + (handRatio - 0.5f) * (GetNode("..").GetChildren().Count-1) *  Size.X * scaleSize * 3/5 ;

		if (dragging)
		{
			var offset = startMousePos-GetViewport().GetMousePosition();
			if (offset > new Vector2(10, 10) || offset < new Vector2(-10, -10))
			{
				ZIndex = 13;
			}
			
			GlobalPosition = GetViewport().GetMousePosition() + mouseOffset;

		}
		else
		{
			if (!selected)
			{ 
				ZIndex = GetIndex();

				if (tweenSelect != null && tweenSelect.IsRunning())
				{
					tweenSelect.Kill();
				}
				tweenSelect = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
				tweenSelect.TweenProperty(this, "position", new Vector2(xPos, yPos), 0.5f);
			}
			else
			{
				ZIndex = GetIndex();

				if (tweenSelect != null && tweenSelect.IsRunning())
				{
					tweenSelect.Kill();
				}
				tweenSelect = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quart);
				tweenSelect.TweenProperty(this, "position", new Vector2(xPos, yPos - 100), 0.5f);
			}
		}
	}
}

public partial class Hand : Node
{
	public static List<Card> selectedCards = new List<Card>();

	public override void _Ready()
	{ 
		Array values = Enum.GetValues(typeof(Value));
		Array suits = Enum.GetValues(typeof(Suit));
		float x = 525;
		for (int i = 0; i < 13; i++)
		{
			Random randomValue = new Random();
			Random randomSuit = new Random();

			Card newCard = new Card((Value)values.GetValue(randomValue.Next(values.Length)), (Suit)suits.GetValue(randomSuit.Next(suits.Length)), x, 1400);
			AddChild(newCard);

			x += 200;
		}
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
		foreach (Card card in GetChildren())
		{
			if (Card.draggedCard != null)
			{
				if (Card.draggedCard.Position.X > card.Position.X && card.GetIndex() > Card.draggedCard.GetIndex())
				{
					MoveChild(card, card.GetIndex()-1);
					MoveChild(Card.draggedCard, card.GetIndex()+1);
	
				}
				if (Card.draggedCard.Position.X < card.Position.X && card.GetIndex() < Card.draggedCard.GetIndex())
				{
					MoveChild(card, card.GetIndex()+1);
					MoveChild(Card.draggedCard, card.GetIndex()-1);
				}
			}
		}
	}
}
