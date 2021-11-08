using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Monitor : MonoBehaviour
{

    static public void console(int freq, params object[] args)
    {
        if (Time.frameCount % freq != 0) return;

        Debug.Log("┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");

        StringBuilder sb = new StringBuilder();
        sb.Append("┃ ");

        foreach (object arg in args)
        {
            sb.Append(arg.ToString()+" ");
        }
        Debug.Log(sb.ToString() + "\n");
        Debug.Log("┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");
    }

}
