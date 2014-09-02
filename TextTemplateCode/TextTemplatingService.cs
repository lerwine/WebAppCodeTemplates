using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextTemplateCode
{
    public class TextTemplatingService : IDisposable
    {
        public void LogErrors(System.CodeDom.Compiler.CompilerErrorCollection c){ throw new NotImplementedException(); }
        public Boolean LoadIncludeText(System.String a, System.String v, System.String c){ throw new NotImplementedException(); }
        public System.AppDomain ProvideTemplatingAppDomain(System.String a){ throw new NotImplementedException(); }
        public System.String ResolveAssemblyReference(System.String a){ throw new NotImplementedException(); }
        public System.Type ResolveDirectiveProcessor(System.String a){ throw new NotImplementedException(); }
        public System.String ResolvePath(System.String a){ throw new NotImplementedException(); }
        public System.String ResolveParameterValue(System.String a, System.String b, System.String c){ throw new NotImplementedException(); }
        public void SetFileExtension(System.String a){ throw new NotImplementedException(); }
        public void SetOutputEncoding(System.Text.Encoding a, Boolean b){ throw new NotImplementedException(); }
        public System.Object GetHostOption(System.String a){ throw new NotImplementedException(); }
        public System.String ProcessTemplate(System.String a, System.String b, object c, System.Object d){ throw new NotImplementedException(); }
        public System.String PreprocessTemplate(System.String a, System.String b, object c, System.String d, System.String e, System.String[] f){ throw new NotImplementedException(); }
        public void BeginErrorSession(){ throw new NotImplementedException(); }
        public Boolean EndErrorSession(){ throw new NotImplementedException(); }
        public void DebugTemplateAsync(System.String a, System.String b, object c, System.Object d){ throw new NotImplementedException(); }
        public System.Object GetService(System.Type a){ throw new NotImplementedException(); }
        public object CreateSession(){ throw new NotImplementedException(); }
        public void set_InputFile(System.String a){ throw new NotImplementedException(); }
        public System.Runtime.Remoting.ObjRef CreateObjRef(System.Type a){ throw new NotImplementedException(); }
        public System.Object InitializeLifetimeService(){ throw new NotImplementedException(); }
        public System.Object GetLifetimeService(){ throw new NotImplementedException(); }
        public System.String ToString(){ throw new NotImplementedException(); }
        public Boolean Equals(System.Object a){ throw new NotImplementedException(); }
        public Int32 GetHashCode(){ throw new NotImplementedException(); }
        public System.Type GetType(){ throw new NotImplementedException(); }
        public System.Collections.Generic.IList<System.String> StandardAssemblyReferences;
        public System.Collections.Generic.IList<System.String> StandardImports;
        public System.String TemplateFile;
        public object Host;
        public object Engine;
        public System.String InputFile;
        public object Callback;
        public System.Object Hierarchy;
        public object Session;
        public System.String NamespaceHintName;
        public System.String IncludeContentType;

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
