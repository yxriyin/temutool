using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

public class test : MonoBehaviour
{
    public InputField input;
    public Button unlock;
    public Transform Root;
    public Button path;
    public Button PSDPath;


    public Button rename;
    public Button rename1;
    public Button rename2;
    public Button rename3;
    public Button rename4;

    public Button xuanzejianhuodanBtn;
    public Button tongjihuizongBtn;

    public Button quchuqianzhuiBtn;

    public Button quchuhoushuiBtn;

    public Button fuzhitupianBtn;

    public Button CopyPSDBtn;

    public Button CombineBtn;

    public Button comparePathBtn;
    public Button compareBtn;

    public Button wenzihuizongBtn;

    public Button wannengBtn;

    public Text title;
    public Text timeText;
    string chosePath;
    string comparePath;
    string psdPathStr;
    public Transform Panel;
    string skcTxtPath;
    string jianhuodanPath;
    // Start is called before the first frame update
    public Camera saveRTCamera;

    public Dropdown dropdown;
    public class skcData
    {
        public int skuindex = 6;
        public int skcindex = 2;
        public Dictionary<string, string> sku2skcDic = new Dictionary<string, string>();
        public Dictionary<string, int> skcCountDic = new Dictionary<string, int>();
        public Dictionary<string, string> skc2dealDic = new Dictionary<string, string>();
        public Dictionary<string, string> sku2ageDic = new Dictionary<string, string>();
        public Dictionary<string, int> age2CountDic = new Dictionary<string, int>();
        public Dictionary<string, string> sku2txtDic = new Dictionary<string, string>();
    }

    public class jianhuodanData
    {
        public List<List<string>> skuList = new List<List<string>>();
        public List<string> txtNameList = new List<string>();
    }

    private void Awake()
    {
        string time = PlayerPrefs.GetString("time", "");
        if(time == "")
        {
            PlayerPrefs.SetString("time", DateTime.Now.ToShortDateString());
            unlock.gameObject.SetActive(false);
            return;
        }
        DateTime t = DateTime.Parse(time);
        TimeSpan spane = DateTime.UtcNow.Subtract(t);
        if(spane.Days > 30)
        {
            GameObject.Destroy(Panel.gameObject);
            for(int i = 0; i < 10; i++)
            {
                GameObject.Destroy(Root.GetChild(i).gameObject);
            }
            unlock.gameObject.SetActive(true);
        }
        else
        {
            unlock.gameObject.SetActive(false);
        }
        
    }
    public static string GetMd5Hash(string input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert byte array to a hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
    void Unlock()
    {
        string str = DateTime.UtcNow.ToShortDateString();
        string md5 = GetMd5Hash(str);
        UnityEngine.Debug.Log(md5);
        if(md5.Substring(10, 6).ToLower() == input.text.ToLower())
        {
            PlayerPrefs.SetString("time", DateTime.UtcNow.ToShortDateString());
            UnityEngine.Debug.Log("解锁成功，重新启动程序");
        }
    }

    void Start()
    {
        
        path.onClick.AddListener(PathClick);
        PSDPath.onClick.AddListener(PSDPathClick);
        rename.onClick.AddListener(renameClick);
        rename1.onClick.AddListener(renameClick1);
        rename2.onClick.AddListener(renameClick2);
        rename3.onClick.AddListener(renameClick3);
        rename4.onClick.AddListener(renameClick4);

        xuanzejianhuodanBtn.onClick.AddListener(xuanzejianhuodan);
        tongjihuizongBtn.onClick.AddListener(tongjijianhuodan);

        quchuqianzhuiBtn.onClick.AddListener(quchuqianzui);
        quchuhoushuiBtn.onClick.AddListener(quchuhouzhui);
        fuzhitupianBtn.onClick.AddListener(fuzhitupian);

        unlock.onClick.AddListener(Unlock);

        CopyPSDBtn.onClick.AddListener(CopyPSDClick);

        CombineBtn.onClick.AddListener(CombineAllFileClick);

        comparePathBtn.onClick.AddListener(ComparePathClick);

        compareBtn.onClick.AddListener(compareClick);

        wenzihuizongBtn.onClick.AddListener(wenzihuizongClick);

        wannengBtn.onClick.AddListener(wangnengClick);
        chosePath = getCacheString();
        comparePath = getCompareString();
        psdPathStr = getPSDString();
        skcTxtPath = getCacheTxtPath();
        jianhuodanPath = getCacheJianhuodanPath();
        Panel.gameObject.SetActive(false);
        saveRTCamera.enabled = false;
    }

    void duibiSPU()
    {
        string[] lines = File.ReadAllLines(jianhuodanPath);
        List<string> spusuc = new List<string>();
        List<string> spuall = new List<string>();
        for (int i = 0; i < lines.Length; i++)
        {
            string[] arr = lines[i].Split(',');
            if(arr[0] != "")
            {
                spusuc.Add(arr[0]);
            }
            if(arr[1] != "")
            {
                spuall.Add(arr[1]);
            }
        }

        List<string> result = new List<string>();
        for(int i = 0; i < spuall.Count; i++)
        {
            if(spusuc.Contains(spuall[i]))
            {
                continue;
            }
            result.Add(spuall[i]);
        }


        string str = "";
        for(int i = 0; i < result.Count; i++)
        {
            str += result[i] + " ";
        }

        Debug.Log(str);
        File.WriteAllText(@"C:\Users\yxriy\Desktop\1.txt", str);
    }

    void wangnengClick()
    {
        duibiSPU();
        //csvfilterfiles();
    }

    void csvfilterfiles()
    {
        string csvFileName = Path.Combine(chosePath, "PSD挑选.csv");
        string[] arr = File.ReadAllLines(csvFileName);

        string[] files = Directory.GetFiles(chosePath);
        string newDir = Path.Combine(chosePath, "newPSD");
        createDir(newDir);
        for(int i = 1; i < arr.Length; i++)
        {
            string skc = arr[i];
            for(int j = 0; j < files.Length; j++)
            {
                if(files[j].IndexOf(skc) >= 0)
                {
                    string filename = Path.GetFileName(files[j]);
                    File.Copy(files[j], Path.Combine(newDir, filename), true);
                }
            }
        }
    }
    void files2skcList()
    {
        string[] files = Directory.GetFiles(chosePath);
        List<string> result = new List<string>();
        result.Add("skc");
        for(int i = 0; i < files.Length; i++)
        {
            string skc = Path.GetFileNameWithoutExtension(files[i]);
            result.Add(skc);
        }

        File.WriteAllLines(Path.Combine(chosePath, "skc.csv"), result);
    }

    string getCacheString()
    {
        return PlayerPrefs.GetString("path");
    }

    string getCompareString()
    {
        return PlayerPrefs.GetString("comparePath");
    }

    string getPSDString()
    {
        return PlayerPrefs.GetString("psdPath");
    }

    string getCacheTxtPath()
    {
        return PlayerPrefs.GetString("txtpath");
    }


    string getCacheJianhuodanPath()
    {
        return PlayerPrefs.GetString("jianhuodanpath");
    }

    void CopyPSDClick()
    {
        string[] dirs = Directory.GetDirectories(chosePath);
        string[] files = Directory.GetFiles(psdPathStr);
        for(int i = 0; i < dirs.Length; i++)
        {
            string[] arr = dirs[i].Split('_');
            if(arr.Length <= 0)
            {
                continue;
            }

            string skcWithPath = arr[0];
            string skc = Path.GetFileName(skcWithPath);
            for(int j = 0; j < files.Length; j++)
            {
                if(files[j].IndexOf(skc) >= 0)
                {
                    //UnityEngine.Debug.LogError(files[j]);
                    string newFileName = Path.GetFileName(files[j]);
                    File.Copy(files[j], Path.Combine(dirs[i], newFileName), true);
                    break;
                }
            }
        }
    }

    void CombineAllFileClick()
    {
        createDir("piliangkoutu");
        string[] dirs = Directory.GetDirectories(chosePath);
        string combinePath = Path.Combine(chosePath, "piliangkoutu");
        for(int i = 0; i < dirs.Length; i++)
        {
            if(dirs[i].Contains("piliangkoutu"))
            {
                continue;
            }
            string[] files = Directory.GetFiles(dirs[i]);
            for(int j = 0; j < files.Length; j++)
            {
                string newFileName = Path.GetFileName(files[j]);
                File.Copy(files[j], Path.Combine(combinePath, newFileName), true);
            }
            
        }
    }

    void xuanzejianhuodan()
    {
        jianhuodanPath = EditorUtility.OpenFilePanel("Chose Path", jianhuodanPath, "");
        UnityEngine.Debug.Log(jianhuodanPath);
        PlayerPrefs.SetString("jianhuodanpath", jianhuodanPath);
        PlayerPrefs.Save();
    }

    void PathClick()
    {
        chosePath = EditorUtility.OpenFolderPanel("Chose Path", chosePath, "");
        UnityEngine.Debug.Log(chosePath);
        PlayerPrefs.SetString("path", chosePath);
        PlayerPrefs.Save();
    }

    void ComparePathClick()
    {
        comparePath = EditorUtility.OpenFolderPanel("Chose Path", comparePath, "");
        UnityEngine.Debug.Log(comparePath);
        PlayerPrefs.SetString("comparePath", comparePath);
        PlayerPrefs.Save();
    }

    void PSDPathClick()
    {
        psdPathStr = EditorUtility.OpenFolderPanel("Chose Path", psdPathStr, "");
        UnityEngine.Debug.Log(psdPathStr);
        PlayerPrefs.SetString("psdPath", psdPathStr);
        PlayerPrefs.Save();
    }

    public static bool IsNumber(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return false;
        const string pattern = "^[0-9]*$";
        Regex rx = new Regex(pattern);
        return rx.IsMatch(s);
    }


    void compareClick()
    {
        string[] files1 = Directory.GetFiles(chosePath);
        string[] files2 = Directory.GetFiles(comparePath);

        List<string> originInValidList = new List<string>();
        List<string> targetInValidList = new List<string>();
        List<string> originValidDic = new List<string>();
        List<string> targetValidDic = new List<string>();
        for (int i = 0; i < files1.Length; i++)
        {
            string filename = Path.GetFileName(files1[i]);
            if(filename.IndexOf("DS_Store") >= 0 || filename.EndsWith(".txt"))
            {
                continue;
            }
            string pattern = @"^\d+\.png$";
            Regex rx = new Regex(pattern);
            if(!rx.IsMatch(filename))
            {
                originInValidList.Add(filename);
            }
            else
            {
                string[] arr = filename.Split('.');
                string sku = arr[0];
                originValidDic.Add(sku);
            }
        }

        for (int i = 0; i < files2.Length; i++)
        {
            string filename = Path.GetFileName(files2[i]);
            if (filename.IndexOf("DS_Store") >= 0)
            {
                continue;
            }
            string pattern = @"^\d+(\[.*\])?(_\d{1,3})?(\[.*\])?\.png";
            Regex rx = new Regex(pattern);
            if (!rx.IsMatch(filename))
            {
                targetInValidList.Add(filename);
            }
            else
            {
                string[] arr = filename.Split('.');
                string sku = arr[0];
                targetValidDic.Add(sku);
            }
        }

        for(int i = 0; i < originValidDic.Count; i++)
        {
            string originSku = originValidDic[i];
            bool flag = false;
            for(int j = 0; j < targetValidDic.Count; j++)
            {
                string targetSku = targetValidDic[j];
                if(targetSku.IndexOf(originSku) == 0)
                {
                    targetValidDic.RemoveAt(j);
                    flag = true;
                    break;
                }
            }
            if(flag == true)
            {
                originValidDic.RemoveAt(i);
                i--;
            }
        }

        List<string> result = new List<string>();
        string txt = "原始图命名不规范：";
        for(int i = 0; i < originInValidList.Count; i++)
        {
            txt += originInValidList[i] + "   ";
        }
        result.Add(txt);
        txt = "设计图命名不规范：";
        for (int i = 0; i < targetInValidList.Count; i++)
        {
            txt += targetInValidList[i] + "   ";
        }
        result.Add(txt);
        txt = "原始图多余：";
        for (int i = 0; i < originValidDic.Count; i++)
        {
            txt += originValidDic[i] + "   ";
        }
        result.Add(txt);
        txt = "设计图多余：";
        for (int i = 0; i < targetValidDic.Count; i++)
        {
            txt += targetValidDic[i] + "   ";
        }
        result.Add(txt);
        for(int i = 0; i < result.Count; i++)
        {
            UnityEngine.Debug.Log(result[i]);
        }
        File.WriteAllLines(Path.Combine(chosePath, "对比结果.txt"), result);
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
        int allImgCount = 0;


        for (int i = 0; i < list.Count; i++)
        {
            List<string> skulist = list[i];
            allImgCount = (skulist.Count + itemList.Length - 1) / itemList.Length;
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
                    title.text = data.txtNameList[i] + "_" + imgCount.ToString() + "/" + allImgCount.ToString() + "页";
                    timeText.text = DateTime.UtcNow.ToShortDateString();
                    saveImgPng(data.txtNameList[i], j + 1);
                    yield return new WaitForSeconds(1);
                    clearItem(itemList);
                }
            }
            if (count != 0)
            {
                count = 0;
                imgCount++;
                title.text = data.txtNameList[i] + "_" + imgCount.ToString() + "/" + allImgCount.ToString() + "页";
                saveImgPng(data.txtNameList[i], skulist.Count);
            }
            yield return new WaitForSeconds(1);
            imgCount = 0;
            allImgCount = 0;
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

    void wenzihuizongClick()
    {
        string[] numstr = { "一", "二", "三", "四", "五", "六", "七", "八", "九" };
        string[] lines = File.ReadAllLines(jianhuodanPath);
        string title = "定制SKU,图片路径";

        string[] titles = lines[0].Split(',');
        int dingzhiIndex = 9;
        int skcIndex = 2;
        int skuIndex = 8;
        for (int i = 0; i < titles.Length; i++)
        {
            if (titles[i] == "定制区域")
            {
                dingzhiIndex = i;
            }

            if (titles[i] == "商品SKC ID")
            {
                skcIndex = i;
            }

            if (titles[i] == "定制SKU")
            {
                skuIndex = i;
            }
        }

        for (int i = 1; i <= 9; i++)
        {
            title += "," + "定制区域" + numstr[i - 1];
        }

        int index = 1;
        Dictionary<int, List<string>> dic = new Dictionary<int, List<string>>();
        string preLineStr = "";
        for(int i = 1; i < lines.Length; i++)
        {
            string[] content = lines[i].Split(',');
            if(content[0] == "")
            {
                List<string> list = new List<string>();
                if(dic.ContainsKey(index))
                {
                    list = dic[index];
                }
                else
                {
                    dic[index] = list;
                }

                if(preLineStr != "")
                {
                    list.Add(preLineStr);
                    preLineStr = "";
                }

                list.Add(content[dingzhiIndex + 1]);
            }
            else
            {
                index = i;
                preLineStr = content[dingzhiIndex + 1];
            }
        }

        Dictionary<string, List<string>> allSkcDic = new Dictionary<string, List<string>>();
        
        for (int i = 1; i < lines.Length; i++)
        {
            if(dic.ContainsKey(i))
            {
                List<string> list = dic[i];
                string[] arr = lines[i].Split(',');
                string sku = arr[skuIndex];
                string skc = arr[skcIndex];
                List<string> newResult = new List<string>();
                if (!allSkcDic.ContainsKey(skc))
                {
                    newResult.Add(title);
                    allSkcDic[skc] = newResult;
                }
                else
                {
                    newResult = allSkcDic[skc];
                }

                string newStr = sku;
                string filePath = dropdown.options[dropdown.value].text;
                filePath = Path.Combine(filePath, skc);
                filePath = filePath + "/" + sku + ".png";
                newStr += "," + filePath;
                for (int j = 0; j < list.Count; j++)
                {
                    newStr += "," + list[j];
                }

                for(int j = list.Count; j < 9; j++)
                {
                    newStr += ",";
                }
                newResult.Add(newStr);
            }
        }

        
        foreach(var skc in allSkcDic.Keys)
        {
            string dir = Path.GetDirectoryName(jianhuodanPath);
            string newPath = skc + "_custom_text.csv";
            string newDir = Path.Combine(dir, newPath);
            File.WriteAllLines(newDir, allSkcDic[skc].ToArray());
        }
        
    }

    void fuzhitupian()
    {
        string[] lines = File.ReadAllLines(jianhuodanPath);
        int skuIndex = 1;
        int numIndex = 3;


        string[] titles = lines[0].Split(',');
        for (int i = 0; i < titles.Length; i++)
        {
            if (titles[i] == "定制SKU")
            {
                skuIndex = i;
            }
            if (titles[i] == "数量")
            {
                numIndex = i;
            }
        }

        string dir = Path.GetDirectoryName(jianhuodanPath);
        string[] files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);


        for (int i = 1; i < lines.Length; i++)
        {
            string[] arr = lines[i].Split(',');
            UnityEngine.Debug.Log(i);
            if(int.Parse(arr[numIndex]) > 1)
            {
                int copyNum = int.Parse(arr[numIndex]) - 1;
                string sku = arr[skuIndex];
                for(int j = 0; j < files.Length; j++)
                {
                    if(files[j].IndexOf(sku) >= 0)
                    {
                        string newDir = Path.GetDirectoryName(files[j]);
                        string newPath = Path.GetFileNameWithoutExtension(files[j]);
                        string ext = Path.GetExtension(files[j]);
                        for(int k = 0; k < copyNum; k++)
                        {
                            File.Copy(files[j], Path.Combine(newDir, newPath + "_" + k + ext), true);
                        }
                       
                    }
                }
            }
        }
    }

    void renameClick3()
    {
        skcTxtPath = EditorUtility.OpenFolderPanel("Chose Path", skcTxtPath, "");
        UnityEngine.Debug.Log(skcTxtPath);
        PlayerPrefs.SetString("txtpath", skcTxtPath);
        PlayerPrefs.Save();

    }

    void tongjijianhuodan()
    {
        string[] arr = File.ReadAllLines(jianhuodanPath);
        int beihuoindex = 0;
        int numindex = 3;
        string[] titles = arr[0].Split(',');
        for(int i = 0; i < titles.Length; i++)
        {
            if(titles[i] == "备货单号")
            {
                beihuoindex = i;
            }

            if(titles[i] == "数量")
            {
                numindex = i;
            }
        }

        Dictionary<string, int> huizongDic = new Dictionary<string, int>();

        for(int i = 1; i < arr.Length; i++)
        {
            string[] infos = arr[i].Split(',');
            string beihuodan = infos[beihuoindex];
            int num = int.Parse(infos[numindex]);
            if(!huizongDic.ContainsKey(beihuodan))
            {
                huizongDic[beihuodan] = 0;
            }

            huizongDic[beihuodan] += num;
        }

        string dir = Path.GetDirectoryName(jianhuodanPath);
        List<string> saveArr = new List<string>();
        string title = "备货单号,件数";
        saveArr.Add(title);

        foreach(var key in huizongDic.Keys)
        {
            string result = key + "," + huizongDic[key];
            saveArr.Add(result);
        }

        File.WriteAllLines(Path.Combine(dir, "拣货单汇总.csv"), saveArr.ToArray());

    }

    void quchuhouzhui()
    {
        string[] files = Directory.GetFiles(chosePath);
        createDir("quhouzui");
        for (int i = 0; i < files.Length; i++)
        {
            string filename = Path.GetFileName(files[i]);
            string[] arr = filename.Split('_');
            string newFileName = arr[0] + Path.GetExtension(filename);
            
            if(arr.Length <= 1)
            {
                newFileName = arr[0];
            }


            string dir = Path.GetDirectoryName(files[i]);
            dir = Path.Combine(dir, "quhouzui");
            File.Copy(files[i], Path.Combine(dir, newFileName), true);
        }
    }

    void quchuqianzui()
    {
        string[] files = Directory.GetFiles(chosePath);
        for(int i = 0; i < files.Length; i++)
        {
            string filename = Path.GetFileName(files[i]);
            string[] arr = filename.Split('_');
            int skuIndex = -1;
            int maxLen = -1;
            for (int j = 0; j < arr.Length; j++)
            {
                if(arr[j].Length > maxLen)
                {
                    maxLen = arr[j].Length;
                    skuIndex = j;
                }
            }
            string newFileName = arr[skuIndex];
            if(arr.Length > skuIndex + 1)
            {
                newFileName += "_" + arr[skuIndex + 1];
            }
            string dir = Path.GetDirectoryName(files[i]);
            File.Move(files[i], Path.Combine(dir, newFileName));
        }
    }


    string getFileNameWithTxt(string fileName, string sku, skcData data)
    {
        Dictionary<string, string> sku2TxtDic = data.sku2txtDic;
        string txt = "";
        if (sku2TxtDic.ContainsKey(sku))
        {
            txt = sku2TxtDic[sku];
        }
        string newFileName = fileName;
        if(txt != "")
        {
            string filenamewithoutext = Path.GetFileNameWithoutExtension(fileName);
            newFileName = filenamewithoutext + "[" + txt + "]" + Path.GetExtension(fileName);
        }

        return newFileName;
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
                string newFileName = getFileNameWithTxt(Path.GetFileName(files[j]), sku, data);
                fenlei(skc, sku, data, savePath, files[j], newFileName);
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
                        if (!data.sku2skcDic.ContainsKey(sku))
                        {
                            UnityEngine.Debug.LogError("sku do not exists:" + sku);
                            continue;
                        }
                        
                        string skc = data.sku2skcDic[sku];
                        string newFileName = getFileNameWithTxt(Path.GetFileName(files1[j]), sku, data);
                        fenlei(skc, sku, data, savePath, files1[j], newFileName);
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
                    if(ext.ToLower().IndexOf("png") >= 0)
                    {
                        File.Copy(files[j], Path.Combine(newPath, ext), true);
                    }
                    else
                    {
                        byte[] srcBytes = File.ReadAllBytes(files[j]);
                        Texture2D temp = new Texture2D(0, 0);
                        temp.LoadImage(srcBytes);
                        byte[] dstBytes = temp.EncodeToPNG();
                        string newFileName = arr[1];
                        newFileName += ".png";
                        File.WriteAllBytes(Path.Combine(newPath, newFileName), dstBytes);
                    }
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

        bool hasSkcDealCSV = false;
        int useIndex = 0;
        int checkNum = 1;

        Dictionary<string, string> skc2DealDic = new Dictionary<string, string>();
        for (int i = 0; i < csvs.Length; i++)
        {
            if(csvs[i].IndexOf("SKC_图片处理方式") >= 0)
            {
                hasSkcDealCSV = true;
                checkNum = 2;
                if(i == 0)
                {
                    useIndex = 1;
                }

                string[] skcdealArr = File.ReadAllLines(csvs[i]);
                int skcIndex = 1;
                int dealIndex = 2;
                for(int j = 1; j < skcdealArr.Length; j++)
                {
                    string[] arr1 = skcdealArr[j].Split(',');
                    skc2DealDic[arr1[skcIndex]] = arr1[dealIndex];
                }

                break;
            }
        }

        

        if (csvs.Length != checkNum)
        {
            UnityEngine.Debug.LogError("csv number is not valid:" + csvs.Length);
            return null;
        }

        string[] lines = File.ReadAllLines(csvs[useIndex]);

        string[] titles = lines[0].Split(',');
        int skuindex = 6;
        int skcindex = 2;
        int agesIndex = 5;
        int txtIndex = 8;
        Dictionary<string, string> sku2skcDic = new Dictionary<string, string>();
        Dictionary<string, int> skcCountDic = new Dictionary<string, int>();
        Dictionary<string, string> sku2ageDic = new Dictionary<string, string>();
        Dictionary<string, int> age2CountDic = new Dictionary<string, int>();
        Dictionary<string, string> sku2TxtDic = new Dictionary<string, string>();
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
            if(titles[i] == "商品属性集")
            {
                agesIndex = i;
            }
            if(titles[i] == "文字内容")
            {
                txtIndex = i;
            }
        }

        Dictionary<int, string> hasTxtDic = new Dictionary<int, string>();
        for (int i = 1; i < lines.Length; i++)
        {
            string[] contents = lines[i].Split(',');
            string txt = contents[txtIndex];
            if(txt == @"\")
            {
                continue;
            }
            hasTxtDic[i - 1] = txt;
        }

        for (int i = 1; i < lines.Length; i++)
        {
            string[] contents = lines[i].Split(',');
            string sku = contents[skuindex];
            string skc = contents[skcindex];
            if(sku == "")
            {
                continue;
            }
            if(hasTxtDic.ContainsKey(i))
            {
                sku2TxtDic[sku] = hasTxtDic[i];
            }
            sku2skcDic[sku] = skc;
            if(contents[agesIndex].IndexOf('-') >= 0)
            {
                string ageInfo = contents[agesIndex];
                sku2ageDic[sku] = ageInfo;
                string skcAndAgeInfo = skc + ageInfo;
                if(!age2CountDic.ContainsKey(skcAndAgeInfo))
                {
                    age2CountDic[skcAndAgeInfo] = 0;
                }
                age2CountDic[skcAndAgeInfo]++;
            }
            else
            {
                sku2ageDic[sku] = "";
            }
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
        data.skc2dealDic = skc2DealDic;
        data.sku2ageDic = sku2ageDic;
        data.age2CountDic = age2CountDic;
        data.sku2txtDic = sku2TxtDic;
        return data;
    }

    void fenlei(string skc, string sku, skcData data, string savepath, string filenameFullPath, string fileWithExt)
    {
        string newPath = skc + "_" + data.skcCountDic[skc];
        if (data.skc2dealDic.ContainsKey(skc))
        {
            newPath += "_" + data.skc2dealDic[skc];
        }
        string resultPath = Path.Combine(savepath, newPath);
        if (!Directory.Exists(resultPath))
        {
            Directory.CreateDirectory(resultPath);
        }

        string ageInfo = data.sku2ageDic[sku];
        if(ageInfo != "")
        {
            int count = 0;
            string skcAndAgeInfo = skc + ageInfo;
            if(data.age2CountDic.ContainsKey(skcAndAgeInfo))
            {
                count = data.age2CountDic[skcAndAgeInfo];
            }
            string agePath = Path.Combine(resultPath, ageInfo + "-" + count);
            if (!Directory.Exists(agePath))
            {
                Directory.CreateDirectory(agePath);
            }

            File.Copy(filenameFullPath, Path.Combine(agePath, fileWithExt), true);
        }
        else
        {
            File.Copy(filenameFullPath, Path.Combine(resultPath, fileWithExt), true);
        }

        
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
                    string sku = arr[1];
                    string skc = data.sku2skcDic[sku];

                    fenlei(skc, sku, data, newPath, files[j], ext);
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
