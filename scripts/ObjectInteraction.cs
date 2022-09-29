using UnityEngine;
using System.Collections;

public abstract class ObjectInteraction : MonoBehaviour
{
    // Use this for initialization
    protected virtual void Start(){
    }

    // Update is called once per frame
    protected void Update(){

    }


    #region Abstract Methods
    //abstract methods
    abstract public string getName(); 
    #endregion
}
