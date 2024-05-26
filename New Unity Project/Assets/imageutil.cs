using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class imageutil : MonoBehaviour
{
    public Image originImg;
    public float xsize;
    public float zsize;

    public int baseNum = 8;
    public Transform Panel;
    // Start is called before the first frame update
    Dictionary<int, List<GameObject>> dic = new Dictionary<int, List<GameObject>>();
    float canvasScale;
    void Start()
    {
        CanvasScaler s = GetComponent<CanvasScaler>();
        canvasScale = s.scaleFactor;
        _up(1, originImg.gameObject, true);
        _left(1, originImg.gameObject, true);
        _left(1, originImg.gameObject, false);
        _up(1, originImg.gameObject, false);

        List<GameObject> list = dic[1];
        
    }

    public float wtimes;
    public float htimes;
    public float scaletimes;
    public Canvas canvas;
    void _left(int pow, GameObject obj, bool flag)
    {
        Vector3[] uparr = left(originImg.transform.localPosition, 1f, flag);
        for (int i = 0; i < uparr.Length; i++)
        {
            GameObject img = GameObject.Instantiate(obj);
            img.transform.parent = Panel;
            img.transform.localPosition = uparr[i];
            img.transform.SetAsFirstSibling();
            img.transform.localScale *= Mathf.Pow(scaletimes, pow);
            img.transform.localRotation = Quaternion.LookRotation(img.transform.position - obj.transform.position);
            img.transform.localRotation *= Quaternion.Euler(0f, 90f, 0f);
            
            if(!dic.ContainsKey(pow))
            {
                dic[pow] = new List<GameObject>();
            }

            dic[pow].Add(img);
        }
    }

    float padding = 0.15f;
    private Vector3[] left(Vector3 pos, float times, bool flag)
    {
        Vector3[] arr = new Vector3[2];
        arr[0] = pos;
        arr[0].x -= originImg.preferredWidth * wtimes * times;
        if (flag)
        {
            arr[0].y += originImg.preferredHeight * htimes * times * padding;
        }
        else
        {
            arr[0].y -= originImg.preferredHeight * htimes * times * padding;
        }

        arr[1].x += originImg.preferredWidth * wtimes * times;
        if (flag)
        {
            arr[1].y += originImg.preferredHeight * htimes * times * padding;
        }
        else
        {
            arr[1].y -= originImg.preferredHeight * htimes * times * padding;
        }
        return arr;
    }

    void _up(int pow, GameObject obj, bool flag)
    {
        Vector3[] uparr = up(originImg.transform.localPosition, 0.5f, flag);
        for (int i = 0; i < uparr.Length; i++)
        {
            GameObject img = GameObject.Instantiate(obj);
            img.transform.parent = Panel;
            img.transform.localPosition = uparr[i];
            img.transform.SetAsFirstSibling();
            img.transform.localScale *= Mathf.Pow(scaletimes,pow);
            img.transform.localRotation = Quaternion.LookRotation(img.transform.position - obj.transform.position);
            img.transform.localRotation *= Quaternion.Euler(0f, 90f, 0f);

            if (!dic.ContainsKey(pow))
            {
                dic[pow] = new List<GameObject>();
            }

            dic[pow].Add(img);
        }
    }

    private Vector3[] up(Vector3 pos, float times, bool flag)
    {
        Vector3[] arr = new Vector3[2];
        arr[0] = pos;
        arr[0].x -= originImg.preferredWidth * wtimes * times;
        if (flag)
        {
            arr[0].y += originImg.preferredHeight * htimes * times;
        }
        else
        {
            arr[0].y -= originImg.preferredHeight * htimes * times;
        }

        arr[1].x += originImg.preferredWidth * wtimes * times;
        if (flag)
        {
            arr[1].y += originImg.preferredHeight * htimes * times;
        }
        else
        {
            arr[1].y -= originImg.preferredHeight * htimes * times;
        }
        return arr;
    }

    void circle(int num)
    {
        for(int i = 0; i < num; i++)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
