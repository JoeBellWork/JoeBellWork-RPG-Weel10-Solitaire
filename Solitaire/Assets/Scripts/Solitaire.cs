using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Solitaire : MonoBehaviour
{

    public Sprite[] cardFaces;
    public GameObject cardPrefab;
    public GameObject deckButton;

    public static string[] suits = new string[] { "C", "D", "H", "S" };
    public static string[] values = new string[] {"A","2","3","4","5","6","7","8","9","10", "J", "Q", "K" };
    public GameObject[] bottemPos;
    public GameObject[] topPos;

    public List<string> deck;
    public List<string>[] bottems;
    public List<string>[] tops;


    private List<string> bottem0 = new List<string>();
    private List<string> bottem1 = new List<string>();
    private List<string> bottem2 = new List<string>();
    private List<string> bottem3 = new List<string>();
    private List<string> bottem4 = new List<string>();
    private List<string> bottem5 = new List<string>();
    private List<string> bottem6 = new List<string>();



    public List<string> tripsOnDisplay = new List<string>();
    public List<List<string>> deckTrips = new List<List<string>>();
    private int trips, tripsRemaider, deckLocation;
    public List<string> discardPile = new List<string>();



    // Start is called before the first frame update
    void Start()
    {
        bottems = new List<string>[] { bottem0, bottem1, bottem2, bottem3, bottem4, bottem5, bottem6 };
        playcards();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playcards()
    {
        deck = GenerateDeck();
        shuffle(deck);

        foreach (string card in deck)
        {
            print(card);            
        }
        solitaireSort();
        StartCoroutine(SolitaireDeal());
        sortDeckIntoTrips();
    }


    public static List<string> GenerateDeck()
    {
        List<string> newDeck = new List<string>();
        foreach (string s in suits)
        {
            foreach (string v in values)
            {
                newDeck.Add(s + v);
            }
        }

        return newDeck;
    }


    void shuffle<T>(List<T> list)
    {
        System.Random random = new System.Random();
        int n = list.Count;
        while(n>1)
        {
            int k = random.Next(n);
            n--;
            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }



    IEnumerator SolitaireDeal()
    {
        for (int i = 0; i < 7; i++)
        {
            float yOffset = 0;
            float ZOffset = 0.03f;
            foreach (string card in bottems[i])
            {
                yield return new WaitForSeconds(0.05f);

                GameObject newCard = Instantiate(cardPrefab, new Vector3(bottemPos[i].transform.position.x, bottemPos[i].transform.position.y - yOffset, bottemPos[i].transform.position.z - ZOffset), Quaternion.identity, bottemPos[i].transform);
                newCard.name = card;
                newCard.GetComponent<Selectable>().row = i;
                if (card == bottems[i][bottems[i].Count - 1])
                {
                    newCard.GetComponent<Selectable>().faceup = true;
                }
                yOffset = yOffset + 0.5f;
                ZOffset = ZOffset + 0.03f;
                discardPile.Add(card);

            }            
        }

        foreach(string card in discardPile)
        {
            if(deck.Contains(card))
            {
                deck.Remove(card);
            }
        }
        discardPile.Clear();
    }


    void solitaireSort()
    {
        for(int i = 0; i < 7; i++)
        {
            for(int j= i; j < 7; j++)
            {
                bottems[j].Add(deck.Last<string>());
                deck.RemoveAt(deck.Count - 1);
            }
        }
    }



    void sortDeckIntoTrips()
    {
        trips = deck.Count / 3;
        tripsRemaider = deck.Count % 3;
        deckTrips.Clear();

        int modifer = 0;
        for(int i = 0; i < trips; i++)
        {
            List<string> myTrips = new List<string>();
            for(int j = 0; j < 3; j++)
            {
                myTrips.Add(deck[j + modifer]);
            }
            deckTrips.Add(myTrips);
            modifer = modifer + 3;
        }

        if(tripsRemaider !=0)
        {
            List<string> myRemainder = new List<string>();
            modifer = 0;
            for(int k = 0; k < tripsRemaider; k++)
            {
                myRemainder.Add(deck[deck.Count - tripsRemaider + modifer]);
                modifer++;
            }
            deckTrips.Add(myRemainder);
            trips++;
        }
        deckLocation = 0;
    }




    public void dealFromDeck()
    {
        foreach(Transform child in deckButton.transform)
        {
            if (child.CompareTag("Card"))
            {
                deck.Remove(child.name);
                discardPile.Add(child.name);
                Destroy(child.gameObject);
            }
        }


        if(deckLocation < trips)
        {
            tripsOnDisplay.Clear();
            float xOffset = 2.5f;
            float zOffset = -0.2f;

            foreach (string card in deckTrips[deckLocation])
            {
                GameObject newTopCard = Instantiate(cardPrefab, new Vector3(deckButton.transform.position.x + xOffset, deckButton.transform.position.y, deckButton.transform.position.z + zOffset), Quaternion.identity, deckButton.transform);
                xOffset = xOffset + 0.5f;
                zOffset = zOffset - 0.2f;
                newTopCard.name = card;
                tripsOnDisplay.Add(card);
                newTopCard.GetComponent<Selectable>().faceup = true;
                newTopCard.GetComponent<Selectable>().inDeckPile = true;
            }
            deckLocation++;
        }
        else
        {
            reStackTopDeck();
        }
    }

    void reStackTopDeck()
    {

        deck.Clear();
        foreach(string card in discardPile)
        {
            deck.Add(card);
        }
        discardPile.Clear();
        sortDeckIntoTrips();
    }

}
