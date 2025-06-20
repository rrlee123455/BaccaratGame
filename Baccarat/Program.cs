﻿// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Microsoft.VisualBasic;
public class Baccarat
{
    public static void Main()
    {
        // Ctrl + / to un/comment blocks of lines
        Deck deck = new Deck();
        Console.Write("\nFilling shoe...");
        int numberOfDecks = 1;
        deck.FillDeck(numberOfDecks); // Fill the deck with 8 decks of cards
        deck.PrintDeck();
        Console.WriteLine("\nShuffling shoe\n");
        deck.ShuffleDeck();
        deck.InsertCutCard();
        deck.PrintDeck();
        deck.Deal(numberOfDecks * 13); // Deal enough hands to reach cut card
        //deck.PrintDeck();
        //Console.WriteLine(deck.cards.Count);
        //deck.CheckAllRules(false);
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

public struct EVbyRunningCount
{
    public double PlayerEV;
    public double BankerEV;
}
public class Deck
{
    public List<Card> cards = new List<Card>();
    private Random rng = new Random();
    private List<Card> playerCards = new List<Card>();
    private List<Card> bankerCards = new List<Card>();
    private bool playerThirdCardExists = false;
    private bool bankerThirdCardExists = false;
    private string listOfWinners = "";
    Dictionary<int, EVbyRunningCount> EVRC = new Dictionary<int, EVbyRunningCount>()
    {
        { -25, new EVbyRunningCount { PlayerEV = -0.0083, BankerEV = -0.0146 } },
        { -24, new EVbyRunningCount { PlayerEV = -0.0081, BankerEV = -0.0147 } },
        { -23, new EVbyRunningCount { PlayerEV = -0.0076, BankerEV = -0.0152 } },
        { -22, new EVbyRunningCount { PlayerEV = -0.0083, BankerEV = -0.0145 } },
        { -21, new EVbyRunningCount { PlayerEV = -0.0085, BankerEV = -0.0144 } },
        { -20, new EVbyRunningCount { PlayerEV = -0.0089, BankerEV = -0.014 } },
        { -19, new EVbyRunningCount { PlayerEV = -0.0087, BankerEV = -0.0142 } },
        { -18, new EVbyRunningCount { PlayerEV = -0.0088, BankerEV = -0.0141 } },
        { -17, new EVbyRunningCount { PlayerEV = -0.0091, BankerEV = -0.0137 } },
        { -16, new EVbyRunningCount { PlayerEV = -0.0092, BankerEV = -0.0136 } },
        { -15, new EVbyRunningCount { PlayerEV = -0.0094, BankerEV = -0.0135 } },
        { -14, new EVbyRunningCount { PlayerEV = -0.0095, BankerEV = -0.0134 } },
        { -13, new EVbyRunningCount { PlayerEV = -0.0098, BankerEV = -0.0131 } },
        { -12, new EVbyRunningCount { PlayerEV = -0.0098, BankerEV = -0.0131 } },
        { -11, new EVbyRunningCount { PlayerEV = -0.0101, BankerEV = -0.0128 } },
        { -10, new EVbyRunningCount { PlayerEV = -0.0101, BankerEV = -0.0128 } },
        { -9, new EVbyRunningCount { PlayerEV = -0.0103, BankerEV = -0.0126 } },
        { -8, new EVbyRunningCount { PlayerEV = -0.0105, BankerEV = -0.0123 } },
        { -7, new EVbyRunningCount { PlayerEV = -0.0107, BankerEV = -0.0122 } },
        { -6, new EVbyRunningCount { PlayerEV = -0.0109, BankerEV = -0.012 } },
        { -5, new EVbyRunningCount { PlayerEV = -0.0112, BankerEV = -0.0117 } },
        { -4, new EVbyRunningCount { PlayerEV = -0.0114, BankerEV = -0.0115 } },
        { -3, new EVbyRunningCount { PlayerEV = -0.0117, BankerEV = -0.0113 } },
        { -2, new EVbyRunningCount { PlayerEV = -0.0119, BankerEV = -0.011 } },
        { -1, new EVbyRunningCount { PlayerEV = -0.0121, BankerEV = -0.0108 } },
        { 0, new EVbyRunningCount { PlayerEV = -0.0124, BankerEV = -0.0105 } },
        { 1, new EVbyRunningCount { PlayerEV = -0.0126, BankerEV = -0.0103 } },
        { 2, new EVbyRunningCount { PlayerEV = -0.0129, BankerEV = -0.0101 } },
        { 3, new EVbyRunningCount { PlayerEV = -0.0132, BankerEV = -0.0098 } },
        { 4, new EVbyRunningCount { PlayerEV = -0.0133, BankerEV = -0.0097 } },
        { 5, new EVbyRunningCount { PlayerEV = -0.0136, BankerEV = -0.0094 } },
        { 6, new EVbyRunningCount { PlayerEV = -0.0138, BankerEV = -0.0092 } },
        { 7, new EVbyRunningCount { PlayerEV = -0.0139, BankerEV = -0.009 } },
        { 8, new EVbyRunningCount { PlayerEV = -0.0141, BankerEV = -0.0088 } },
        { 9, new EVbyRunningCount { PlayerEV = -0.0142, BankerEV = -0.0088 } },
        { 10, new EVbyRunningCount { PlayerEV = -0.0144, BankerEV = -0.0086 } },
        { 11, new EVbyRunningCount { PlayerEV = -0.0145, BankerEV = -0.0084 } },
        { 12, new EVbyRunningCount { PlayerEV = -0.0147, BankerEV = -0.0083 } },
        { 13, new EVbyRunningCount { PlayerEV = -0.0149, BankerEV = -0.0081 } },
        { 14, new EVbyRunningCount { PlayerEV = -0.015, BankerEV = -0.008 } },
        { 15, new EVbyRunningCount { PlayerEV = -0.0151, BankerEV = -0.0079 } },
        { 16, new EVbyRunningCount { PlayerEV = -0.0153, BankerEV = -0.0076 } },
        { 17, new EVbyRunningCount { PlayerEV = -0.0153, BankerEV = -0.0077 } },
        { 18, new EVbyRunningCount { PlayerEV = -0.0155, BankerEV = -0.0075 } },
        { 19, new EVbyRunningCount { PlayerEV = -0.0159, BankerEV = -0.0071 } },
        { 20, new EVbyRunningCount { PlayerEV = -0.0157, BankerEV = -0.0072 } },
        { 21, new EVbyRunningCount { PlayerEV = -0.0155, BankerEV = -0.0075 } },
        { 22, new EVbyRunningCount { PlayerEV = -0.0159, BankerEV = -0.0071 } },
        { 23, new EVbyRunningCount { PlayerEV = -0.0165, BankerEV = -0.0065 } },
        { 24, new EVbyRunningCount { PlayerEV = -0.0155, BankerEV = -0.0075 } },
        { 25, new EVbyRunningCount { PlayerEV = -0.0176, BankerEV = -0.0055 } },
    };
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
        listOfWinners = ""; //reset list of winners
    }

    public void InsertCutCard()
    {
        //Insert a cut card at a random position between 12.5% and 25% of the deck
        int cutCardPosition = rng.Next((int)(this.cards.Count * 0.125), (int)(this.cards.Count * 0.25));
        Card cutCard = new Card(0, Card.Suits.Spades); // Cut card is 0 of spades
        this.cards.Insert(cutCardPosition, cutCard);
    }

    public void CheckAllRules(bool test)
    {
        if (test)
        {
            for (int i = 0; i <= 9; i++)
            {
                for (int j = 0; j <= 9; j++)
                {
                    for (int k = 0; k <= 9; k++)
                    {
                        playerCards.Clear();
                        bankerCards.Clear();
                        playerThirdCardExists = false;
                        bankerThirdCardExists = false;
                        // Deal two cards to each player
                        Console.WriteLine("Dealing cards...");

                        playerCards.Add(new Card(i, Card.Suits.Diamonds, 1));
                        Console.WriteLine($"Player receives: {playerCards[0].Name}");
                        playerCards.Add(new Card(0, Card.Suits.Diamonds, 1));
                        Console.WriteLine($"Player receives: {playerCards[1].Name}");
                        Console.WriteLine($"Player Baccarat Value: {PlayerTotalBaccaratValue()}");

                        bankerCards.Add(new Card(j, Card.Suits.Diamonds, 1));
                        Console.WriteLine($"Banker receives: {bankerCards[0].Name}");
                        bankerCards.Add(new Card(0, Card.Suits.Diamonds, 1));
                        Console.WriteLine($"Banker receives: {bankerCards[1].Name}");
                        Console.WriteLine($"Banker Baccarat Value: {BankerTotalBaccaratValue()}");

                        if (NaturalWin())
                        {
                            Console.WriteLine("Natural win! No third card drawn.");
                            DetemineWinnerAndSave(ref listOfWinners);
                            Console.WriteLine($"List of winners: {listOfWinners}");
                            //Console.WriteLine($"Hands remaining: {i - 1}");
                        }
                        else
                        {
                            //PlayerThirdCardRule();
                            if (PlayerTotalBaccaratValue() <= 5)
                            {
                                playerCards.Add(new Card(k, Card.Suits.Diamonds, 1));
                                playerThirdCardExists = true;
                                Console.WriteLine($"Player total <= 5: Player draws {playerCards[2].Name}");
                                Console.WriteLine($"Player total: {PlayerTotalBaccaratValue()}");
                            }

                            //BankerThirdCardRule();
                            //banker <= 2
                            //banker == 3 and player 3rd card = 8
                            //banker == 4 and player 3rd card btwn 2-7 inclusive
                            //banker == 5 and player 3rd card btwn 4-7 inclusive
                            //banker == 6 and player 3rd card btwn 6-7 inclusive
                            if (BankerTotalBaccaratValue() <= 2 ||
                                (playerThirdCardExists &&
                                ((BankerTotalBaccaratValue() == 3 && playerCards[2].BaccaratValue != 8) ||
                                (BankerTotalBaccaratValue() == 4 && playerCards[2].BaccaratValue >= 2 && playerCards[2].BaccaratValue <= 7) ||
                                (BankerTotalBaccaratValue() == 5 && playerCards[2].BaccaratValue >= 4 && playerCards[2].BaccaratValue <= 7) ||
                                (BankerTotalBaccaratValue() == 6 && playerCards[2].BaccaratValue >= 6 && playerCards[2].BaccaratValue <= 7)
                                ))
                                // If the player stood pat (i.e. has only two cards), the banker regards only their own hand and acts according
                                // to the same rule as the player, i.e. the banker draws a third card with hands 5 or less and stands with 6 or 7.
                                || (!playerThirdCardExists && BankerTotalBaccaratValue() <= 5)
                            )
                            {
                                bankerCards.Add(new Card(9, Card.Suits.Diamonds, 1));
                                bankerThirdCardExists = true;
                                Console.WriteLine($"Banker 3rd rule applies and draws {bankerCards[2].Name}");
                                Console.WriteLine($"Banker total: {BankerTotalBaccaratValue()}");
                            }

                            DetemineWinnerAndSave(ref listOfWinners);
                            Console.WriteLine($"List of winners: {listOfWinners}");
                            //Console.WriteLine($"Hands remaining: {i - 1}");
                        }

                    }
                }
            }
        }
    }

    public void Deal(int i)
    {
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

            Console.WriteLine($"Running count: {RunningCount()}");
            Console.WriteLine($"Player EV: {EVRC[RunningCount()].PlayerEV}");
            Console.WriteLine($"Banker EV: {EVRC[RunningCount()].BankerEV}");
            i--;
        }
    }

    public int RunningCount()
    {
        // Running count from https://wizardofodds.com/games/baccarat/card-counting/
        // But values are reversed because we're counting cards left in the deck, not ones already dealt
        int runningCount = 0;
        foreach (Card card in this.cards)
        {
            if (card.BaccaratValue == 9 || card.BaccaratValue == 0) // Face cards and 9s
            {
                continue; // No value change for face cards and 9s
            }
            else if (card.BaccaratValue <= 4)
            {
                runningCount -= 1; // Low cards
            }
            else
            {
                runningCount += 1; // High cards
            }
        }
        return runningCount;
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
            ExportToCSV();
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
            ((BankerTotalBaccaratValue() == 3 && playerCards[2].BaccaratValue != 8) ||
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

    public void ExportToCSV()
    {
        using (StreamWriter sw = new StreamWriter("results.txt", false))
        {
            sw.WriteLine(listOfWinners);
        }
        Console.WriteLine("File exported!");
    }
}