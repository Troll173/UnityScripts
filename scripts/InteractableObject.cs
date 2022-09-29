using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;

public abstract class InteractableObject : MonoBehaviour
{
	#region public variables 
	public float reachRange = 1.8f;
	public List<ObjectInteraction> interactions = new List<ObjectInteraction>();
	#endregion

	public bool isInteracting = false;

	private Camera fpsCam;
	private GameObject player;

	private bool playerEntered;
	private bool showInteractMsg;
	private GUIStyle guiStyle;
	private string msg;

	private int rayLayerMask;


	// Use this for initialization
	//make it protected virtual incase the child class needs to access Start as well
	protected virtual void Start(){

		//Initialize moveDrawController if script is enabled.
		player = GameObject.FindGameObjectWithTag("Player");

		fpsCam = Camera.main;
		if (fpsCam == null) { //a reference to Camera is required for rayasts
			Debug.LogError("A camera tagged 'MainCamera' is missing.");
		}

		//the layer used to mask raycast for interactable objects only
		LayerMask iRayLM = LayerMask.NameToLayer("InteractRaycast");
		rayLayerMask = 1 << iRayLM.value;

		//setup GUI style settings for user prompts
		setupGui();

		onInit(player);
	}



	protected void Update(){
		if (playerEntered){

			//center point of viewport in World space.
			Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
			RaycastHit hit;

			//if raycast hits a collider on the rayLayerMask
			if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, reachRange, rayLayerMask)){

				MoveableObject moveableObject = null;
				//is the object of the collider player is looking at the same as me?
				if (!isEqualToParent(hit.collider, out moveableObject)){
					return;
				}


                if (!isInteracting) {
                    showInteractMsg = true;
					msg = getGuiMsg();

					if (Input.GetKeyUp(KeyCode.E) || Input.GetButtonDown("Fire1")){
						msg = "";
						showInteractMsg = false;
						isInteracting = true;
						disablePlayer();
						onStartInteraction(); 
					}

                }
            }else{
				showInteractMsg = false;
			}
		}

	}


	private void disablePlayer(){
		player.GetComponent<FirstPersonController>().enabled = false;
	} 

	void OnTriggerEnter(Collider other){
		if (other.gameObject == player){  //player collided with trigger
			playerEntered = true; 
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject == player){ //player has exited trigger
			playerEntered = false;
			//hide interact message as player may not have been looking at object when they left
			showInteractMsg = false;
		}
	}


	void endInteraction(){
		isInteracting = false;
		onEndInteraction(); 
	}



	//is current gameObject equal to the gameObject of other.  check its parents
	private bool isEqualToParent(Collider other, out MoveableObject draw){
		draw = null;
		bool rtnVal = false;
		try{
			int maxWalk = 6;
			draw = other.GetComponent<MoveableObject>();

			GameObject currentGO = other.gameObject;
			for(int i=0;i<maxWalk;i++){

				if (currentGO.Equals(gameObject)){
					rtnVal = true;	
					if (draw== null) draw = currentGO.GetComponentInParent<MoveableObject>();
					break;			//exit loop early.
				}

				//not equal to if reached this far in loop. move to parent if exists.
				if (currentGO.transform.parent != null){
					currentGO = currentGO.transform.parent.gameObject;
				}
			}
		} 
		catch (System.Exception e){
			Debug.Log(e.Message);
		}
			
		return rtnVal;

	}
		

	#region GUI Config

	//configure the style of the GUI
	private void setupGui(){
		guiStyle = new GUIStyle();
		guiStyle.fontSize = 16;
		guiStyle.fontStyle = FontStyle.Bold;
		guiStyle.normal.textColor = Color.white;
		msg = "";
	}

	

	void OnGUI(){
		if (showInteractMsg) {
			GUI.Label(new Rect (50,Screen.height - 50,200,50), msg,guiStyle);
		}
	}
	//End of GUI Config --------------
	#endregion

	#region Abstract Methods
	//abstract methods
	abstract public void onInit(GameObject player);
	abstract public void onStartInteraction();
	abstract public void onEndInteraction();
	abstract public string getGuiMsg();

	#endregion
}
