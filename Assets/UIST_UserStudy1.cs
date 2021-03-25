using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

using System.IO;
using System.Text;
using System;

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

   


    // Start is called before the first frame update
    async void Start()
    {

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

        //gazeCubeObj = GameObject.Find("GazeCube"); //マイフレーム取得する必要ある？



        //if (!isInObject)
        //{
        //    return;
        //}

        if (isShortInhale)
        {
            ClickAction();
            isShortInhale = false;
            Debug.Log("isClicked");

            isDragModeOn = true;
        }
        else if (isShortExhale)
        {
            ClickAction();
            isShortExhale = false;
            Debug.Log("isClicked");

            isDragModeOn = false;
        }
        else
        {

        }

        if (isLongInhale)
        {
            Debug.Log("Looooooooooooong Inhale");
            DrawingAction();
        }
        else if (isLongExhale)
        {
            PushingAction();
        }

        //ApplicationStudy();

     


    }



    private void ClickAction()
    { 
        //視線がオブジェクト内部にあるとき
        if (isInObject)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.red;
        }

    }

    private void PushingAction()
    {
        //TODO: 奥に押し込む
        if (isInObject) {
            gameObject.transform.position += new Vector3(0,0,velocity * Time.deltaTime);
        }
    }

    private void DrawingAction()
    {
        //TODO: 手前に引き寄せる
        if (isInObject)
        {
            gameObject.transform.position -= new Vector3(0, 0, velocity * Time.deltaTime);
        }
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

    }

    private void OnTriggerStay(Collider other)
    {
        isInObject = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isInObject = false;
    }


    Vector3 Sphere_scale_is_selected = new Vector3(0.1f, 0.1f, 0.1f);
    Vector3 Sphere_scale_not_selected = new Vector3(0.15f, 0.15f, 0.15f);
    private bool isMoveModeOn;
    private bool isDragModeOn;



}