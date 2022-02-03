using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace MSDNDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            //para capturar os dados html de uma página
            var wc = new WebClient();
            string pagina = wc.DownloadString("https://social.msdn.microsoft.com/forums/pt-br/home?sort=lastpostdesc");

            //é necessário especificar que vc quer usar o HtmlDocument do HtmlAgilityPack, se não
            //ele ira utilizar o da dll padrão
            var htmlDocument = new HtmlAgilityPack.HtmlDocument();

            //para jogar os dados que baixamos dentro dessa htmlDocument
            htmlDocument.LoadHtml(pagina);

            //limpar o grid
            dataGridView1.Rows.Clear();

            string id = string.Empty;
            string titulo = string.Empty;
            string postagem = string.Empty;
            string exibicao = string.Empty;
            string resposta = string.Empty;
            string link = string.Empty;

            //criando um laço de repetição para pegar cada objeto dentro de threadList(vimos dandos f12
            //na página para ver onde se encaixa as perguntas, então pegamos cada filho node)
            foreach (HtmlNode node in htmlDocument.GetElementbyId("threadList").ChildNodes)
            {
                if(node.Attributes.Count > 0)
                {
                    //para o link conseguimos pegar por um atributo do node
                    id = node.Attributes["data-threadid"].Value;
                    link = "https://social.msdn.microsoft.com/Forums/pt-BR/" + id;
                    // para o titulo precisamos acessar os descendentes
                    titulo = node.Descendants().First(x => x.Id.Equals("threadTitle_" + id)).InnerText;
                    postagem = WebUtility.HtmlDecode(node.Descendants().First(x => x.Attributes["class"] !=null && x.Attributes["class"].Value.Equals("lastpost")).InnerText.Replace("\n","").Replace("  ",""));
                    exibicao = WebUtility.HtmlDecode(node.Descendants().First(x => x.Attributes["class"] != null && x.Attributes["class"].Value.Equals("viewcount")).InnerText);
                    resposta = WebUtility.HtmlDecode(node.Descendants().First(x => x.Attributes["class"] != null && x.Attributes["class"].Value.Equals("replycount")).InnerText);
                    //webUtilitu.htmlDecode para poder tirar os caracteres especificos do html

                    if (!string.IsNullOrEmpty(titulo))
                    {
                        dataGridView1.Rows.Add(titulo, postagem, exibicao, resposta, link);
                    }
                }
            }
        }
        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4)
                Process.Start(new ProcessStartInfo(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()));
        }
    }
}
