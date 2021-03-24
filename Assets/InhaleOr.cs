using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class InhaleOr : MonoBehaviour
{
    private bool resetCircleSize;

    public Vector3 dV = new Vector3(0.01f, 0.01f, 0.01f);

    public bool isInhale;

    // Start is called before the first frame update
    async void Start()
    {

        var sharedMemory = MemoryMappedFile.OpenExisting("SharedMemory");

        var circleScale = this.transform.localScale;

        var inhaletime = 0;

        


        await Task.Run(() =>
        {


            while (true)
            {
                Debug.Log("");
                using (var accessor = sharedMemory.CreateViewAccessor())
                {

                    isInhale = accessor.ReadBoolean(1);
                    Debug.Log("isInhale  " + isInhale);

                   
                  
                    

                }

                Thread.Sleep(100);


            }
        });
    }

    // Update is called once per frame
    void Update()
    {
       
        // TODO: 吸っているときの風のエフェクト

            if (isInhale)
            {
                Debug.Log("isInhale-----------------");
                this.transform.localScale += dV;

            }
            else
            {
                this.transform.localScale -= dV;
            }
        
    }
}
