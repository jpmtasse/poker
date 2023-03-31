namespace poker_hands;

internal class Card
{
    public enum Suit { HEARTS, SPADES, DIAMONDS, CLUBS}
    public enum Value { TWO=2, THREE, FOUR, FIVE, SIX, SEVEN, EIGHT, NINE, TEN, JACK, QUEEN, KING, ACE}

    public Suit suit { get; set; }
    public Value value { get; set; }
}

internal class DeckOfCards : Card {
    const int total_cards = 52;
    private Card[] cards;
    private int deck_pos = 0;

    public DeckOfCards()
    {
        cards = new Card[total_cards];
        setUpDeck();
    }

    public Card[] getDeck { get { return cards; } }

    public void setUpDeck() {
        int i = 0;
        foreach (Suit s in Enum.GetValues(typeof(Suit))) {
            foreach (Value v in Enum.GetValues(typeof(Value)))
            {
                cards[i++] = new Card { suit = s, value = v };
            }
        }
        shuffleCards();
    }
    public void shuffleCards(int N=1024) {
        Random r = new Random();
        Card current_card;
        //int N = 1024;

        for (int s = 0; s < N; s++) {
            for (int i = 0; i < total_cards; i++) {
                int card_index = r.Next(13);
                current_card = cards[i];
                cards[i] = cards[card_index];
                cards[card_index] = current_card;
            }
        }
    }

    public Card[]? getHand() {
        Card[] hand = new Card[5];
        if (deck_pos + 5 >= total_cards) {
            return null;
        }
        for (int i = 0; i < 5; i++)
        {
            hand[i] = cards[i + deck_pos];
        }
        deck_pos += 5;
        return hand;
    }

    public void resetDeck() {
        shuffleCards();
        deck_pos = 0;
    }

    public static Card[] SortHand(Card[] hand)
    {
        var query_hand = from h in hand orderby h.value select h;  // this could also be done using an iterator
        Card[] new_hand = new Card[hand.Length];
        int i = 0;
        foreach (var e in query_hand.ToList())
        {
            new_hand[i++] = e;
        }
        return new_hand;
    }
}

public enum Hand { 
    Nothing,
    OnePair,
    TwoPair,
    ThreeKind, //Per exercise description we are not considering this type
    Straight,
    Flush,
    FullHouse, //nor this
    FourKind   // or this
}

public struct HandValue{
    public int total { get; set; }
    public int high_card { get; set; }
}

class HandEvaluator: Card{
    private int heartsSum;
    private int diamondSum;
    private int clubSum;
    private int spadesSUm;
    private Card[] all_cards;
    private HandValue hand_value;

    public HandEvaluator(Card[] sorted_hand) {
        heartsSum = 0;
        diamondSum = 0;
        clubSum = 0;
        spadesSUm = 0;
        all_cards = new Card[5];
        for (int i = 0; i < 5; i++) {
            all_cards[i] = sorted_hand[i];
        }
        hand_value = new HandValue();
    }

    public HandValue HandValues
    {
        get { return hand_value; }
        set { hand_value = value; }
    }

    public Card[] Cards {
        get { return all_cards; }
        set {
            for (int i = 0; i < 5; i++) {
                all_cards[i] = value[i];
            }
        }
    }

    public Hand EvaluateHand() {
        // Assign value to hand
        getNumberOfSuit();
        if (FourOfAKInd())
            return Hand.FourKind;
        else if (FullHouse())
            return Hand.FullHouse;
        else if (Flush())
            return Hand.Flush;
        else if (Straight())
            return Hand.Straight;
        else if (ThreeOfAKind())
            return Hand.ThreeKind;
        else if (TwoPairs())
            return Hand.TwoPair;
        else if (OnePair())
            return Hand.OnePair;
        else
            hand_value.high_card = (int)all_cards[4].value;
            return Hand.Nothing;
    }

    private void getNumberOfSuit() {
        foreach (var element in all_cards) {
            if (element.suit == Card.Suit.HEARTS)
            {
                heartsSum++;
            }
            else if (element.suit == Card.Suit.DIAMONDS)
            {
                diamondSum++;
            }
            else if (element.suit == Card.Suit.CLUBS)
            {
                clubSum++;
            }
            else if (element.suit == Card.Suit.SPADES)
            {
                spadesSUm++;
            }
            // else { } // Error handling should go here
        }
    }

    private bool FourOfAKInd() {
        if (all_cards[0].value == all_cards[1].value && all_cards[0].value == all_cards[2].value && all_cards[0].value == all_cards[3].value) { //needs improvements
            hand_value.total = (int)all_cards[0].value * 4;
            hand_value.high_card = (int)all_cards[4].value;
            return true;
        }
        return false;
    }

    private bool FullHouse() {
        if ((all_cards[0].value == all_cards[1].value && all_cards[0].value == all_cards[2].value && all_cards[3].value == all_cards[4].value) ||
            (all_cards[0].value == all_cards[1].value && all_cards[2].value == all_cards[3].value && all_cards[2].value == all_cards[4].value)
            ) {
            hand_value.total = (int)(all_cards[0].value) + (int)(all_cards[1].value) + (int)(all_cards[2].value) + (int)(all_cards[3].value) + (int)(all_cards[4].value);
            return true;
        }
        return false;
    }

    private bool Flush() {
        if (heartsSum == 5 || diamondSum == 5 || clubSum == 5 || spadesSUm == 5) {
            hand_value.total = (int)all_cards[4].value; //if flush player with higher card win
            return true;
        }
        return false;
    }

    private bool Straight() {
        if (all_cards[0].value + 1 == all_cards[1].value && all_cards[1].value + 1 == all_cards[2].value && all_cards[2].value + 1 == all_cards[3].value && all_cards[3].value + 1 == all_cards[4].value) {
            hand_value.total = (int)all_cards[4].value;
            return true;
        }
        return false;
    }

    private bool getEqualValues(params int[] positions) {
        bool default_value = true;
        for (int i = 0; i < positions.Length - 1; i++) {
            default_value = default_value && (all_cards[positions[i]].value == all_cards[positions[i + 1]].value);
        }
        return default_value;
    }

    private bool ThreeOfAKind() {
        // Pivot with pos 2
        if (getEqualValues(0, 1, 2) || getEqualValues(1, 2, 3) || getEqualValues(2, 3, 4)) {
            hand_value.total = (int)all_cards[2].value * 3;
            //hand_value.high_card = (int)all_cards[3].value;
            return true;
        }
        return false;
    }

    private bool TwoPairs() {
        // Pivot with pos 1
        if (all_cards[0].value == all_cards[1].value && all_cards[2].value == all_cards[3].value) {
            hand_value.total = (int)all_cards[1].value * 2 + (int)all_cards[3].value * 2;
            hand_value.high_card = (int)all_cards[4].value;
            return true;
        }
        else if (all_cards[0].value == all_cards[1].value && all_cards[3].value == all_cards[4].value)
        {
            hand_value.total = (int)all_cards[1].value * 2 + (int)all_cards[3].value * 2;
            hand_value.high_card = (int)all_cards[2].value;
            return true;
        }
        else if (all_cards[1].value == all_cards[2].value && all_cards[3].value == all_cards[4].value)
        {
            hand_value.total = (int)all_cards[1].value * 2 + (int)all_cards[3].value * 2;
            hand_value.high_card = (int)all_cards[0].value;
            return true;
        }
        return false;
    }

    private bool OnePair() {
        if (all_cards[0].value == all_cards[1].value) {
            hand_value.total = (int)all_cards[0].value * 2;
            hand_value.high_card = (int)all_cards[4].value;
            return true;
        }
        else if (all_cards[1].value == all_cards[2].value)
        {
            hand_value.total = (int)all_cards[1].value * 2;
            hand_value.high_card = (int)all_cards[4].value;
            return true;
        }
        else if (all_cards[2].value == all_cards[3].value)
        {
            hand_value.total = (int)all_cards[2].value * 2;
            hand_value.high_card = (int)all_cards[4].value;
            return true;
        }
        else if (all_cards[3].value == all_cards[4].value)
        {
            hand_value.total = (int)all_cards[3].value * 2;
            hand_value.high_card = (int)all_cards[2].value;
            return true;
        }
        return false;
    }
}