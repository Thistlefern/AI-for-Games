using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionTreeController : MonoBehaviour
{
    public Agent agent;
    public bool isPrey;
    public bool isAlpha;
    public float health;
    public float fatigue;
    public float thirst;
    public float hunger;
    public float perception;
    public float sneak;

    public IDecision decisionTreeRoot;

    // set up your tree here, assigning additional decisions to the left and right of the tree
    void Start()
    {
        decisionTreeRoot = new PrintDecision(true);
    }

    void Update()
    {
        decisionTreeRoot.MakeDecision();
    }
}
