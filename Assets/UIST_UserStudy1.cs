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

    public GameObject gazeCubeObj;

    public bool isInObject = false;

    public bool isInhale = false;
    public bool isShortInhale = false;
    public bool isShortExhale = false;
    public bool isLongExhale = false;
    public bool isLongInhale = false;

    // Start is called before the first frame update
    async void Start()
    {

        //呼吸情報を記録しているところにアクセス
        var sharedMemory = MemoryMappedFile.OpenExisting("SharedMemory");

        //視線オブジェクトを取得
        gazeCubeObj = GameObject.Find("GazeCube");

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
    }

    void Update()
    {

        //gazeCubeObj = GameObject.Find("GazeCube"); //マイフレーム取得する必要ある？

        if (!isInObject)
        {
            return;
        }

        if (isShortInhale)
        {
            ClickAction();
            isShortInhale = false;
            Debug.Log("isClicked");

            //gameObject.transform.localScale = Sphere_scale_is_selected;
            gameObject.GetComponent<Renderer>().material.color = Color.red;


            isDragModeOn = true;
        }
        else if (isShortExhale)
        {
            ClickAction();
            isShortExhale = false;
            Debug.Log("isClicked");

            //gameObject.transform.localScale = Sphere_scale_not_selected;

            isDragModeOn = false;

            gameObject.GetComponent<Renderer>().material.color = Color.blue;
        }
        else
        {

        }

        if (isLongInhale)
        {
            DrawingAction();
        }
        else if (isLongExhale)
        {
            PushingAction();
        }

        ApplicationStudy();

     


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
                this.transform.position = gazeCubeObj.transform.position;

            }
            else
            {
                gameObject.transform.localScale = Sphere_scale_not_selected;
            }
        }
    }

    private void ClickAction()
    {
        //TODO: オブジェクトにエフェクトを加える
        gameObject.transform.localScale = Sphere_scale_is_selected;
    }

    private void PushingAction()
    {
        //TODO: 奥に押し込む
    }

    private void DrawingAction()
    {
        //TODO: 手前に引き寄せる
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

    // Update is called once per frame

}