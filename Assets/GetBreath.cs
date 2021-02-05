using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class GetBreath : MonoBehaviour
{

    public bool breathOn = false;

    public GameObject gazeCubeObj;

    public bool isStay = false;

    // Start is called before the first frame update
    async void Start()
    {
        var sharedMemory = MemoryMappedFile.OpenExisting("SharedMemory");

       // gazeCubeObj = GameObject.Find("GazeCube");

        

        await Task.Run(() =>
        {
            

            while (true)
            {
                using (var accessor = sharedMemory.CreateViewAccessor())
                {

                    var isSelected = accessor.ReadBoolean(1);

                    //Debug.Log("isSelected  " + isSelected);

                    breathOn = isSelected;
                    
                }

                Thread.Sleep(500);

                
            }
        });
    }

    void OnTriggerEnter(Collider t)
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        isStay = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isStay = false;
    }

    // Update is called once per frame
    void Update()
    {

        Debug.Log("Stay is " + isStay);

        if (Input.GetKeyDown(KeyCode.P))
        {
            gazeCubeObj = GameObject.Find("GazeCube");
            Debug.Log(gazeCubeObj);
        }
      

        if (isStay)
        {
            if (breathOn)
            {
                //Debug.Log("Test");

                gameObject.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                this.transform.position = gazeCubeObj.transform.position;


            }
            else
            {
                gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            }
        }

 


    }
}
