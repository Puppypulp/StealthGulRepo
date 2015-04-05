using UnityEngine;
using System.Collections;

public class FSM 
{
	protected MonoBehaviour sourceScript;
	public string currentState = "none";
	public delegate void UpdateMethod();

	public UpdateMethod OnUpdate;

	// Constructor takes in an instance of the mono that instantiated this object
	public FSM(MonoBehaviour source)
	{
		sourceScript = source;
	}
	// If the player doesn't define an update method then this will be used instead
	private void OnUpdateNull()
	{}

	// Mehtod to send parent script to another state. Takes in a delegate of type void actinf as a function pointer
	public void GoToState(string startMethod, UpdateMethod onUpdateMethod)
	{
		// Whatever the current state is is stopped here
		sourceScript.StopCoroutine(currentState);
		// The new routine is started
		sourceScript.StartCoroutine(startMethod);
		// The state machine sets the new state to the current state to keep track of it
		currentState = startMethod;
		// Now it sets the new method to be called every from the delegate arg passed to this method from parent script;
		OnUpdate = onUpdateMethod;
	}

	// WIP idea is to have an end state to stop all coroutines in the parent script. Not currently working.
	public void EndState()
	{
		sourceScript.StopAllCoroutines();
	}
}
