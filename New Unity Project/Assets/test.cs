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
    public Button rename2;
    string chosePath;
    // Start is called before the first frame update

    public class skcData
    {
        public int skuindex = 6;
        public int skcindex = 2;
        public Dictionary<string, string> sku2skcDic = new Dictionary<string, string>();
        public Dictionary<string, int> skcCountDic = new Dictionary<string, int>();
    }

    void Start()
    {
        path.onClick.AddListener(PathClick);
        rename.onClick.AddListener(renameClick);
        rename1.onClick.AddListener(renameClick1);
        rename2.onClick.AddListener(renameClick2);
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

    void createDir(string str)
    {
        string[] dirs = Directory.GetDirectories(chosePath);
        string newPath = Path.Combine(chosePath, str);
        if (!Directory.Exists(newPath))
        {
            Directory.CreateDirectory(newPath);
        }
    }

    void renameClick2()
    {
        createDir("dingzhitupian");
        skcData data = fillSkcData();
        string savePath = Path.Combine(chosePath, "dingzhitupian");
        string koutupath = Path.Combine(chosePath, "koutu");
        string[] files = Directory.GetFiles(koutupath);
        for (int j = 0; j < files.Length; j++)
        {
            {
                string sku = Path.GetFileNameWithoutExtension(files[j]);
                string skc = data.sku2skcDic[sku];
                fenlei(skc, data, savePath, files[j], Path.GetFileName(files[j]));
            }
            //
        }



        string[] dirs = Directory.GetDirectories(chosePath);
        for(int i = 0; i < dirs.Length; i++)
        {
            string dir = dirs[i];
            if(dir.IndexOf("piliangkoutu") >= 0)
            {
                string piliangpath = dir;
                string[] files1 = Directory.GetFiles(piliangpath);
                for (int j = 0; j < files1.Length; j++)
                {
                    {
                        string filename = Path.GetFileNameWithoutExtension(files1[j]);
                        string[] arr = filename.Split('_');
                        if(arr.Length <= 1)
                        {
                            continue;
                        }
                        string sku = arr[0];
                        string skc = data.sku2skcDic[sku];
                        fenlei(skc, data, savePath, files1[j], Path.GetFileName(files1[j]));
                    }
                    //
                }
                break;
            }
        }
    }

    void renameClick1()
    {
        createDir("koutu");
        string[] dirs = Directory.GetDirectories(chosePath);
        string newPath = Path.Combine(chosePath, "koutu");
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

    skcData fillSkcData()
    {
        skcData data = new skcData();
        string[] dirs = Directory.GetDirectories(chosePath);

        string[] csvs = Directory.GetFiles(chosePath, "*.csv");
        if (csvs.Length != 1)
        {
            UnityEngine.Debug.LogError("csv number is not valid:" + csvs.Length);
            return null;
        }

        string[] lines = File.ReadAllLines(csvs[0]);

        string[] titles = lines[0].Split(',');
        int skuindex = 6;
        int skcindex = 2;

        Dictionary<string, string> sku2skcDic = new Dictionary<string, string>();
        Dictionary<string, int> skcCountDic = new Dictionary<string, int>();

        for (int i = 0; i < titles.Length; i++)
        {
            if (titles[i] == "定制SKU")
            {
                skuindex = i;
            }
            if (titles[i] == "商品SKC ID")
            {
                skcindex = i;
            }
        }

        for (int i = 1; i < lines.Length; i++)
        {
            string[] contents = lines[i].Split(',');
            string sku = contents[skuindex];
            string skc = contents[skcindex];
            sku2skcDic[sku] = skc;
            if (!skcCountDic.ContainsKey(skc))
            {
                skcCountDic[skc] = 0;
            }
            skcCountDic[skc]++;
        }

        data.skcindex = skcindex;
        data.skuindex = skuindex;
        data.skcCountDic = skcCountDic;
        data.sku2skcDic = sku2skcDic;
        return data;
    }

    void fenlei(string skc, skcData data, string savepath, string fileNameFullPath, string fileWithExt)
    {
        string resultPath = Path.Combine(savepath, skc + "-" + data.skcCountDic[skc]);
        if (!Directory.Exists(resultPath))
        {
            Directory.CreateDirectory(resultPath);
        }

        File.Copy(fileNameFullPath, Path.Combine(resultPath, fileWithExt), true);
    }

    void renameClick()
    {
        skcData data = fillSkcData();
        string[] dirs = Directory.GetDirectories(chosePath);
        createDir("koutu");
        string newPath = Path.Combine(chosePath, "koutu");
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

                    string skc = data.sku2skcDic[arr[1]];

                    fenlei(skc, data, newPath, files[j], ext);
                }
                //
            }
        }
    }
}
