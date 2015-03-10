using UnityEngine;
using System.Collections;


public class PlayerControllerOld : MonoBehaviour //unity needs things serialized to see them in instructor
{
	public float speed; ///allows the speed of the character to be fixed
	//public Text countText;
	//public Text winText;
	private int count;

	void Start ()
	{
		count = 0;
		/*SetCountText(); //DONT enter the "void" part, stupid
		winText.text = ""; */
	}

	void FixedUpdate () ///FixedUpdate happens before every frame
	{
		///The input for player control
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		///Simplest way to move the character
		Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

		GetComponent<Rigidbody>().AddForce (movement * speed * Time.deltaTime);

		///makes it so the seagul doesn't spin
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
	}

	void OnTriggerEnter(Collider other) 
	{
		if(other.gameObject.tag == "PickUp")
		{
			other.gameObject.SetActive(false);
			count = count + 1;
			//SetCountText();
		}
	}
	/*Creating a function to be called. Will display the count
	void SetCountText()
	{
		countText.text = "Count: " + count.ToString ();
		if(count >=8)
		{
			winText.text = "You Won, Nigga!";
		}
	} */
}
