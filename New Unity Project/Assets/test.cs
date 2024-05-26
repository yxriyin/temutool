using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
public class test : MonoBehaviour
{
    public Button path;
    public Button rename;
    public Button rename1;
    string chosePath;
    // Start is called before the first frame update
    void Start()
    {
        path.onClick.AddListener(PathClick);
        rename.onClick.AddListener(renameClick);
        rename1.onClick.AddListener(renameClick1);
        chosePath = getCacheString();
    }

    string getCacheString()
    {
        return PlayerPrefs.GetString("path");
    }

    void PathClick()
    {
        chosePath = EditorUtility.OpenFolderPanel("Chose Path", chosePath, "");
        UnityEngine.Debug.Log(chosePath);
        PlayerPrefs.SetString("path", chosePath);
        PlayerPrefs.Save();
    }

    void renameClick1()
    {
        string[] dirs = Directory.GetDirectories(chosePath);
        string newPath = Path.Combine(chosePath, "koutu");
        if (!Directory.Exists(newPath))
        {
            Directory.CreateDirectory(newPath);
        }
        for (int i = 0; i < dirs.Length; i++)
        {
            string dir = dirs[i];
            string[] files = Directory.GetFiles(dir);
            for (int j = 0; j < files.Length; j++)
            {
                UnityEngine.Debug.Log(files[j]);

                if (files[j].IndexOf("消费者上传原图") >= 0)
                {
                    string str = Path.GetDirectoryName(files[j]);
                    int index = str.LastIndexOf('-');
                    string str1 = str.Substring(index);
                    string[] arr = str1.Split('-');
                    string ext = arr[1] + Path.GetExtension(files[j]);
                    File.Copy(files[j], Path.Combine(newPath, ext));
                }
                //
            }
        }
    }

    void renameClick()
    {
        string[] dirs = Directory.GetDirectories(chosePath);

        string[] csvs = Directory.GetFiles(chosePath, "*.csv");
        if(csvs.Length != 1)
        {
            UnityEngine.Debug.LogError("csv number is not valid:" + csvs.Length);
            return;
        }

        string[] lines = File.ReadAllLines(csvs[0]);

        string[] titles = lines[0].Split(',');
        int skuindex = 6;
        int skcindex = 2;

        Dictionary<string, string> sku2skcDic = new Dictionary<string, string>();
        Dictionary<string, int> skcCountDic = new Dictionary<string, int>();

        for (int i = 0; i < titles.Length; i++)
        {
            if(titles[i] == "定制SKU")
            {
                skuindex = i;
            }
            if(titles[i] == "商品SKC ID")
            {
                skcindex = i;
            }
        }

        for(int i = 1; i < lines.Length; i++)
        {
            string[] contents = lines[i].Split(',');
            string sku = contents[skuindex];
            string skc = contents[skcindex];
            sku2skcDic[sku] = skc;
            if(!skcCountDic.ContainsKey(skc))
            {
                skcCountDic[skc] = 0;
            }
            skcCountDic[skc]++;
        }

        string newPath = Path.Combine(chosePath, "koutu");
        if(!Directory.Exists(newPath))
        {
            Directory.CreateDirectory(newPath);
        }
        for(int i = 0; i < dirs.Length; i++)
        {
            string dir = dirs[i];
            string[] files = Directory.GetFiles(dir);
            for(int j = 0; j < files.Length; j++)
            {
                UnityEngine.Debug.Log(files[j]);
                
                if(files[j].IndexOf("消费者上传原图") >= 0)
                {
                    string str = Path.GetDirectoryName(files[j]);
                    int index = str.LastIndexOf('-');
                    string str1 = str.Substring(index);
                    string[] arr = str1.Split('-');
                    string ext = arr[1] + Path.GetExtension(files[j]);

                    string skc = sku2skcDic[arr[1]];

                    string resultPath = Path.Combine(newPath, skc + "-" + skcCountDic[skc]);
                    if (!Directory.Exists(resultPath))
                    {
                        Directory.CreateDirectory(resultPath);
                    }

                    File.Copy(files[j], Path.Combine(resultPath, ext), true);
                }
                //
            }
        }
    }
}
