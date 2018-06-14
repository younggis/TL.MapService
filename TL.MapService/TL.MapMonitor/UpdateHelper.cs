using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace TL.MapMonitor
{
    public class UpdateHelper
    {
        //分辨率
        private static readonly double[] Resolution = new double[]
        {
            156543.03390625, 78271.516953125, 39135.7584765625, 19567.87923828125, 9783.939619140625, 4891.9698095703125,
            2445.9849047851562, 1222.9924523925781, 611.4962261962891, 305.74811309814453, 152.87405654907226,
            76.43702827453613, 38.218514137268066, 19.109257068634033, 9.554628534317017, 4.777314267158508,
            2.388657133579254, 1.194328566789627, 0.5971642833948135, 0.29858214169740677, 0.14929107084870338,
            0.07464553542435169, 0.037322767712175846, 0.018661383856087923, 0.009330691928043961, 0.004665345964021981,
            0.0023326729820109904, 0.0011663364910054952, 5.831682455027476E-4, 2.915841227513738E-4,
            1.457920613756869E-4
        };
        //起始点
        private static readonly double[] Origin = new double[]
        {
            -2.003750834E7, 2.003750834E7
        };
        //切片大小
        private static readonly int tilesize = 256;

        //更新最小等级
        private static readonly int Updateminzoom = int.Parse(ConfigurationManager.AppSettings["updateminzoom"]);
        //更新最大等级
        private static readonly int Updatemaxzoom = int.Parse(ConfigurationManager.AppSettings["updatemaxzoom"]);

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
        //生成文件目录，删除文件和缓存
        public static bool DeleteByUrl(string maptype, double x1, double y1, double x2, double y2)
        {
            bool result;
            for (var i = Updateminzoom; i <= Updatemaxzoom; i++)
            {
                int[] grid = GetAllColRow(x1, y1, x2, y2, i);
                for (var j = grid[0]; j <= grid[2]; j++)
                {
                    for (var k = grid[3]; k <= grid[1]; k++)
                    {
                        result = WriteFileHelper.DeleteFile(maptype, j.ToString(), k.ToString(), i.ToString());
                        if (result == false)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}