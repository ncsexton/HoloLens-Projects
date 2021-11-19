using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;
using System.Threading;

public class client : MonoBehaviour
{
    //=================

    public string key = "A60A5770FE5E7AB200BA9CFC94E4E8B0";
    public string IV = "1234567887654321";

    //public AesCryptoServiceProvider AEScryptoProvider = new AesCryptoServiceProvider();

    public TcpClient client2 = new TcpClient();
    public String hostname = "192.168.0.111";  //"10.105.195.9";
    public NetworkStream networkStream;
    public String message = "HelloWorld Nathan 1";

    public StreamWriter writer;
    public string path;

    public bool mRunning;
    public Thread mThread;
    public byte[] buff = new byte[1024];

    //=================

    public GameObject message2;
    public GameObject Cube;
    public GameObject playSpace;
    //private const int listenPort = 9050;
    private TcpClient listener = null;
    private IPEndPoint groupEP;
    //private const string targetIP = "localhost"; 
    public GameObject obj;
    public GameObject cylinder;
    public GameObject playerCam;
    //Int32 port = 9050;
    public void createCube()
    {
        obj = Instantiate(Cube, playSpace.transform.position, Quaternion.identity);
    }
    public void Awake()
    {
        path = Application.persistentDataPath + "/test9.txt";
        StartListener();
    }

    //Listener needs to be running in order to recieve any data
    public void StartListener()
    {
        writer = new StreamWriter(path, true);
        //System.Diagnostics.Debug.Log("Client2 is: ");
        //System.Diagnostics.Debug.Log(client2);
        client2.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        client2.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
        client2.Connect(hostname, 9050);


        networkStream = client2.GetStream();
        networkStream.ReadTimeout = 20000;
        mRunning = true;

        using (Aes myAes = Aes.Create())
        {
            myAes.Key = Encoding.Default.GetBytes("A60A5770FE5E7AB200BA9CFC94E4E8B0");
            myAes.IV = Encoding.Default.GetBytes("1234567887654321");

            byte[][] data_for_aes = new byte[2][];

            data_for_aes[0] = myAes.Key;
            data_for_aes[1] = myAes.IV;

            ParameterizedThreadStart ts = new ParameterizedThreadStart(receive);
            mThread = new Thread(ts);
            mThread.Start(data_for_aes);
            ParameterizedThreadStart ts2 = new ParameterizedThreadStart(send);
            mThread = new Thread(ts2);
            mThread.Start(data_for_aes);
        }
    }

    public void send(object t)
    {
        byte[][] args = (byte[][])t;
        if (args[0] == null || args[0].Length <= 0)
            throw new ArgumentNullException("Key");
        if (args[1] == null || args[1].Length <= 0)
            throw new ArgumentNullException("IV");

        byte[] encrypted;

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = args[0];
            aesAlg.IV = args[1];
            aesAlg.Padding = PaddingMode.Zeros;
            //aesAlg.Mode = CipherMode.CBC;

            System.Diagnostics.Debug.WriteLine("Sending time stamps");
            while (mRunning)
            {
                try
                {

                    if (networkStream.CanWrite)
                    {

                        double epoch = ((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds);
                        string mess = "desk: " + epoch.ToString();
                        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                        using (MemoryStream msEncrypt = new MemoryStream())
                        {
                            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                            {
                                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                                {
                                    //Write all data to the stream.
                                    swEncrypt.Write(mess);
                                }
                                encrypted = msEncrypt.ToArray();
                            }
                        }
                        print("I have encrypted");
                        networkStream.Write(encrypted, 0, encrypted.Length);
                        print("I have sent");
                        System.Threading.Thread.Sleep(50);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("ERRRRRROOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOR");
                    }

                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                    System.Diagnostics.Debug.WriteLine("EXCEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEPTION");
                }
            }
        }
    }

    public void receive(object t)
    {
        byte[][] args = (byte[][])t;
        if (args[0] == null || args[0].Length <= 0)
            throw new ArgumentNullException("Key");
        if (args[1] == null || args[1].Length <= 0)
            throw new ArgumentNullException("IV");
        
        string plaintext = null;

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = args[0];
            aesAlg.IV = args[1];
            aesAlg.Padding = PaddingMode.Zeros;
            //aesAlg.Mode = CipherMode.CBC;

            System.Diagnostics.Debug.WriteLine("Server Start");
            while (mRunning)
            {
                // check if new connections are pending, if not, be nice and sleep 100ms
                if (networkStream.CanRead)
                {
                    networkStream.Read(buff, 0, buff.Length);

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for decryption.
                    using (MemoryStream msDecrypt = new MemoryStream(buff))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {

                                // Read the decrypted bytes from the decrypting stream
                                // and place them in a string.
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                    double cur_time = ((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds);
                    //System.Diagnostics.Debug.WriteLine("This is what I received" + Encoding.Default.GetString(result));
                    writer.WriteLine("Local time " + cur_time.ToString() + " Remote time " + plaintext);

                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("THis is a receive error");
                }
            }
        }
    }
    public void stopListening()
    {
        mRunning = false;
    }
    private void Update()
    {
        
    }
    //Takes in a String, in x,y,z and returns the vector version

    public void OnApplicationQuit()
    { // stop listening thread
        stopListening(); // wait for listening thread to terminate (max. 500ms)
        mThread.Join(500);
        client2.Close();
        writer.Close();
    }
    public static Vector3 StringToVector3(string sVector)
    {
        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }
}