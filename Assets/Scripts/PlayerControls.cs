using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script to handle the player controls 
public class PlayerControls : MonoBehaviour
{
    bool newClick = false;

    private Vector3[] boxPoints;
    private Vector3 boxStart;
    private Vector3 boxEnd;

    LineRenderer selectionBox;
    int boxLength;
    bool boxVisibility = false;

    BoxCollider2D selectionCollider;

    UnitSelection selector;

    [SerializeField] float timeThreshold = 1;
    [SerializeField] float dragThreshold = 1;
    float[] clickTimer = new float[2];



    // Start is called before the first frame update
    void Start()
    {
        // Create the selection box lineRenderer
        selectionBox = GameObject.Find("SelectionBox").GetComponent<LineRenderer>();
        //Debug.Log(selectionBox);
        boxPoints = new Vector3[selectionBox.positionCount];
        //Debug.Log(selectionBox.positionCount);
        selectionCollider = GameObject.Find("SelectionBox").GetComponent<BoxCollider2D>();

        selector = GameObject.Find("SelectionBox").GetComponent<UnitSelection>();
    }

    // Modify the lineRenderer and BoxCollider based on the position of
    // boxStart (initial click) and boxEnd (current mouse position) to create
    // a box that varies in size
    void DrawBox()
    {   
        // Check the visibility of the line renderer and box collider
        if (boxVisibility)
        {
            // Move the box to initial mouseposition 
            selectionBox.transform.position = boxStart;

            boxStart.z = 0;
            boxEnd.z = 0;

            float x = (boxEnd.x - boxStart.x);
            float y = (boxEnd.y - boxStart.y);

            // Create points for each corner of the box
            boxPoints[0] = boxStart;
            boxPoints[1] = new Vector3(boxStart.x, boxStart.y + y, 0); 
            boxPoints[2] = new Vector3(boxStart.x + x, boxStart.y + y, 0); 
            boxPoints[3] = new Vector3(boxStart.x + x, boxStart.y, 0);
            boxPoints[4] = boxStart;

            //Debug.Log("Point 1: " + boxStart);
            //Debug.Log("Point 2: " + boxEnd);

            // Set the points of the line renderer and enable in the scene
            selectionBox.SetPositions(boxPoints);
            selectionBox.enabled = boxVisibility;

            // Set the position and visibility of the box collider
            selectionCollider.enabled = boxVisibility;
            selectionCollider.offset = new Vector2(x/2, y/2);
            selectionCollider.size = new Vector2(Mathf.Abs(x), Mathf.Abs(y));
        }
        else
        {
            // Disable the line renderer 
            selectionBox.enabled = boxVisibility;
            selectionCollider.enabled = boxVisibility;

        }

    }

    // Update is called once per frame
    void Update()
    {
        // Register button hold pressed
        if (Input.GetButtonDown("Fire1"))
        {
            newClick = true;
            // get initial mouse position
            boxStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // start click timer
            clickTimer[0] = Time.time;
            //Debug.Log(clickTimer[0]);

        }

        // Register button hold
        if (Input.GetMouseButton(0))
        {
            // update click timer
            clickTimer[1] = Time.time;
            // get current mouse position
            boxEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // determine if mouse is being dragged
            if (boxEnd != boxStart)
            {
                // determine if this is a new button click
                if (newClick)
                {
                    // clear selection list to select new units
                    selector.unitsList.Clear();
                    newClick = false;
                    //Debug.Log("List cleared");

                }

                //Debug.Log("Selection Drag");
                // update selection box
                boxVisibility = true;
                DrawBox();
            }
            

        }
        // Register button release
        if (Input.GetButtonUp("Fire1"))
        {
            newClick = true;

            // determine type of button press (short click)
            if ((clickTimer[1]- clickTimer[0]) < timeThreshold)
            {
                float xDiff = Mathf.Abs(boxEnd.x - boxStart.x);
                float yDiff = Mathf.Abs(boxEnd.y - boxStart.y);

                if (xDiff < dragThreshold && xDiff < dragThreshold)
                {
                    // short clicks update position the units should move to
                    ZombieUnit zombie;
                    foreach (string name in selector.unitsList)
                    {
                        zombie = GameObject.Find(name).GetComponent<ZombieUnit>();
                        zombie.desiredPosition = new Vector2(boxStart.x, boxStart.y);
                        zombie.moving = true;
                        //Debug.Log(name);
                    }
                }
            }

            // disable the line renderer and the box collider of the selectionBox
            boxVisibility = false;
            DrawBox();

            // reset click timer
            clickTimer[0] = 0;
            clickTimer[1] = 0;
        }

    }
}
