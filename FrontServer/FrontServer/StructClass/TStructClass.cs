using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrontServer.StructClass
{
    public class EBMIndexTmp
    {
        public string IndexItemID { get; set; }
        public string S_EBM_id { get; set; }
        public string S_EBM_original_network_id { get; set; }

        public string S_EBM_start_time { get; set; }
        public string S_EBM_end_time { get; set; }
        public string S_EBM_type { get; set; }
        public string S_EBM_class { get; set; }
        public string S_EBM_level { get; set; }
        public string List_EBM_resource_code { get; set; }
        public string BL_details_channel_indicate { get; set; }
        public string DesFlag { get; set; }
        public string S_details_channel_transport_stream_id { get; set; }
        public string S_details_channel_program_number { get; set; }
        public string S_details_channel_PCR_PID { get; set; }

        public object DeliverySystemDescriptor { get; set; }

        public List<ProgramStreamInfotmp> List_ProgramStreamInfo;

        public int descriptor_tag { get; set; }

        public string ExtraData { get; set; }
    }

    public class ProgramStreamInfotmp
    {
        public string B_stream_type { get; set; }
        public string S_elementary_PID { get; set; }

        public object Descriptor2 { get; set; }//由于序列化问题改为 object类型
    }


    public class CableDeliverySystemDescriptortmp
    {
        public string B_FEC_inner { get; set; }
        public string B_FEC_outer { get; set; }
        public string B_Modulation { get; set; }
        public string D_frequency { get; set; }
        public string D_Symbol_rate { get; set; }
    }


    public class TerristrialDeliverySystemDescriptortmp
    {
        public string B_FEC { get; set; }
        public string B_Frame_header_mode { get; set; }
        public string B_Interleaveing_mode { get; set; }
        public string B_Modulation { get; set; }
        public string B_Number_of_subcarrier { get; set; }
        public string D_Centre_frequency { get; set; }
        public string L_Other_frequency_flag { get; set; }
        public string L_Sfn_mfn_flag { get; set; }
    }
}
