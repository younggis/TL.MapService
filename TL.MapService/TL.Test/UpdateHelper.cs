using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TL.MapService;

namespace TL.Test
{
    class UpdateHelper
    {
        //分辨率
        private static readonly double[] Resolution = new double[]
        {
            156543.03392800014,  78271.51696399994, 39135.75848200009, 19567.87924099992, 9783.93962049996, 4891.96981024998,
            2445.98490512499, 1222.992452562495, 611.4962262813797, 305.74811314055756, 152.87405657041106,
            76.43702828507324, 38.21851414253662, 19.10925707126831, 9.554628535634155, 4.77731426794937,
            2.388657133974685, 1.1943285668550503, 0.5971642835598172, 0.29858214164761665
        };
        //起始点
        private static readonly double[] Origin = new double[]
        {
            -2.0037508342787E7, 2.0037508342787E7 
        };
        //切片大小
        private static readonly int tilesize = 256;

        private static readonly int zoom = 18;

        //根据坐标计算行列号
        public static int[] GetColRow(double x, double y, int z)
        {
            int col = (int)Math.Floor(Math.Abs(Origin[0] - x) / (tilesize * Resolution[z]));
            int row = (int)Math.Floor(Math.Abs(Origin[1] - y) / (tilesize * Resolution[z]));
            return new int[] { col, row };
        }

        //计算行列号范围
        public static int[] GetAllColRow(double x1, double y1, double x2, double y2, int z)
        {
            int[] min = GetColRow(x1, y1, z);
            int[] max = GetColRow(x2, y2, z);
            return new[]
            {
                min[0], min[1], max[0], max[1]
            };
        }

        private static int successcount = 0;//成功次数
        private static int errorcount = 0;//失败次数
        //下载文件
        public static bool DownloadByUrl(string maptype, double x1, double y1, double x2, double y2,int minzoom,int maxzoom,string url)
        {
            string requesturl;
            for (var i = minzoom; i <= maxzoom; i++)
            {
                int[] grid = GetAllColRow(x1, y1, x2, y2, i);
                for (var j = grid[0]; j <= grid[2]; j++)
                {
                    for (var k = grid[3]; k <= grid[1]; k++)
                    {
                        requesturl = url +"?"+ "maptype="+maptype+"&x="+k+"&y="+j+"&z="+i;
                        Request(requesturl);
                    }
                }
            }
            return true;
        }

        public static void Request(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 6000;
            request.KeepAlive = false;
            request.Method = "GET";
            request.ServicePoint.ConnectionLimit = 512;
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.1.4322)"; // 
            request.AllowAutoRedirect = true; //是否允许302
            HttpWebResponse response = null;
            Stream stream = null;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
                stream = response.GetResponseStream();
                successcount++;
                Console.WriteLine("{0}，请求成功！,成功次数：{1}",DateTime.Now,successcount);
            }
            catch (Exception e)
            {
                string base404 = "iVBORw0KGgoAAAANSUhEUgAAAQAAAAEAAQMAAABmvDolAAAAA1BMVEX8+fKNaX6qAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAAH0lEQVRo3u3BAQ0AAADCoPdPbQ43oAAAAAAAAAAAvg0hAAABfxmcpwAAAABJRU5ErkJggg==";
                byte[] bytes = Convert.FromBase64String(base404);
                Stream strem = new MemoryStream(bytes);
                errorcount++;
                Log4Helper.WriteLog(url+"\n");
                Console.WriteLine("{0}，请求失败！,失败次数：{1}", DateTime.Now, errorcount);
            }
            finally
            {
                request.Abort();
                if (response != null)
                {
                    response.Close();
                }
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }
    }
}
