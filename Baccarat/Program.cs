// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
public class Baccarat
{
    public static void Main()
    {
        Deck deck = new Deck();
        Console.Write("\n\n\nFilling shoe\n.\n.\n.\n");
        int numberOfDecks = 500;
        deck.FillDeck(numberOfDecks); // Fill the deck with 8 decks of cards
        deck.PrintDeck();
        Console.WriteLine("\n\nShuffling shoe\n.\n.\n.\n\n\n");
        deck.ShuffleDeck();
        deck.InsertCutCard();
        deck.PrintDeck();
        deck.Deal(numberOfDecks * 13); //Deal enough hands to reach cut card
        deck.PrintDeck();
        Console.WriteLine(deck.cards.Count);
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
    public Card()
    {}
}

public class Deck
{
    public List<Card> cards = new List<Card>();
    private Random rng = new Random();
    private List<Card> playerCards = new List<Card>();
    private List<Card> bankerCards = new List<Card>();
    private bool playerThirdCardExists = false;
    private bool bankerThirdCardExists = false;
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

    public void Deal(int i)
    {
        string listOfWinners = string.Empty;
        while (i > 0)
        {
            playerCards.Clear();
            bankerCards.Clear();
            playerThirdCardExists = false;
            // Deal two cards to each player
            Console.WriteLine("\n\nDealing cards...");
            playerCards.Add(Draw());
            Console.WriteLine($"Player receives: {playerCards[0].Name}");

            playerCards.Add(Draw());
            Console.WriteLine($"Player receives: {playerCards[1].Name}");
            Console.WriteLine($"Player Baccarat Value: {PlayerTotalBaccaratValue()}");

            bankerCards.Add(Draw());
            Console.WriteLine($"\nBanker receives: {bankerCards[0].Name}");

            bankerCards.Add(Draw());
            Console.WriteLine($"Banker receives: {bankerCards[1].Name}");
            Console.WriteLine($"Banker Baccarat Value: {BankerTotalBaccaratValue()}");

            if (NaturalWin())
            {
                Console.WriteLine("\nNatural win! No third card drawn.");
                DetemineWinnerAndSave(ref listOfWinners);
                Console.WriteLine($"List of winners: {listOfWinners}");
                Console.WriteLine($"Hands remaining: {i - 1}");
            }
            else
            {                
                PlayerThirdCardRule();
                BankerThirdCardRule();
                DetemineWinnerAndSave(ref listOfWinners);
                Console.WriteLine($"List of winners: {listOfWinners}");
                Console.WriteLine($"Hands remaining: {i - 1}");
            }
            i--;
        }        
    }

    public Card Draw()
    {
        CheckCutCard();
        Card card = this.cards[this.cards.Count - 1];
        this.cards.RemoveAt(this.cards.Count - 1);
        return card;
    }

    public void CheckCutCard()
    {
        if (this.cards.Count == 0)
        {
            //Console.WriteLine("No cards left in the deck. Shuffling...");
            //FillDeck(1); // Refill the deck with 1 deck of cards
            //ShuffleDeck();
            //InsertCutCard();
        }
        else if (this.cards[this.cards.Count - 1].Value == 0) // Check for cut card
        {
            Console.WriteLine("Cut card drawn");
            Environment.Exit(0);
            //this.cards.RemoveAt(this.cards.Count - 1); // Remove cut card
            //ShuffleDeck();
            //InsertCutCard(); // Reinsert cut card after shuffling
        }
    }

    public int PlayerTotalBaccaratValue()
    {
        int playerTotal = 0;
        foreach (Card card in playerCards)
        {
            playerTotal += card.BaccaratValue;
        }
        return playerTotal % 10;
    }

    public int BankerTotalBaccaratValue()
    {
        int bankerTotal = 0;
        foreach (Card card in bankerCards)
        {
            bankerTotal += card.BaccaratValue;
        }
        return bankerTotal % 10;
    }

    public bool NaturalWin()
    {
        return PlayerTotalBaccaratValue() == 8 || PlayerTotalBaccaratValue() == 9 ||
               BankerTotalBaccaratValue() == 8 || BankerTotalBaccaratValue() == 9;
    }

    public void PlayerThirdCardRule()
    {
        if (PlayerTotalBaccaratValue() <= 5)
        {
            playerCards.Add(Draw());
            playerThirdCardExists = true;
            Console.WriteLine($"\nPlayer total <= 5: Player draws {playerCards[2].Name}");
            Console.WriteLine($"Player total: {PlayerTotalBaccaratValue()}");
        }
        // do nothing
    }

    public void BankerThirdCardRule()
    {
        //banker <= 2
        //banker == 3 and player 3rd card = 8
        //banker == 4 and player 3rd card btwn 2-7 inclusive
        //banker == 5 and player 3rd card btwn 4-7 inclusive
        //banker == 6 and player 3rd card btwn 6-7 inclusive
        if (BankerTotalBaccaratValue() <= 2 ||
            (playerThirdCardExists &&
            ((BankerTotalBaccaratValue() == 3 && playerCards[2].BaccaratValue == 8) ||
            (BankerTotalBaccaratValue() == 4 && playerCards[2].BaccaratValue >= 2 && playerCards[2].BaccaratValue <= 7) ||
            (BankerTotalBaccaratValue() == 5 && playerCards[2].BaccaratValue >= 4 && playerCards[2].BaccaratValue <= 7) ||
            (BankerTotalBaccaratValue() == 6 && playerCards[2].BaccaratValue >= 6 && playerCards[2].BaccaratValue <= 7)
            ))
            // If the player stood pat (i.e. has only two cards), the banker regards only their own hand and acts according 
            // to the same rule as the player, i.e. the banker draws a third card with hands 5 or less and stands with 6 or 7.
            || (!playerThirdCardExists && BankerTotalBaccaratValue() <= 5)
        )
        {
            bankerCards.Add(Draw());
            bankerThirdCardExists = true;
            Console.WriteLine($"\nBanker 3rd rule applies and draws {bankerCards[2].Name}");
            Console.WriteLine($"Banker total: {BankerTotalBaccaratValue()}");
        }
        // do nothing
    }

    public void DetemineWinnerAndSave(ref string listOfWinners)
    {
        if (PlayerTotalBaccaratValue() > BankerTotalBaccaratValue())
        {
            Console.WriteLine("\nPlayer wins!");
            listOfWinners += "P";
        }
        else if (PlayerTotalBaccaratValue() < BankerTotalBaccaratValue())
        {
            Console.WriteLine("\nBanker wins!");
            listOfWinners += "B";
        }
        else
        {
            Console.WriteLine("\nIt's a tie!");
            listOfWinners += "T";
        }
    }
}