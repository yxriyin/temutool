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
    public Button rename3;
    public Button rename4;
    public Text title;
    string chosePath;
    public Transform Panel;
    string skcTxtPath;
    // Start is called before the first frame update
    public Camera saveRTCamera;
    public class skcData
    {
        public int skuindex = 6;
        public int skcindex = 2;
        public Dictionary<string, string> sku2skcDic = new Dictionary<string, string>();
        public Dictionary<string, int> skcCountDic = new Dictionary<string, int>();
    }

    public class jianhuodanData
    {
        public List<List<string>> skuList = new List<List<string>>();
        public List<string> txtNameList = new List<string>();
    }

    void Start()
    {
        
        path.onClick.AddListener(PathClick);
        rename.onClick.AddListener(renameClick);
        rename1.onClick.AddListener(renameClick1);
        rename2.onClick.AddListener(renameClick2);
        rename3.onClick.AddListener(renameClick3);
        rename4.onClick.AddListener(renameClick4);
        chosePath = getCacheString();
        skcTxtPath = getCacheTxtPath();
        Panel.gameObject.SetActive(false);
        saveRTCamera.enabled = false;
    }

    string getCacheString()
    {
        return PlayerPrefs.GetString("path");
    }

    string getCacheTxtPath()
    {
        return PlayerPrefs.GetString("txtpath");
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

    void renameClick4()
    {
        Panel.gameObject.SetActive(true);
        StartCoroutine(delaySaveImg());
    }

    void saveImgPng(string txtName, int index)
    {
        string name = txtName + "_" + index;
        saveRTCamera.enabled = true;
        saveRTCamera.Render();
        SaveRenderTextureToPNG(name, saveRTCamera.targetTexture, skcTxtPath);
        saveRTCamera.enabled = false;
    }

    IEnumerator delaySaveImg()
    {
        item[] itemList = Panel.GetComponentsInChildren<item>();
        jianhuodanData data = filljianhuodanData();
        List<List<string>> list = data.skuList;
        clearItem(itemList);
        int count = 0;
        int imgCount = 0;
        for (int i = 0; i < list.Count; i++)
        {
            List<string> skulist = list[i];
            
            for (int j = 0; j < skulist.Count; j++)
            {
                string sku = skulist[j];
                string path = findPath(sku);
                int index = j % itemList.Length;
                itemList[index].setData((j + 1).ToString(), sku, path);
                count++;
                if (count == itemList.Length)
                {
                    count = 0;
                    imgCount++;
                    title.text = data.txtNameList[i] + "_" + imgCount.ToString() + "页";
                    saveImgPng(data.txtNameList[i], j + 1);
                    yield return new WaitForSeconds(1);
                    clearItem(itemList);
                }
            }
            if (count != 0)
            {
                count = 0;
                imgCount++;
                title.text = data.txtNameList[i] + "_" + imgCount.ToString() + "页";
                saveImgPng(data.txtNameList[i], skulist.Count);
            }
            yield return new WaitForSeconds(1);
            imgCount = 0;
            clearItem(itemList);
        }
    }

    void clearItem(item[] itemList)
    {
        for(int i = 0; i < itemList.Length; i++)
        {
            itemList[i].clear();
        }
    }

    string findPath(string sku)
    {
        string[] files = Directory.GetFiles(chosePath);
        for(int i = 0; i < files.Length; i++)
        {
            if(files[i].IndexOf(sku) >= 0)
            {
                return files[i];
            }
        }

        return "";
    }

    void renameClick3()
    {
        skcTxtPath = EditorUtility.OpenFolderPanel("Chose Path", skcTxtPath, "");
        UnityEngine.Debug.Log(skcTxtPath);
        PlayerPrefs.SetString("txtpath", skcTxtPath);
        PlayerPrefs.Save();

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
                if(!data.sku2skcDic.ContainsKey(sku))
                {
                    continue;
                }
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
                    File.Copy(files[j], Path.Combine(newPath, ext), true);
                }
                //
            }
        }
    }

    jianhuodanData filljianhuodanData()
    {
        jianhuodanData data = new jianhuodanData();
        string[] dirs = Directory.GetDirectories(skcTxtPath);

        string[] csvs = Directory.GetFiles(skcTxtPath, "*.txt");
        if (csvs.Length < 1)
        {
            UnityEngine.Debug.LogError("txt number is not valid:" + csvs.Length);
            return null;
        }
        List<List<string>> skuList = new List<List<string>>();

        List<string> txtNameList = new List<string>();
        for(int j = 0; j < csvs.Length; j++)
        {
            string txtName = Path.GetFileNameWithoutExtension(csvs[j]);
            txtNameList.Add(txtName);
            string[] lines = File.ReadAllLines(csvs[j]);
            List<string> list = new List<string>();
            for (int i = lines.Length - 1; i >= 0; i--)
            {
                if (lines[i].IndexOf("DZ") == 0)
                {
                    string[] arr = lines[i].Split(' ');
                    list.Add(arr[0].Substring(2));
                }
            }

            skuList.Add(list);
        }
       

        data.skuList = skuList;
        data.txtNameList = txtNameList;
        return data;
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

    private void SaveRenderTextureToPNG(string textureName, RenderTexture renderTexture, string savePath)
    {
        string path = Path.Combine(savePath, textureName + ".png");
        if (path.Length != 0)
        {
            var newTex = new Texture2D(renderTexture.width, renderTexture.height);
            RenderTexture.active = renderTexture;
            newTex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            newTex.Apply();

            byte[] pngData = newTex.EncodeToPNG();
            if (pngData != null)
            {
                File.WriteAllBytes(path, pngData);
            }
            Debug.Log(path);
        }
    }

}
