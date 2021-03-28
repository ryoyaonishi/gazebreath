using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public class UIST_UserStudy2 : MonoBehaviour
{
    [SerializeField]
    public string name = "";

    [SerializeField]
    public float velocity = 0.01f;

    public GameObject gazeCubeObject;

    public GameObject manipulateObject;

    public GameObject goalForward;
    public GameObject goalBackward;

    public bool isInObject = false;

    public bool isInhale = false;
    public bool isShortInhale = false;
    public bool isShortExhale = false;
    public bool isLongExhale = false;
    public bool isLongInhale = false;

    public Vector3 objectPosition = new Vector3(0, 0, 0);

    private StreamWriter writer_for_exp2;

    private float exp2_start_time = 0.0f;


    private int exp_count_sum = 2;
    private int exp_count_i = 0;

    private List<int> randomList = new List<int>();
    private bool isForward = true;



    // Start is called before the first frame update
    void Start()
    {
        var filename = "Exp2" + name + velocity.ToString() + ".csv";

        //writer_for_exp2 = new StreamWriter(@"exp2_onishi_01.csv", false, Encoding.GetEncoding("Shift_JIS"));
        writer_for_exp2 = new StreamWriter(@filename, false, Encoding.GetEncoding("Shift_JIS"));

        manipulateObject = transform.Find("ManipulatedObject").gameObject;

        goalForward = transform.Find("GoalForward").gameObject;

        goalBackward = transform.Find("GoalBackward").gameObject;

        //視線オブジェクトを取得
        gazeCubeObject = GameObject.Find("GazeCube");

        //List       

        for (int i = 0; i < exp_count_sum; i++)
        {
            randomList.Add(0);
            randomList.Add(1);
            randomList.Add(2);
            randomList.Add(3);
        }

        Shuffle<int>(randomList);


        //呼吸情報を記録しているところにアクセス
        var sharedMemory = MemoryMappedFile.OpenExisting("SharedMemory");
        //呼吸情報を取得
        GetData(sharedMemory);

        

    }

    async void GetData(MemoryMappedFile sharedMemory)
    {
        await Task.Run(() =>
        {
            while (true)
            {

                using (var accessor = sharedMemory.CreateViewAccessor())
                {


                    isInhale = accessor.ReadBoolean(0);

                    if (accessor.ReadBoolean(1))
                    {
                        isShortInhale = accessor.ReadBoolean(1);
                        accessor.Write(1, false);
                    }
                    if (accessor.ReadBoolean(2))
                    {
                        isShortExhale = accessor.ReadBoolean(2);
                        accessor.Write(2, false);

                        //if (isExperiment)
                        //{
                        //    writer_for_detect_exhale.WriteLine(Time.time + ",detectExhale");
                        //}

                    }

                    isLongInhale = accessor.ReadBoolean(3);
                    isLongExhale = accessor.ReadBoolean(4);

                }

                Thread.Sleep(10);



            }
        });


    }


    public List<T> Shuffle<T>(List<T> list)
    {

        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(0, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }

        return list;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.B))
        {
            exp2_start(writer_for_exp2, exp_count_i);

        }

        //誤差+-0.05以内なら，赤く点灯
        if(0.15 < manipulateObject.transform.localPosition.z && manipulateObject.transform.localPosition.z <= 0.35)
        {
            if(!isForward)
            manipulateObject.GetComponent<Renderer>().material.color = new Color(255, 0, 0, 50);

        }
        else if (-0.35 < manipulateObject.transform.localPosition.z && manipulateObject.transform.localPosition.z <= -0.15)
        {
            if(isForward)
            manipulateObject.GetComponent<Renderer>().material.color = new Color(255, 0, 0, 50);
        }
        else
        {
            manipulateObject.GetComponent<Renderer>().material.color = new Color(255, 255, 255, 50);

        }

        writer_for_exp2.WriteLine(Time.time + "," + manipulateObject.transform.localPosition.z);
    }

    public void exp2_start(StreamWriter writer, int i)
    {
        //writer.WriteLine("Start");

        exp2_start_time = Time.time;

        //場所を変える
        var placeNum = randomList[i];

        writer.WriteLine("placeNum," + placeNum);

        if (placeNum == 0)
        {
            this.transform.position = new Vector3(0.18f, 0.1f, 1.1f);
        }
        else if (placeNum == 1)
        {
            this.transform.position = new Vector3(0.18f, -0.1f, 1.1f);
        }
        else if (placeNum == 2)
        {
            this.transform.position = new Vector3(-0.18f, 0.1f, 1.1f);
        }
        else if (placeNum == 3)
        {
            this.transform.position = new Vector3(-0.18f, -0.1f, 1.1f);
        }

        writer.WriteLine("isForward," + isForward);

        //forwardかbackwartか
        if (isForward)
        {
            goalForward.GetComponent<Renderer>().material.color = new Color(0, 0, 255, 10);
            goalBackward.GetComponent<Renderer>().material.color = new Color(0, 0, 255, 0);
            isForward = false;
        }
        else
        {
            goalForward.GetComponent<Renderer>().material.color = new Color(0, 0, 255, 0);
            goalBackward.GetComponent<Renderer>().material.color = new Color(0, 0, 255, 10);
            isForward = true;
        }


        exp_count_i++;
        Debug.Log(exp_count_i);
        //Debug.Log(randomList.Count);

        manipulateObject.transform.localPosition = new Vector3(0, 0, 0);


        if (exp_count_i >= randomList.Count)
        {
            //writer.WriteLine("EXPERIMENT_FINISH");
            Debug.Log("EXPERIMENT 1 FINISHED");
            exp_count_i = 0;
        }

    }

    void OnTriggerEnter(Collider t)
    {

    }

    private void OnTriggerStay(Collider other)
    {
        manipulateObject.transform.localScale = new Vector3(1.1f,1.1f,0.2f);

        var z = manipulateObject.transform.localPosition.z;

        if (-0.5 <= z)
        {
            if (isLongExhale)
            {
                manipulateObject.transform.localPosition += new Vector3(0, 0, velocity * Time.deltaTime);
            }
        }
        else
        {
            
            manipulateObject.transform.localPosition = new Vector3(0, 0, -0.5f);
        }

        if(z <= 0.5)
        {      
            if (isLongInhale)
            {
                manipulateObject.transform.localPosition -= new Vector3(0, 0, velocity * Time.deltaTime);
            }
        }
        else
        {
            manipulateObject.transform.localPosition = new Vector3(0, 0, 0.5f);
        }

    }

    private void OnTribggerExit(Collider other)
    {

        manipulateObject.transform.localScale = new Vector3(1,1,0.2f);

    }

    private void OnApplicationQuit()
    {
        writer_for_exp2.Dispose();

    }
}