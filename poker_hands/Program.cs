
using poker_hands;

internal class Program
{
    private static void Main(string[] args)
    {
        Controller controller;
        View view = new View(new Controller());
        controller = view.controller;
        while (true) {
            controller.DrawHands();
            view.refresh();
        }
    }
}


internal class Controller {
    private poker_hands.DeckOfCards deck;
    public poker_hands.Card[] hand1 { get; set; }
    public poker_hands.Card[] hand2 { get; set; }
    public bool result { get; set; }
    public poker_hands.Hand result1 { get; set; }
    public poker_hands.Hand result2 { get; set; }
    public Controller() {
        deck = new poker_hands.DeckOfCards();
    }

    public void DrawHands() {
        hand1 = deck.getHand();
        hand2 = deck.getHand();

        if (hand1 == null || hand2 == null)
        {
            deck.resetDeck();
            deck.shuffleCards();
            return;
        }

        poker_hands.HandEvaluator hand_evaluator1 = new poker_hands.HandEvaluator(poker_hands.DeckOfCards.SortHand(hand1));
        poker_hands.HandEvaluator hand_evaluator2 = new poker_hands.HandEvaluator(poker_hands.DeckOfCards.SortHand(hand2));

        result1 = hand_evaluator1.EvaluateHand();
        result2 = hand_evaluator2.EvaluateHand();

        if (result1 == result2)
        {
            if (hand_evaluator1.HandValues.total == hand_evaluator2.HandValues.total)
            {
                result = hand_evaluator1.HandValues.high_card > hand_evaluator2.HandValues.high_card;
                // there is  the case when high card is equal
                // couple soulutions here overload an operator or add a method that recalculate higher card
                // in which case is better to have the logic in a higher order class (composition)
            }
            else {
                result = hand_evaluator1.HandValues.total > hand_evaluator2.HandValues.total;
            }
        }
        else {
            result = result1 > result2;
        }
    }

    static public String getStringHand(poker_hands.Card[] hands) {
        String s = new String("");
        foreach (poker_hands.Card h in hands) {
            s += $"{h.suit.ToString()} {h.value.ToString()}\n";
            // s += " ";
        }
        return s;
    }
}

public enum GameMode
{
    Text,
    DOS,
    GUI,
    DirectX
}


internal class View {
    GameMode game_mode;
    public Controller controller { get; }
    public View(Controller controller, GameMode game_mode= GameMode.Text)
    {
        this.game_mode = game_mode;
        this.controller = controller;
    }



    public void refresh() {
        if (game_mode == GameMode.Text)
        {
            PrintText();
        }
        else if (game_mode == GameMode.DOS)
        {
        }
        else if (game_mode == GameMode.GUI)
        {
        }
        else if (game_mode == GameMode.DirectX)
        {
        }
    }

    public void PrintText() {
        Console.Clear();
        if (controller.hand1 == null || controller.hand2 == null)
        {
            Console.WriteLine("Deck is empy. Start a new deck");
        }
        else
        {
            Console.WriteLine(Controller.getStringHand(controller.hand1));
            Console.WriteLine(Controller.getStringHand(controller.hand2));
            Console.WriteLine($"Computer one hand is {controller.result1} and Computer two hand is {controller.result2}");
            if (controller.result)
                Console.WriteLine("Winner is Computer One");
            else
                Console.WriteLine("Winner is Computer Two");
        }
        Console.WriteLine("\n\nPress any key to continue....");
        Task.Factory.StartNew(() => Console.ReadKey()).Wait(TimeSpan.FromSeconds(120.0));
    }
}