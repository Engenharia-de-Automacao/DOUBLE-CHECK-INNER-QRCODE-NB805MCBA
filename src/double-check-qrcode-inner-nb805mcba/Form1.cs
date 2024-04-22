using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace double_check_qrcode_inner_nb805mcba
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtIMEI.Select();
        }
        DataTable dt = new DataTable();
        string retornoDuplicidade;
        int cnt_registroSN;
        int cnt_registroIMEI;
        int cnt_registroQRCODE;

        public int QUERY_MYSQL_RETURN_COUNT_SN(string server, string query)
        {
            int count = 0;

            MySqlConnection config = new MySqlConnection(server);
            MySqlCommand queryCommand = new MySqlCommand(query, config);
            config.Open();

            object resultado = queryCommand.ExecuteScalar();
            if (resultado != null && resultado != "")
            {
                count = Convert.ToInt32(resultado);
            }
            else
            {
                count = 0;
            }

            config.Close();

            return count;
        }

        public int QUERY_MYSQL_RETURN_COUNT_IMEI(string server, string query)
        {
            int count = 0;

            MySqlConnection config = new MySqlConnection(server);
            MySqlCommand queryCommand = new MySqlCommand(query, config);
            config.Open();

            object resultado = queryCommand.ExecuteScalar();
            if (resultado != null && resultado != "")
            {
                count = Convert.ToInt32(resultado);
            }
            else
            {
                count = 0;
            }

            config.Close();

            return count;
        }

        public int QUERY_MYSQL_RETURN_COUNT_QRCODE(string server, string query)
        {
            int count = 0;

            MySqlConnection config = new MySqlConnection(server);
            MySqlCommand queryCommand = new MySqlCommand(query, config);
            config.Open();

            object resultado = queryCommand.ExecuteScalar();
            if (resultado != null && resultado != "")
            {
                count = Convert.ToInt32(resultado);
            }
            else
            {
                count = 0;
            }

            config.Close();

            return count;
        }

        string imeiScanneado;
        string sniScanneado;
        string qrcodeScanneado;

        private void txtIMEI_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtIMEI.Text != "" && txtIMEI.Text.StartsWith("35"))
            {
                imeiScanneado = txtIMEI.Text;
                txtIMEI.Enabled = false;
                txtSN.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                txtIMEI.Text = "";
                txtIMEI.Focus();
            }
        }

        private void txtSN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtSN.Text != "" && txtSN.Text.StartsWith("1NB") && txtSN.Text.Length == 15)
            {
                sniScanneado = txtSN.Text;
                txtSN.Enabled = false;
                txtQRCODE.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                txtSN.Text = "";
                txtSN.Focus();
            }
        }

        private void txtQRCODE_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtQRCODE.Text != "" && txtQRCODE.Text.Length > 20)
            {
                qrcodeScanneado = txtQRCODE.Text;
                txtQRCODE.Enabled = false;
                timerValidacao.Enabled = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                txtQRCODE.Text = "";
                txtQRCODE.Focus();
            }
        }

        public void QUERY_MYSQL(string server, string query)
        {
            MySqlConnection Conexao = new MySqlConnection(server);
            MySqlCommand Query = new MySqlCommand();
            Query.Connection = Conexao;
            Conexao.Open();

            Query.CommandText = query;
            Query.ExecuteNonQuery();
            Conexao.Close();

        }

        private void timerReset_Tick(object sender, EventArgs e)
        {
            timerReset.Enabled = false;

            txtIMEI.Text = "";
            txtSN.Text = "";
            txtQRCODE.Text = "";

            //lblStatus.Text = "Aguardando bipes...";
            //lblStatus.BackColor = Color.LightGray;
            //panel1.BackColor = Color.LightGray;

            txtIMEI.Enabled = true;
            txtSN.Enabled = true;
            txtQRCODE.Enabled = true;

            txtIMEI.Focus();
        }

        private void timerValidcao_Tick(object sender, EventArgs e)
        {
            timerValidacao.Enabled = false;

            try
            {
                cnt_registroSN = QUERY_MYSQL_RETURN_COUNT_SN(Conexao.Conexao_short_tasks, "SELECT COUNT(ID) FROM check_inner_master_retrabalho_nb805mcba WHERE SN = '" + txtSN.Text + "'");
                cnt_registroIMEI = QUERY_MYSQL_RETURN_COUNT_IMEI(Conexao.Conexao_short_tasks, "SELECT COUNT(ID) FROM check_inner_master_retrabalho_nb805mcba WHERE IMEI = '" + txtIMEI.Text + "'");
                cnt_registroQRCODE = QUERY_MYSQL_RETURN_COUNT_QRCODE(Conexao.Conexao_short_tasks, "SELECT COUNT(ID) FROM check_inner_master_retrabalho_nb805mcba WHERE QRCODE = '" + txtQRCODE.Text + "'");
            }
            catch
            {

            }

            if (cnt_registroSN > 0 && cnt_registroIMEI > 0 && cnt_registroQRCODE > 0)
            {
                lblStatus.Text = "Caixa duplicada, favor separar a caixa.";
                lblStatus.BackColor = Color.Red;
                timerReset.Enabled = true;
            }
            else
            {
                if (qrcodeScanneado.Contains(imeiScanneado) && qrcodeScanneado.Contains(sniScanneado))
                {
                    lblStatus.Text = "Aprovado!";
                    lblStatus.BackColor = Color.Green;

                    try
                    {
                        QUERY_MYSQL(Conexao.Conexao_short_tasks, "INSERT INTO check_inner_master_retrabalho_nb805mcba (SN, IMEI, QRCODE, STATUS, DATA_HORA)" +
                                            "VALUES ('" + txtSN.Text + "','" + txtIMEI.Text + "','" + txtQRCODE.Text + "','APROVADO', NOW())");
                    }
                    catch
                    {

                    }

                    timerReset.Enabled = true;
                }
                else
                {
                    lblStatus.Text = "Reprovado!";
                    lblStatus.BackColor = Color.Red;
                    try
                    {
                        QUERY_MYSQL(Conexao.Conexao_short_tasks, "INSERT INTO check_inner_master_retrabalho_nb805mcba (SN, IMEI, QRCODE, STATUS, DATA_HORA)" +
                                            "VALUES ('" + txtSN.Text + "','" + txtIMEI.Text + "','" + txtQRCODE.Text + "','REPROVADO', NOW())");
                    }
                    catch
                    {

                    }
                    timerReset.Enabled = true;
                }
            }
        }
    }
}
