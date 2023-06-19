using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage
{
    public int Amount;
    public SelectableObject Dealer;

    public Damage(int amt, SelectableObject dealer) {
        Amount = amt;
        Dealer = dealer;
    }

    public int getAmount() { return Amount; }
    public SelectableObject getDealer() { return Dealer; }
}
