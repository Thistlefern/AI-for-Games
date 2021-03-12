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

    public class EvaluateDecision : IDecision
    {
        public bool branch = false;

        // default constructor
        public EvaluateDecision() { }

        // parameterized constructor allows you to define branch on construction
        public EvaluateDecision(bool val)
        {
            branch = val;
        }

        // evaluate the decision
        public IDecision MakeDecision()
        {
            Debug.Log(branch ? "Yes" : "No");
            return null;
        }
    }
    public class RangeCheckDecision : IDecision
    {
        public Transform myTransform;
        public Transform target;

        public IDecision isInRange;
        public IDecision isNotInRange;

        public float range = 1.0f;

        public IDecision MakeDecision()
        {
            bool rangeTest = (myTransform.position - target.position).magnitude <= range;

            if (rangeTest)
            {
                return isInRange;
            }
            else
            {
                return isNotInRange;
            }
        }
    }

    public class FleeDecision : IDecision
    {
        public IDecision flee;
        public IDecision MakeDecision()
        {
            flee.MakeDecision();
            return flee;
        }
    }

    void Start()
    {
        decisionTreeRoot = new EvaluateDecision(true);
    }

    void Update()
    {
        IDecision currentDecision = decisionTreeRoot;
        while(currentDecision != null)
        {
            decisionTreeRoot.MakeDecision();
        }
    }
}
