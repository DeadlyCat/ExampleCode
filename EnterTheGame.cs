using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;

public class EnterTheGame : MonoBehaviour
{
    public GameObject loading;
    [SerializeField]GameObject Form;
    [SerializeField] GameObject FormREG;
    [SerializeField] GameObject FormENTER;
    [SerializeField] GameObject ButtonPlay;
    [SerializeField] InputField[] InputEnter; // 0 - login 1-password
    [SerializeField] InputField[] InputReg; // 0 -login 1 -paswwrod 2 -confirmpassword
    [SerializeField] Text Error;
    [SerializeField] Image panel;
    string loginError;
    string PasswordError;
    [SerializeField] Text[] RegError;
    [SerializeField] Text[] EnterError;
    string account = "";
    string res;
   
    string Link = "https://animedance.ru/"; 
    private char[] allsym = new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'z', 'x', 'c', 'v', 'b', 'n', 'm', 'Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P', 'A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L', 'Z', 'X', 'C', 'V', 'B', 'N', 'M' };
    void Start()
    {
        
        account = "";
        panel.gameObject.SetActive(false);
        Error.text = "";
        loginError = "";
        PasswordError = "";
       
        for (int i =0; i < RegError.Length; i++)
        {
            RegError[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < EnterError.Length; i++)
        {
            EnterError[i].gameObject.SetActive(false);
        }
        Form.SetActive(false);
        FormREG.SetActive(false);
        FormENTER.SetActive(false);
        ButtonPlay.SetActive(true);

    }
    public void ClickPlay()
    {
        
        if (!PlayerPrefs.HasKey("Account") || PlayerPrefs.GetString("Account") == "")
        {
            panel.gameObject.SetActive(true);
            Form.SetActive(true);
            ButtonPlay.SetActive(false);
            
        }
        else
        {
            account = PlayerPrefs.GetString("Account");
            string login;
            string password;
            string[] words = account.Split(new char[] { ',' });
            login = words[0];
            password = words[1];
            StartCoroutine(SendStatys(login, password));
            
        }
    }
    public void ClickReg()
    {
        Form.SetActive(false);
        FormREG.SetActive(true); 
    }
    public void ClickEnter()
    {
        Form.SetActive(false);
        FormENTER.SetActive(true); 
    }
    public void ClickBack()
    {
        Form.SetActive(true);
        FormENTER.SetActive(false);
        FormREG.SetActive(false);
        for (int i = 0; i < RegError.Length; i++)
        {
            RegError[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < EnterError.Length; i++)
        {
            EnterError[i].gameObject.SetActive(false);
        }
    }
    public void SendRegForm()
    {
        loginError = "";
        PasswordError = "";
       
        
        for (int i = 0; i < InputReg.Length; i++)
        {
            
                if(InputReg[i].text !="")
            {
                
                   char[] sym = InputReg[i].text.ToCharArray();
                   if(sym.Length < 4 && InputReg[i].tag =="Login")
                     {
                        RegError[i].gameObject.SetActive(true);
                        break;
                       
                     }
                   if (sym.Length < 6 && InputReg[i].tag != "Login")
                    {
                        RegError[i].gameObject.SetActive(true);
                        break;

                    }

                int count = 0;
                    for (int s = 0; s < sym.Length; s++)
                    {
                        for (int a = 0; a < allsym.Length; a++)
                        {

                            if (sym[s] == allsym[a])
                            {
                                count++;
                            }

                        }
                    }
                    if (count != sym.Length)
                    {
                        RegError[i].gameObject.SetActive(true);

                    }
                    else
                    {
                        RegError[i].gameObject.SetActive(false);
                    }
                    if(InputReg[i].tag == "Confim")
                {
                    if(InputReg[i].text != InputReg[i - 1].text)
                    {
                        RegError[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        RegError[i].gameObject.SetActive(false);
                    }
                }
            }
                
            else
            {
                RegError[i].gameObject.SetActive(true);
            }
            
            
        }
        int NoneCount = 0;
        for (int a = 0; a < RegError.Length; a++)
        {
            if (RegError[a].gameObject.activeSelf == true)
            {
                if (a == 0)
                {
                    loginError = "Enter a valid login!!";
                }
                if (a == 1)
                {
                    PasswordError = "Enter a valid password!!";
                    break;
                }
                if (a == 2)
                {
                    PasswordError = "Passwords do not match!!";
                }

            }
            else
            {
                NoneCount++;
            }
        }
        if (NoneCount == RegError.Length)
        {
            
            StartCoroutine(SendReg());
            
            
        }


    }
    IEnumerator SendReg()
    {
        account += InputReg[0].text + "," + InputReg[1].text;
        List<IMultipartFormSection> formReg = new List<IMultipartFormSection>();
        formReg.Add(new MultipartFormDataSection("Login", InputReg[0].text));
        formReg.Add(new MultipartFormDataSection("Password", InputReg[1].text));
        formReg.Add(new MultipartFormDataSection("Statys", "Reg"));
        UnityWebRequest www = UnityWebRequest.Post(Link, formReg);
       // www.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36");
        yield return www.SendWebRequest() ;
        if (!www.isNetworkError || !www.isHttpError)
        {
            
            if(DownloadHandlerBuffer.GetContent(www).Trim() == "thisloginisbusy")
            {
                loginError = "This logis is busy";
                account = "";
            }
            else
            {
                Modules.AddDiamonds(100);
                PlayerPrefs.SetString("Account", account);
                StopCoroutine(SendReg());
                LoadMenu();
            }
          
        }
        
       
       

    }
    
    IEnumerator SendEnter()
    {
        List<IMultipartFormSection> formEnter = new List<IMultipartFormSection>();
        formEnter.Add(new MultipartFormDataSection("Login", InputEnter[0].text));
        formEnter.Add(new MultipartFormDataSection("Password", InputEnter[1].text));
        formEnter.Add(new MultipartFormDataSection("Statys", "Enter"));
        UnityWebRequest www = UnityWebRequest.Post(Link, formEnter);
       // www.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36");
        yield return www.SendWebRequest();
        if (!www.isNetworkError || !www.isHttpError)
        {
            if(DownloadHandlerBuffer.GetContent(www).Trim() == "loginfailed")
            {
                loginError = "incorrect login or password!";
                account = "";
            }
            else
            {
                PlayerPrefs.SetString("Account", account);
                 StopCoroutine(SendEnter());
                LoadMenu();
            }   
        }
        
       
    }
    IEnumerator SendStatys(string login,string password)
    {
        List<IMultipartFormSection> formEnter = new List<IMultipartFormSection>();
        formEnter.Add(new MultipartFormDataSection("Login", login));
        formEnter.Add(new MultipartFormDataSection("Password", password));
        formEnter.Add(new MultipartFormDataSection("Statys", "Enter"));
        UnityWebRequest www = UnityWebRequest.Post(Link, formEnter);
     //   www.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36");
        yield return www.SendWebRequest();
        if (!www.isNetworkError || !www.isHttpError)
        {
            if (DownloadHandlerBuffer.GetContent(www).Trim() == "loginfailed")
            {
                loginError = "incorrect login or password!";
                panel.gameObject.SetActive(true);
                account = "";
                PlayerPrefs.SetString("Account", "");
                Form.SetActive(true);
            }
            else
            {
               
                LoadMenu();
            }
        }
        
        StopCoroutine(SendStatys(login,password));
    }
    public void SendEnterForm()
    {
        loginError = "";
        PasswordError = "";


        for (int i = 0; i < InputEnter.Length; i++)
        {

            if (InputEnter[i].text != "")
            {

                char[] sym = InputEnter[i].text.ToCharArray();
                if (sym.Length < 4 && InputEnter[i].tag == "Login")
                {
                    EnterError[i].gameObject.SetActive(true);
                    break;

                }
                if (sym.Length < 6 && InputEnter[i].tag != "Login")
                {
                    EnterError[i].gameObject.SetActive(true);
                    break;

                }

                int count = 0;
                for (int s = 0; s < sym.Length; s++)
                {
                    for (int a = 0; a < allsym.Length; a++)
                    {

                        if (sym[s] == allsym[a])
                        {
                            count++;
                        }

                    }
                }
                if (count != sym.Length)
                {
                    EnterError[i].gameObject.SetActive(true);

                }
                else
                {
                    EnterError[i].gameObject.SetActive(false);
                }

            }
            else
            {
                EnterError[i].gameObject.SetActive(true);
            }


        }
        int NoneCount = 0;
        for (int a = 0; a < EnterError.Length; a++)
        {
            if (EnterError[a].gameObject.activeSelf == true)
            {
                if (a == 0)
                {
                    loginError = "Enter a valid login!!";
                }
                if (a == 1)
                {
                    PasswordError = "Enter a valid password!!";
                    break;
                }
               

            }
            else
            {
                NoneCount++;
            }
        }
        if (NoneCount == EnterError.Length)
        {
            account += InputEnter[0].text + "," + InputEnter[1].text;
            StartCoroutine(SendEnter());
           

        }

    }
    // Update is called once per frame
    void Update()
    {
        Error.text = loginError + "\n" + PasswordError;
    }
    public void LoadMenu()
    {
        panel.gameObject.SetActive(false);
        Error.text = "";
        loginError = "";
        PasswordError = "";

        for (int i = 0; i < RegError.Length; i++)
        {
            RegError[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < EnterError.Length; i++)
        {
            EnterError[i].gameObject.SetActive(false);
        }
        Form.SetActive(false);
        FormREG.SetActive(false);
        FormENTER.SetActive(false);
        ButtonPlay.SetActive(true);
        loading.SetActive(true);
       
        string account = PlayerPrefs.GetString("Account");
        string[] words = account.Split(new char[] { ',' });
        Debug.Log(PlayerPrefs.GetString("Account")); 
        string login = words[0];
        StartCoroutine(SendCloth(login));

        
    }
    IEnumerator SendCloth(string login)
    {
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("Login", login));
        form.Add(new MultipartFormDataSection("Statys", "CallClocth"));
        UnityWebRequest www = UnityWebRequest.Post(Link, form);
       // www.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36");
        yield return www.SendWebRequest();
        res = DownloadHandlerBuffer.GetContent(www).Trim();

        string[] Cotegories = res.Split(new char[] { '/' });
        string Use = Cotegories[0];
        string HasHats = Cotegories[1];
        string HasHairs = Cotegories[2];
        string HasTops = Cotegories[3];
        string HasBotoms = Cotegories[4];
        string HasShoes = Cotegories[5];
        string HasBody = Cotegories[6];
        string HasEyes = Cotegories[7];
        string[] UseCloth = Use.Split(new char[] { ',' });       
        PlayerPrefs.SetInt("HatsIdUse", Convert.ToInt32(UseCloth[0]));
        PlayerPrefs.SetInt("HairsIdUse", Convert.ToInt32(UseCloth[1]));
        PlayerPrefs.SetInt("TopsIdUse", Convert.ToInt32(UseCloth[2]));
        PlayerPrefs.SetInt("BottomIdUse", Convert.ToInt32(UseCloth[3]));
        PlayerPrefs.SetInt("ShoesIdUse", Convert.ToInt32(UseCloth[4]));
        PlayerPrefs.SetInt("BodyIdUse", Convert.ToInt32(UseCloth[5]));
        PlayerPrefs.SetInt("EyeIdUse", Convert.ToInt32(UseCloth[6]));
        PlayerPrefs.SetString("TotalCountHants", HasHats);
        PlayerPrefs.SetString("TotalCountHairs", HasHairs);
        PlayerPrefs.SetString("TotalCountTops", HasTops);
        PlayerPrefs.SetString("TotalCountBottoms", HasBotoms);
        PlayerPrefs.SetString("TotalCountShoes", HasShoes);
        PlayerPrefs.SetString("TotalCountBody", HasBody);
        PlayerPrefs.SetString("TotalCountEyes", HasEyes);
        SceneManager.LoadScene("Menu");
        StopCoroutine(SendCloth(login));
        
       
    }
    
}
