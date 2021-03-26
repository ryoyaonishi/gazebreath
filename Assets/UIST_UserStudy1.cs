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

    private bool isExp_Dwell;
    private bool isExp_Breath;

    private float exp1_startTime = 0.0f;

    private float dwell_startTime = 0.0f;

    

    // Start is called before the first frame update
    async void Start()
    {
        writer_for_exp1_breath = new StreamWriter(@"SaveData_onishi_b.csv", true, Encoding.GetEncoding("Shift_JIS"));

        writer_for_exp1_dwell = new StreamWriter(@"SaveData_onishi_d.csv", true, Encoding.GetEncoding("Shift_JIS"));



        //呼吸情報を記録しているところにアクセス
        var sharedMemory = MemoryMappedFile.OpenExisting("SharedMemory");

        //呼吸情報を取得
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

                Thread.Sleep(500);

            }
        });

        //視線オブジェクトを取得
        gazeCubeObject = GameObject.Find("GazeCube");

        //このオブジェクトの位置を取得
        objectPosition = this.gameObject.transform.position;

        


    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.B))
        {
            gameObject.GetComponent<Renderer>().material.color = new Color32(0,255,0,50);

            isExp_Breath = true;
            isExp_Dwell = false;
            writer_for_exp1_breath.WriteLine("Start");

            exp1_startTime = Time.time;

        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            gameObject.GetComponent<Renderer>().material.color = new Color32(0, 255, 0, 50);

            isExp_Breath = false;
            isExp_Dwell = true;
            
            writer_for_exp1_dwell.WriteLine("Start");

            exp1_startTime = Time.time;
        }


        if (isShortInhale)
        {
            //視線がオブジェクト内部にあるとき
            if (isInObject)
            {
                ClickAction();
                Debug.Log("isClicked");
                isDragModeOn = true;
            }
      
            isShortInhale = false;

        }
        else if (isShortExhale)
        {
            if (isInObject)
            {
                ClickAction();
                Debug.Log("isClicked");
                isDragModeOn = false;
            }

            isShortExhale = false;

        }

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

        //ApplicationStudy();


    }



    private void ClickAction()
    {

        gameObject.GetComponent<Renderer>().material.color = Color.red;

        if (Time.time > exp1_startTime)
        {
            writer_for_exp1_breath.WriteLine(Time.time - exp1_startTime);
            writer_for_exp1_breath.WriteLine("Finish");
        }


    }

    private void PushingAction()
    {
        //TODO: 奥に押し込む

        gameObject.transform.position += new Vector3(0, 0, velocity * Time.deltaTime);
        gameObject.GetComponent<Renderer>().material.color = Color.red;

    }

    private void DrawingAction()
    {
        //TODO: 手前に引き寄せる

        gameObject.transform.position -= new Vector3(0, 0, velocity * Time.deltaTime);
        gameObject.GetComponent<Renderer>().material.color = Color.blue;

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
        if (isExp_Dwell)
        {
            dwell_startTime = Time.time;

        }
    }

    private void OnTriggerStay(Collider other)
    {
        isInObject = true;

        gameObject.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);


        if (isExp_Dwell)
        {
            if (Time.time - dwell_startTime > 700)
            {
                ClickAction();
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        isInObject = false;

        gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        if (isExp_Dwell)
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
    }
}