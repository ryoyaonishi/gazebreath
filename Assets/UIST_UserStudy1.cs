using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public class UIST_UserStudy1 : MonoBehaviour
{

    public GameObject gazeCubeObject;

    public GameObject eyeObject;

    public bool isInObject = false;

    public bool isInhale = false;
    public bool isShortInhale = false;
    public bool isShortExhale = false;
    public bool isLongExhale = false;
    public bool isLongInhale = false;

    [SerializeField]
    public float velocity = 0.01f;

    public Vector3 objectPosition = new Vector3(0, 0, 0);

    private StreamWriter writer_for_exp1_breath;
    private StreamWriter writer_for_exp1_dwell;

    private bool isExp1_Dwell;
    private bool isExp1_Breath;

    private float exp1_startTime = 0.0f;

    private float dwell_startTime = 0.0f;

    private int exp_count_sum = 4;
    private int exp_count_i = 0;

    private List<int> randomList = new List<int>();



    // Start is called before the first frame update
     void Start()
    {
        writer_for_exp1_breath = new StreamWriter(@"breath_onishi_01.csv", true, Encoding.GetEncoding("Shift_JIS"));

        writer_for_exp1_dwell = new StreamWriter(@"dwell_onishi_01.csv", true, Encoding.GetEncoding("Shift_JIS"));

        



        //呼吸情報を記録しているところにアクセス
        var sharedMemory = MemoryMappedFile.OpenExisting("SharedMemory");

        //視線オブジェクトを取得
        gazeCubeObject = GameObject.Find("GazeCube");

        //このオブジェクトの位置を取得
        objectPosition = this.gameObject.transform.position;

        //List       
        
        for (int i = 0; i < exp_count_sum; i++)
        {
            randomList.Add(0);
            randomList.Add(1);
            randomList.Add(2);
            randomList.Add(3);
        }

        Shuffle<int>(randomList);


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
                    }

                    isLongInhale = accessor.ReadBoolean(3);
                    isLongExhale = accessor.ReadBoolean(4);

                }

                Thread.Sleep(100);

                

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

            isExp1_Breath = true;
            isExp1_Dwell = false;

            exp1_start(writer_for_exp1_breath, exp_count_i);


        }
        if (Input.GetKeyDown(KeyCode.D))
        {

            isExp1_Breath = false;
            isExp1_Dwell = true;

            exp1_start(writer_for_exp1_dwell, exp_count_i);
        }

        exp1();

        //exp2();

        //ApplicationStudy();


    }

    public void exp1_start(StreamWriter writer, int i)
    {
        writer.WriteLine("Start");

        exp1_startTime = Time.time;
        
        //色を変える
        gameObject.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 100);

        //場所を変える
        var placeNum = randomList[i];

        writer.WriteLine(placeNum);

        if (placeNum == 0)
        {
            this.transform.position = new Vector3(0.2f, 0.1f, 1.1f);
        }
        else if (placeNum == 1)
        {
            this.transform.position = new Vector3(0.2f, -0.1f, 1.1f);
        }
        else if (placeNum == 2)
        {
            this.transform.position = new Vector3(-0.2f, 0.1f, 1.1f);
        }
        else if(placeNum == 3)
        {
            this.transform.position = new Vector3(-0.2f, -0.1f, 1.1f);
        }

 
        exp_count_i++;
        Debug.Log(exp_count_i);
        Debug.Log(randomList.Count);

        
        if(exp_count_i >= randomList.Count)
        {
            writer.WriteLine("EXPERIMENT_FINISH");
            Debug.Log("EXPERIMENT 1 FINISHED");
            exp_count_i = 0;
        }
       
    }

   



    private void exp1()
    {



        //if (isShortInhale)
        //{
        //    //視線がオブジェクト内部にあるとき
        //    if (isInObject)
        //    {
        //        ClickAction();
        //        //Debug.Log("isClicked");
        //        isDragModeOn = true;
        //    }

        //    isShortInhale = false;

        //}

        if (isShortExhale)
        {
            if (isInObject)
            {
                if(isExp1_Breath || isExp1_Dwell)

                ClickAction();
                //Debug.Log("isClicked");
                isDragModeOn = false;
            }

            isShortExhale = false;

        }
    }

    private void exp2()
    {
        if (isLongInhale)
        {
            if (isInObject)
            {
                DrawingAction();
            }

        }
        else if (isLongExhale)
        {
            if (isInObject)
            {
                PushingAction();
            }

        }
        else
        {
            gameObject.GetComponent<Renderer>().material.color = new Color(255, 255, 255, 50);

        }
    }


    private void ClickAction()
    {

        gameObject.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 0);

        if (Time.time > exp1_startTime)
        {
            writer_for_exp1_breath.WriteLine(Time.time - exp1_startTime);
            writer_for_exp1_breath.WriteLine("Finish");
            writer_for_exp1_breath.WriteLine("");
        }

        isExp1_Breath = false;
        isExp1_Dwell = false;
        
    }



    private void PushingAction()
    {
        //TODO: 奥に押し込む

        gameObject.transform.position += new Vector3(0, 0, velocity * Time.deltaTime);
        gameObject.GetComponent<Renderer>().material.color = Color.red;

        //gazeCubeObject.transform.position = gameObject.transform.position;

    }

    private void DrawingAction()
    {
        //TODO: 手前に引き寄せる

        gameObject.transform.position -= new Vector3(0, 0, velocity * Time.deltaTime);
        gameObject.GetComponent<Renderer>().material.color = Color.blue;


        //gazeCubeObject.transform.position = gameObject.transform.position;

    }

    private void ApplicationStudy()
    {
        //ドラッグアンドドロップ
        //視線を早く動かすと，isInObjectじゃなくなってしまうため，動かなくなる
        if (isInObject)
        {
            if (isDragModeOn)
            {
                gameObject.transform.localScale = Sphere_scale_is_selected;
                this.transform.position = gazeCubeObject.transform.position;

            }
            else
            {
                gameObject.transform.localScale = Sphere_scale_not_selected;
            }
        }
    }



    void OnTriggerEnter(Collider t)
    {
        if (isExp1_Dwell)
        {
            dwell_startTime = Time.time;

        }
    }

    private void OnTriggerStay(Collider other)
    {
        isInObject = true;

        gameObject.transform.localScale = new Vector3(0.11f, 0.11f, 0.11f);

        //Debug.Log(Time.time - dwell_startTime);

        if (isExp1_Dwell)
        {
            if (Time.time - dwell_startTime > 0.7f)
            {
                ClickAction();
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        isInObject = false;

        gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        if (isExp1_Dwell)
        {
            dwell_startTime = 0;
        }
    }


    Vector3 Sphere_scale_is_selected = new Vector3(0.1f, 0.1f, 0.1f);
    Vector3 Sphere_scale_not_selected = new Vector3(0.15f, 0.15f, 0.15f);
    private bool isMoveModeOn;
    private bool isDragModeOn;
    private bool isInTargetObject;

    private void OnApplicationQuit()
    {
        writer_for_exp1_breath.Dispose();
        writer_for_exp1_dwell.Dispose();
        
        
    }
}