using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace TextTemplateCode
{
    /// <summary>
    /// This class inherits from the same class that a *.tt file when it is transformed.
    /// This can be used to help you investigate what a text transformation file does.
    /// </summary>
    public class TTInstance : Microsoft.VisualStudio.TextTemplating.TextTransformation
    {
        #region Members which are declared only to emulate what you'd find when a .tt file is being transformed

        TextTemplatingService Host { get; set; }

        public override string TransformText()
        {
            throw new NotImplementedException();
        }

        #endregion

        EnvDTE.DTE _dte = null;
        EnvDTE.DTE DTE
        {
            get
            {
                if (this._dte == null)
                    this._dte = (EnvDTE.DTE)(((IServiceProvider)(this.Host)).GetService(typeof(EnvDTE.DTE)));

                return this._dte;
            }
        }

        EnvDTE.Project _currentProject = null;
        EnvDTE.Project CurrentProject
        {
            get
            {
                if (this._currentProject == null)
                    this._currentProject = this.CurrentProjectItem.ContainingProject;

                return this._currentProject;
            }
        }

        EnvDTE.ProjectItem _currentProjectItem = null;
        EnvDTE.ProjectItem CurrentProjectItem
        {
            get
            {

                if (this._currentProjectItem == null)
                    this._currentProjectItem = this.DTE.Solution.FindProjectItem(this.Host.TemplateFile);

                return this._currentProjectItem;
            }
        }

        string _defaultNamespace = null;
        string DefaultNamespace
        {
            get
            {
                if (this._defaultNamespace == null && (this._defaultNamespace = this.CurrentProject.Properties.Item("DefaultNamespace").Value as string) == null)
                    this._defaultNamespace = "";

                return this._defaultNamespace;
            }
        }

        string _customToolNamespace = null;
        string CustomToolNamespace
        {
            get
            {
                if (this._customToolNamespace == null &&
                        (this._customToolNamespace = this.CurrentProjectItem.Properties.Item("CustomToolNamespace").Value as string) == null)
                    this._customToolNamespace = "";

                return this._customToolNamespace;
            }
        }

        string _currentItemNamespace = null;
        string CurrentItemNamespace
        {
            get
            {
                if (this._currentItemNamespace == null && (this._currentItemNamespace = this.CustomToolNamespace) == "" &&
                        (this._currentItemNamespace = System.Runtime.Remoting.Messaging.CallContext.LogicalGetData("NamespaceHint") as string) == null)
                    this._currentItemNamespace = this.DefaultNamespace;

                return this._currentItemNamespace;
            }
        }

        string _defaultClassName = null;
        string DefaultClassName
        {
            get
            {
                if (this._defaultClassName == null)
                {
                    string fn = System.IO.Path.GetFileNameWithoutExtension(this.Host.TemplateFile);
                    if (Regex.IsMatch(fn, @"^\d"))
                        fn = "N" + fn;

                    this._defaultClassName = Regex.Replace(fn, @"[^a-z\d_]", (Match m) => String.Format("_x{0:X4}_", (int)(m.Value[0])), RegexOptions.IgnoreCase);
                }

                return this._defaultClassName;
            }
        }
    }
}
