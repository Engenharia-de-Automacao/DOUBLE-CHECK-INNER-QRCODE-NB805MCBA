using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace double_check_qrcode_inner_nb805mcba
{
    internal class Conexao
    {
        private static string _Conexao_short_tasks = "server=10.1.0.35;user=balanca;password=multilaser;database=Short_tasks;Charset=utf8";

        public static string Conexao_short_tasks
        {
            get { return _Conexao_short_tasks; }
            set { _Conexao_short_tasks = value; }
        }
    }
}
    