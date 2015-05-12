using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace Erwine.Leonard.T.ExampleWebApplication
{
    public partial class DefaultPage : System.Web.UI.Page
    {
        public static readonly Regex LeadWs = new Regex(@"^\s+(?=\S)", RegexOptions.Compiled | RegexOptions.Multiline);
        public static readonly Regex NewLine = new Regex(@"(\r\n?|\n)", RegexOptions.Compiled);

        private void Literal_Load(object sender, object obj)
        {
            string s;
            try
            {
                Erwine.Leonard.T.LoggingModule.TraceDataObject tdo= new LoggingModule.TraceDataObject(obj);
                using (MemoryStream ms = new MemoryStream())
                {
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        CloseOutput = false,
                        Encoding = Encoding.UTF8,
                        Indent = true
                    };
                    using (XmlWriter writer = XmlWriter.Create(ms, settings))
                    {
                        DataContractSerializer ser = new DataContractSerializer(tdo.GetType());
                        ser.WriteObject(writer, tdo);
                        writer.Flush();
                    }
                    s = settings.Encoding.GetString(ms.ToArray());
                }
            }
            catch (Exception exc)
            {
                s = exc.ToString();
            }
            (sender as Literal).Text = DefaultPage.NewLine.Replace(DefaultPage.LeadWs.Replace(this.Server.HtmlEncode(s),
                DefaultPage.SpaceToNbsp), "<br />");
        }

        private static string SpaceToNbsp(Match m)
        {
            return new String(m.Value.SelectMany(c => (c == '\t') ? "&nbsp;&nbsp;&nbsp;&nbsp;" : "&nbsp;").ToArray());
        }

        protected void Literal1_Load(object sender, EventArgs e)
        {
            this.Literal_Load(sender, null);
        }

        protected void Literal2_Load(object sender, EventArgs e)
        {
            this.Literal_Load(sender, new object());
        }

        protected void Literal3_Load(object sender, EventArgs e)
        {
            this.Literal_Load(sender, "");
        }

        protected void Literal4_Load(object sender, EventArgs e)
        {
            this.Literal_Load(sender, "Test\r\nMe!");
        }

        protected void Literal5_Load(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(@"Data source=LENNYPC\SQLEXPRESS;initial catalog=PhotoLocker;integrated security=True"))
                    {
                        connection.Open();
                        using (SqlCommand cmd = new SqlCommand("SELECT * FROM MyTableDoesNotExist", connection))
                            cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception exc)
                {
                    try
                    {
                        throw new ArgumentException("Cannot run command", "sender");
                    }
                    catch (Exception ee)
                    {
                        throw new AggregateException("THis is the combo", exc, ee);
                    }
                }
            }
            catch (Exception finalExc)
            {
                this.Literal_Load(sender, finalExc);
            }
        }
    }
}