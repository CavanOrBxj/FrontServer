using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrontServer
{
    class BBSHelper
    {

        public static string CreateEBM_ID()
        {
            string ebm_id = "";
            ebm_id = SingletonInfo.GetInstance().ebm_id_front;
            // string ebm_id_behind = SingletonInfo.GetInstance().ebm_id_behind;
            string datatime = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0');
            if (datatime == SingletonInfo.GetInstance().ebm_id_behind)
            {
                SingletonInfo.GetInstance().ebm_id_count += 1;
            }
            else
            {
                SingletonInfo.GetInstance().ebm_id_behind = datatime;
                SingletonInfo.GetInstance().ebm_id_count = 1;
            }
            ebm_id += SingletonInfo.GetInstance().ebm_id_behind + SingletonInfo.GetInstance().ebm_id_count.ToString().PadLeft(4, '0');
            return ebm_id;
        }




    }
}
