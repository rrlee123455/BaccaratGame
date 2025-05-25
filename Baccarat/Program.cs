// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
public class Baccarat
{
    public static void Main()
    {
        Deck deck = new Deck();
        deck.FillDeck(1); // Fill the deck with 8 decks of cards
        deck.PrintDeck();
        Console.WriteLine("\n\nSHUFFLINGGGGGGGGGGGGGGGGGGGG\n\n");
        deck.ShuffleDeck();
        deck.InsertCutCard();
        deck.PrintDeck();
        deck.Deal();
	}
}

public class Card
{
	public enum Suits
	{
		Spades, Diamonds, Clubs, Hearts
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
    
    public int BaccaratValue
    {   //2-9, face value, 10 = 10, 11 = J, 12 = Q, 13 = K, 14 = A
        get
        {
            if (Value == 14)
            {
                return 1; // Ace counts as 1
            }
            else if (Value >= 10)
            {
                return 0; // Face cards and above count as 0 in Baccarat
            }
            return Value; // 2-9 count as their face value
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
    public List<Card> cards = new List<Card>();
    private Random rng = new Random();
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
            cards.Add(new Card(val, suit, deckNumber));
        }
    }

    public void PrintDeck()
    {
        foreach (Card card in this.cards)
        {
            Console.WriteLine(card.Name);
        }
    }

    public void ShuffleDeck()
    {   //fisher-yates shuffle https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
        for (int i = this.cards.Count - 1; i > 0; i--)
        {
            int j = rng.Next(0, i + 1);
            (this.cards[i], this.cards[j]) = (this.cards[j], this.cards[i]);
        }
    }

    public void InsertCutCard()
    {
        //Insert a cut card at a random position between 12.5% and 25% of the deck
        int cutCardPosition = rng.Next((int)(this.cards.Count * 0.125), (int)(this.cards.Count * 0.25));
        Card cutCard = new Card(0, Card.Suits.Spades); // Cut card is 0 of spades
        this.cards.Insert(cutCardPosition, cutCard);
    }

    public void Deal()
    {
        // Deal two cards to each player
        Console.WriteLine("Dealing cards...");
        for (int i = 0; i < 2; i++)
        {
            Card playerCard = this.cards[this.cards.Count - 1];
            this.cards.RemoveAt(this.cards.Count - 1);
            Console.WriteLine($"Player {i + 1} receives: {playerCard.Name}");
            Card playerCard2 = this.cards[this.cards.Count - 1];
            this.cards.RemoveAt(this.cards.Count - 1);
            Console.WriteLine($"Player {i + 1} receives: {playerCard2.Name}");
            Console.WriteLine($"Player {i + 1} Baccarat Value: {(playerCard.BaccaratValue + playerCard2.BaccaratValue) % 10}");
        }
    }
}