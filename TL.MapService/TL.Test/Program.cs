using MSXML2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace TL.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestFileExist();
            Console.WriteLine("地图类型？");
            string maptype = Console.ReadLine();
            Console.WriteLine("下载地址？");
            string url = Console.ReadLine();
            Console.WriteLine("最大经度？");
            string maxlong = Console.ReadLine();
            Console.WriteLine("最小经度？");
            string minlong = Console.ReadLine();
            Console.WriteLine("最大纬度？");
            string maxlat = Console.ReadLine();
            Console.WriteLine("最小纬度？");
            string minlat = Console.ReadLine();
            Console.WriteLine("最小下载等级？");
            string minzoom = Console.ReadLine();
            Console.WriteLine("最大下载等级？");
            string maxzoom = Console.ReadLine();
            Console.WriteLine("你要下载{0}地图，从第{5}到第{6}等级，经度范围为[{1}]到[{2}],纬度范围为[{3}]到[{4}],下载地址为：{7}", maptype, minlong, maxlong, minlat, maxlat, minzoom, maxzoom, url);
            Console.WriteLine("确认下载？Y/N");
            string result = Console.ReadLine();
            if (result == "Y")
            {
                Vector2D topleft = new Vector2D();
                topleft.X = double.Parse(minlong);
                topleft.Y = double.Parse(maxlat);
                Vector2D bottomright = new Vector2D();
                bottomright.X = double.Parse(maxlong);
                bottomright.Y = double.Parse(minlat);
                Vector2D screentopleft = lonLat2Mercator(topleft);
                Vector2D screenbottomright = lonLat2Mercator(bottomright);

                UpdateHelper.DownloadByUrl(maptype, screentopleft.X, screenbottomright.Y, screenbottomright.X,
                    screentopleft.Y, int.Parse(minzoom), int.Parse(maxzoom), url);
                Console.WriteLine("下载完成。。。");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("取消下载。。。");
                Console.ReadKey();
            }
        }
        //经纬度转墨卡托
        public static Vector2D lonLat2Mercator(Vector2D lonLat)
        {
            Vector2D mercator = new Vector2D();
            double x = lonLat.X * 20037508.34 / 180;
            double y = Math.Log(Math.Tan((90 + lonLat.Y) * Math.PI / 360)) / (Math.PI / 180);
            y = y * 20037508.34 / 180;
            mercator.X = x;
            mercator.Y = y;
            return mercator;
        }
        public class Vector2D
        {
           public double X { get; set; }
           public double Y { get; set; }
        }

        public static void TestWCF()
        {
            string url = "http://202.100.190.46:6080/MapService.svc";
            XMLHTTP http = new MSXML2.XMLHTTP();
            try
            {
                http.open("GET", url, false, null, null);
                http.send(url);
                int status = http.status;
                if (status == 200)
                {
                    Console.WriteLine("成功"); Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("不可用status:" + status.ToString()); Console.ReadKey();
                }
            }
            catch
            {
                Console.WriteLine("不可用"); Console.ReadKey();
            }
        }

        public static void TestNginx()
        {
            string url = "http://localhost:88/Nginxstatus";
            XMLHTTP http = new MSXML2.XMLHTTP();
            try
            {
                http.open("GET", url, false, null, null);
                http.send(url);
                int status = http.status;
                if (status == 200)
                {
                    Console.WriteLine("成功");
                    Console.WriteLine(System.Text.Encoding.Default.GetString(http.responseBody));
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("不可用status:" + status.ToString()); Console.ReadKey();
                }
            }
            catch
            {
                Console.WriteLine("不可用"); Console.ReadKey();
            }
        }

        public static void TestFileExist()
        {
            Console.WriteLine(DateTime.Now.TimeOfDay.ToString());
            string filepath = @"C:\Users\younggis\Pictures\server11.png";
            if (File.Exists(filepath))
            {
                Console.WriteLine(DateTime.Now.TimeOfDay.ToString());
                Console.WriteLine("成功");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine(DateTime.Now.TimeOfDay.ToString());
                Console.WriteLine("成功");
                Console.ReadKey();
            }
        }
    }

}
