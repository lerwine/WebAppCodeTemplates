using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Erwine.Leonard.T.ExampleWebApplication
{
    public partial class SiteMasterPage : System.Web.UI.MasterPage
    {
        public string PageTitle
        {
            get
            {
                if (String.IsNullOrWhiteSpace(this.PageTitleLiteral.Text))
                    this.PageTitleLiteral.Text = (String.IsNullOrWhiteSpace(this.Page.Title)) ? this.Page.Title : this.DefaultTitle;

                return this.PageTitleLiteral.Text;
            }
            set { this.PageTitleLiteral.Text = (value == null) ? "" : value.Trim(); }
        }

        public string MainHeading
        {
            get
            {
                if (String.IsNullOrWhiteSpace(this.MainHeadingLiteral.Text))
                    this.MainHeadingLiteral.Text = (String.IsNullOrWhiteSpace(this.Page.Title)) ? this.Page.Title : this.DefaultTitle;

                return this.MainHeadingLiteral.Text;
            }
            set { this.MainHeadingLiteral.Text = (value == null) ? "" : value.Trim(); }
        }

        public string SubHeading
        {
            get { return this.SubHeadingLiteral.Text; }
            set { this.SubHeadingLiteral.Text = (value == null) ? "" : value.Trim(); }
        }

        public string FooterText
        {
            get { return this.FooterTextLiteral.Text; }
            set { this.FooterTextLiteral.Text = (value == null) ? "" : value.Trim(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.SubHeadingH2.Visible = true;
            this.FooterTextPanel.Visible = true;
        }

        private string _defaultTitle = null;

        public string DefaultTitle
        {
            get
            {
                if (this._defaultTitle == null)
                {
                    System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    this._defaultTitle = assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyTitleAttribute), false).OfType<System.Reflection.AssemblyTitleAttribute>()
                        .Where(a => !String.IsNullOrWhiteSpace(a.Title)).Select(a => a.Title).DefaultIfEmpty(assembly.FullName).First();
                }

                return this._defaultTitle;
            }
        }

        protected void TitleTextLiteral_PreRender(object sender, EventArgs e)
        {
            Literal literal = sender as Literal;

            if (String.IsNullOrWhiteSpace(literal.Text))
                literal.Text = (String.IsNullOrWhiteSpace(this.Page.Title)) ? this.DefaultTitle : this.Page.Title;
        }

        protected void SubHeadingLiteral_PreRender(object sender, EventArgs e)
        {
            this.SubHeadingH2.Visible = !String.IsNullOrWhiteSpace(this.SubHeadingLiteral.Text);
        }

        protected void FooterTextLiteral_PreRender(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(this.FooterTextLiteral.Text))
                return;

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            string copyright = assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyCopyrightAttribute), false).OfType<System.Reflection.AssemblyCopyrightAttribute>()
                .Where(a => !String.IsNullOrWhiteSpace(a.Copyright)).Select(a => a.Copyright).FirstOrDefault();
            if (copyright == null)
                this.FooterTextPanel.Visible = false;
            else
                this.FooterTextLiteral.Text = Server.HtmlEncode(copyright);
        }
    }
}
