using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UserInput : MonoBehaviour
{
    private Solitaire solitaire;

    public GameObject slot1;
    private float timer, doubleClickTimer = 0.3f;
    private int clickCount = 0;


    // Start is called before the first frame update
    void Start()
    {
        solitaire = FindObjectOfType<Solitaire>();
        slot1 = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(clickCount == 1)
        {
            timer += Time.deltaTime;
        }
        if(clickCount == 3)
        {
            timer = 0;
            clickCount = 0;
        }
        if(timer > doubleClickTimer)
        {
            timer = 0;
            clickCount = 0;
        }

        getMouseClick();
    }


    void getMouseClick()
    {
        if(Input.GetMouseButton(0))
        {
            clickCount++;

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),Vector2.zero);

            if(hit)
            {
                if(hit.collider.CompareTag("Deck"))
                {
                    Deck();
                }
                else if(hit.collider.CompareTag("Card"))
                {
                    Card(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Top"))
                {
                    Top(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Bottem"))
                {
                    Bottem(hit.collider.gameObject);
                }
            }
        }
    }




    void Deck()
    {
        
        solitaire.dealFromDeck();
    }

    void Card(GameObject selected)
    {   
        if(!selected.GetComponent<Selectable>().faceup)
        {

            if (!blocked(selected))
            {
                selected.GetComponent<Selectable>().faceup = true;
                slot1 = this.gameObject;
            }
            
        }
        else if(selected.GetComponent<Selectable>().inDeckPile)
        {
            if(!blocked(selected))
            {
                if(slot1 == selected)
                {
                    if(doubleClick())
                    {

                    }
                }
                else
                {
                    slot1 = selected;
                }                
            }
        }

        if(slot1 == this.gameObject)
        {
            slot1 = selected;
        }
        else if(slot1 != selected)
        {
            if(stackable(selected))
            {
                Stack(selected);       
            }
            else
            {
                slot1 = selected;
            }
            
        }

        else if(slot1 == selected)
        {
            if(doubleClick())
            {
                autoStack(selected);
            }
        }
    }        
    void Top(GameObject selected)
    {
        if(slot1.CompareTag("Card"))
        {
            if(slot1.GetComponent<Selectable>().value == 1)
            {
                Stack(selected);
            }
        }
    }

    void Bottem(GameObject selected)
    {
        if(slot1.CompareTag("Card"))
        {
            if(slot1.GetComponent<Selectable>().value == 13)
            {
                Stack(selected);
            }
        }
    }



    bool stackable(GameObject selected)
    {
        Selectable s1 = slot1.GetComponent<Selectable>();
        Selectable s2 = selected.GetComponent<Selectable>();


        if (!s2.inDeckPile)
        {
            if (s2.top)
            {
                if (s1.suit == s2.suit || (s1.value == 1 && s2.value == null))
                {
                    if (s1.value == s2.value + 1)
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (s1.value == s2.value - 1)
                {
                    bool card1Red = true;
                    bool card2Red = true;

                    if (s1.suit == "C" || s1.suit == "S")
                    {
                        card1Red = false;
                    }
                    if (s2.suit == "C" || s2.suit == "S")
                    {
                        card2Red = false;
                    }

                    if (card1Red == card2Red)
                    {
                        print("not Stackable");
                        return false;
                    }
                    else
                    {
                        print("are Stackable");
                        return true;
                    }
                }
            }
        }
        return false;
    }


    void Stack(GameObject selected)
    {
        Selectable s1 = slot1.GetComponent<Selectable>();
        Selectable s2 = selected.GetComponent<Selectable>();
        float yOffset = 0.3f;

        if(s2.top || !s2.top && s1.value == 13)
        {
            yOffset = 0f;
        }

        slot1.transform.position = new Vector3(selected.transform.position.x, selected.transform.position.y - yOffset, selected.transform.position.z - 0.01f);
        slot1.transform.parent = selected.transform;

        if(s1.inDeckPile)
        {
            solitaire.tripsOnDisplay.Remove(slot1.name);
        }
        else if(s1.top && s2.top && s1.value == 1)
        {
            solitaire.topPos[s1.row].GetComponent<Selectable>().value = 0;
            solitaire.topPos[s1.row].GetComponent<Selectable>().suit = null;
        }
        else if(s1.top)
        {
            solitaire.topPos[s1.row].GetComponent<Selectable>().value = s1.value - 1;
        }
        else
        {
            solitaire.bottems[s1.row].Remove(slot1.name);
        }
        s1.inDeckPile = false;
        s1.row = s2.row;

        if(s2.top)
        {
            solitaire.topPos[s1.row].GetComponent<Selectable>().value = s1.value;
            solitaire.topPos[s1.row].GetComponent<Selectable>().suit = s1.suit;
            s1.top = true;
        }
        else
        {
            s1.top = false;
        }
        slot1 = this.gameObject;
    }



    bool blocked(GameObject selected)
    {
        Selectable s2 = selected.GetComponent<Selectable>();
        if (s2.inDeckPile == true)
        {
            if (s2.name == solitaire.tripsOnDisplay.Last())
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else 
        { 
            if(s2.name == solitaire.bottems[s2.row].Last())
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }

    void autoStack(GameObject selected)
    {
        for(int i = 0; i < solitaire.topPos.Length; i++)
        {
            Selectable stack = solitaire.topPos[i].GetComponent<Selectable>();
            if(selected.GetComponent<Selectable>().value == 1)
            {
                if(solitaire.topPos[i].GetComponent<Selectable>().value == 0)
                {
                    slot1 = selected;
                    Stack(stack.gameObject);
                    break;
                }
            }
            else
            {
                if((solitaire.topPos[i].GetComponent<Selectable>().suit == slot1.GetComponent<Selectable>().suit) && (solitaire.topPos[i].GetComponent<Selectable>().value == slot1.GetComponent<Selectable>().value - 1))
                {
                    if(hasNoChildren(slot1))
                    {
                        slot1 = selected;
                        string LastCardName = stack.suit + stack.value.ToString();
                        if (stack.value == 1)
                        {
                            LastCardName = stack.suit + "A";
                        }
                        if (stack.value == 11)
                        {
                            LastCardName = stack.suit + "J";
                        }
                        if (stack.value == 12)
                        {
                            LastCardName = stack.suit + "Q";
                        }
                        if (stack.value == 13)
                        {
                            LastCardName = stack.suit + "K";
                        }
                        GameObject lastCard = GameObject.Find(LastCardName);
                        Stack(lastCard);
                        break;
                    }                    

                }
            }

        }
    }

    bool hasNoChildren(GameObject card)
    {
        int i = 0;
        foreach(Transform child in card.transform)
        {
            i++;
        }
        if(i == 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }



    bool doubleClick()
    {
        if(timer <doubleClickTimer && clickCount == 2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
