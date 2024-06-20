using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PlayArea : Sprite2D
{
	public static List<Card> lastPlayedCards = new List<Card>();
	public static bool playingCards = false;

	public override void _Ready()
	{
		ZIndex = -1;	
	}
	public override void _Input(InputEvent @event)
	{
    	if (@event is InputEventMouseButton mouseEvent && mouseEvent.IsPressed() && mouseEvent.ButtonIndex == MouseButton.Left)
    	{	
			if (GetRect().HasPoint(ToLocal(mouseEvent.GlobalPosition)) && Hand.selectedCards.Any())
			{
				lastPlayedCards.Clear();
				playingCards = true;
			}
    	}
	}
}
