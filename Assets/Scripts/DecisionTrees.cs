using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionTrees : MonoBehaviour
{
    public interface IDesicion
    {
        IDesicion MakeDecision();

        // You may want to add properties to retrieve the left or right children of the decision, if you'd like
    }
    // The PrintDecision type implements the IDecision interface
    // - When a type implements an interface, it must define all members defined by the interface. This case, that's just the MakeDecision() method.
    public class PrintDecision : IDesicion
    {
        public bool branch = false;

        // default constructor
        public PrintDecision() { }

        // parameterized constructor allows you to define branch on construction
        public PrintDecision(bool branch)
        {
            this.branch = branch;
        }
        
        // evaluate the decision
        public IDesicion MakeDecision()
        {
            Debug.Log(branch ? "Yes" : "No");
            return null;
        }
    }

    public class DecisionTreeController : MonoBehaviour
    {
        public Agent agent;

        public IDesicion desicionTreeRoot;

        // set up your tree here, assigning additional decisions to the left and right of the tree
        void Start()
        {
            desicionTreeRoot = new PrintDecision(true);
        }

        void Update()
        {
            desicionTreeRoot.MakeDecision();
        }
    }
}
