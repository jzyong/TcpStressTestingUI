using System;
using System.Collections.Generic;
using System.IO;
using Core.Util.Time;
using UnityEngine;

namespace Core.Util
{
    /**
     * 自定义日志写文件
     */
    public class Log
    {
        public static string[] LogPaths = {"./log"};

        /**
         * 输出通用日志文件
         */
        public static void Println(string param)
        {
            string sFilePath = LogPaths[0];
            string sFileName = DateTime.Now.ToString("yyyyMMdd") + ".txt";
            //文件的绝对路径
            sFileName = Path.Combine(sFilePath, sFileName);
            //验证路径是否存在,不存在则创建
            if (!Directory.Exists(LogPaths[0]))
            {
                Directory.CreateDirectory(sFilePath);
            }

            FileStream fs;
            StreamWriter sw;
            if (File.Exists(sFileName))
                //验证文件是否存在，有则追加，无则创建
            {
                fs = new FileStream(sFileName, FileMode.Append, FileAccess.Write);
            }
            else
            {
                fs = new FileStream(sFileName, FileMode.Create, FileAccess.Write);
            }

            sw = new StreamWriter(fs);
            //日志内容
            string log = TimeUtil.CurrentFormatTime() + " ----> " + param;
            sw.WriteLine(log);
            Debug.Log(param);
            sw.Close();
            fs.Close();
        }

        /**
         * 记录数据
         */
        public static void Record(string fileName, string message)
        {
            string sFilePath = LogPaths[0];
            //文件的绝对路径
            fileName = Path.Combine(sFilePath, fileName);
            //验证路径是否存在,不存在则创建
            if (!Directory.Exists(LogPaths[0]))
            {
                Directory.CreateDirectory(sFilePath);
            }

            FileStream fs;
            StreamWriter sw;
            if (File.Exists(fileName))
                //验证文件是否存在，有则追加，无则创建
            {
                fs = new FileStream(fileName, FileMode.Append, FileAccess.Write);
            }
            else
            {
                fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            }

            sw = new StreamWriter(fs);
            //日志内容
            sw.WriteLine(message);
            Debug.Log(message);
            sw.Close();
            fs.Close();
        }

        /**
         * 记录数据
         */
        public static void Records(string fileName, List<string> messages)
        {
            string sFilePath = LogPaths[0];
            //文件的绝对路径
            fileName = Path.Combine(sFilePath, fileName);
            //验证路径是否存在,不存在则创建
            if (!Directory.Exists(LogPaths[0]))
            {
                Directory.CreateDirectory(sFilePath);
            }

            FileStream fs;
            StreamWriter sw;
            if (File.Exists(fileName))
                //验证文件是否存在，有则追加，无则创建
            {
                fs = new FileStream(fileName, FileMode.Append, FileAccess.Write);
            }
            else
            {
                fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            }

            sw = new StreamWriter(fs);
            //日志内容
            foreach (var message in messages)
            {
                sw.WriteLine(message);
            }

            sw.Close();
            fs.Close();
        }
    }
}