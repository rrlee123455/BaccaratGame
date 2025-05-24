// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
public class Baccarat
{
	public static void Main()
	{
		Deck deck = new Deck();
		deck.FillDeck(8); // Fill the deck with 8 decks of cards
		deck.PrintDeck();
	}
}

public class Card
{
	public enum Suits
	{
		Hearts, Diamonds, Clubs, Spades
	}

	public int Value
	{
		get;
		set;
	}

	public Suits Suit
	{
		get;
		set;
	}

    public int DeckNumber
    {
        get;
        set;
    }

	//Used to get full name, also useful if you want to just get the named value
    public string NamedValue
    {
        get
        {
            string name = string.Empty;
            switch (Value)
            {
                case (14):
                    name = "Ace";
                    break;
                case (13):
                    name = "King";
                    break;
                case (12):
                    name = "Queen";
                    break;
                case (11):
                    name = "Jack";
                    break;
                default:
                    name = Value.ToString();
                    break;
            }

            return name;
        }
    }

	public string Name
	{
		get
		{
			return NamedValue + " of " + Suit.ToString() + ", deck number " + DeckNumber;
		}
	}

	public Card(int Value, Suits Suit, int DeckNumber = 1)
    {
        this.Value = Value;
        this.Suit = Suit;
        this.DeckNumber = DeckNumber;
    }
}

public class Deck
{
	public List<Card> Cards = new List<Card>();
	public void FillDeck(int NumberofDecks)
	{
        int totalCards = NumberofDecks * 52;
		//Can use a single loop utilising the mod operator % and Math.Floor
        //Using division based on 13 cards in a suit
        for (int i = 0; i < totalCards; i++)
        {
            Card.Suits suit = (Card.Suits)(Math.Floor((decimal)i / 13) % 4);
            //Add 2 to value as a cards start at 2
            int val = i % 13 + 2;
            int deckNumber = (int)Math.Floor((decimal)i / 52) + 1; // Calculate the deck number
            Cards.Add(new Card(val, suit, deckNumber));
        }
	}
	
	public void PrintDeck()
	{
		foreach(Card card in this.Cards)
		{
			Console.WriteLine(card.Name);
		}
	}
}